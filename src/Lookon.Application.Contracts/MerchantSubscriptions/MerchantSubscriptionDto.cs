using LookOn.Enums;
using System;
using Volo.Abp.Application.Dtos;

namespace LookOn.MerchantSubscriptions
{
    public class MerchantSubscriptionDto : FullAuditedEntityDto<Guid>
    {
        public DateTime?          StartDateTime      { get; set; }
        public DateTime?          EndDateTime        { get; set; }
        public SubscriptionType   SubscriptionType   { get; set; }
        public SubscriptionStatus SubscriptionStatus { get; set; }
        public SubscriptionMethod SubscriptionMethod { get; set; }
        public DateTime?          NotificationDate   { get; set; }
        public bool               NotificationSent   { get; set; }
        public DateTime?          NotificationSentAt { get; set; }
        public Guid               MerchantId         { get; set; }
        public SubscriptionConfig SubscriptionConfig { get; set; }
    }
}