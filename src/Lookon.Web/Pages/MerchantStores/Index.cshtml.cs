using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;
using Volo.Abp.AspNetCore.Mvc.UI.Bootstrap.TagHelpers.Form;
using LookOn.MerchantStores;
using LookOn.Shared;

namespace LookOn.Web.Pages.MerchantStores
{
    public class IndexModel : AbpPageModel
    {
        public string NameFilter { get; set; }
        public string CodeFilter { get; set; }
        [SelectItems(nameof(ActiveBoolFilterItems))]
        public string ActiveFilter { get; set; }

        public List<SelectListItem> ActiveBoolFilterItems { get; set; } =
            new List<SelectListItem>
            {
                new SelectListItem("", ""),
                new SelectListItem("Yes", "true"),
                new SelectListItem("No", "false"),
            };
        [SelectItems(nameof(PlatformLookupList))]
        public Guid? PlatformIdFilter { get; set; }
        public List<SelectListItem> PlatformLookupList { get; set; } = new List<SelectListItem>
        {
            new SelectListItem(string.Empty, "")
        };

        private readonly IMerchantStoresAppService _merchantStoresAppService;

        public IndexModel(IMerchantStoresAppService merchantStoresAppService)
        {
            _merchantStoresAppService = merchantStoresAppService;
        }

        public async Task OnGetAsync()
        {
            PlatformLookupList.AddRange((
                    await _merchantStoresAppService.GetPlatformLookupAsync(new LookupRequestDto
                    {
                        MaxResultCount = LimitedResultRequestDto.MaxMaxResultCount
                    })).Items.Select(t => new SelectListItem(t.DisplayName, t.Id.ToString())).ToList()
            );

            await Task.CompletedTask;
        }
    }
}