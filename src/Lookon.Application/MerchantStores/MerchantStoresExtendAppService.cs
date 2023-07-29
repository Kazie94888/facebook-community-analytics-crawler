using System;
using System.Threading.Tasks;
using LookOn.Permissions;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;

namespace LookOn.MerchantStores;

[RemoteService(IsEnabled = false)]
[Authorize(LookOnPermissions.MerchantStores.Default)]
public class MerchantStoresExtendAppService : LookOnAppService, IMerchantStoresExtendAppService
{
    private readonly IMerchantStoreRepository _merchantStoreRepo;

    public MerchantStoresExtendAppService(IMerchantStoreRepository merchantStoreRepo)
    {
        _merchantStoreRepo = merchantStoreRepo;
    }

    public async Task<MerchantStoreDto> GetByMerchantAsync(Guid merchantId)
    {
        var merchantStore = await _merchantStoreRepo.FirstOrDefaultAsync(x => x.MerchantId == merchantId);
        return ObjectMapper.Map<MerchantStore, MerchantStoreDto>(merchantStore);
    }

    public async Task<MerchantStoreDto> GetByStoreCodeAsync(string storeCode)
    {
        var merchantStore = await _merchantStoreRepo.FirstOrDefaultAsync(x => x.Code == storeCode);
        return ObjectMapper.Map<MerchantStore, MerchantStoreDto>(merchantStore);
    }
}