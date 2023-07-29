using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;
using Volo.Abp.AspNetCore.Mvc.UI.Bootstrap.TagHelpers.Form;
using LookOn.Platforms;
using LookOn.Shared;

namespace LookOn.Web.Pages.Platforms
{
    public class IndexModel : AbpPageModel
    {
        public string NameFilter { get; set; }
        public string DescriptionFilter { get; set; }
        public string UrlFilter { get; set; }
        public string LogoUrlFilter { get; set; }

        private readonly IPlatformsAppService _platformsAppService;

        public IndexModel(IPlatformsAppService platformsAppService)
        {
            _platformsAppService = platformsAppService;
        }

        public async Task OnGetAsync()
        {

            await Task.CompletedTask;
        }
    }
}