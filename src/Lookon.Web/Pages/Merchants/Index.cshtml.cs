using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;
using Volo.Abp.AspNetCore.Mvc.UI.Bootstrap.TagHelpers.Form;
using LookOn.Merchants;
using LookOn.Shared;

namespace LookOn.Web.Pages.Merchants
{
    public class IndexModel : AbpPageModel
    {
        public string NameFilter { get; set; }
        public string PhoneFilter { get; set; }
        public string AddressFilter { get; set; }
        public string EmailFilter { get; set; }
        public string FaxFilter { get; set; }
        [SelectItems(nameof(CategoryLookupList))]
        public Guid? CategoryIdFilter { get; set; }
        public List<SelectListItem> CategoryLookupList { get; set; } = new List<SelectListItem>
        {
            new SelectListItem(string.Empty, "")
        };

        private readonly IMerchantsAppService _merchantsAppService;

        public IndexModel(IMerchantsAppService merchantsAppService)
        {
            _merchantsAppService = merchantsAppService;
        }

        public async Task OnGetAsync()
        {
            CategoryLookupList.AddRange((
                    await _merchantsAppService.GetCategoryLookupAsync(new LookupRequestDto
                    {
                        MaxResultCount = LimitedResultRequestDto.MaxMaxResultCount
                    })).Items.Select(t => new SelectListItem(t.DisplayName, t.Id.ToString())).ToList()
            );

            await Task.CompletedTask;
        }
    }
}