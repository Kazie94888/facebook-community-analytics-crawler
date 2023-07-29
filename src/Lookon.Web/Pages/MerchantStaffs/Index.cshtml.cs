using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LookOn.MerchantUsers;
using LookOn.Shared;
using Microsoft.AspNetCore.Mvc.Rendering;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Mvc.UI.Bootstrap.TagHelpers.Form;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;

namespace LookOn.Web.Pages.MerchantStaffs
{
    public class IndexModel : LookOnPageModel
    {
        [SelectItems(nameof(IsActiveBoolFilterItems))]
        public string IsActiveFilter { get; set; }

        public List<SelectListItem> IsActiveBoolFilterItems { get; set; } =
            new List<SelectListItem>
            {
                new SelectListItem("", ""),
                new SelectListItem("Yes", "true"),
                new SelectListItem("No", "false"),
            };
        [SelectItems(nameof(AppUserLookupList))]
        public Guid? AppUserIdFilter { get; set; }
        
        public Guid MerchantIdFilter { get; set; }
        public List<SelectListItem> AppUserLookupList { get; set; } = new List<SelectListItem>
        {
            new SelectListItem(string.Empty, "")
        };

        private readonly IMerchantUsersAppService _merchantUsersAppService;

        public IndexModel(IMerchantUsersAppService merchantUsersAppService)
        {
            _merchantUsersAppService = merchantUsersAppService;
        }

        public async Task OnGetAsync()
        {
            if(IsMerchantRole())
            {
                var currentMerchant = await CurrentMerchant();
                if (currentMerchant != null)
                {
                    MerchantIdFilter = currentMerchant.Id;
                }
            }
            AppUserLookupList.AddRange((
                    await _merchantUsersAppService.GetAppUserLookupAsync(new LookupRequestDto
                    {
                        MaxResultCount = LimitedResultRequestDto.MaxMaxResultCount
                    })).Items.Select(t => new SelectListItem(t.DisplayName, t.Id.ToString())).ToList()
            );

            await Task.CompletedTask;
        }
    }
}