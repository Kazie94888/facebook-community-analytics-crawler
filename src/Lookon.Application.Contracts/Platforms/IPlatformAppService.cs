using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace LookOn.Platforms
{
    public interface IPlatformsAppService : IApplicationService
    {
        Task<PagedResultDto<PlatformDto>> GetListAsync(GetPlatformsInput input);

        Task<PlatformDto> GetAsync(Guid id);

        Task DeleteAsync(Guid id);

        Task<PlatformDto> CreateAsync(PlatformCreateDto input);

        Task<PlatformDto> UpdateAsync(Guid id, PlatformUpdateDto input);
    }
}