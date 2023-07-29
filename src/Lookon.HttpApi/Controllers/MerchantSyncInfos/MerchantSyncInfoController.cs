using LookOn.Shared;
using System;
using System.Threading.Tasks;
using LookOn.Enums;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Application.Dtos;
using LookOn.MerchantSyncInfos;

namespace LookOn.Controllers.MerchantSyncInfos
{
    [RemoteService]
    [Area("app")]
    [ControllerName("MerchantSyncInfo")]
    [Route("api/app/merchant-sync-infos")]
    public class MerchantSyncInfoController : AbpController, IMerchantSyncInfosAppService
    {
        private readonly IMerchantSyncInfosAppService _merchantSyncInfosAppService;

        public MerchantSyncInfoController(IMerchantSyncInfosAppService merchantSyncInfosAppService)
        {
            _merchantSyncInfosAppService = merchantSyncInfosAppService;
        }

        [HttpGet]
        public Task<PagedResultDto<MerchantSyncInfoWithNavigationPropertiesDto>> GetListAsync(GetMerchantSyncInfosInput input)
        {
            return _merchantSyncInfosAppService.GetListAsync(input);
        }

        [HttpGet]
        [Route("with-navigation-properties/{id}")]
        public Task<MerchantSyncInfoWithNavigationPropertiesDto> GetWithNavigationPropertiesAsync(Guid id)
        {
            return _merchantSyncInfosAppService.GetWithNavigationPropertiesAsync(id);
        }

        [HttpGet]
        [Route("{id}")]
        public virtual Task<MerchantSyncInfoDto> GetAsync(Guid id)
        {
            return _merchantSyncInfosAppService.GetAsync(id);
        }

        [HttpGet]
        [Route("get-by-merchantId/{merchantId}")]
        public Task<MerchantSyncInfoDto> GetByMerchantIdAsync(Guid merchantId)
        {
            return _merchantSyncInfosAppService.GetByMerchantIdAsync(merchantId);
        }

        [HttpGet]
        [Route("merchant/{merchantId}/status")]
        public Task<MerchantSyncStatus> GetMerchantSyncStatus(Guid merchantId)
        {
            return _merchantSyncInfosAppService.GetMerchantSyncStatus(merchantId);
        }

        [HttpGet]
        [Route("merchant-lookup")]
        public Task<PagedResultDto<LookupDto<Guid?>>> GetMerchantLookupAsync(LookupRequestDto input)
        {
            return _merchantSyncInfosAppService.GetMerchantLookupAsync(input);
        }

        [HttpPost]
        public virtual Task<MerchantSyncInfoDto> CreateAsync(MerchantSyncInfoCreateDto input)
        {
            return _merchantSyncInfosAppService.CreateAsync(input);
        }

        [HttpPut]
        [Route("{id}")]
        public virtual Task<MerchantSyncInfoDto> UpdateAsync(Guid id, MerchantSyncInfoUpdateDto input)
        {
            return _merchantSyncInfosAppService.UpdateAsync(id, input);
        }

        [HttpDelete]
        [Route("{id}")]
        public virtual Task DeleteAsync(Guid id)
        {
            return _merchantSyncInfosAppService.DeleteAsync(id);
        }

        [HttpPost]
        [Route("{id}/force-sync-orders")]
        public virtual Task ForceSyncOrders(Guid id)
        {
            return _merchantSyncInfosAppService.ForceSyncOrders(id);
        }

        [HttpGet]
        [Route("get-merchant-social-sync-infos")]
        public Task<PagedResultDto<MerchantSocialUserSyncInfo>> GetMerchantSocialSyncInfos()
        {
            return _merchantSyncInfosAppService.GetMerchantSocialSyncInfos();
        }

        [HttpGet]
        [Route("get-merchant-social-sync-info")]
        public Task<MerchantSocialUserSyncInfo> GetMerchantSocialSyncInfo(Guid merchantSyncInfoId, string socialCommunityId)
        {
            return _merchantSyncInfosAppService.GetMerchantSocialSyncInfo(merchantSyncInfoId, socialCommunityId);
        }

        [HttpPost]
        [Route("update-merchant-user-sync-info-status")]
        public Task UpdateMerchantUserScanStatus(MerchantSocialUserSyncInfo merchantSocialUserSyncInfo)
        {
            return _merchantSyncInfosAppService.UpdateMerchantUserScanStatus(merchantSocialUserSyncInfo);
        }
    }
}