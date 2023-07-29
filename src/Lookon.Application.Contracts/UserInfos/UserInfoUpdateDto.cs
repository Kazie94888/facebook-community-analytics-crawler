using System;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Account;

namespace LookOn.UserInfos
{
    public class UserInfoUpdateDto
    {
        public         string    IdentificationNumber   { get; set; }
        public virtual bool      IsNotificationAccepted { get; set; }
        public virtual DateTime? NotificationAcceptedAt { get; set; }
        public         Guid?     AppUserId              { get; set; }
    }

    public class UserUpdateProfileDto
    {
        public ProfilePictureType Type      { get; set; }
        public string             FileName  { get; set; }
        public byte[]             FileBytes { get; set; }
    }
}