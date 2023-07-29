using System;
using Volo.Abp.Application.Dtos;

namespace LookOn.UserInfos
{
    public class UserInfoDto : FullAuditedEntityDto<Guid>
    {
        public         string    IdentificationNumber   { get; set; }
        public virtual bool      IsNotificationAccepted { get; set; }
        public virtual DateTime? NotificationAcceptedAt { get; set; }
        public         Guid?     AppUserId              { get; set; }
    }
}