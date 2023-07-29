using LookOn.Users;

using System;
using Volo.Abp.Application.Dtos;

namespace LookOn.UserInfos
{
    public class UserInfoWithNavigationPropertiesDto
    {
        public UserInfoDto UserInfo { get; set; }

        public AppUserDto AppUser { get; set; }

    }
}