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
    [ControllerName("MerchantStoreExtend")]
    [Route("api/app/merchant-stores-extend")]

    public class MerchantStoreExtendController : AbpController, IMerchantStoresExtendAppService
    {
        private readonly IMerchantStoresExtendAppService _merchantStoresExtendAppService;

        public MerchantStoreExtendController(IMerchantStoresExtendAppService merchantStoresExtendAppService)
        {
            _merchantStoresExtendAppService = merchantStoresExtendAppService;
        }

        [HttpGet]
        [Route("get-by-merchant/{merchantId}")]
        public Task<MerchantStoreDto> GetByMerchantAsync(Guid merchantId)
        {
            return _merchantStoresExtendAppService.GetByMerchantAsync(merchantId);
        }

        [HttpGet]
        [Route("get-by-merchant-store-code/{storeCode}")]
        public Task<MerchantStoreDto> GetByStoreCodeAsync(string storeCode)
        {
            return _merchantStoresExtendAppService.GetByStoreCodeAsync(storeCode);
        }
    }
}