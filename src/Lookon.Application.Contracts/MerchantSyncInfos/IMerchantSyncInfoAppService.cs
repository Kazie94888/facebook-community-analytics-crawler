using LookOn.Shared;
using System;
using System.Threading.Tasks;
using LookOn.Enums;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace LookOn.MerchantSyncInfos
{
    public interface IMerchantSyncInfosAppService : IApplicationService
    {
        Task<PagedResultDto<MerchantSyncInfoWithNavigationPropertiesDto>> GetListAsync(GetMerchantSyncInfosInput input);
        Task<MerchantSyncInfoWithNavigationPropertiesDto> GetWithNavigationPropertiesAsync(Guid id);
        Task<MerchantSyncInfoDto> GetAsync(Guid id);
        Task<MerchantSyncInfoDto> GetByMerchantIdAsync(Guid merchantId);
         Task<MerchantSyncStatus> GetMerchantSyncStatus(Guid merchantId);
        // Task<MerchantSyncStatus> GetPage1MerchantSyncStatus(Guid merchantId);
        // Task<MerchantSyncStatus> GetPage2MerchantSyncStatus(Guid merchantId);
        Task<PagedResultDto<LookupDto<Guid?>>> GetMerchantLookupAsync(LookupRequestDto input);
        Task DeleteAsync(Guid id);
        Task<MerchantSyncInfoDto> CreateAsync(MerchantSyncInfoCreateDto input);
        Task<MerchantSyncInfoDto> UpdateAsync(Guid id, MerchantSyncInfoUpdateDto input);
        Task<PagedResultDto<MerchantSocialUserSyncInfo>> GetMerchantSocialSyncInfos();
        Task<MerchantSocialUserSyncInfo> GetMerchantSocialSyncInfo(Guid merchantSyncInfoId, string socialCommunityId);
        Task UpdateMerchantUserScanStatus(MerchantSocialUserSyncInfo merchantSocialUserSyncInfo);
        Task ForceSyncOrders(Guid id);
    }
}