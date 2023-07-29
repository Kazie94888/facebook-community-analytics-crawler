using System;
using System.Threading.Tasks;
using LookOn.Merchants;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace LookOn.MerchantSocialCommunity
{
    public interface IMerchantSocialCommunityAppService : IApplicationService
    {
        Task<PagedResultDto<MerchantSocialCommunityDto>> GetMerchantSocialCommunities(MerchantSocialCommunityFilterDto filterDto);
        Task<MerchantSocialCommunityDto>                 GetMerchantSocialCommunity(MerchantSocialCommunityRequest request);
        Task                                             UpdateMerchantSocialCommunity(MerchantSocialCommunityDto merchantSocialCommunity);
    }
}