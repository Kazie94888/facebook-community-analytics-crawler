using System;
using System.Threading.Tasks;
using LookOn.Merchants;
using LookOn.MerchantSocialCommunity;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Mvc;

namespace LookOn.Controllers.MerchantSocialCommunities
{
    [RemoteService]
    [Area("app")]
    [ControllerName("MerchantSocialCommunities")]
    [Route("api/app/merchant-social-communities")]

    public class MerchantSocialCommunityController : AbpController, IMerchantSocialCommunityAppService
    {
        private readonly IMerchantSocialCommunityAppService _socialCommunityAppService;
        public MerchantSocialCommunityController(IMerchantSocialCommunityAppService socialCommunityAppService)
        {
            _socialCommunityAppService = socialCommunityAppService;
        }
        
        [HttpGet]
        public Task<PagedResultDto<MerchantSocialCommunityDto>> GetMerchantSocialCommunities(MerchantSocialCommunityFilterDto filterDto)
        {
            return _socialCommunityAppService.GetMerchantSocialCommunities(filterDto);
        }

        [HttpGet]
        [Route("get-merchant-social-community")]
        public Task<MerchantSocialCommunityDto> GetMerchantSocialCommunity(MerchantSocialCommunityRequest request)
        {
            return _socialCommunityAppService.GetMerchantSocialCommunity(request);
        }

        [HttpPut]
        public Task UpdateMerchantSocialCommunity(MerchantSocialCommunityDto merchantSocialCommunity)
        {
            return _socialCommunityAppService.UpdateMerchantSocialCommunity(merchantSocialCommunity);
        }
    }
}