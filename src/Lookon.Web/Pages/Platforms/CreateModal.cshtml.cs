using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Volo.Abp.Application.Dtos;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using LookOn.Platforms;

namespace LookOn.Web.Pages.Platforms
{
    public class CreateModalModel : LookOnPageModel
    {
        [BindProperty]
        public PlatformCreateDto Platform { get; set; }

        private readonly IPlatformsAppService _platformsAppService;

        public CreateModalModel(IPlatformsAppService platformsAppService)
        {
            _platformsAppService = platformsAppService;
        }

        public async Task OnGetAsync()
        {
            Platform = new PlatformCreateDto();
            await Task.CompletedTask;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            await _platformsAppService.CreateAsync(Platform);
            return NoContent();
        }
    }
}