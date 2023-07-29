using LookOn.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace LookOn.MerchantSubscriptions
{
    public class MerchantSubscriptionUpdateDto
    {
        public DateTime? StartDateTime { get; set; }
        public DateTime? EndDateTime { get; set; }
        [Required]
        public SubscriptionType SubscriptionType { get; set; }
        [Required]
        public SubscriptionStatus SubscriptionStatus { get; set; }
        public DateTime? NotificationDate { get; set; }
        public bool NotificationSent { get; set; }
        public DateTime? NotificationSentAt { get; set; }
        public Guid MerchantId { get; set; }
    }
}