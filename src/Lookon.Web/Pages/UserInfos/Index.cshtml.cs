using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;
using Volo.Abp.AspNetCore.Mvc.UI.Bootstrap.TagHelpers.Form;
using LookOn.UserInfos;
using LookOn.Shared;

namespace LookOn.Web.Pages.UserInfos
{
    public class IndexModel : AbpPageModel
    {
        public string IdentificationNumberFilter { get; set; }

        private readonly IUserInfosAppService _userInfosAppService;

        public IndexModel(IUserInfosAppService userInfosAppService)
        {
            _userInfosAppService = userInfosAppService;
        }

        public async Task OnGetAsync()
        {
            await Task.CompletedTask;
        }
    }
}