// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Threading.Tasks;
// using LookOn.Core.Extensions;
// using LookOn.Dashboards.DashboardBases;
// using LookOn.Dashboards.Page3;
// using LookOn.Enums;
// using LookOn.Integrations.Datalytis.Components.Repositories.Interfaces;
// using LookOn.Integrations.Datalytis.Models.Entities;
// using LookOn.Integrations.Haravan.Components.Repositories.Interfaces;
// using LookOn.Merchants;
// using LookOn.MerchantSyncInfos;
//
// namespace LookOn.Dashboards;
//
// public class DashboardPage3DataSourceManager : LookOnManager
// {
//     private readonly IMerchantRepository                   _merchantRepo;
//     private readonly IHaravanCustomerRepository            _haravanCustomerRepo;
//     private readonly IDatalytisUserRepository              _datalytisUserRepo;
//     private readonly IDatalytisUserSocialInsightRepository _insightRepo;
//     private readonly IMerchantSyncInfoRepository           _merchantSyncInfoRepo;
//     
//     public DashboardPage3DataSourceManager(IMerchantRepository         merchantRepo, IHaravanCustomerRepository haravanCustomerRepo, IDatalytisUserRepository datalytisUserRepo, IDatalytisUserSocialInsightRepository insightRepo,
//                                            IMerchantSyncInfoRepository merchantSyncInfoRepo)
//     {
//         _merchantRepo             = merchantRepo;
//         _haravanCustomerRepo      = haravanCustomerRepo;
//         _datalytisUserRepo        = datalytisUserRepo;
//         _insightRepo              = insightRepo;
//         _merchantSyncInfoRepo     = merchantSyncInfoRepo;
//     }
//     
//     public async Task<Page3DataSource> GetPage3Data(Page3DataRequest request)
//     {
//         var social = await GetPage3DataSocial(request);
//         if (social is null) return null;
//
//         return new Page3DataSource {SocialData = social};
//     }
//
//     private async Task<Page3DataSourceSocial> GetPage3DataSocial(Page3DataRequest request)
//     {
//         var merchantSyncInfo = await _merchantSyncInfoRepo.GetAsync(_ => _.MerchantId == request.MerchantId);
//         if (merchantSyncInfo is null) return null;
//         
//         var page1SyncCompleted = merchantSyncInfo.Page1SyncStatus.IsIn(MerchantSyncStatus.SyncSocialInsightRequestCompleted,
//                                                                        MerchantSyncStatus.SyncSocialInsightScanCompleted,
//                                                                        MerchantSyncStatus.DashboardDataReady);
//         var page2SyncCompleted = merchantSyncInfo.Page2SyncStatus.IsIn(MerchantSyncStatus.SyncSocialInsightRequestCompleted,
//                                                                        MerchantSyncStatus.SyncSocialInsightScanCompleted,
//                                                                        MerchantSyncStatus.DashboardDataReady);
//
//         if (!page1SyncCompleted && !page2SyncCompleted) return null;
//         
//         var insightScan   = merchantSyncInfo.SocialScan.InsightScan;
//         var socialUserIds = insightScan.Page2UserNoPurchaseIds.Union(insightScan.Page1UserIds).Distinct().ToList();
//         var sampleUsers   = await _datalytisUserRepo.GetListAsync(_ => socialUserIds.Contains(_.Id));
//
//         var socialUids     = insightScan.Page2InsightNoPurchaseSocialUids.Union(insightScan.Page1InsightSocialUids).Distinct().ToList();
//         var sampleInsights = await _insightRepo.GetListAsync(_ => socialUids.Contains(_.Uid));
//         
//         var sampleUsersNoPurchased = await _datalytisUserRepo.GetListAsync(_ => insightScan.Page2UserNoPurchaseIds.Contains(_.Id));
//
//         sampleUsers            = CollectDatalytisUsers(sampleUsers,            request.Filter);
//         sampleUsersNoPurchased = CollectDatalytisUsers(sampleUsersNoPurchased, request.Filter);
//         var uids = sampleUsers.Select(user => user.Uid).Union(sampleUsersNoPurchased.Select(user => user.Uid)).Distinct().ToList();
//         sampleInsights = sampleInsights.Where(insight => uids.Contains(insight.Uid)).ToList();
//
//         return new Page3DataSourceSocial {SocialInsights = sampleInsights, SocialUsers = sampleUsers, SocialUsersNoPurchase = sampleUsersNoPurchased};
//     }
//     
//     private static List<DatalytisUser> CollectDatalytisUsers(List<DatalytisUser> datalytisUsers, Page3Filter filter)
//     {
//         if (filter != null)
//         {
//             // GenderTypes
//             if (filter.GenderTypes.IsNotNullOrEmpty())
//             {
//                 datalytisUsers = datalytisUsers.Where(user => filter.GenderTypes.Contains(user.Gender)).ToList();
//             }
//
//             // RelationshipStatus
//             if (filter.RelationshipStatus.IsNotNullOrEmpty())
//             {
//                 datalytisUsers = datalytisUsers.Where(user => filter.RelationshipStatus.Contains(user.RelationshipStatus)).ToList();
//             }
//
//             // AgeSegmentEnums
//             if (filter.AgeSegmentEnums.IsNotNullOrEmpty())
//             {
//                 var userGroups = datalytisUsers.Where(x => x.Birthday.HasValue).GroupBy(x => MetricGenericCalculator.MapAgeSegment(x.Birthday.Value)).ToList();
//                 userGroups     = userGroups.Where(users => filter.AgeSegmentEnums.Contains(users.Key)).ToList();
//                 datalytisUsers = userGroups.SelectMany(users => users).ToList();
//             }
//
//             // City
//             if (filter.Cities.IsNotNullOrEmpty())
//             {
//                 var userGroupsByCity = datalytisUsers.Where(user => user.City.IsNotNullOrWhiteSpace())
//                                                      .GroupBy(x => MetricGenericCalculator.MapLocationByProvince(x.City).Value)
//                                                      .ToList();
//                 userGroupsByCity = userGroupsByCity.Where(users => filter.Cities.Contains(users.Key)).ToList();
//                 datalytisUsers   = userGroupsByCity.SelectMany(users => users).ToList();
//             }
//
//             // TODOO: Car Owners
//             // TODOO: House Owner
//         }
//         
//         return datalytisUsers;
//     }
// }