using LookOn.Shared;
using System;
using System.Threading.Tasks;
using LookOn.Users;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace LookOn.UserInfos
{
    public interface IUserInfosAppService : IApplicationService
    {
        Task<PagedResultDto<UserInfoWithNavigationPropertiesDto>> GetListAsync(GetUserInfosInput input);

        Task<UserInfoWithNavigationPropertiesDto> GetWithNavigationPropertiesAsync(Guid id);

        Task<UserInfoDto> GetAsync(Guid id);
        Task<UserInfoDto> GetUserInfo(Guid userId);
        Task<UserDto> GetAppUser(Guid userId);

        Task<PagedResultDto<LookupDto<Guid?>>> GetAppUserLookupAsync(LookupRequestDto input);

        Task DeleteAsync(Guid id);

        Task<UserInfoDto> CreateAsync(UserInfoCreateDto input);

        Task<UserInfoDto> UpdateAsync(Guid                 id,     UserInfoUpdateDto    input);
        Task<UserDto>     UpdateAppUser(Guid               id,     UserDto              input);
        Task              UpdateProfilePictureAppUser(Guid userId, UserUpdateProfileDto input);
        Task<UserInfoDto> UpdateNotificationStatus(bool                 notificationAccepted = true);
    }
}