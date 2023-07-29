using LookOn.Shared;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Application.Dtos;
using LookOn.MerchantStores;

namespace LookOn.Controllers.MerchantStores
{
    [RemoteService]
    [Area("app")]
    [ControllerName("MerchantStore")]
    [Route("api/app/merchant-stores")]

    public class MerchantStoreController : AbpController, IMerchantStoresAppService
    {
        private readonly IMerchantStoresAppService _merchantStoresAppService;

        public MerchantStoreController(IMerchantStoresAppService merchantStoresAppService)
        {
            _merchantStoresAppService = merchantStoresAppService;
        }

        [HttpGet]
        public Task<PagedResultDto<MerchantStoreWithNavigationPropertiesDto>> GetListAsync(GetMerchantStoresInput input)
        {
            return _merchantStoresAppService.GetListAsync(input);
        }

        [HttpGet]
        [Route("with-navigation-properties/{id}")]
        public Task<MerchantStoreWithNavigationPropertiesDto> GetWithNavigationPropertiesAsync(Guid id)
        {
            return _merchantStoresAppService.GetWithNavigationPropertiesAsync(id);
        }

        [HttpGet]
        [Route("{id}")]
        public virtual Task<MerchantStoreDto> GetAsync(Guid id)
        {
            return _merchantStoresAppService.GetAsync(id);
        }

        [HttpGet]
        [Route("merchant-lookup")]
        public Task<PagedResultDto<LookupDto<Guid?>>> GetMerchantLookupAsync(LookupRequestDto input)
        {
            return _merchantStoresAppService.GetMerchantLookupAsync(input);
        }

        [HttpGet]
        [Route("platform-lookup")]
        public Task<PagedResultDto<LookupDto<Guid?>>> GetPlatformLookupAsync(LookupRequestDto input)
        {
            return _merchantStoresAppService.GetPlatformLookupAsync(input);
        }

        [HttpPost]
        public virtual Task<MerchantStoreDto> CreateAsync(MerchantStoreCreateDto input)
        {
            return _merchantStoresAppService.CreateAsync(input);
        }

        [HttpPut]
        [Route("{id}")]
        public virtual Task<MerchantStoreDto> UpdateAsync(Guid id, MerchantStoreUpdateDto input)
        {
            return _merchantStoresAppService.UpdateAsync(id, input);
        }

        [HttpDelete]
        [Route("{id}")]
        public virtual Task DeleteAsync(Guid id)
        {
            return _merchantStoresAppService.DeleteAsync(id);
        }
    }
}