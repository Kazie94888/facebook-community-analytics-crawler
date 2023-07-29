using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LookOn.Core.Extensions;
using LookOn.Core.Shared.Enums;
using LookOn.Dashboards.Page3;
using LookOn.Enums;
using LookOn.Merchants;
using LookOn.MerchantSubscriptions;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace LookOn.Web.Pages.SegmentInsights;

public class Index : LookOnPageModel
{
    public MerchantDto             Merchant               { get; set; }
    public MerchantSubscriptionDto MerchantSubscription   { get; set; }
    public bool                    IsTrial                { get; set; }
    public bool                    IsEndTrial             { get; set; }
    public bool                    IsActiveSubscription   { get; set; }
    public bool                    IsNotificationAccepted { get; set; }
    public bool                    IsCommunityIntegration  { get; set; } = true;
    public bool                    IsCommunityVerified     { get; set; } = true;

    public Index()
    {
    }

    public async Task OnGetAsync()
    {
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
        IsCommunityVerified = IsCommunityIntegration
                          && Merchant.Communities.Any(_ => _.VerificationStatus == SocialCommunityVerificationStatus.Approved);
        IsNotificationAccepted = await NotificationAccepted();
    }
}