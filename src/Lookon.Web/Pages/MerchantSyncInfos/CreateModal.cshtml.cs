using LookOn.Shared;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Volo.Abp.Application.Dtos;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using LookOn.MerchantSyncInfos;

namespace LookOn.Web.Pages.MerchantSyncInfos
{
    public class CreateModalModel : LookOnPageModel
    {
        [BindProperty]
        public MerchantSyncInfoCreateDto MerchantSyncInfo { get; set; }

        private readonly IMerchantSyncInfosAppService _merchantSyncInfosAppService;

        public CreateModalModel(IMerchantSyncInfosAppService merchantSyncInfosAppService)
        {
            _merchantSyncInfosAppService = merchantSyncInfosAppService;
        }

        public async Task OnGetAsync()
        {
            MerchantSyncInfo = new MerchantSyncInfoCreateDto();

            await Task.CompletedTask;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            await _merchantSyncInfosAppService.CreateAsync(MerchantSyncInfo);
            return NoContent();
        }
    }
}