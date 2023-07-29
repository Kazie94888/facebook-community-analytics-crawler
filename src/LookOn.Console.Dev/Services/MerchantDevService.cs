using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using LookOn.Core.Extensions;
using LookOn.Enums;
using LookOn.Integrations.Datalytis.Components.Repositories.Interfaces;
using LookOn.Integrations.Datalytis.Configs;
using LookOn.Integrations.Datalytis.Models.Entities;
using LookOn.Integrations.Haravan;
using LookOn.Integrations.Haravan.Components.Repositories.Interfaces;
using LookOn.Integrations.Haravan.Configs;
using LookOn.Integrations.Haravan.Models.RawModels;
using LookOn.Merchants;
using LookOn.MerchantStores;
using LookOn.MerchantSubscriptions;
using LookOn.MerchantSyncInfos;
using LookOn.MerchantUsers;
using LookOn.UserInfos;
using LookOn.Users;
using Volo.Abp.Domain.Repositories;

namespace LookOn.Console.Dev.Services;

public class MerchantDevService : DevService
{
    private MerchantSubscriptionManager _merchantSubscriptionManager;

    // repo
    private          IMerchantRepository             _merchantRepository;
    private          IMerchantSyncInfoRepository     _merchantSyncInfoRepository;
    private          IMerchantUserRepository         _merchantUserRepository;
    private          IMerchantStoreRepository        _merchantStoreRepository;
    private          IMerchantSubscriptionRepository _merchantSubscriptionRepository;
    private          IRepository<AppUser, Guid>      _userRepository;
    private          IHaravanOrderRepository         _haravanOrderRepository;
    private          IHaravanCustomerRepository      _haravanCustomerRepository;
    private          IHaravanStoreRepository         _haravanStoreRepository;
    private readonly IRepository<HRVOrderRaw>        _haravanOrderRawRepo;
    private readonly IUserInfoRepository             _userInfoRepository;
    private readonly MerchantSubscriptionManager     _subscriptionManager;
    private readonly IDatalytisUserRepository        _datalytisUserRepository;
    private readonly HaravanSyncManager              _haravanSyncManager;

    public MerchantDevService(IMerchantRepository             merchantRepository,
                              MerchantSubscriptionManager     merchantSubscriptionManager,
                              IMerchantSyncInfoRepository     merchantSyncInfoRepository,
                              IMerchantStoreRepository        merchantStoreRepository,
                              IMerchantUserRepository         merchantUserRepository,
                              IMerchantSubscriptionRepository merchantSubscriptionRepository,
                              IRepository<AppUser, Guid>      userRepository,
                              IHaravanOrderRepository         haravanOrderRepository,
                              IHaravanCustomerRepository      haravanCustomerRepository,
                              IHaravanStoreRepository         haravanStoreRepository,
                              IRepository<HRVOrderRaw>        haravanOrderRawRepo,
                              IUserInfoRepository             userInfoRepository,
                              MerchantSubscriptionManager     subscriptionManager,
                              IDatalytisUserRepository        datalytisUserRepository,
                              HaravanSyncManager              haravanSyncManager)
    {
        _merchantRepository             = merchantRepository;
        _merchantSubscriptionManager    = merchantSubscriptionManager;
        _merchantSyncInfoRepository     = merchantSyncInfoRepository;
        _merchantStoreRepository        = merchantStoreRepository;
        _merchantUserRepository         = merchantUserRepository;
        _merchantSubscriptionRepository = merchantSubscriptionRepository;
        _userRepository                 = userRepository;
        _haravanOrderRepository         = haravanOrderRepository;
        _haravanCustomerRepository      = haravanCustomerRepository;
        _haravanStoreRepository         = haravanStoreRepository;
        _haravanOrderRawRepo            = haravanOrderRawRepo;
        _userInfoRepository             = userInfoRepository;
        _subscriptionManager            = subscriptionManager;
        _datalytisUserRepository        = datalytisUserRepository;
        _haravanSyncManager         = haravanSyncManager;
    }

    public async Task InitMerchantData()
    {
        await InitMerchantSubscriptions();

        // await InitMerchantSyncInfo();
        await InitMerchantEmail();
    }

    public async Task InitMerchant()
    {
        var merchants = await _merchantRepository.GetListAsync(x => x.GetSocialCommunityIds() != null || x.GetSocialCommunityIds().Any());
        foreach (var merchant in merchants)
        {
            merchant.Communities = merchant.GetSocialCommunityIds()
                                           .Select(x =>
                                            {
                                                var community = new MerchantSocialCommunity
                                                    { Url = $"https://facebook.com/{x}", SocialCommunityId = x };
                                                return community;
                                            })
                                           .ToList();
        }

        await _merchantRepository.UpdateManyAsync(merchants);
    }

    public async Task InitMerchantSubscriptions()
    {
        var merchants = await _merchantRepository.GetListAsync();
        foreach (var merchant in merchants)
        {
            await _merchantSubscriptionManager.SetSubscription(merchant.Id, SubscriptionType.Starter, DateTime.UtcNow.AddDays(-1));
        }
    }

    // public async Task InitMerchantSyncInfo()
    // {
    //     foreach (var merchant in await _merchantRepository.GetListAsync())
    //     {
    //         var merchantSyncInfo = await _merchantSyncInfoRepository.FirstOrDefaultAsync(x => x.MerchantId == merchant.Id);
    //         if (merchantSyncInfo == null)
    //         {
    //             await _merchantSyncInfoRepository.InsertAsync(new MerchantSyncInfo()
    //             {
    //                 MerchantId    = merchant.Id,
    //                 MerchantEmail = merchant.Email,
    //                 Page1SyncStatus    = MerchantSyncStatus.Pending,
    //                 Page2SyncStatus    = MerchantSyncStatus.Pending,
    //                 SocialScan = new MerchantSocialScan
    //                 {
    //                     UserScans = merchant.GetSocialCommunityIds().Select(socialCommunityId => new MerchantUserScan
    //                                          {
    //                                              SocialCommunityId  = socialCommunityId,
    //                                              CommunityType      = SocialCommunityType.FacebookPage,
    //                                              VerificationStatus = SocialCommunityVerificationStatus.Approved
    //                                          })
    //                                         .ToList(),
    //                 },
    //                 EcomScan = new MerchantEcomScan { OrderSyncIntervalInDays = HaravanGlobalConfig.SyncOrderIntervalDays }
    //             });
    //         }
    //     }
    // }

    public async Task InitMerchantEmail()
    {
        // merchants
        var merchants = await _merchantRepository.GetListAsync();

        // 1. merchant user
        var merchantUsers = await _merchantUserRepository.GetListAsync();
        foreach (var item in merchantUsers)
        {
            var merchant = merchants.FirstOrDefault(_ => _.Id == item.MerchantId);
            if (merchant is not null)
            {
                item.MerchantEmail = merchant.Email;
            }
        }

        if (merchantUsers.IsNotNullOrEmpty()) await _merchantUserRepository.UpdateManyAsync(merchantUsers);

        // 2. merchant sync
        var syncs = await _merchantSyncInfoRepository.GetListAsync();
        foreach (var item in syncs)
        {
            var merchant = merchants.FirstOrDefault(_ => _.Id == item.MerchantId);
            if (merchant is not null)
            {
                item.MerchantEmail = merchant.Email;
            }
        }

        if (syncs.IsNotNullOrEmpty()) await _merchantSyncInfoRepository.UpdateManyAsync(syncs);

        // 3. merchant store
        var stores = await _merchantStoreRepository.GetListAsync();
        foreach (var item in stores)
        {
            var merchant = merchants.FirstOrDefault(_ => _.Id == item.MerchantId);
            if (merchant is not null)
            {
                item.MerchantEmail = merchant.Email;
            }
        }

        if (stores.IsNotNullOrEmpty()) await _merchantStoreRepository.UpdateManyAsync(stores);

        // 4. merchant sub
        var subs = await _merchantSubscriptionRepository.GetListAsync();
        foreach (var item in subs)
        {
            var merchant = merchants.FirstOrDefault(_ => _.Id == item.MerchantId);
            if (merchant is not null)
            {
                item.MerchantEmail = merchant.Email;
            }
        }

        if (subs.IsNotNullOrEmpty()) await _merchantSubscriptionRepository.UpdateManyAsync(subs);
    }

    public async Task RemoveMerchant()
    {
        // 1. clear merchant
        var merchants = await _merchantRepository.GetListAsync(_ => _.Email == "thi.hoang.vcr@gmail.com");
        foreach (var merchant in merchants.Where(merchant => merchant is not null))
        {
            await _merchantRepository.HardDeleteAsync(merchant);

            // 2. clear merchant sub
            var subs = await _merchantSubscriptionRepository.GetListAsync();
            foreach (var item in subs.Where(_ => _.MerchantId == merchant.Id))
            {
                await _merchantSubscriptionRepository.HardDeleteAsync(item);
            }

            // 3. clear merchant user
            var merchantUsers = await _merchantUserRepository.GetListAsync();
            foreach (var item in merchantUsers.Where(_ => _.MerchantId == merchant.Id))
            {
                await _merchantUserRepository.DeleteAsync(item);
            }

            // 4. clear merchant store
            var stores = await _merchantStoreRepository.GetListAsync();
            foreach (var item in stores.Where(_ => _.MerchantId == merchant.Id))
            {
                await _merchantStoreRepository.HardDeleteAsync(item);
            }

            // 5. clear merchant sync
            var syncs = await _merchantSyncInfoRepository.GetListAsync();
            foreach (var item in syncs.Where(_ => _.MerchantId == merchant.Id))
            {
                await _merchantSyncInfoRepository.HardDeleteAsync(item);
            }

            // 6. clear account user 
            var user = await _userRepository.GetAsync(_ => _.Id == merchant.OwnerAppUserId);
            await _userRepository.HardDeleteAsync(user);

            var userInfo = await _userInfoRepository.GetAsync(_ => _.AppUserId == user.Id);
            await _userInfoRepository.HardDeleteAsync(userInfo);

            // 7. clear haravan orders
            var orders = await _haravanOrderRepository.GetListAsync(_ => _.MerchantId == merchant.Id);
            if (orders.IsNotNullOrEmpty())
            {
                foreach (var batch in orders.Partition(100))
                {
                    await _haravanOrderRepository.DeleteManyAsync(batch);
                }
            }

            // 8. clear haravan raw orders
            var rawOrders = await _haravanOrderRawRepo.GetListAsync(_ => _.MerchantId == merchant.Id);
            if (rawOrders.IsNotNullOrEmpty())
            {
                foreach (var batch in rawOrders.Partition(100))
                {
                    await _haravanOrderRawRepo.DeleteManyAsync(batch);
                }
            }

            // 9. clear haravan stores
            var haravanStores = await _haravanStoreRepository.GetListAsync(_ => _.MerchantId == merchant.Id);
            if (haravanStores.IsNotNullOrEmpty()) await _haravanStoreRepository.HardDeleteAsync(haravanStores);

            //10. clear haravan customers
            var haravanCustomers = await _haravanCustomerRepository.GetListAsync(_ => _.MerchantId == merchant.Id);
            if (haravanCustomers.IsNotNullOrEmpty())
            {
                foreach (var batch in haravanCustomers.Partition(100))
                {
                    await _haravanCustomerRepository.DeleteManyAsync(batch);
                }
            }
        }
    }

    public async Task DeleteUser(string email)
    {
        var user = await _userRepository.GetAsync(_ => _.Email == email);
        await _userRepository.HardDeleteAsync(user);
        
        var merchantUsers = await _merchantUserRepository.GetListAsync();
        foreach (var item in merchantUsers.Where(_ => _.AppUserId == user.Id))
        {
            await _merchantUserRepository.DeleteAsync(item);
        }

        var userInfo = await _userInfoRepository.GetAsync(_ => _.AppUserId == user.Id);
        await _userInfoRepository.HardDeleteAsync(userInfo);
    }

    public async Task CleanMerchantData()
    {
        var merchantUsers         = await _merchantUserRepository.GetListAsync();
        var merchantSubscriptions = await _merchantSubscriptionRepository.GetListAsync();
        var userInfos             = await _userInfoRepository.GetListAsync();
        var merchants             = await _merchantRepository.GetListAsync();
        var merchantIds           = merchants.Select(_ => _.Id).ToList();
        var users                 = await _userRepository.GetListAsync();
        var userIds               = users.Select(_ => _.Id).ToList();
        foreach (var item in merchantUsers.Where(item => !merchantIds.Contains(item.MerchantId) || !userIds.Contains(item.AppUserId)))
        {
            await _merchantUserRepository.DeleteAsync(item);
        }

        foreach (var item in merchantSubscriptions.Where(item => !merchantIds.Contains(item.MerchantId)))
        {
            await _merchantSubscriptionRepository.HardDeleteAsync(item);
        }

        foreach (var userInfo in userInfos.Where(userInfo => !userInfo.AppUserId.HasValue || !userIds.Contains(userInfo.AppUserId.Value)))
        {
            await _userInfoRepository.HardDeleteAsync(userInfo);
        }
    }

    public async Task CleanHaravanData()
    {
        var haravanOrders = await _haravanOrderRepository.GetListAsync();
        var merchants     = await _merchantRepository.GetListAsync();
        var merchantIds   = merchants.Select(_ => _.Id).ToList();
        var stores        = await _haravanStoreRepository.GetListAsync();
        var storeIds      = stores.Select(_ => _.Id).ToList();
        foreach (var item in haravanOrders.Where(item => !merchantIds.Contains(item.MerchantId.Value) || !storeIds.Contains(item.StoreId.Value)))
        {
            await _haravanOrderRepository.DeleteAsync(item);
        }
    }

    public async Task ClearMerchantUser()
    {
        var users = await _userRepository.GetListAsync();
        foreach (var user in users.Where(user => user.UserName != "admin"))
        {
            await _userRepository.HardDeleteAsync(user);
        }
    }

    public async Task TestData()
    {
        var merchant   = await _merchantRepository.GetAsync(_ => _.Email == "ctps123@gmail.com");
        var merchantId = merchant.Id;
        if (EnumerableExtensions.IsNullOrEmpty(merchant.GetSocialCommunityIds())) return;

        var activeSubscription = await _subscriptionManager.GetActiveSubscription(merchantId);
        if (activeSubscription is null) return;
        var subscriptionConfig = activeSubscription.SubscriptionConfig;

        var merchantSyncInfo = await _merchantSyncInfoRepository.GetAsync(_ => _.MerchantId == merchantId);
        var ecomCustomers    = await _haravanCustomerRepository.GetListAsync(_ => _.MerchantId == merchantId && _.Phone != null && _.Phone != "");
        ecomCustomers = ecomCustomers.DistinctBy(c => c.Phone).ToList();

        var orders    = await _haravanOrderRepository.GetListAsync(_ => _.MerchantId == merchantId);
        var rawOrders = await _haravanOrderRawRepo.GetListAsync(_ => _.MerchantId    == merchantId);

        var ecomCustomerPhones = ecomCustomers.Select(_ => _.Phone.ToInternationalPhoneNumberFromVN()).ToList();

        // 2.1. get all social users from merchant pages
        // 2.2. only select <subscriptionConfig.MaxSocialUser> users to do DEMOGRAPHIC
        // need to prepare data for all communities            
        var socialUsers   = new List<DatalytisUser>();
        var socialUserDic = new Dictionary<string, List<DatalytisUser>>();
        foreach (var socialCommunityId in merchant.GetSocialCommunityIds())
        {
            var users = await _datalytisUserRepository.GetListAsync(_ => _.Phone != null
                                                                && _.Phone != string.Empty
                                                                && _.Uid   != null
                                                                && _.Uid   != string.Empty
                                                                && _.MerchantFbPageIds.Contains(socialCommunityId));

            users = users.DistinctBy(u => u.Phone).ToList();
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
    }

    public async Task Clone()
    {
        var merchant   = await _merchantRepository.GetAsync(_ => _.Email == "ctps123@gmail.com");
        var merchantId = merchant.Id;
        if (EnumerableExtensions.IsNullOrEmpty(merchant.GetSocialCommunityIds())) return;

        var activeSubscription = await _subscriptionManager.GetActiveSubscription(merchantId);
        if (activeSubscription is null) return;
        var subscriptionConfig = activeSubscription.SubscriptionConfig;

        var merchantSyncInfo = await _merchantSyncInfoRepository.GetAsync(_ => _.MerchantId == merchantId);
        var ecomCustomers    = await _haravanCustomerRepository.GetListAsync(_ => _.MerchantId == merchantId && _.Phone != null && _.Phone != "");
                ecomCustomers = ecomCustomers.DistinctBy(c => c.Phone).ToList();

                var ecomCustomerPhones = ecomCustomers.Select(_ => _.Phone.ToInternationalPhoneNumberFromVN()).ToList();

                var haravanUsers = await _datalytisUserRepository.GetListAsync();
                var ecomUsers = haravanUsers.Where(_ => ecomCustomerPhones.Contains(_.Phone)).ToList();

                // 2.1. get all social users from merchant pages
                // 2.2. only select <subscriptionConfig.MaxSocialUser> users to do DEMOGRAPHIC
                // need to prepare data for all communities            
                var socialUsers   = new List<DatalytisUser>();
                var socialUserDic = new Dictionary<string, List<DatalytisUser>>();
                foreach (var socialCommunityId in merchant.GetSocialCommunityIds())
                {
                    var users = await _datalytisUserRepository.GetListAsync(_ => _.Phone != null
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
    }

    public async Task GetRawOrders()
    {
        var merchant   = await _merchantRepository.GetAsync(_ => _.Email == "ctps123@gmail.com");
        var merchantId = merchant.Id;
        await _haravanSyncManager.SyncRawOrders(merchantId);
        await _haravanSyncManager.SyncOrders(merchant.Id);
    }
}