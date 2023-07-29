using LookOn.Shared;
using LookOn.Merchants;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Volo.Abp.Application.Dtos;
using LookOn.MerchantStores;

namespace LookOn.Web.Pages.MerchantStores
{
    public class EditModalModel : LookOnPageModel
    {
        [HiddenInput]
        [BindProperty(SupportsGet = true)]
        public Guid Id { get; set; }

        [BindProperty]
        public MerchantStoreUpdateDto MerchantStore { get; set; }

        public MerchantDto Merchant { get; set; }
        public List<SelectListItem> PlatformLookupList { get; set; } = new List<SelectListItem>
        {
            new SelectListItem(" — ", "")
        };

        private readonly IMerchantStoresAppService _merchantStoresAppService;

        public EditModalModel(IMerchantStoresAppService merchantStoresAppService)
        {
            _merchantStoresAppService = merchantStoresAppService;
        }

        public async Task OnGetAsync()
        {
            var merchantStoreWithNavigationPropertiesDto = await _merchantStoresAppService.GetWithNavigationPropertiesAsync(Id);
            MerchantStore = ObjectMapper.Map<MerchantStoreDto, MerchantStoreUpdateDto>(merchantStoreWithNavigationPropertiesDto.MerchantStore);

            Merchant = merchantStoreWithNavigationPropertiesDto.Merchant;
            PlatformLookupList.AddRange((
                        await _merchantStoresAppService.GetPlatformLookupAsync(new LookupRequestDto
                        {
                            MaxResultCount = LimitedResultRequestDto.MaxMaxResultCount
                        })).Items.Select(t => new SelectListItem(t.DisplayName, t.Id.ToString())).ToList()
            );

        }

        public async Task<IActionResult> OnPostAsync()
        {
            await _merchantStoresAppService.UpdateAsync(Id, MerchantStore);
            return NoContent();
        }
    }
}