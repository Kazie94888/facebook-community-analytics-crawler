using LookOn.Shared;
using LookOn.Users;
using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
using LookOn.Consts;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using LookOn.Permissions;
using LookOn.UserInfos;
using Volo.Abp.Account;
using Volo.Abp.Account.Settings;
using Volo.Abp.BlobStoring;
using Volo.Abp.SettingManagement;
using Volo.Abp.Users;

namespace LookOn.UserInfos
{
    [RemoteService(IsEnabled = false)]
    [Authorize(LookOnPermissions.UserInfos.Default)]
    public class UserInfosAppService : LookOnAppService, IUserInfosAppService
    {
        private readonly IUserInfoRepository                            _userInfoRepository;
        private readonly IRepository<AppUser, Guid>                     _appUserRepository;
        private          IBlobContainer<AccountProfilePictureContainer> _accountProfilePictureContainer;
        private readonly UserInfoManager                                _userInfoManager;

        private ISettingManager _settingManager;
        public UserInfosAppService(IUserInfoRepository userInfoRepository, IRepository<AppUser, Guid> appUserRepository, ISettingManager settingManager, IBlobContainer<AccountProfilePictureContainer> accountProfilePictureContainer,
                                   UserInfoManager     userInfoManager)
        {
            _userInfoRepository             = userInfoRepository; _appUserRepository = appUserRepository;
            _settingManager                 = settingManager;
            _accountProfilePictureContainer = accountProfilePictureContainer;
            _userInfoManager           = userInfoManager;
        }

        public virtual async Task<PagedResultDto<UserInfoWithNavigationPropertiesDto>> GetListAsync(GetUserInfosInput input)
        {
            if (CurrentUser.IsInRole(RolesConsts.Merchant))
            {
                input.AppUserId = CurrentUser.Id;
            }
            var totalCount = await _userInfoRepository.GetCountAsync(input.FilterText, input.IdentificationNumber, input.AppUserId);
            var items = await _userInfoRepository.GetListWithNavigationPropertiesAsync(input.FilterText, input.IdentificationNumber, input.AppUserId, input.Sorting, input.MaxResultCount, input.SkipCount);

            return new PagedResultDto<UserInfoWithNavigationPropertiesDto>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<UserInfoWithNavigationProperties>, List<UserInfoWithNavigationPropertiesDto>>(items)
            };
        }

        public virtual async Task<UserInfoWithNavigationPropertiesDto> GetWithNavigationPropertiesAsync(Guid id)
        {
            return ObjectMapper.Map<UserInfoWithNavigationProperties, UserInfoWithNavigationPropertiesDto>
                (await _userInfoRepository.GetWithNavigationPropertiesAsync(id));
        }

        public virtual async Task<UserInfoDto> GetAsync(Guid id)
        {
            return ObjectMapper.Map<UserInfo, UserInfoDto>(await _userInfoRepository.GetAsync(id));
        }

        public virtual async Task<UserInfoDto> GetUserInfo(Guid userId)
        {
            var userInfo = await _userInfoRepository.FirstOrDefaultAsync(_ => _.AppUserId == userId);
            if (userInfo is null)
            {
                var newUserInfo = new UserInfo
                {
                    AppUserId = userId
                };
                userInfo = await _userInfoRepository.InsertAsync(newUserInfo);
            }

            return ObjectMapper.Map<UserInfo, UserInfoDto>(userInfo);
        }
        
        public virtual async Task<UserDto> GetAppUser(Guid userId)
        {
            return ObjectMapper.Map<AppUser, UserDto>(await _appUserRepository.GetAsync(userId));
        }

        public virtual async Task<PagedResultDto<LookupDto<Guid?>>> GetAppUserLookupAsync(LookupRequestDto input)
        {
            var query = (await _appUserRepository.GetQueryableAsync())
                .WhereIf(!string.IsNullOrWhiteSpace(input.Filter),
                    x => x.UserName != null &&
                         x.UserName.Contains(input.Filter));

            var lookupData = await query.PageBy(input.SkipCount, input.MaxResultCount).ToDynamicListAsync<AppUser>();
            var totalCount = query.Count();
            return new PagedResultDto<LookupDto<Guid?>>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<AppUser>, List<LookupDto<Guid?>>>(lookupData)
            };
        }

        [Authorize(LookOnPermissions.UserInfos.Delete)]
        public virtual async Task DeleteAsync(Guid id)
        {
            await _userInfoRepository.DeleteAsync(id);
        }

        [Authorize(LookOnPermissions.UserInfos.Create)]
        public virtual async Task<UserInfoDto> CreateAsync(UserInfoCreateDto input)
        {

            var userInfo = ObjectMapper.Map<UserInfoCreateDto, UserInfo>(input);

            userInfo = await _userInfoRepository.InsertAsync(userInfo, autoSave: true);
            return ObjectMapper.Map<UserInfo, UserInfoDto>(userInfo);
        }

        [Authorize(LookOnPermissions.UserInfos.Edit)]
        public virtual async Task<UserInfoDto> UpdateAsync(Guid id, UserInfoUpdateDto input)
        {

            var userInfo = await _userInfoRepository.GetAsync(id);
            ObjectMapper.Map(input, userInfo);
            userInfo = await _userInfoRepository.UpdateAsync(userInfo, autoSave: true);
            return ObjectMapper.Map<UserInfo, UserInfoDto>(userInfo);
        }
        
        [Authorize(LookOnPermissions.UserInfos.Edit)]
        public virtual async Task<UserDto> UpdateAppUser(Guid id, UserDto input)
        {
            var appUser = await _appUserRepository.GetAsync(id);
            ObjectMapper.Map(input, appUser);
            await _appUserRepository.UpdateAsync(appUser, true);
            return ObjectMapper.Map<AppUser, UserDto>(appUser);
        }

        public async Task UpdateProfilePictureAppUser(Guid userId, UserUpdateProfileDto input)
        {
            await _settingManager.SetForUserAsync(CurrentUser.GetId(), AccountSettingNames.ProfilePictureSource, input.Type.ToString());

            var userIdText = CurrentUser.GetId().ToString();

            if (input.Type != ProfilePictureType.Image)
            {
                if (await _accountProfilePictureContainer.ExistsAsync(userIdText))
                {
                    await _accountProfilePictureContainer.DeleteAsync(userIdText);
                }
            }
            else
            {
                if (input.FileBytes == null)
                {
                    throw new NoImageProvidedException();
                }
                using var stream = new MemoryStream(input.FileBytes);
                await _accountProfilePictureContainer.SaveAsync(userIdText, stream, true);
            }
        }

        public async Task<UserInfoDto> UpdateNotificationStatus(bool notificationAccepted = true)
        {
            return ObjectMapper.Map<UserInfo, UserInfoDto>(await _userInfoManager.UpdateNotificationStatus(CurrentUser.GetId(), notificationAccepted));
        }
    }
}