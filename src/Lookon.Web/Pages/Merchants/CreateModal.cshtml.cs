using LookOn.Shared;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Volo.Abp.Application.Dtos;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using LookOn.Merchants;

namespace LookOn.Web.Pages.Merchants
{
    public class CreateModalModel : LookOnPageModel
    {
        [BindProperty]
        public MerchantCreateDto Merchant { get; set; }

        public List<SelectListItem> CategoryLookupList { get; set; } = new List<SelectListItem>
        {
            new SelectListItem(" — ", "")
        };

        private readonly IMerchantsAppService _merchantsAppService;

        public CreateModalModel(IMerchantsAppService merchantsAppService)
        {
            _merchantsAppService = merchantsAppService;
        }

        public async Task OnGetAsync()
        {
            Merchant = new MerchantCreateDto();
            CategoryLookupList.AddRange((
                                    await _merchantsAppService.GetCategoryLookupAsync(new LookupRequestDto
                                    {
                                        MaxResultCount = LimitedResultRequestDto.MaxMaxResultCount
                                    })).Items.Select(t => new SelectListItem(t.DisplayName, t.Id.ToString())).ToList()
                        );

            await Task.CompletedTask;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            await _merchantsAppService.CreateAsync(Merchant);
            return NoContent();
        }
    }
}