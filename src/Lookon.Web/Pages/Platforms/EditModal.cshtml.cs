using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Volo.Abp.Application.Dtos;
using LookOn.Platforms;

namespace LookOn.Web.Pages.Platforms
{
    public class EditModalModel : LookOnPageModel
    {
        [HiddenInput]
        [BindProperty(SupportsGet = true)]
        public Guid Id { get; set; }

        [BindProperty]
        public PlatformUpdateDto Platform { get; set; }

        private readonly IPlatformsAppService _platformsAppService;

        public EditModalModel(IPlatformsAppService platformsAppService)
        {
            _platformsAppService = platformsAppService;
        }

        public async Task OnGetAsync()
        {
            var platform = await _platformsAppService.GetAsync(Id);
            Platform = ObjectMapper.Map<PlatformDto, PlatformUpdateDto>(platform);

        }

        public async Task<IActionResult> OnPostAsync()
        {
            await _platformsAppService.UpdateAsync(Id, Platform);
            return NoContent();
        }
    }
}