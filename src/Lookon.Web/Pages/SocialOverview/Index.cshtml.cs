using System;
using System.Linq;
using System.Threading.Tasks;
using LookOn.Core.Extensions;
using LookOn.Core.Shared.Enums;
using LookOn.Dashboards.Page2;
using LookOn.Enums;
using LookOn.Merchants;
using LookOn.MerchantSubscriptions;
using LookOn.MerchantSyncInfos;
using LookOn.UserInfos;
using Volo.Abp.Users;

namespace LookOn.Web.Pages.SocialOverview;

public class IndexModel : LookOnPageModel
{
    public           MerchantDto                  Merchant                { get; set; }
    public           MerchantSubscriptionDto      MerchantSubscription    { get; set; }
    public           bool                         IsTrial                 { get; set; }
    public           bool                         IsEndTrial              { get; set; }
    public           bool                         IsActiveSubscription    { get; set; }
    public           MerchantSyncStatus           MerchantSyncStatus { get; set; }
    public           Page2MetricDto               Page2Metric             { get; set; } = new Page2MetricDto();
    private readonly IPage2AppService             _page2AppService;
    private readonly IMerchantSyncInfosAppService _merchantSyncInfosAppService;
    private readonly IUserInfosAppService         _userInfosAppService;
    public           bool                         IsNotificationAccepted { get; set; }
    public           bool                         IsCommunityIntegration  { get; set; } = true;
    public           bool                         IsCommunityVerified     { get; set; } = true;

    public IndexModel(IPage2AppService page2AppService, IUserInfosAppService userInfosAppService, IMerchantSyncInfosAppService merchantSyncInfosAppService)
    {
        _page2AppService             = page2AppService;
        _userInfosAppService         = userInfosAppService;
        _merchantSyncInfosAppService = merchantSyncInfosAppService;
    }

    public async Task OnGetAsync()
    {
        // IsActiveSubscription = await IsSubscriptionActive();
        // if (!IsActiveSubscription)
        // {
        //     return;
        // }

        Merchant             = await CurrentMerchant();
        MerchantSubscription = await GetSubscription(Merchant.Id);
        IsTrial              = IsTrialSubscription(MerchantSubscription);
        IsEndTrial           = IsEndTrialSubscription(MerchantSubscription);
        IsActiveSubscription = IsSubscriptionActive(MerchantSubscription);
        if (!IsActiveSubscription)
        {
            return;
        }
            
        IsCommunityIntegration = Merchant.Communities.IsNotNullOrEmpty();
        IsCommunityVerified    = IsCommunityIntegration && Merchant.Communities.Any(_ => _.VerificationStatus == SocialCommunityVerificationStatus.Approved);
        if (IsCommunityVerified)
        {
            Page2Metric = await _page2AppService.GetMetrics(new GetMetricsInput { MerchantId = Merchant.Id, SocialCommunityIds = Merchant.GetSocialCommunityIds() });
        }
        else
        {
            Page2Metric = null;
        }

        if (Page2Metric is null)
        {
            MerchantSyncStatus = await _merchantSyncInfosAppService.GetMerchantSyncStatus(Merchant.Id);
        }

        IsNotificationAccepted = await NotificationAccepted();
    }
}