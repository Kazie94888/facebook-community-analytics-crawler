using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace LookOn.MerchantStores;

public interface IMerchantStoresExtendAppService : IApplicationService
{
    Task<MerchantStoreDto> GetByMerchantAsync(Guid merchantId);
    Task<MerchantStoreDto> GetByStoreCodeAsync(string storeCode);
}