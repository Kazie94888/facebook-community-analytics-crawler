using LookOn.Shared;
using LookOn.Users;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using LookOn.Core.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Volo.Abp.Application.Dtos;
using LookOn.Merchants;

namespace LookOn.Web.Pages.Merchants
{
    public class EditModalModel : LookOnPageModel
    {
        [HiddenInput]
        [BindProperty(SupportsGet = true)]
        public Guid Id { get; set; }

        [BindProperty]
        public MerchantUpdateDto Merchant { get; set; }
        
        [BindProperty]
        public string FbPageIdsString { get; set; }

        public AppUserDto AppUser { get; set; }
        public List<SelectListItem> CategoryLookupList { get; set; } = new List<SelectListItem>
        {
            new SelectListItem(" — ", "")
        };

        private readonly IMerchantsAppService _merchantsAppService;

        public EditModalModel(IMerchantsAppService merchantsAppService)
        {
            _merchantsAppService = merchantsAppService;
        }

        public async Task OnGetAsync()
        {
            var merchantWithNavigationPropertiesDto = await _merchantsAppService.GetWithNavigationPropertiesAsync(Id);
            Merchant        = ObjectMapper.Map<MerchantDto, MerchantUpdateDto>(merchantWithNavigationPropertiesDto.Merchant);
            FbPageIdsString = Merchant.GetSocialCommunityIds().JoinAsString(";");
            AppUser         = merchantWithNavigationPropertiesDto.AppUser;
            CategoryLookupList.AddRange((
                        await _merchantsAppService.GetCategoryLookupAsync(new LookupRequestDto
                        {
                            MaxResultCount = LimitedResultRequestDto.MaxMaxResultCount
                        })).Items.Select(t => new SelectListItem(t.DisplayName, t.Id.ToString())).ToList()
            );

        }

        public async Task<IActionResult> OnPostAsync()
        {
            await _merchantsAppService.UpdateAsync(Id, Merchant);
            return NoContent();
        }
    }
}