using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Castle.Core.Logging;
using LookOn.Core.Extensions;
using LookOn.Core.Helpers;
using LookOn.Enums;
using LookOn.Exceptions;
using LookOn.Merchants;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.SettingManagement;

namespace LookOn.Web.Pages.MerchantConnects
{
    public class ConnectCommunityModalModel : LookOnPageModel
    {
        [BindProperty]            public Guid                      MerchantId          { get; set; }
        [BindProperty] [Required] public string                    SocialCommunityName { get; set; }
        [BindProperty] [Required] public string                    CommunityUrl         { get; set; }
        [BindProperty]            public string                    Description         { get; set; }
        private readonly                 IMerchantsAppService      _merchantsAppService;
        private readonly                 IMerchantExtendAppService _merchantExtendAppService;

        public ConnectCommunityModalModel(IMerchantsAppService merchantsAppService, IMerchantExtendAppService merchantExtendAppService)
        {
            _merchantsAppService      = merchantsAppService;
            _merchantExtendAppService = merchantExtendAppService;
        }

        public async Task OnGetAsync()
        {
            await Task.CompletedTask;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // validate Facebook Url is valid
            var isFacebookUrlValid = FacebookHelper.IsValidUrl(CommunityUrl.Trim());
            if (!isFacebookUrlValid) throw new UserFriendlyException(L["ConnectFacebook.WarningMessage.FacebookUrlInValid"]);
            
            var merchant = await _merchantExtendAppService.GetCurrentMerchantAsync();
            if (merchant != null)
            {
                var invalidCommunity = false;
                var facebookId       = await FacebookHelper.GetFacebookId(CommunityUrl.Trim());
                if (facebookId.IsNullOrSpace())
                {
                    facebookId = null;
                    await SlackHelper.Log("Add Community Page",
                                          $"{merchant.Name} ({merchant.Email}): Can not get Facebook Id from url: {CommunityUrl}");
                    invalidCommunity = true;

                }
                
                if (merchant.Communities.Where(_ => _.SocialCommunityId.IsNotNullOrEmpty()).Select(_ => _.SocialCommunityId).Contains(facebookId)
                 || merchant.Communities.Select(_ => _.SocialCommunityName.Trim().ToLower()).Contains(SocialCommunityName.Trim().ToLower()))
                {
                    throw new UserFriendlyException(L["ConnectCommunity.WarningMessage.CommunityUrlDuplicate"]);
                }
                
                merchant.Communities.Add(new MerchantSocialCommunityDto
                {
                    SocialCommunityName = SocialCommunityName.Trim(),
                    Url                 = CommunityUrl.Trim(),
                    Description         = Description,
                    SocialCommunityId   = facebookId,
                    CommunityType       = SocialCommunityType.FacebookPage,
                    VerificationStatus  = invalidCommunity ? SocialCommunityVerificationStatus.InvalidCommunity : SocialCommunityVerificationStatus.Pending
                });
                var merchantUpdate = ObjectMapper.Map<MerchantDto, MerchantUpdateDto>(merchant);

                await _merchantsAppService.UpdateAsync(merchant.Id, merchantUpdate);

                // send notification to admin in case the community page is adding
                await _merchantsAppService.SendNewCommunityNotification(merchant.Id, CommunityUrl.Trim(), invalidCommunity, SocialCommunityName.Trim());
            }

            return NoContent();
        }
    }
}