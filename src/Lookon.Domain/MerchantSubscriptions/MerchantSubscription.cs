using LookOn.Enums;
using LookOn.Merchants;
using System;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;
using JetBrains.Annotations;
using Volo.Abp;

namespace LookOn.MerchantSubscriptions
{
    public class MerchantSubscription : FullAuditedEntity<Guid>, IMultiTenant
    {
        public virtual Guid? TenantId { get; set; }

        public virtual DateTime? StartDateTime { get; set; }

        public virtual DateTime? EndDateTime { get; set; }

        public virtual SubscriptionType SubscriptionType { get; set; }

        public virtual SubscriptionStatus SubscriptionStatus { get; set; }
        public virtual SubscriptionMethod SubscriptionMethod { get; set; }
        

        public virtual DateTime? NotificationDate { get; set; }

        public virtual bool NotificationSent { get; set; }

        public virtual DateTime? NotificationSentAt { get; set; }
        public         Guid      MerchantId         { get; set; }
        public         string         MerchantEmail                  { get; set; }

        public SubscriptionConfig SubscriptionConfig { get; set; }

        public MerchantSubscription()
        {

        }

        public MerchantSubscription(Guid id, Guid merchantId, SubscriptionType subscriptionType, SubscriptionStatus subscriptionStatus, bool notificationSent, DateTime? startDateTime = null, DateTime? endDateTime = null, DateTime? notificationDate = null, DateTime? notificationSentAt = null)
        {
            Id = id;
            SubscriptionType = subscriptionType;
            SubscriptionStatus = subscriptionStatus;
            NotificationSent = notificationSent;
            StartDateTime = startDateTime;
            EndDateTime = endDateTime;
            NotificationDate = notificationDate;
            NotificationSentAt = notificationSentAt;
            MerchantId = merchantId;
        }
    }
}