using LookOn.Shared;
using System;
using System.Threading.Tasks;
using LookOn.MerchantSocialCommunity;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace LookOn.Merchants
{
    public interface IMerchantsAppService : IApplicationService
    {
        Task<PagedResultDto<MerchantWithNavigationPropertiesDto>> GetListAsync(GetMerchantsInput          input);
        Task<MerchantWithNavigationPropertiesDto>                 GetWithNavigationPropertiesAsync(Guid   id);
        Task<MerchantDto>                                         GetAsync(Guid                           id);
        Task<PagedResultDto<LookupDto<Guid>>>                     GetAppUserLookupAsync(LookupRequestDto  input);
        Task<PagedResultDto<LookupDto<Guid?>>>                    GetCategoryLookupAsync(LookupRequestDto input);
        Task                                                      DeleteAsync(Guid                        id);
        Task<MerchantDto>                                         CreateAsync(MerchantCreateDto           input);
        Task<MerchantDto>                                         UpdateAsync(Guid                        id,         MerchantUpdateDto input);
        Task<MetricConfigsDto>                                    UpdateMetricConfigures(Guid             id,         MetricConfigsDto  input);
        Task                                                      SendNewCommunityNotification(Guid    merchantId, string            url, bool invalidCommunity, string communityName);
        Task DeleteCommunity(MerchantSocialCommunityRequest request);
    }
}