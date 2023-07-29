using LookOn.Shared;
using LookOn.Merchants;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using LookOn.MerchantSocialCommunity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Volo.Abp.Application.Dtos;
using LookOn.MerchantSyncInfos;

namespace LookOn.Web.Pages.MerchantSocialCommunities
{
    public class EditSocialCommunityModal : LookOnPageModel
    {
        [HiddenInput] [BindProperty(SupportsGet = true)] public Guid   MerchantSyncInfoId  { get; set; }
        [HiddenInput] [BindProperty(SupportsGet = true)] public Guid   MerchantId          { get; set; }
        [HiddenInput] [BindProperty(SupportsGet = true)] public string SocialCommunityName { get; set; }

        [BindProperty] public MerchantSocialCommunityDto MerchantSocialCommunity { get; set; }

        private readonly IMerchantSocialCommunityAppService _merchantSocialCommunityAppService;

        public EditSocialCommunityModal(IMerchantSocialCommunityAppService merchantSocialCommunityAppService)
        {
            _merchantSocialCommunityAppService = merchantSocialCommunityAppService;
        }

        public async Task OnGetAsync()
        {
            MerchantSocialCommunity = await _merchantSocialCommunityAppService.GetMerchantSocialCommunity(new MerchantSocialCommunityRequest()
            {
                MerchantId = MerchantId,
                CommunityName = SocialCommunityName
            });
        }

        public async Task<IActionResult> OnPostAsync()
        {
            await _merchantSocialCommunityAppService.UpdateMerchantSocialCommunity(MerchantSocialCommunity);
            return NoContent();
        }
    }
}