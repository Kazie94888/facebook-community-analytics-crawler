using LookOn.Shared;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Application.Dtos;
using LookOn.UserInfos;
using LookOn.Users;

namespace LookOn.Controllers.UserInfos
{
    [RemoteService]
    [Area("app")]
    [ControllerName("UserInfo")]
    [Route("api/app/user-infos")]

    public class UserInfoController : AbpController, IUserInfosAppService
    {
        private readonly IUserInfosAppService _userInfosAppService;

        public UserInfoController(IUserInfosAppService userInfosAppService)
        {
            _userInfosAppService = userInfosAppService;
        }

        [HttpGet]
        public Task<PagedResultDto<UserInfoWithNavigationPropertiesDto>> GetListAsync(GetUserInfosInput input)
        {
            return _userInfosAppService.GetListAsync(input);
        }

        [HttpGet]
        [Route("with-navigation-properties/{id}")]
        public Task<UserInfoWithNavigationPropertiesDto> GetWithNavigationPropertiesAsync(Guid id)
        {
            return _userInfosAppService.GetWithNavigationPropertiesAsync(id);
        }

        [HttpGet]
        [Route("{id}")]
        public virtual Task<UserInfoDto> GetAsync(Guid id)
        {
            return _userInfosAppService.GetAsync(id);
        }

        [HttpGet]
        [Route("get-user-info")]
        public Task<UserInfoDto> GetUserInfo(Guid userId)
        {
            return _userInfosAppService.GetUserInfo(userId);
        }

        [HttpGet]
        [Route("get-app-user")]
        public Task<UserDto> GetAppUser(Guid userId)
        {
            return _userInfosAppService.GetAppUser(userId);
        }

        [HttpGet]
        [Route("app-user-lookup")]
        public Task<PagedResultDto<LookupDto<Guid?>>> GetAppUserLookupAsync(LookupRequestDto input)
        {
            return _userInfosAppService.GetAppUserLookupAsync(input);
        }

        [HttpPost]
        public virtual Task<UserInfoDto> CreateAsync(UserInfoCreateDto input)
        {
            return _userInfosAppService.CreateAsync(input);
        }

        [HttpPut]
        [Route("{id}")]
        public virtual Task<UserInfoDto> UpdateAsync(Guid id, UserInfoUpdateDto input)
        {
            return _userInfosAppService.UpdateAsync(id, input);
        }

        [HttpPut]
        [Route("update-app-user")]
        public Task<UserDto> UpdateAppUser(Guid id, UserDto input)
        {
            return _userInfosAppService.UpdateAppUser(id, input);
        }

        
        [HttpPost, DisableRequestSizeLimit]
        [Route("update-profile-image/{userId}")]
        public Task UpdateProfilePictureAppUser(Guid userId, UserUpdateProfileDto input)
        {
            return _userInfosAppService.UpdateProfilePictureAppUser(userId, input);
        }

        [HttpPost]
        [Route("update-notification-status")]
        public Task<UserInfoDto> UpdateNotificationStatus(bool notificationAccepted = true)
        {
            return _userInfosAppService.UpdateNotificationStatus(notificationAccepted);
        }

        [HttpDelete]
        [Route("{id}")]
        public virtual Task DeleteAsync(Guid id)
        {
            return _userInfosAppService.DeleteAsync(id);
        }
    }
}