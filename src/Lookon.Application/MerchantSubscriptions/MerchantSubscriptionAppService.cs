using LookOn.Shared;
using LookOn.Merchants;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
using LookOn.Enums;
using LookOn.HaravanWebhooks;
using LookOn.MerchantStores;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using LookOn.Permissions;
using LookOn.MerchantSubscriptions;
using LookOn.RequestResponses;

namespace LookOn.MerchantSubscriptions
{
    [RemoteService(IsEnabled = false)]
    [Authorize(LookOnPermissions.MerchantSubscriptions.Default)]
    public class MerchantSubscriptionsAppService : LookOnAppService, IMerchantSubscriptionsAppService
    {
        private readonly IMerchantSubscriptionRepository _merchantSubscriptionRepository;
        private readonly IRepository<Merchant, Guid>     _merchantRepository;
        private readonly MerchantSubscriptionManager     _subscriptionManager;
        private readonly IMerchantStoreRepository        _merchantStoreRepository;

        public MerchantSubscriptionsAppService(IMerchantSubscriptionRepository merchantSubscriptionRepository,
                                               IRepository<Merchant, Guid>     merchantRepository,
                                               MerchantSubscriptionManager     subscriptionManager,
                                               IMerchantStoreRepository        merchantStoreRepository)
        {
            _merchantSubscriptionRepository = merchantSubscriptionRepository;
            _merchantRepository             = merchantRepository;
            _subscriptionManager            = subscriptionManager;
            _merchantStoreRepository   = merchantStoreRepository;
        }

        public virtual async Task<PagedResultDto<MerchantSubscriptionWithNavigationPropertiesDto>> GetListAsync(GetMerchantSubscriptionsInput input)
        {
            var totalCount = await _merchantSubscriptionRepository.GetCountAsync(input.FilterText,
                                                                                 input.StartDateTimeMin,
                                                                                 input.StartDateTimeMax,
                                                                                 input.EndDateTimeMin,
                                                                                 input.EndDateTimeMax,
                                                                                 input.SubscriptionType,
                                                                                 input.SubscriptionStatus,
                                                                                 input.NotificationDateMin,
                                                                                 input.NotificationDateMax,
                                                                                 input.NotificationSent,
                                                                                 input.NotificationSentAtMin,
                                                                                 input.NotificationSentAtMax,
                                                                                 input.MerchantId);
            var items = await _merchantSubscriptionRepository.GetListWithNavigationPropertiesAsync(input.FilterText,
                                                                                                   input.StartDateTimeMin,
                                                                                                   input.StartDateTimeMax,
                                                                                                   input.EndDateTimeMin,
                                                                                                   input.EndDateTimeMax,
                                                                                                   input.SubscriptionType,
                                                                                                   input.SubscriptionStatus,
                                                                                                   input.NotificationDateMin,
                                                                                                   input.NotificationDateMax,
                                                                                                   input.NotificationSent,
                                                                                                   input.NotificationSentAtMin,
                                                                                                   input.NotificationSentAtMax,
                                                                                                   input.MerchantId,
                                                                                                   input.Sorting,
                                                                                                   input.MaxResultCount,
                                                                                                   input.SkipCount);

            return new PagedResultDto<MerchantSubscriptionWithNavigationPropertiesDto>
            {
                TotalCount = totalCount,
                Items = ObjectMapper
                   .Map<List<MerchantSubscriptionWithNavigationProperties>, List<MerchantSubscriptionWithNavigationPropertiesDto>>(items)
            };
        }

        public virtual async Task<MerchantSubscriptionWithNavigationPropertiesDto> GetWithNavigationPropertiesAsync(Guid id)
        {
            return ObjectMapper
               .Map<MerchantSubscriptionWithNavigationProperties,
                    MerchantSubscriptionWithNavigationPropertiesDto>(await _merchantSubscriptionRepository.GetWithNavigationPropertiesAsync(id));
        }

        public virtual async Task<MerchantSubscriptionDto> GetAsync(Guid id)
        {
            var entity = await _subscriptionManager.GetActiveSubscription(id);
            return ObjectMapper.Map<MerchantSubscription, MerchantSubscriptionDto>(entity);

            // return ObjectMapper.Map<MerchantSubscription, MerchantSubscriptionDto>(await _merchantSubscriptionRepository.GetAsync(id));
        }

        public virtual async Task<PagedResultDto<LookupDto<Guid>>> GetMerchantLookupAsync(LookupRequestDto input)
        {
            var query = (await _merchantRepository.GetQueryableAsync()).WhereIf(!string.IsNullOrWhiteSpace(input.Filter),
                                                                                x => x.Name != null && x.Name.Contains(input.Filter));

            var lookupData = await query.PageBy(input.SkipCount, input.MaxResultCount).ToDynamicListAsync<Merchant>();
            var totalCount = query.Count();
            return new PagedResultDto<LookupDto<Guid>> { TotalCount = totalCount, Items = ObjectMapper.Map<List<Merchant>, List<LookupDto<Guid>>>(lookupData) };
        }

        [Authorize(LookOnPermissions.MerchantSubscriptions.Delete)]
        public virtual async Task DeleteAsync(Guid id)
        {
            await _merchantSubscriptionRepository.DeleteAsync(id);
        }

        [Authorize(LookOnPermissions.MerchantSubscriptions.Create)]
        public virtual async Task<MerchantSubscriptionDto> CreateAsync(MerchantSubscriptionCreateDto input)
        {
            if (input.MerchantId == default)
            {
                throw new UserFriendlyException(Err["MerchantUser.RequiredField", L["Merchant"]]);
            }

            var merchant = await _merchantRepository.GetAsync(input.MerchantId);

            var merchantSubscription = ObjectMapper.Map<MerchantSubscriptionCreateDto, MerchantSubscription>(input);
            merchantSubscription.MerchantEmail      = merchant.Email;
            merchantSubscription.TenantId           = CurrentTenant.Id;
            merchantSubscription.SubscriptionConfig = _subscriptionManager.InitSubscriptionConfig(merchantSubscription.SubscriptionType);
            merchantSubscription                    = await _merchantSubscriptionRepository.InsertAsync(merchantSubscription, autoSave: true);
            return ObjectMapper.Map<MerchantSubscription, MerchantSubscriptionDto>(merchantSubscription);
        }

        [Authorize(LookOnPermissions.MerchantSubscriptions.Edit)]
        public virtual async Task<MerchantSubscriptionDto> UpdateAsync(Guid id, MerchantSubscriptionUpdateDto input)
        {
            if (input.MerchantId == default)
            {
                throw new UserFriendlyException(Err["MerchantUser.RequiredField", L["Merchant"]]);
            }

            var merchantSubscription = await _merchantSubscriptionRepository.GetAsync(id);

            // check if the merchant subscription is changed then update the subscription config
            var isSubTypeChanged = merchantSubscription.SubscriptionType != input.SubscriptionType;

            // map input to entity
            ObjectMapper.Map(input, merchantSubscription);
            if (isSubTypeChanged)
            {
                merchantSubscription.SubscriptionConfig = _subscriptionManager.InitSubscriptionConfig(merchantSubscription.SubscriptionType);
            }

            merchantSubscription = await _merchantSubscriptionRepository.UpdateAsync(merchantSubscription, autoSave: true);
            return ObjectMapper.Map<MerchantSubscription, MerchantSubscriptionDto>(merchantSubscription);
        }

        public Task SetSubscription(SetSubscriptionInput input)
        {
            return _subscriptionManager.SetSubscription(input.MerchantId, input.SubscriptionType, input.From);
        }

        public Task UpdateSubscriptionStatus(UpdateSubscriptionStatusInput input)
        {
            return _subscriptionManager.UpdateSubscriptionStatus(input.MerchantSubscriptionId,input.SubscriptionStatus);
        }

        [AllowAnonymous]
        public async Task SetSubscriptionByWebhook(AppSubscriptionInput input)
        {
            var merchantStore    = await _merchantStoreRepository.FirstOrDefaultAsync(x => x.Code == input.StoreCode);
            var subscriptionType = _subscriptionManager.GetSubscriptionType(input.PlanId);
            if (merchantStore is { MerchantId: { } })
            {
                if (input.CreatedAt != null) 
                    await _subscriptionManager.SetSubscription(merchantStore.MerchantId.Value, subscriptionType, input.CreatedAt.Value);
            }
        }

        [AllowAnonymous]
        public async Task CancelSubscription(AppSubscriptionInput input)
        {
            var merchantStore = await _merchantStoreRepository.FirstOrDefaultAsync(x => x.Code == input.StoreCode);
            if (merchantStore is { MerchantId: { } })
            {
                if (input.CreatedAt != null)
                {
                    var currentSubscription = await _subscriptionManager.GetActiveSubscription(merchantStore.MerchantId.Value);
                    if (currentSubscription != null)
                    {
                        currentSubscription.SubscriptionStatus = SubscriptionStatus.Canceled;
                        await _merchantSubscriptionRepository.UpdateAsync(currentSubscription);
                    }
                }
            }
        }

        public async Task<MerchantSubscriptionDto> GetActiveSubscription(Guid merchantId)
        {
            var entity = await _subscriptionManager.GetActiveSubscription(merchantId);
            return ObjectMapper.Map<MerchantSubscription, MerchantSubscriptionDto>(entity);
        }
    }
}