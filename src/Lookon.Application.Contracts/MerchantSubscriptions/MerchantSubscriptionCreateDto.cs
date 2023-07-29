using LookOn.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace LookOn.MerchantSubscriptions
{
    public class MerchantSubscriptionCreateDto
    {
        public DateTime? StartDateTime { get; set; }
        public DateTime? EndDateTime { get; set; }
        [Required]
        public SubscriptionType SubscriptionType { get; set; } = ((SubscriptionType[])Enum.GetValues(typeof(SubscriptionType)))[0];
        [Required]
        public SubscriptionStatus SubscriptionStatus { get; set; } = ((SubscriptionStatus[])Enum.GetValues(typeof(SubscriptionStatus)))[0];
        public DateTime? NotificationDate { get; set; }
        public bool NotificationSent { get; set; }
        public DateTime? NotificationSentAt { get; set; }
        public Guid MerchantId { get; set; }
    }
}