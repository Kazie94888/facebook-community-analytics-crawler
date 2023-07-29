using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LookOn.Core.Extensions;
using LookOn.Enums;
using LookOn.Integrations.Haravan.Components.Repositories.Interfaces;
using LookOn.Integrations.Haravan.Models.Entities;
using LookOn.Integrations.Haravan.Models.RawModels;
using LookOn.MerchantStores;
using LookOn.MerchantSubscriptions;
using LookOn.MerchantSyncInfos;
using LookOn.MerchantUsers;
using LookOn.UserInfos;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Users;

namespace LookOn.Merchants;

public class MerchantManager : LookOnManager
{
    private readonly IMerchantRepository             _merchantRepo;
    private readonly IMerchantUserRepository         _merchantUserRepo;
    private readonly IMerchantStoreRepository        _merchantStoreRepo;
    private readonly IMerchantSyncInfoRepository     _merchantSyncInfoRepo;
    private readonly IMerchantSubscriptionRepository _merchantSubscriptionRepo;
    private readonly IHaravanStoreRepository         _haravanStoreRepo;

    private readonly MerchantSubscriptionManager _subscriptionManager;

    public MerchantManager(IMerchantRepository             merchantRepo,
                           IMerchantUserRepository         merchantUserRepo,
                           IMerchantStoreRepository        merchantStoreRepo,
                           IHaravanStoreRepository         haravanStoreRepo,
                           IMerchantSyncInfoRepository     merchantSyncInfoRepo,
                           IMerchantSubscriptionRepository merchantSubscriptionRepo,
                           MerchantSubscriptionManager     subscriptionManager)
    {
        _merchantRepo             = merchantRepo;
        _merchantUserRepo         = merchantUserRepo;
        _merchantStoreRepo        = merchantStoreRepo;
        _haravanStoreRepo         = haravanStoreRepo;
        _merchantSyncInfoRepo     = merchantSyncInfoRepo;
        _merchantSubscriptionRepo = merchantSubscriptionRepo;
        _subscriptionManager = subscriptionManager;
    }

    public async Task InitMerchant(string     ownerEmail,
                                   Guid       ownerAppUserId,
                                   HRVShopRaw shopInfo,
                                   string     accessToken,
                                   string     refreshToken)
    {
        if (ownerEmail.IsNullOrSpace()) return;
        ownerEmail = ownerEmail.Trim().ToLower();
        
        var existingMerchant = await _merchantRepo.FirstOrDefaultAsync(m => m.Email == ownerEmail);
        if (existingMerchant is not null) return;

        var merchant = new Merchant
        {
            Name           = shopInfo.Name,
            Email          = shopInfo.Email,
            Address        = shopInfo.Address1,
            Phone          = shopInfo.Phone,
            OwnerAppUserId = ownerAppUserId
        };
        await _merchantRepo.InsertAsync(merchant, true);
        await _merchantUserRepo.InsertAsync(new MerchantUser { AppUserId = ownerAppUserId, IsActive = true, MerchantId = merchant.Id }, true);

        // 1.1. Merchant Store
        var merchantStore = await _merchantStoreRepo.FirstOrDefaultAsync(x => x.MerchantId == merchant.Id);
        if (merchantStore == null)
        {
            merchantStore = new MerchantStore
            {
                MerchantId    = merchant.Id,
                MerchantEmail = shopInfo.Email,
                Name          = shopInfo.Name,
                Code          = shopInfo.Id.ToString(),
                Active        = false
            };
            await _merchantStoreRepo.InsertAsync(merchantStore, true);
        }

        // 1.2. Haravan store
        var haravanStore = await _haravanStoreRepo.FirstOrDefaultAsync(x => x.Email == ownerEmail);
        if (haravanStore == null)
        {
            haravanStore = new HaravanStore
            {
                MerchantId      = merchant.Id,
                MerchantStoreId = merchantStore.Id,
                StoreName       = merchant.Name,
                Address         = merchant.Address ?? string.Empty,
                Email           = merchant.Email,
                Phone           = merchant.Phone,
                AppUserId       = ownerAppUserId,
                Token           = new HRVTokenRaw() { AccessToken = accessToken, RefreshToken = refreshToken }
            };
            await _haravanStoreRepo.InsertAsync(haravanStore);
        }
        else
        {
            haravanStore.Token = new HRVTokenRaw() { AccessToken = accessToken, RefreshToken = refreshToken };
            await _haravanStoreRepo.UpdateAsync(haravanStore);
        }

        // 2. init merchant sync info
        var syncInfo = new MerchantSyncInfo { MerchantId = merchant.Id, MerchantEmail = ownerEmail, };
        await _merchantSyncInfoRepo.InsertAsync(syncInfo);

        // 3. init merchant subscription
        var subscription = new MerchantSubscription
        {
            MerchantId = merchant.Id,
            SubscriptionType = SubscriptionType.Trial,
            SubscriptionStatus = SubscriptionStatus.Active,
            SubscriptionConfig = _subscriptionManager.InitSubscriptionConfig(SubscriptionType.Starter),
            StartDateTime = DateTime.UtcNow,
            EndDateTime = DateTime.UtcNow.AddDays(7)
        };
        await _merchantSubscriptionRepo.InsertAsync(subscription);
    }

    public async Task<List<Merchant>> GetMerchants()
    {
        return await _merchantRepo.GetListAsync();
    }

    public async Task<Merchant> GetMerchant(Guid merchantId)
    {
        return await _merchantRepo.GetAsync(merchantId);
    }
}