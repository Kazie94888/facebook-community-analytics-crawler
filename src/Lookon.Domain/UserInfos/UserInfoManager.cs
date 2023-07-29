using System;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace LookOn.UserInfos;

public class UserInfoManager : LookOnManager
{
    private readonly IUserInfoRepository _userInfoRepository;

    public UserInfoManager(IUserInfoRepository userInfoRepository)
    {
        _userInfoRepository = userInfoRepository;
    }

    public async Task<UserInfo> InitUserInfo(Guid userId)
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

        return userInfo;
    }

    public async Task<UserInfo> UpdateNotificationStatus(Guid userId, bool notificationAccepted = true)
    {
        var userInfo = await _userInfoRepository.FirstOrDefaultAsync(_ => _.AppUserId == userId);
        if (userInfo is null) return null;
        userInfo.IsNotificationAccepted = notificationAccepted;
        userInfo.NotificationAcceptedAt = DateTime.UtcNow;
        return await _userInfoRepository.UpdateAsync(userInfo);
    }
}