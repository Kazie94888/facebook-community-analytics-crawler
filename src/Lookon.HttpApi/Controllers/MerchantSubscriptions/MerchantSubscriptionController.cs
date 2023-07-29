using LookOn.Shared;
using System;
using System.Threading.Tasks;
using LookOn.HaravanWebhooks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Application.Dtos;
using LookOn.MerchantSubscriptions;
using LookOn.RequestResponses;

namespace LookOn.Controllers.MerchantSubscriptions
{
    [RemoteService]
    [Area("app")]
    [ControllerName("MerchantSubscription")]
    [Route("api/app/merchant-subscriptions")]
    public class MerchantSubscriptionController : AbpController, IMerchantSubscriptionsAppService
    {
        private readonly IMerchantSubscriptionsAppService _merchantSubscriptionsAppService;

        public MerchantSubscriptionController(IMerchantSubscriptionsAppService merchantSubscriptionsAppService)
        {
            _merchantSubscriptionsAppService = merchantSubscriptionsAppService;
        }

        [HttpGet]
        public Task<PagedResultDto<MerchantSubscriptionWithNavigationPropertiesDto>> GetListAsync(GetMerchantSubscriptionsInput input)
        {
            return _merchantSubscriptionsAppService.GetListAsync(input);
        }

        [HttpGet]
        [Route("with-navigation-properties/{id}")]
        public Task<MerchantSubscriptionWithNavigationPropertiesDto> GetWithNavigationPropertiesAsync(Guid id)
        {
            return _merchantSubscriptionsAppService.GetWithNavigationPropertiesAsync(id);
        }

        [HttpGet]
        [Route("{id}")]
        public virtual Task<MerchantSubscriptionDto> GetAsync(Guid id)
        {
            return _merchantSubscriptionsAppService.GetAsync(id);
        }

        [HttpGet]
        [Route("merchant-lookup")]
        public Task<PagedResultDto<LookupDto<Guid>>> GetMerchantLookupAsync(LookupRequestDto input)
        {
            return _merchantSubscriptionsAppService.GetMerchantLookupAsync(input);
        }

        [HttpPost]
        public virtual Task<MerchantSubscriptionDto> CreateAsync(MerchantSubscriptionCreateDto input)
        {
            return _merchantSubscriptionsAppService.CreateAsync(input);
        }

        [HttpPut]
        [Route("{id}")]
        public virtual Task<MerchantSubscriptionDto> UpdateAsync(Guid id, MerchantSubscriptionUpdateDto input)
        {
            return _merchantSubscriptionsAppService.UpdateAsync(id, input);
        }

        [HttpPost]
        [Route("update-subscription-status")]
        public Task UpdateSubscriptionStatus(UpdateSubscriptionStatusInput input)
        {
            return _merchantSubscriptionsAppService.UpdateSubscriptionStatus(input);
        }

        [HttpDelete]
        [Route("{id}")]
        public virtual Task DeleteAsync(Guid id)
        {
            return _merchantSubscriptionsAppService.DeleteAsync(id);
        }
        
        #region Proceed new subscription
        [HttpPost]
        [Route("set-subscription")]
        public Task SetSubscription(SetSubscriptionInput input)
        {
            return _merchantSubscriptionsAppService.SetSubscription(input);
        }
        
        [HttpPost]
        [Route("set-subscription-by-webhook")]
        public Task SetSubscriptionByWebhook(AppSubscriptionInput input)
        {
            return _merchantSubscriptionsAppService.SetSubscriptionByWebhook(input);
        }
        
        [HttpPost]
        [Route("cancel-subscription")]
        public Task CancelSubscription(AppSubscriptionInput input)
        {
            return _merchantSubscriptionsAppService.CancelSubscription(input);
        }

        [HttpGet]
        [Route("active-subscription")]
        public Task<MerchantSubscriptionDto> GetActiveSubscription(Guid merchantId)
        {
            return _merchantSubscriptionsAppService.GetActiveSubscription(merchantId);
        }

        #endregion
    }
}