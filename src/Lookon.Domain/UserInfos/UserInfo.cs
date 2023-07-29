using LookOn.Users;
using System;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;
using JetBrains.Annotations;
using Volo.Abp;

namespace LookOn.UserInfos
{
    public class UserInfo : FullAuditedEntity<Guid>
    {
        [CanBeNull]
        public virtual string IdentificationNumber { get;      set; }
        public virtual bool      IsNotificationAccepted { get; set; }
        public virtual DateTime? NotificationAcceptedAt { get; set; }
        public         Guid?     AppUserId              { get; set; }

        public UserInfo()
        {
        }

        public UserInfo(Guid id, string identificationNumber, bool isNotificationAccepted, DateTime notificationAcceptedAt)
        {
            Id                     = id;
            IdentificationNumber   = identificationNumber;
            IsNotificationAccepted = isNotificationAccepted;
            NotificationAcceptedAt = notificationAcceptedAt;
        }
    }
}