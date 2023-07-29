using LookOn.Shared;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Volo.Abp.Application.Dtos;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using LookOn.UserInfos;

namespace LookOn.Web.Pages.UserInfos
{
    public class CreateModalModel : LookOnPageModel
    {
        [BindProperty]
        public UserInfoCreateDto UserInfo { get; set; }

        private readonly IUserInfosAppService _userInfosAppService;

        public CreateModalModel(IUserInfosAppService userInfosAppService)
        {
            _userInfosAppService = userInfosAppService;
        }

        public async Task OnGetAsync()
        {
            UserInfo = new UserInfoCreateDto();

            await Task.CompletedTask;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            await _userInfosAppService.CreateAsync(UserInfo);
            return NoContent();
        }
    }
}