using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LookOn.Core.Extensions;
using LookOn.Core.Shared.Enums;
using LookOn.Dashboards.Page1;
using LookOn.Enums;
using LookOn.Merchants;
using LookOn.MerchantSubscriptions;
using LookOn.MerchantSyncInfos;
using LookOn.UserInfos;
using Volo.Abp.Users;

namespace LookOn.Web.Pages.TransationInsights;

public class IndexModel : LookOnPageModel
{
    public TimeFrameType           TimeFrameType        { get; set; }
    public MerchantDto             Merchant             { get; set; }
    public List<MerchantDto>       Merchants            { get; set; }
    public MerchantSubscriptionDto MerchantSubscription { get; set; }
    public bool                    IsTrial              { get; set; }
    public bool                    IsEndTrial           { get; set; }
    public bool                    IsActiveSubscription { get; set; }
    public string FromDay
    {
        get
        {
            switch (TimeFrameType)
            {
                case TimeFrameType.Daily:
                {
                    return DateTime.UtcNow.AddDays(-1).ToString("dd/MM");
                }

                case TimeFrameType.Weekly:
                {
                    return DateTime.UtcNow.AddDays(-7).ToString("dd/MM");
                }

                case TimeFrameType.Monthly:
                {
                    return DateTime.UtcNow.AddDays(-30).ToString("dd/MM");
                }
                    ;

                default:
                {
                    return DateTime.UtcNow.AddDays(-1).ToString("dd/MM");
                }
            }
        }
    }
    public bool               IsNotificationAccepted  { get; set; }
    public Page1MetricDto     Page1Metric             { get; set; } = new Page1MetricDto();
    public string             TimeTitle               { get; set; }
    public bool               IsMasterAdmin           { get; set; }
    public string             MerchantName            { get; set; }
    public MerchantSyncStatus MerchantSyncStatus { get; set; }
    public bool               IsCommunityIntegration   { get; set; } = true;
    public bool               IsCommunityVerified      { get; set; } = true;

    // managers 
    private readonly IMerchantExtendAppService    _merchantExtendAppService;
    private readonly IMerchantSyncInfosAppService _merchantSyncInfosAppService;
    private readonly IPage1AppService             _page1AppService;
    private readonly IUserInfosAppService         _userInfosAppService;

    public IndexModel(IMerchantExtendAppService    merchantExtendAppService,
                      IPage1AppService             page1AppService,
                      IUserInfosAppService         userInfosAppService,
                      IMerchantSyncInfosAppService merchantSyncInfosAppService)
    {
        _merchantExtendAppService    = merchantExtendAppService;
        _page1AppService             = page1AppService;
        _userInfosAppService         = userInfosAppService;
        Merchants                    = new List<MerchantDto>();
        _merchantSyncInfosAppService = merchantSyncInfosAppService;
    }

    public async Task OnGetAsync(string timeFrameTypeString = "weekly")
    {
        IsMasterAdmin = IsMasterAdmin();
        MerchantName  = L["ChooseMerchant"];

        var timeFrameType = timeFrameTypeString.ToEnumOrDefault<TimeFrameType>();
        TimeTitle = timeFrameType switch
        {
            TimeFrameType.Daily   => "1day",
            TimeFrameType.Weekly  => "7days",
            TimeFrameType.Monthly => "30days",
            _                     => throw new ArgumentOutOfRangeException(nameof(timeFrameType), timeFrameType, null)
        };
        ViewData["TimeTitle"] = TimeTitle;

        TimeFrameType = timeFrameType;
        if (IsMasterAdmin)
        {
            Merchants = await _page1AppService.GetMerchants();
            var merchantEmail = HttpContext.Request.Query["merchantemail"].ToString();
            if (merchantEmail.IsNotNullOrSpace())
            {
                Merchant = Merchants.FirstOrDefault(_ => string.Equals(_.Email, merchantEmail, StringComparison.CurrentCultureIgnoreCase));
            }

            if (Merchant is null)
            {
                Page1Metric = null;
                return;
            }
        }
        else
        {
            Merchant = await CurrentMerchant();
        }
        
        MerchantName         = Merchant.Name;
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
            Page1Metric = await _page1AppService.GetMetrics(new GetMetricsInput
            {
                MerchantId         = Merchant.Id,
                TimeFrameType      = timeFrameType,
                From               = GetFromDateTime(timeFrameType),
                To                 = GetEndDateTime(),
                SocialCommunityIds = Merchant.GetSocialCommunityIds(),
            });

            ViewData["Ecom_RetentionThresholdInMonth"] = Merchant.MetricConfigs.Ecom_RetentionThresholdInMonth;
            ViewData["OrderTotalKPI"] = Merchant.MetricConfigs.OrderTotalKPI;
        }
        else
        {
            Page1Metric = null;
        }

        if (Page1Metric is null)
        {
            MerchantSyncStatus = await _merchantSyncInfosAppService.GetMerchantSyncStatus(Merchant.Id);
        }

        IsNotificationAccepted = await NotificationAccepted();
    }
}