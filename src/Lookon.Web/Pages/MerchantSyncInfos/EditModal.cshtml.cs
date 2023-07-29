using LookOn.Shared;
using LookOn.Merchants;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Volo.Abp.Application.Dtos;
using LookOn.MerchantSyncInfos;

namespace LookOn.Web.Pages.MerchantSyncInfos
{
    public class EditModalModel : LookOnPageModel
    {
        [HiddenInput]
        [BindProperty(SupportsGet = true)]
        public Guid Id { get; set; }

        [BindProperty]
        public MerchantSyncInfoUpdateDto MerchantSyncInfo { get; set; }

        public MerchantDto Merchant { get; set; }

        private readonly IMerchantSyncInfosAppService _merchantSyncInfosAppService;

        public EditModalModel(IMerchantSyncInfosAppService merchantSyncInfosAppService)
        {
            _merchantSyncInfosAppService = merchantSyncInfosAppService;
        }

        public async Task OnGetAsync()
        {
            var merchantSyncInfoWithNavigationPropertiesDto = await _merchantSyncInfosAppService.GetWithNavigationPropertiesAsync(Id);
            MerchantSyncInfo = ObjectMapper.Map<MerchantSyncInfoDto, MerchantSyncInfoUpdateDto>(merchantSyncInfoWithNavigationPropertiesDto.MerchantSyncInfo);

            Merchant = merchantSyncInfoWithNavigationPropertiesDto.Merchant;

        }

        public async Task<IActionResult> OnPostAsync()
        {
            await _merchantSyncInfosAppService.UpdateAsync(Id, MerchantSyncInfo);
            return NoContent();
        }
    }
}