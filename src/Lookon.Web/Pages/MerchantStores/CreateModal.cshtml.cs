using LookOn.Shared;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Volo.Abp.Application.Dtos;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using LookOn.MerchantStores;

namespace LookOn.Web.Pages.MerchantStores
{
    public class CreateModalModel : LookOnPageModel
    {
        [BindProperty]
        public MerchantStoreCreateDto MerchantStore { get; set; }

        public List<SelectListItem> PlatformLookupList { get; set; } = new List<SelectListItem>
        {
            new SelectListItem(" — ", "")
        };

        private readonly IMerchantStoresAppService _merchantStoresAppService;

        public CreateModalModel(IMerchantStoresAppService merchantStoresAppService)
        {
            _merchantStoresAppService = merchantStoresAppService;
        }

        public async Task OnGetAsync()
        {
            MerchantStore = new MerchantStoreCreateDto();
            PlatformLookupList.AddRange((
                                    await _merchantStoresAppService.GetPlatformLookupAsync(new LookupRequestDto
                                    {
                                        MaxResultCount = LimitedResultRequestDto.MaxMaxResultCount
                                    })).Items.Select(t => new SelectListItem(t.DisplayName, t.Id.ToString())).ToList()
                        );

            await Task.CompletedTask;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            await _merchantStoresAppService.CreateAsync(MerchantStore);
            return NoContent();
        }
    }
}