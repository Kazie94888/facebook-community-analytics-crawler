using LookOn.Users;
using LookOn.Merchants;
using System;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;
using JetBrains.Annotations;
using Volo.Abp;

namespace LookOn.MerchantUsers
{
    public class MerchantUser : AuditedEntity<Guid>
    {
        public virtual bool   IsActive      { get; set; }
        public         Guid   AppUserId     { get; set; }
        public         Guid   MerchantId    { get; set; }
        public         string MerchantEmail { get; set; }

        public MerchantUser()
        {
        }

        public MerchantUser(Guid id, Guid appUserId, Guid merchantId, bool isActive)
        {
            Id         = id;
            IsActive   = isActive;
            AppUserId  = appUserId;
            MerchantId = merchantId;
        }
    }
}