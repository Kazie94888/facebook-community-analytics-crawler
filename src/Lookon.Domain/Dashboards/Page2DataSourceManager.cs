// using System;
// using System.Collections.Generic;
// using System.Diagnostics;
// using System.Linq;
// using System.Threading.Tasks;
// using LookOn.Consts;
// using LookOn.Core.Extensions;
// using LookOn.Dashboards.Page2;
// using LookOn.Emails;
// using LookOn.Enums;
// using LookOn.Integrations.Datalytis.Components.Repositories.Interfaces;
// using LookOn.Integrations.Datalytis.Configs;
// using LookOn.Integrations.Datalytis.Models.Entities;
// using LookOn.Integrations.Haravan.Components.Repositories.Interfaces;
// using LookOn.Merchants;
// using LookOn.MerchantSubscriptions;
// using LookOn.MerchantSyncInfos;
// using LookOn.SystemConfigs;
//
// namespace LookOn.Dashboards;
//
// public class Page2DataSourceManager : LookOnManager
// {
//     private readonly IMerchantRepository                   _merchantRepo;
//     private readonly IHaravanCustomerRepository            _haravanCustomerRepo;
//     private readonly IDatalytisUserRepository              _datalytisUserRepo;
//     private readonly IDatalytisUserSocialInsightRepository _insightRepo;
//     private readonly IMerchantSyncInfoRepository           _merchantSyncInfoRepo;
//
//     // managers
//     private readonly MerchantSubscriptionManager _subscriptionManager;
//     private readonly SystemConfigManager         _systemConfigManager;
//     private readonly EmailManager                _emailManager;
//
//     public Page2DataSourceManager(IMerchantSyncInfoRepository           merchantSyncInfoRepo,
//                                   IMerchantRepository                   merchantRepo,
//                                   MerchantSubscriptionManager           subscriptionManager,
//                                   SystemConfigManager                   systemConfigManager,
//                                   IHaravanCustomerRepository            haravanCustomerRepo,
//                                   IDatalytisUserRepository              datalytisUserRepo,
//                                   IDatalytisUserSocialInsightRepository insightRepo,
//                                   EmailManager                          emailManager)
//     {
//         _merchantSyncInfoRepo = merchantSyncInfoRepo;
//         _merchantRepo         = merchantRepo;
//         _subscriptionManager  = subscriptionManager;
//         _systemConfigManager  = systemConfigManager;
//         _haravanCustomerRepo  = haravanCustomerRepo;
//         _datalytisUserRepo    = datalytisUserRepo;
//         _insightRepo          = insightRepo;
//         _emailManager         = emailManager;
//     }
//
//     public async Task<Page2DataSource> GetPage2Data(Page2DataRequest request)
//     {
//         var social = await GetPage2DataSocial(request);
//         if (social is null) return null;
//
//         return new Page2DataSource { SocialData = social };
//     }
//
//     private async Task<Page2DataSourceSocial> GetPage2DataSocial(Page2DataRequest request)
//     {
//         var merchantSyncInfo = await _merchantSyncInfoRepo.GetAsync(_ => _.MerchantId == request.MerchantId);
//         if (merchantSyncInfo is null
//          || !merchantSyncInfo.Page2SyncStatus.IsIn(MerchantSyncStatus.SyncSocialInsightRequestCompleted,
//                                                    MerchantSyncStatus.SyncSocialInsightScanCompleted,
//                                                    MerchantSyncStatus.DashboardDataReady))
//             return null;
//
//         var insightScan             = merchantSyncInfo.SocialScan.InsightScan;
//         var sampleUsers             = await _datalytisUserRepo.GetListAsync(_ => insightScan.Page2UserNoPurchaseIds.Contains(_.Id));
//         var sampleInsights          = await _insightRepo.GetListAsync(_ => insightScan.Page2InsightNoPurchaseSocialUids.Contains(_.Uid));
//         var sampleUsersHasPurchased = await _datalytisUserRepo.GetListAsync(_ => insightScan.Page2UserHasPurchaseIds.Contains(_.Id));
//
//         return new Page2DataSourceSocial { SocialUsersNoPurchase = sampleUsers, SocialInsights = sampleInsights, SocialUsersHasPurchase = sampleUsersHasPurchased };
//     }
//
//     public async Task PreparePage2Data(Guid merchantId)
//     {
//         var merchant = await _merchantRepo.GetAsync(merchantId);
//         if (merchant.GetSocialCommunityIds().IsNullOrEmpty()) return;
//
//         var activeSubscription = await _subscriptionManager.GetActiveSubscription(merchantId);
//         if (activeSubscription is null) return;
//         var subscriptionConfig = activeSubscription.SubscriptionConfig;
//
//         var merchantSyncInfo = await _merchantSyncInfoRepo.GetAsync(_ => _.MerchantId == merchantId);
//         var page1SyncStatus  = merchantSyncInfo.Page1SyncStatus;
//         var page2SyncStatus  = merchantSyncInfo.Page2SyncStatus;
//
//         var validPage1 = page1SyncStatus.IsIn(MerchantSyncStatus.SyncSocialUserSyncCompleted,
//                                               MerchantSyncStatus.SyncSocialInsightRequestInitiated,
//                                               MerchantSyncStatus.SyncSocialInsightRequestCompleted,
//                                               MerchantSyncStatus.SyncSocialInsightScanCompleted,
//                                               MerchantSyncStatus.DashboardDataReady);
//         var validPage2 = !page2SyncStatus.IsIn(MerchantSyncStatus.SyncSocialInsightRequestInitiated,
//                                                MerchantSyncStatus.SyncSocialInsightRequestCompleted,
//                                                MerchantSyncStatus.DashboardDataReady);
//         if (validPage1 && validPage2)
//         {
//             var systemConfig = await _systemConfigManager.GetOrInit();
//
//             var ecomCustomers = await _haravanCustomerRepo.GetListAsync(_ => _.MerchantId == merchantId && _.Phone != null && _.Phone != "");
//             ecomCustomers = ecomCustomers.DistinctBy(c => c.Phone).ToList();
//
//             // 2.1. get all social users from merchant pages
//             // 2.2. only select <subscriptionConfig.MaxSocialUser> users to do DEMOGRAPHIC
//             // need to prepare data for all communities            
//             var socialUsers = new List<DatalytisUser>();
//
//             var socialUserDic = new Dictionary<string, List<DatalytisUser>>();
//             foreach (var socialCommunityId in merchant.GetSocialCommunityIds())
//             {
//                 var users = await _datalytisUserRepo.GetListAsync(_ => _.Phone != null && _.Phone != string.Empty && _.MerchantFbPageIds.Contains(socialCommunityId));
//                 users = users.DistinctBy(u => u.Phone).Where(u => u.Birthday != null).ToList();
//                 socialUserDic.Add(socialCommunityId, users);
//             }
//
//             var totalUsers = socialUserDic.Values.SelectMany(_ => _).Count();
//             if (totalUsers is not 0)
//             {
//                 foreach (var dicEntry in socialUserDic)
//                 {
//                     var amount = (dicEntry.Value.Count / totalUsers) * totalUsers;
//                     socialUsers.AddRange(dicEntry.Value.Take(amount).ToList());
//                 }
//
//                 // 2.3. Get all social user that like page and do not have any order in Haravan system
//                 var ecomCustomersPhones = ecomCustomers.Select(_ => _.Phone.ToInternationalPhoneNumberFromVN()).ToList();
//                 var nonPurchasedUsers   = socialUsers.Where(user => !ecomCustomersPhones.Contains(user.Phone.ToInternationalPhoneNumberFromVN())).ToList();
//                 var purchasedUsers      = socialUsers.Where(user => ecomCustomersPhones.Contains(user.Phone.ToInternationalPhoneNumberFromVN())).ToList();
//
//                 var insightCount = subscriptionConfig.MaxSocialUserInsight + ((ecomCustomers.Count - subscriptionConfig.MaxSocialUserInsight) * subscriptionConfig.MaxSocialUserInsightAddInPercent);
//                 var insights = nonPurchasedUsers.Take((int) insightCount).ToList();
//             
//                 nonPurchasedUsers = nonPurchasedUsers.Take(subscriptionConfig.MaxSocialUser).ToList();
//                 purchasedUsers    = purchasedUsers.Take(subscriptionConfig.MaxSocialUser).ToList();
//                 
//                 var insightUids = insights.Select(_ => _.Uid).ToList();
//                 merchantSyncInfo.SocialScan.InsightScan ??= new MerchantInsightScan();
//
//                 merchantSyncInfo.SocialScan.InsightScan.Page2InsightNoPurchaseSocialUids = insightUids;
//                 merchantSyncInfo.SocialScan.InsightScan.Page2UserNoPurchaseIds           = nonPurchasedUsers.Select(user => user.Id).ToList();
//                 merchantSyncInfo.SocialScan.InsightScan.Page2UserHasPurchaseIds          = purchasedUsers.Select(user => user.Id).ToList();
//                 
//                 // 2.4. send request insight to job
//                 var insightScanRequests = insightUids.Partition(DatalytisGlobalConfig.DefaultPageSize).Select(batch => new MerchantInsightScanRequest { Uids = batch.ToList(), }).ToList();
//
//                 merchantSyncInfo.SocialScan.InsightScan.Page2InsightNoPurchaseScanRequests = insightScanRequests;
//                 merchantSyncInfo.Page2SyncStatus                                           = MerchantSyncStatus.SyncSocialInsightRequestInitiated;
//             }
//             
//         }
//         else
//         {
//             var noPurchaseScanRequests = merchantSyncInfo.SocialScan.InsightScan.Page2InsightNoPurchaseScanRequests;
//             switch (page2SyncStatus)
//             {
//                 case MerchantSyncStatus.SyncSocialInsightRequestInitiated:
//                 {
//                     var allRequested                                   = noPurchaseScanRequests.IsNotNullOrEmpty() && noPurchaseScanRequests.All(_ => _.IsRequested);
//                     if (allRequested) merchantSyncInfo.Page2SyncStatus = MerchantSyncStatus.SyncSocialInsightRequestCompleted;
//
//                     break;
//                 }
//
//                 case MerchantSyncStatus.SyncSocialInsightRequestCompleted:
//                 case MerchantSyncStatus.SyncSocialInsightScanCompleted:
//                 case MerchantSyncStatus.DashboardDataReady:
//                 {
//                     var allScanned                                   = noPurchaseScanRequests.IsNotNullOrEmpty() && noPurchaseScanRequests.All(_ => _.IsScanned);
//                     if (allScanned) merchantSyncInfo.Page2SyncStatus = MerchantSyncStatus.SyncSocialInsightScanCompleted;
//
//                     var allSynced = noPurchaseScanRequests.IsNotNullOrEmpty() && noPurchaseScanRequests.All(_ => _.SyncStatus == MerchantJobStatus.Completed);
//                     if (allSynced) merchantSyncInfo.Page2SyncStatus = MerchantSyncStatus.DashboardDataReady;
//
//                     break;
//                 }
//
//                 default: return;
//             }
//         }
//
//         await _merchantSyncInfoRepo.UpdateAsync(merchantSyncInfo);
//
//         // send notification to merchant
//         await _emailManager.Send_SyncNotification(DashboardPageConsts.Page2, merchant, merchantSyncInfo.Page2SyncStatus);
//
//         Debug.WriteLine($">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>> MERCHANT SYNC STATUS {merchant.Email} = Page 1 status: {merchantSyncInfo.Page1SyncStatus} - Page 2 status: {merchantSyncInfo.Page2SyncStatus}");
//     }
// }