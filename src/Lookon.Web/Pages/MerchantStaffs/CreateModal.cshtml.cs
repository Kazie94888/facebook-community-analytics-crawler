using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LookOn.Core.Extensions;
using LookOn.MerchantUsers;
using LookOn.Shared;
using LookOn.Users;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Volo.Abp;
using Volo.Abp.Application.Dtos;

namespace LookOn.Web.Pages.MerchantStaffs
{
    public class CreateModalModel : LookOnPageModel
    {
        [BindProperty]
        public MerchantUserCreateDto MerchantUser { get; set; }
        
        [BindProperty]
        public string EmailStaff { get; set; }

        public List<SelectListItem> AppUserLookupListRequired { get; set; } = new List<SelectListItem>
        {
        };

        private readonly IMerchantUsersAppService _merchantUsersAppService;
        private readonly IUserAppService          _userAppService;

        public CreateModalModel(IMerchantUsersAppService merchantUsersAppService, IUserAppService userAppService)
        {
            _merchantUsersAppService = merchantUsersAppService;
            _userAppService     = userAppService;
        }

        public async Task OnGetAsync()
        {
            MerchantUser = new MerchantUserCreateDto(){ IsActive = true};
            AppUserLookupListRequired.AddRange((
                                    await _merchantUsersAppService.GetAppUserLookupAsync(new LookupRequestDto
                                    {
                                        MaxResultCount = LimitedResultRequestDto.MaxMaxResultCount
                                    })).Items.Select(t => new SelectListItem(t.DisplayName, t.Id.ToString())).ToList()
                        );

            if (IsMerchantRole())
            {
                var currentMerchant = await CurrentMerchant();
                if (currentMerchant != null)
                {
                    MerchantUser.MerchantId = currentMerchant.Id;
                }
            }
            await Task.CompletedTask;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!EmailStaff.ValidEmail())
            {
                throw new UserFriendlyException(L["EmailNotValid"]);
            }
            if (IsMerchantRole() && EmailStaff.IsNotNullOrEmpty())
            {
                await _merchantUsersAppService.AddUserAsync(MerchantUser.MerchantId, EmailStaff.Trim());
            }

            if (IsAdminRole())
            {
                await _merchantUsersAppService.CreateAsync(MerchantUser);
            }
            
            return NoContent();
        }
    }
}