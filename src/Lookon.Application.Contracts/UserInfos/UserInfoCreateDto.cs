using System;
using System.ComponentModel.DataAnnotations;

namespace LookOn.UserInfos
{
    public class UserInfoCreateDto
    {
        public         string    IdentificationNumber   { get; set; }
        public virtual bool      IsNotificationAccepted { get; set; }
        public virtual DateTime? NotificationAcceptedAt { get; set; }
        public         Guid?     AppUserId              { get; set; }
    }
}