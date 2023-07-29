using LookOn.Shared;
using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace LookOn.MerchantStores
{
    public interface IMerchantStoresAppService : IApplicationService
    {
        Task<PagedResultDto<MerchantStoreWithNavigationPropertiesDto>> GetListAsync(GetMerchantStoresInput input);

        Task<MerchantStoreWithNavigationPropertiesDto> GetWithNavigationPropertiesAsync(Guid id);

        Task<MerchantStoreDto> GetAsync(Guid id);

        Task<PagedResultDto<LookupDto<Guid?>>> GetMerchantLookupAsync(LookupRequestDto input);

        Task<PagedResultDto<LookupDto<Guid?>>> GetPlatformLookupAsync(LookupRequestDto input);

        Task DeleteAsync(Guid id);

        Task<MerchantStoreDto> CreateAsync(MerchantStoreCreateDto input);

        Task<MerchantStoreDto> UpdateAsync(Guid id, MerchantStoreUpdateDto input);
    }
}