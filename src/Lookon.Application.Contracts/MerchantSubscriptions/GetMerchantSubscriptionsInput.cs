using LookOn.Enums;
using Volo.Abp.Application.Dtos;
using System;

namespace LookOn.MerchantSubscriptions
{
    public class GetMerchantSubscriptionsInput : PagedAndSortedResultRequestDto
    {
        public string FilterText { get; set; }

        public DateTime? StartDateTimeMin { get; set; }
        public DateTime? StartDateTimeMax { get; set; }
        public DateTime? EndDateTimeMin { get; set; }
        public DateTime? EndDateTimeMax { get; set; }
        public SubscriptionType? SubscriptionType { get; set; }
        public SubscriptionStatus? SubscriptionStatus { get; set; }
        public DateTime? NotificationDateMin { get; set; }
        public DateTime? NotificationDateMax { get; set; }
        public bool? NotificationSent { get; set; }
        public DateTime? NotificationSentAtMin { get; set; }
        public DateTime? NotificationSentAtMax { get; set; }
        public Guid? MerchantId { get; set; }

        public GetMerchantSubscriptionsInput()
        {

        }
    }
}