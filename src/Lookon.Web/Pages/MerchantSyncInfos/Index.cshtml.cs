using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;
using Volo.Abp.AspNetCore.Mvc.UI.Bootstrap.TagHelpers.Form;
using LookOn.MerchantSyncInfos;
using LookOn.Shared;

namespace LookOn.Web.Pages.MerchantSyncInfos
{
    public class IndexModel : AbpPageModel
    {
        public string MerchantEmailFilter { get; set; }

        private readonly IMerchantSyncInfosAppService _merchantSyncInfosAppService;

        public IndexModel(IMerchantSyncInfosAppService merchantSyncInfosAppService)
        {
            _merchantSyncInfosAppService = merchantSyncInfosAppService;
        }

        public async Task OnGetAsync()
        {

            await Task.CompletedTask;
        }
    }
}