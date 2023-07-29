using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using LookOn.Permissions;
using LookOn.Platforms;

namespace LookOn.Platforms
{
    [RemoteService(IsEnabled = false)]
    [Authorize(LookOnPermissions.Platforms.Default)]
    public class PlatformsAppService : LookOnAppService, IPlatformsAppService
    {
        private readonly IPlatformRepository _platformRepository;

        public PlatformsAppService(IPlatformRepository platformRepository)
        {
            _platformRepository = platformRepository;
        }

        public virtual async Task<PagedResultDto<PlatformDto>> GetListAsync(GetPlatformsInput input)
        {
            var totalCount = await _platformRepository.GetCountAsync(input.FilterText, input.Name, input.Description, input.Url, input.LogoUrl);
            var items = await _platformRepository.GetListAsync(input.FilterText, input.Name, input.Description, input.Url, input.LogoUrl, input.Sorting, input.MaxResultCount, input.SkipCount);

            return new PagedResultDto<PlatformDto>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<Platform>, List<PlatformDto>>(items)
            };
        }

        public virtual async Task<PlatformDto> GetAsync(Guid id)
        {
            return ObjectMapper.Map<Platform, PlatformDto>(await _platformRepository.GetAsync(id));
        }

        [Authorize(LookOnPermissions.Platforms.Delete)]
        public virtual async Task DeleteAsync(Guid id)
        {
            await _platformRepository.DeleteAsync(id);
        }

        [Authorize(LookOnPermissions.Platforms.Create)]
        public virtual async Task<PlatformDto> CreateAsync(PlatformCreateDto input)
        {

            var platform = ObjectMapper.Map<PlatformCreateDto, Platform>(input);

            platform = await _platformRepository.InsertAsync(platform, autoSave: true);
            return ObjectMapper.Map<Platform, PlatformDto>(platform);
        }

        [Authorize(LookOnPermissions.Platforms.Edit)]
        public virtual async Task<PlatformDto> UpdateAsync(Guid id, PlatformUpdateDto input)
        {

            var platform = await _platformRepository.GetAsync(id);
            ObjectMapper.Map(input, platform);
            platform = await _platformRepository.UpdateAsync(platform, autoSave: true);
            return ObjectMapper.Map<Platform, PlatformDto>(platform);
        }
    }
}