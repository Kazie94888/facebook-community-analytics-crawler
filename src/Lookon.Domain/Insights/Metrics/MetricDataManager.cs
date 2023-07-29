using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using LookOn.Consts;
using LookOn.Core.Extensions;
using LookOn.Emails;
using LookOn.Enums;
using LookOn.Integrations.Datalytis;
using LookOn.Integrations.Datalytis.Components.Repositories.Interfaces;
using LookOn.Integrations.Datalytis.Configs;
using LookOn.Integrations.Datalytis.Models.Entities;
using LookOn.Integrations.Haravan;
using LookOn.Integrations.Haravan.Components.Repositories.Interfaces;
using LookOn.Merchants;
using LookOn.MerchantSubscriptions;
using LookOn.MerchantSyncInfos;
using LookOn.SystemConfigs;

namespace LookOn.Insights.Metrics;

public class MetricDataManager : LookOnManager
{
    private readonly IMerchantRepository                   _merchantRepo;
    private readonly IHaravanCustomerRepository            _haravanCustomerRepo;
    private readonly IHaravanOrderRepository               _haravanOrderRepo;
    private readonly IDatalytisUserRepository              _datalytisUserRepo;
    private readonly IDatalytisUserSocialInsightRepository _insightRepo;
    private readonly IMerchantSyncInfoRepository           _merchantSyncInfoRepo;
    private readonly DatalytisManager                    _datalytisManager;

    // managers
    private readonly MerchantSubscriptionManager _subscriptionManager;
    private readonly SystemConfigManager         _systemConfigManager;
    private readonly EmailManager                _emailManager;

    public MetricDataManager(IMerchantRepository                   merchantRepo,
                             IHaravanCustomerRepository            haravanCustomerRepo,
                             IHaravanOrderRepository               haravanOrderRepo,
                             IDatalytisUserRepository              datalytisUserRepo,
                             IDatalytisUserSocialInsightRepository insightRepo,
                             IMerchantSyncInfoRepository           merchantSyncInfoRepo,
                             MerchantSubscriptionManager           subscriptionManager,
                             SystemConfigManager                   systemConfigManager,
                             EmailManager                          emailManager,
                             DatalytisManager                      datalytisManager)
    {
        _merchantRepo          = merchantRepo;
        _haravanCustomerRepo   = haravanCustomerRepo;
        _haravanOrderRepo      = haravanOrderRepo;
        _datalytisUserRepo     = datalytisUserRepo;
        _insightRepo           = insightRepo;
        _merchantSyncInfoRepo  = merchantSyncInfoRepo;
        _subscriptionManager   = subscriptionManager;
        _systemConfigManager   = systemConfigManager;
        _emailManager          = emailManager;
        _datalytisManager = datalytisManager;
    }

    public async Task PrepareMetricDataSource(Guid merchantId)
    {
        var merchant = await _merchantRepo.GetAsync(merchantId);
        if (EnumerableExtensions.IsNullOrEmpty(merchant.GetSocialCommunityIds())) return;

        var activeSubscription = await _subscriptionManager.GetActiveSubscription(merchantId);
        if (activeSubscription is null) return;
        var subscriptionConfig = activeSubscription.SubscriptionConfig;

        var merchantSyncInfo = await _merchantSyncInfoRepo.GetAsync(_ => _.MerchantId == merchantId);

        var status    = merchantSyncInfo.MerchantSyncStatus;
        var userScans = merchantSyncInfo.SocialScan.UserScans;

        switch (status)
        {
            case MerchantSyncStatus.Pending:
            {
                // 1.0. Scan ecom and wait for completion
                if (merchantSyncInfo.EcomScan.CleanOrderSyncStatus == MerchantJobStatus.Completed)
                {
                    merchantSyncInfo.MerchantSyncStatus = MerchantSyncStatus.SyncEcomOrderCompleted;
                }

                break;
            }

            case MerchantSyncStatus.SyncEcomOrderCompleted:
            {
                // 1.1. Request scan social users (all)
                var allRequested = userScans.IsNotNullOrEmpty() && userScans.All(scan => scan.IsRequestCompleted);
                if (allRequested)
                {
                    merchantSyncInfo.MerchantSyncStatus = MerchantSyncStatus.SyncSocialUserRequestCompleted;
                }
                break;
            }

            case MerchantSyncStatus.SyncSocialUserRequestCompleted:
            {
                // 1.2. wait SCAN
                var allScanned                                      = userScans.IsNotNullOrEmpty() && userScans.All(scan => scan.IsScanCompleted);
                if (allScanned) merchantSyncInfo.MerchantSyncStatus = MerchantSyncStatus.SyncSocialUserScanCompleted;
                break;
            }

            case MerchantSyncStatus.SyncSocialUserScanCompleted:
            {
                // 1.3. wait SYNC
                var allSynced = userScans.IsNotNullOrEmpty() && userScans.All(scan => scan.SyncStatus == MerchantJobStatus.Completed);
                if (allSynced) merchantSyncInfo.MerchantSyncStatus = MerchantSyncStatus.SyncSocialUserSyncCompleted;
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

                    // 2.3. intersect all ecom customers + valid social user (dob)
                    var insightCount = subscriptionConfig.MaxSocialUserInsight
                                     + ((ecomCustomers.Count - subscriptionConfig.MaxSocialUserInsight)
                                      * subscriptionConfig.MaxSocialUserInsightAddInPercent);
                    var insights = socialUsers.Take((int)insightCount).ToList();

                    // 2.4. send request insight to job
                    var insightUids = insights.Select(_ => _.Uid).ToList();
                    merchantSyncInfo.SocialScan.InsightScan = new MerchantInsightScan
                    {
                        InsightSocialUids = insightUids
                    };
                    var insightScanRequests = new List<MerchantInsightScanRequest>();
                    foreach (var batch in insightUids.Partition(DatalytisGlobalConfig.DefaultPageSize))
                    {
                        var request = new MerchantInsightScanRequest { Uids = batch.ToList(), SyncStatus = MerchantJobStatus.Pending };
                        insightScanRequests.Add(request);
                    }

                    merchantSyncInfo.SocialScan.InsightScan.InsightScanRequests = insightScanRequests;
                    merchantSyncInfo.MerchantSyncStatus                         = MerchantSyncStatus.SyncSocialInsightRequestInitiated;
                }

                break;
            }

            case MerchantSyncStatus.SyncSocialInsightRequestInitiated:
            {
                var allRequested = merchantSyncInfo.SocialScan.InsightScan.InsightScanRequests.All(_ => _.IsRequested);
                if (allRequested) merchantSyncInfo.MerchantSyncStatus = MerchantSyncStatus.SyncSocialInsightRequestCompleted;

                break;
            }

            case MerchantSyncStatus.SyncSocialInsightRequestCompleted:
            case MerchantSyncStatus.SyncSocialInsightScanCompleted:
            case MerchantSyncStatus.DashboardDataReady:
            {
                var allScanned = merchantSyncInfo.SocialScan.InsightScan.InsightScanRequests.All(_ => _.IsScanned);
                if (allScanned) merchantSyncInfo.MerchantSyncStatus = MerchantSyncStatus.SyncSocialInsightScanCompleted;

                var allSynced = merchantSyncInfo.SocialScan.InsightScan.InsightScanRequests.All(_ => _.SyncStatus == MerchantJobStatus.Completed);
                if (allSynced) merchantSyncInfo.MerchantSyncStatus = MerchantSyncStatus.DashboardDataReady;

                break;
            }

            default: return;
        }

        await _merchantSyncInfoRepo.UpdateAsync(merchantSyncInfo);

        // send notification to merchant
        await _emailManager.Send_SyncNotification(DashboardPageConsts.PageMetric, merchant, merchantSyncInfo.MerchantSyncStatus);

        Debug.WriteLine($">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>> MERCHANT SYNC STATUS {merchant.Email} = {status}");
    }
}