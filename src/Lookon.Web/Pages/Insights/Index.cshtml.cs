using System;
using System.Linq;
using System.Threading.Tasks;
using LookOn.Core.Extensions;
using LookOn.Core.Shared.Enums;
using LookOn.Enums;
using LookOn.Insights;
using LookOn.Merchants;
using LookOn.MerchantSubscriptions;

namespace LookOn.Web.Pages.Insights
{
    public class IndexModel : LookOnPageModel
    {
        public           MerchantDto             Merchant  { get; set; }
        public           string                  TimeTitle { get; set; }
        private readonly IMetricAppService       _metricAppService;
        public           InsightUserDto          InsightUser            { get; set; }
        public           MerchantSubscriptionDto MerchantSubscription   { get; set; }
        public           bool                    IsTrial                { get; set; }
        public           bool                    IsEndTrial             { get; set; }
        public           bool                    IsActiveSubscription   { get; set; }
        public           bool                    IsCommunityIntegration { get; set; } = true;
        public           bool                    IsCommunityVerified    { get; set; } = true;
        
        public IndexModel(IMetricAppService metricAppService)
        {
            _metricAppService = metricAppService;
        }

        public async Task OnGetAsync()
        {
            Merchant             = await CurrentMerchant();
            MerchantSubscription = await GetSubscription(Merchant.Id);
            IsTrial              = IsTrialSubscription(MerchantSubscription);
            IsEndTrial           = IsEndTrialSubscription(MerchantSubscription);
            IsActiveSubscription = IsSubscriptionActive(MerchantSubscription);
            IsCommunityIntegration = Merchant.Communities.IsNotNullOrEmpty();
            IsCommunityVerified    = IsCommunityIntegration && Merchant.Communities.Any(_ => _.VerificationStatus == SocialCommunityVerificationStatus.Approved);
            if (!IsActiveSubscription)
            {
                return;
            }
            InsightUser = new InsightUserDto()
            {
                EcomUserCount      = 10000,
                SocialUserCount    = 15000,
                IntersectUserCount = 2000
            };
            await Task.CompletedTask;
        }
    }
}