using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using LookOn.Consts;
using LookOn.Core.Extensions;
using LookOn.Dashboards.Page1;
using LookOn.Emails;
using LookOn.Enums;
using LookOn.Integrations.Datalytis.Components.Repositories.Interfaces;
using LookOn.Integrations.Datalytis.Configs;
using LookOn.Integrations.Datalytis.Models.Entities;
using LookOn.Integrations.Haravan.Components.Repositories.Interfaces;
using LookOn.Integrations.Haravan.Configs;
using LookOn.Integrations.Haravan.Models.Entities;
using LookOn.Integrations.Haravan.Models.Enums;
using LookOn.Merchants;
using LookOn.MerchantSubscriptions;
using LookOn.MerchantSyncInfos;
using LookOn.SystemConfigs;
using OfficeOpenXml.FormulaParsing.ExpressionGraph.FunctionCompilers;
using Volo.Abp.Domain.Repositories;

namespace LookOn.Dashboards;

/// <summary>
/// provide data for all page 1 charts (user + insight)
/// </summary>
public class Page1DataSourceManager : LookOnManager
{
    private readonly IMerchantRepository                   _merchantRepo;
    private readonly IHaravanCustomerRepository            _haravanCustomerRepo;
    private readonly IHaravanOrderRepository               _haravanOrderRepo;
    private readonly IDatalytisUserRepository              _datalytisUserRepo;
    private readonly IDatalytisUserSocialInsightRepository _insightRepo;
    private readonly IMerchantSyncInfoRepository           _merchantSyncInfoRepo;

    // managers
    private readonly MerchantSubscriptionManager _subscriptionManager;
    private readonly SystemConfigManager         _systemConfigManager;
    private readonly EmailManager                _emailManager;

    public Page1DataSourceManager(IMerchantRepository                   merchantRepo,
                                  MerchantSubscriptionManager           subscriptionManager,
                                  IDatalytisUserSocialInsightRepository insightRepo,
                                  IDatalytisUserRepository              datalytisUserRepo,
                                  IHaravanCustomerRepository            haravanCustomerRepo,
                                  IMerchantSyncInfoRepository           merchantSyncInfoRepo,
                                  IHaravanOrderRepository               haravanOrderRepo,
                                  SystemConfigManager                   systemConfigManager,
                                  EmailManager                          emailManager)
    {
        _merchantRepo         = merchantRepo;
        _subscriptionManager  = subscriptionManager;
        _insightRepo          = insightRepo;
        _datalytisUserRepo    = datalytisUserRepo;
        _haravanCustomerRepo  = haravanCustomerRepo;
        _merchantSyncInfoRepo = merchantSyncInfoRepo;
        _haravanOrderRepo     = haravanOrderRepo;
        _systemConfigManager  = systemConfigManager;
        _emailManager         = emailManager;
    }

    public async Task InitMerchantSyncInfo(Guid merchantId)
    {
        var merchantSyncInfo = await _merchantSyncInfoRepo.FirstOrDefaultAsync(_ => _.MerchantId == merchantId);
        if (merchantSyncInfo is not null) return;

        var merchant = await _merchantRepo.GetAsync(_ => _.Id == merchantId);
        await _merchantSyncInfoRepo.InsertAsync(new MerchantSyncInfo
        {
            MerchantId      = merchant.Id,
            MerchantEmail   = merchant.Email,
            Page1SyncStatus = MerchantSyncStatus.Pending,
            Page2SyncStatus = MerchantSyncStatus.Pending,
            SocialScan = new MerchantSocialScan
            {
                UserScans = merchant.Communities
                                    .Where(_ => _.VerificationStatus
                                             == SocialCommunityVerificationStatus.Approved
                                             && _.SocialCommunityId != null)
                                    .Select(_ => new MerchantUserScan
                                     {
                                         SocialCommunityId = _.SocialCommunityId
                                     })
                                    .ToList(),
            },
            EcomScan = new MerchantEcomScan
            {
                OrderSyncIntervalInDays = HaravanGlobalConfig.SyncOrderIntervalDays
            }
        });
    }

    public async Task<Page1DataSourceEcom> GetPage1DataEcom(Page1DataRequest request)
    {
        await InitMerchantSyncInfo(request.MerchantId);

        var merchantSyncInfo = await _merchantSyncInfoRepo.GetAsync(_ => _.MerchantId == request.MerchantId);
        if (merchantSyncInfo.Page1SyncStatus < MerchantSyncStatus.SyncEcomOrderCompleted) return null;

        var previousDateTime = request.TimeFrame.GetDateByTimeFrameType(request.From, false);
        var allOrders = await _haravanOrderRepo.GetListAsync(x => x.MerchantId        == request.MerchantId
                                                               && x.FulfillmentStatus == HaravanFulfillmentStatus.Fulfilled
                                                               && x.ConfirmedAt.HasValue
                                                               && x.CreatedAt.HasValue);
        var orders      = allOrders.Where(x => x.CreatedAt.Value >= previousDateTime.Item1 && x.CreatedAt.Value <= request.To).ToList();
        var customerIds = orders.Where(order => order.HaravanCustomerId.HasValue).Select(order => order.HaravanCustomerId.Value).Distinct().ToList();
        var customers =
            await _haravanCustomerRepo.GetListAsync(customer => customer.CustomerId.HasValue && customerIds.Contains(customer.CustomerId.Value));
        customers = customers.DistinctBy(customer => customer.CustomerId).ToList();

        return orders.IsNullOrEmpty() ? null : new Page1DataSourceEcom { EcomOrders = orders, EcomCustomers = customers, AllEcomOrders = allOrders };
    }

    public async Task<Page1DataSourceSocial> GetPage1DataSocial(Page1DataRequest request)
    {
        var merchantSyncInfo = await _merchantSyncInfoRepo.GetAsync(_ => _.MerchantId == request.MerchantId);

        if (merchantSyncInfo is null
         || !merchantSyncInfo.Page1SyncStatus.IsIn(MerchantSyncStatus.SyncSocialInsightRequestCompleted,
                                                   MerchantSyncStatus.SyncSocialInsightScanCompleted,
                                                   MerchantSyncStatus.DashboardDataReady))
            return null;

        var insightScan    = merchantSyncInfo.SocialScan.InsightScan;
        var sampleUsers    = await _datalytisUserRepo.GetListAsync(_ => insightScan.Page1UserIds.Contains(_.Id));
        var sampleInsights = await _insightRepo.GetListAsync(_ => insightScan.Page1InsightSocialUids.Contains(_.Uid));

        return new Page1DataSourceSocial { SocialUsers = sampleUsers, SocialInsights = sampleInsights };
    }

    /// <summary>
    /// Currently we have 2 time frame: 7 and 30 days.
    /// Always prepare data for 30 days.
    /// If 7 days is applied, then
    ///     - Ecom: filter 7-day orders and return to Ecom data
    ///     - Social: no date label, so
    ///             1. intersect of purchased users and social users
    ///             2. pick 5000 of valid users of #1 for social user metrics
    ///             3. pick 2000 of #2 for social insight metrics
    /// </summary>
    /// <param name="merchantId"></param>
    public async Task PreparePage1Data(Guid merchantId)
    {
        // 1. get ecom order, customer
        // 2. get ecom user
        // 3. get social users
        // 4. get social insight = giao cua customer phone va social user phone => datalytis user => uid => gui di lay insight
        
        
        var merchant = await _merchantRepo.GetAsync(merchantId);
        if (merchant.GetSocialCommunityIds().IsNullOrEmpty()) return;

        var activeSubscription = await _subscriptionManager.GetActiveSubscription(merchantId);
        if (activeSubscription is null) return;
        var subscriptionConfig = activeSubscription.SubscriptionConfig;

        var merchantSyncInfo = await _merchantSyncInfoRepo.GetAsync(_ => _.MerchantId == merchantId);
        var status           = merchantSyncInfo.Page1SyncStatus;
        var userScans        = merchantSyncInfo.SocialScan.UserScans;

        switch (status)
        {
            case MerchantSyncStatus.Pending:
            {
                // 1.0. Scan ecom and wait for completion
                if (merchantSyncInfo.EcomScan.CleanOrderSyncStatus == MerchantJobStatus.Completed)
                {
                    merchantSyncInfo.Page1SyncStatus = MerchantSyncStatus.SyncEcomOrderCompleted;
                }

                break;
            }

            case MerchantSyncStatus.SyncEcomOrderCompleted:
            {
                // 1.1. Request scan social users (all)
                var allRequested                                   = userScans.IsNotNullOrEmpty() && userScans.All(scan => scan.IsRequestCompleted);
                if (allRequested) merchantSyncInfo.Page1SyncStatus = MerchantSyncStatus.SyncSocialUserRequestCompleted;
                break;
            }

            case MerchantSyncStatus.SyncSocialUserRequestCompleted:
            {
                // 1.2. wait SCAN
                var allScanned                                   = userScans.IsNotNullOrEmpty() && userScans.All(scan => scan.IsScanCompleted);
                if (allScanned) merchantSyncInfo.Page1SyncStatus = MerchantSyncStatus.SyncSocialUserScanCompleted;
                break;
            }

            case MerchantSyncStatus.SyncSocialUserScanCompleted:
            {
                // 1.3. wait SYNC
                var allSynced = userScans.IsNotNullOrEmpty() && userScans.All(scan => scan.SyncStatus == MerchantJobStatus.Completed);
                if (allSynced) merchantSyncInfo.Page1SyncStatus = MerchantSyncStatus.SyncSocialUserSyncCompleted;
                break;
            }

            case MerchantSyncStatus.SyncSocialUserSyncCompleted:
            {
                // state machine
                // pre-condition:
                // 1. the ecom orders are SYNCED
                // 2. thesocial users are SYNCED

                var systemConfig = await _systemConfigManager.GetOrInit();

                var ecomCustomers = await _haravanCustomerRepo.GetListAsync(_ => _.MerchantId == merchantId && _.Phone != null && _.Phone != "");
                ecomCustomers = ecomCustomers.DistinctBy(c => c.Phone).ToList();

                var ecomCustomerPhones = ecomCustomers.Select(_ => _.Phone.ToInternationalPhoneNumberFromVN()).ToList();

                // 2.1. get all social users from merchant pages
                // 2.2. only select <subscriptionConfig.MaxSocialUser> users to do DEMOGRAPHIC
                // need to prepare data for all communities            
                var socialUsers   = new List<DatalytisUser>();
                var socialUserDic = new Dictionary<string, List<DatalytisUser>>();
                foreach (var socialCommunityId in merchant.GetSocialCommunityIds())
                {
                    var users = await _datalytisUserRepo.GetListAsync(_ => _.Phone != null
                                                                        && _.Phone != string.Empty
                                                                        && _.Uid   != null
                                                                        && _.Uid   != string.Empty
                                                                        && _.MerchantFbPageIds.Contains(socialCommunityId));
                    users = users.DistinctBy(u => u.Phone).Where(u => u.Birthday != null).ToList();
                    socialUserDic.Add(socialCommunityId, users);
                }
                
                var totalUsers = socialUserDic.Values.SelectMany(_ => _).Count();
                if (totalUsers is not 0)
                {
                    foreach (var dicEntry in socialUserDic)
                    {
                        var amount = (dicEntry.Value.Count / totalUsers) * totalUsers;
                        socialUsers.AddRange(dicEntry.Value.Take(amount).ToList());
                    }

                    socialUsers = socialUsers.Where(_ => ecomCustomerPhones.Contains(_.Phone.ToInternationalPhoneNumberFromVN())).ToList();

                    // 2.3. intersect all ecom customers + valid social user (gender + relationship + dob)
                    var insightCount = subscriptionConfig.MaxSocialUserInsight
                                     + ((ecomCustomers.Count - subscriptionConfig.MaxSocialUserInsight) * subscriptionConfig.MaxSocialUserInsightAddInPercent);
                    var insights = socialUsers.Take((int) insightCount).ToList();
                    socialUsers = socialUsers.Take(subscriptionConfig.MaxSocialUser).ToList();

                    // 2.4. send request insight to job
                    var insightUids = insights.Select(_ => _.Uid).ToList();
                    merchantSyncInfo.SocialScan.InsightScan = new MerchantInsightScan
                    {
                        Page1UserIds = socialUsers.Select(_ => _.Id).ToList(), Page1InsightSocialUids = insightUids
                    };
                    var insightScanRequests = new List<MerchantInsightScanRequest>();
                    foreach (var batch in insightUids.Partition(DatalytisGlobalConfig.DefaultPageSize))
                    {
                        var request = new MerchantInsightScanRequest { Uids = batch.ToList(), SyncStatus = MerchantJobStatus.Pending };
                        insightScanRequests.Add(request);
                    }

                    merchantSyncInfo.SocialScan.InsightScan.Page1InsightScanRequests = insightScanRequests;
                    merchantSyncInfo.Page1SyncStatus                                 = MerchantSyncStatus.SyncSocialInsightRequestInitiated;
                }
                break;
            }

            case MerchantSyncStatus.SyncSocialInsightRequestInitiated:
            {
                var allRequested = merchantSyncInfo.SocialScan.InsightScan.Page1InsightScanRequests.All(_ => _.IsRequested);
                if (allRequested) merchantSyncInfo.Page1SyncStatus = MerchantSyncStatus.SyncSocialInsightRequestCompleted;

                break;
            }

            case MerchantSyncStatus.SyncSocialInsightRequestCompleted:
            case MerchantSyncStatus.SyncSocialInsightScanCompleted:
            case MerchantSyncStatus.DashboardDataReady:
            {
                var allScanned = merchantSyncInfo.SocialScan.InsightScan.Page1InsightScanRequests.All(_ => _.IsScanned);
                if (allScanned) merchantSyncInfo.Page1SyncStatus = MerchantSyncStatus.SyncSocialInsightScanCompleted;

                var allSynced =
                    merchantSyncInfo.SocialScan.InsightScan.Page1InsightScanRequests.All(_ => _.SyncStatus == MerchantJobStatus.Completed);
                if (allSynced) merchantSyncInfo.Page1SyncStatus = MerchantSyncStatus.DashboardDataReady;

                break;
            }

            default: return;
        }

        await _merchantSyncInfoRepo.UpdateAsync(merchantSyncInfo);

        // send notification to merchant
        await _emailManager.Send_SyncNotification(DashboardPageConsts.Page1, merchant, merchantSyncInfo.Page1SyncStatus);

        Debug.WriteLine($">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>> MERCHANT SYNC STATUS {merchant.Email} = {status}");
    }
}