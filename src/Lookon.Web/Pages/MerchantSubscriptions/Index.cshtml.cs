using LookOn.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;
using Volo.Abp.AspNetCore.Mvc.UI.Bootstrap.TagHelpers.Form;
using LookOn.MerchantSubscriptions;
using LookOn.Shared;

namespace LookOn.Web.Pages.MerchantSubscriptions
{
    public class IndexModel : AbpPageModel
    {
        public DateTime? StartDateTimeFilterMin { get; set; }

        public DateTime? StartDateTimeFilterMax { get; set; }
        public DateTime? EndDateTimeFilterMin { get; set; }

        public DateTime? EndDateTimeFilterMax { get; set; }
        public SubscriptionType? SubscriptionTypeFilter { get; set; }
        public SubscriptionStatus? SubscriptionStatusFilter { get; set; }
        public DateTime? NotificationDateFilterMin { get; set; }

        public DateTime? NotificationDateFilterMax { get; set; }
        [SelectItems(nameof(NotificationSentBoolFilterItems))]
        public string NotificationSentFilter { get; set; }

        public List<SelectListItem> NotificationSentBoolFilterItems { get; set; } =
            new List<SelectListItem>
            {
                new SelectListItem("", ""),
                new SelectListItem("Yes", "true"),
                new SelectListItem("No", "false"),
            };
        public DateTime? NotificationSentAtFilterMin { get; set; }

        public DateTime? NotificationSentAtFilterMax { get; set; }

        private readonly IMerchantSubscriptionsAppService _merchantSubscriptionsAppService;

        public IndexModel(IMerchantSubscriptionsAppService merchantSubscriptionsAppService)
        {
            _merchantSubscriptionsAppService = merchantSubscriptionsAppService;
        }

        public async Task OnGetAsync()
        {

            await Task.CompletedTask;
        }
    }
}