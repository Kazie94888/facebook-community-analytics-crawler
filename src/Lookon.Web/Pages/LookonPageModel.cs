using System;
using System.Threading.Tasks;
using LookOn.Consts;
using LookOn.Core.Shared.Enums;
using LookOn.Enums;
using LookOn.Localization;
using LookOn.Merchants;
using LookOn.MerchantSubscriptions;
using LookOn.UserInfos;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;
using Volo.Abp.Users;

namespace LookOn.Web.Pages;

/* Inherit your Page Model classes from this class.
 */
public abstract class LookOnPageModel : AbpPageModel
{
    private IMerchantExtendAppService        MerchantExtendAppService        => LazyServiceProvider.LazyGetService<IMerchantExtendAppService>();
    private IMerchantSubscriptionsAppService MerchantSubscriptionsAppService => LazyServiceProvider.LazyGetService<IMerchantSubscriptionsAppService>();
    private IUserInfosAppService UserInfosAppService => LazyServiceProvider.LazyGetService<IUserInfosAppService>();
    protected LookOnPageModel()
    {
        LocalizationResourceType = typeof(LookOnResource);
    }

    public async Task<MerchantDto> CurrentMerchant()
    {
        return await MerchantExtendAppService.GetCurrentMerchantAsync();
    }
    
    public async Task<MerchantSubscriptionDto> GetSubscription(Guid merchantId)
    {
        return await MerchantSubscriptionsAppService.GetActiveSubscription(merchantId);
    }
    
    public bool IsSubscriptionActive(MerchantSubscriptionDto subscription)
    {
        return !(subscription.EndDateTime <= DateTime.UtcNow);
    }

    public bool IsTrialSubscription(MerchantSubscriptionDto subscription)
    {
        return subscription.SubscriptionType == SubscriptionType.Trial;
    }

    public async Task<bool> NotificationAccepted()
    {
        return (await UserInfosAppService.GetUserInfo(CurrentUser.GetId())).IsNotificationAccepted;
    }

    public bool IsEndTrialSubscription(MerchantSubscriptionDto subscription)
    {
        return IsTrialSubscription(subscription) && subscription.StartDateTime < DateTime.UtcNow.AddDays(-7);
    }

    protected DateTime GetFromDateTime(TimeFrameType timeFrameType)
    {
        var currentDateTime = DateTime.UtcNow;
        switch (timeFrameType)
        {
            case TimeFrameType.Daily: return currentDateTime.AddDays(-1);

            case TimeFrameType.Weekly: return currentDateTime.AddDays(-7);

            case TimeFrameType.Monthly: return currentDateTime.AddDays(-30);

            default: return currentDateTime.AddDays(-1);
        }
    }

    protected DateTime GetEndDateTime()
    {
        return DateTime.UtcNow;
    }

    public bool IsAdminRole()
    {
        return CurrentUser.IsInRole(RolesConsts.Admin);
    }

    public bool IsMerchantRole()
    {
        return CurrentUser.IsInRole(RolesConsts.Merchant);
    }

    public bool IsMasterAdmin()
    {
        return CurrentUser.Email == AccountConsts.MasterAdminEmail && CurrentUser.UserName == AccountConsts.MasterAdminUsername;
    }
}
