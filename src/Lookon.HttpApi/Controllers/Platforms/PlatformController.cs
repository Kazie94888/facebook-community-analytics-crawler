using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Application.Dtos;
using LookOn.Platforms;

namespace LookOn.Controllers.Platforms
{
    [RemoteService]
    [Area("app")]
    [ControllerName("Platform")]
    [Route("api/app/platforms")]

    public class PlatformController : AbpController, IPlatformsAppService
    {
        private readonly IPlatformsAppService _platformsAppService;

        public PlatformController(IPlatformsAppService platformsAppService)
        {
            _platformsAppService = platformsAppService;
        }

        [HttpGet]
        public virtual Task<PagedResultDto<PlatformDto>> GetListAsync(GetPlatformsInput input)
        {
            return _platformsAppService.GetListAsync(input);
        }

        [HttpGet]
        [Route("{id}")]
        public virtual Task<PlatformDto> GetAsync(Guid id)
        {
            return _platformsAppService.GetAsync(id);
        }

        [HttpPost]
        public virtual Task<PlatformDto> CreateAsync(PlatformCreateDto input)
        {
            return _platformsAppService.CreateAsync(input);
        }

        [HttpPut]
        [Route("{id}")]
        public virtual Task<PlatformDto> UpdateAsync(Guid id, PlatformUpdateDto input)
        {
            return _platformsAppService.UpdateAsync(id, input);
        }

        [HttpDelete]
        [Route("{id}")]
        public virtual Task DeleteAsync(Guid id)
        {
            return _platformsAppService.DeleteAsync(id);
        }
    }
}