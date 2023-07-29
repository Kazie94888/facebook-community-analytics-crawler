using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LookOn.Merchants;
using LookOn.MerchantUsers;
using LookOn.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Volo.Abp.Application.Dtos;

namespace LookOn.Web.Pages.MerchantStaffs
{
    public class EditModalModel : LookOnPageModel
    {
        [HiddenInput]
        [BindProperty(SupportsGet = true)]
        public Guid Id { get; set; }

        [BindProperty]
        public MerchantUserUpdateDto MerchantUser { get; set; }

        public MerchantDto Merchant { get; set; }
        public List<SelectListItem> AppUserLookupListRequired { get; set; } = new List<SelectListItem>
        {
        };

        private readonly IMerchantUsersAppService _merchantUsersAppService;

        public EditModalModel(IMerchantUsersAppService merchantUsersAppService)
        {
            _merchantUsersAppService = merchantUsersAppService;
        }

        public async Task OnGetAsync()
        {
            var merchantUserWithNavigationPropertiesDto = await _merchantUsersAppService.GetWithNavigationPropertiesAsync(Id);
            MerchantUser = ObjectMapper.Map<MerchantUserDto, MerchantUserUpdateDto>(merchantUserWithNavigationPropertiesDto.MerchantUser);

            Merchant = merchantUserWithNavigationPropertiesDto.Merchant;
            AppUserLookupListRequired.AddRange((
                        await _merchantUsersAppService.GetAppUserLookupAsync(new LookupRequestDto
                        {
                            MaxResultCount = LimitedResultRequestDto.MaxMaxResultCount
                        })).Items.Select(t => new SelectListItem(t.DisplayName, t.Id.ToString())).ToList()
            );

        }

        public async Task<IActionResult> OnPostAsync()
        {
            await _merchantUsersAppService.UpdateAsync(Id, MerchantUser);
            return NoContent();
        }
    }
}