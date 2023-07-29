using LookOn.Shared;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Application.Dtos;
using LookOn.Merchants;
using LookOn.MerchantSocialCommunity;

namespace LookOn.Controllers.Merchants
{
    [RemoteService]
    [Area("app")]
    [ControllerName("Merchant")]
    [Route("api/app/merchants")]
    public class MerchantController : AbpController, IMerchantsAppService
    {
        private readonly IMerchantsAppService _merchantsAppService;

        public MerchantController(IMerchantsAppService merchantsAppService)
        {
            _merchantsAppService = merchantsAppService;
        }

        [HttpGet]
        public Task<PagedResultDto<MerchantWithNavigationPropertiesDto>> GetListAsync(GetMerchantsInput input)
        {
            return _merchantsAppService.GetListAsync(input);
        }

        [HttpGet]
        [Route("with-navigation-properties/{id}")]
        public Task<MerchantWithNavigationPropertiesDto> GetWithNavigationPropertiesAsync(Guid id)
        {
            return _merchantsAppService.GetWithNavigationPropertiesAsync(id);
        }

        [HttpGet]
        [Route("{id}")]
        public virtual Task<MerchantDto> GetAsync(Guid id)
        {
            return _merchantsAppService.GetAsync(id);
        }

        [HttpGet]
        [Route("app-user-lookup")]
        public Task<PagedResultDto<LookupDto<Guid>>> GetAppUserLookupAsync(LookupRequestDto input)
        {
            return _merchantsAppService.GetAppUserLookupAsync(input);
        }

        [HttpGet]
        [Route("category-lookup")]
        public Task<PagedResultDto<LookupDto<Guid?>>> GetCategoryLookupAsync(LookupRequestDto input)
        {
            return _merchantsAppService.GetCategoryLookupAsync(input);
        }

        [HttpPost]
        public virtual Task<MerchantDto> CreateAsync(MerchantCreateDto input)
        {
            return _merchantsAppService.CreateAsync(input);
        }

        [HttpPut]
        [Route("{id}")]
        public virtual Task<MerchantDto> UpdateAsync(Guid id, MerchantUpdateDto input)
        {
            return _merchantsAppService.UpdateAsync(id, input);
        }

        [HttpPut]
        [Route("update-metric-configures")]
        public Task<MetricConfigsDto> UpdateMetricConfigures(Guid id, MetricConfigsDto input)
        {
            return _merchantsAppService.UpdateMetricConfigures(id, input);
        }

        [HttpPost]
        [Route("send-new-community-notification")]
        public Task SendNewCommunityNotification(Guid merchantId, string url, bool invalidCommunity, string communityName)
        {
            return _merchantsAppService.SendNewCommunityNotification(merchantId, url, invalidCommunity, communityName);
        }

        [HttpDelete]
        [Route("delete-community")]
        public Task DeleteCommunity(MerchantSocialCommunityRequest request)
        {
            return _merchantsAppService.DeleteCommunity(request);
        }

        [HttpDelete]
        [Route("{id}")]
        public virtual Task DeleteAsync(Guid id)
        {
            return _merchantsAppService.DeleteAsync(id);
        }
    }
}