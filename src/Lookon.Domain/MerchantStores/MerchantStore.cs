using LookOn.Merchants;
using LookOn.Platforms;
using System;
using System.Collections.Generic;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;
using JetBrains.Annotations;
using LookOn.Integrations.Haravan.Models.RawModels;
using Volo.Abp;

namespace LookOn.MerchantStores
{
    public class MerchantStore : FullAuditedEntity<Guid>, IMultiTenant
    {
        public virtual Guid? TenantId { get; set; }

        [NotNull]
        public virtual string Name { get; set; }

        [CanBeNull]
        public virtual string Code { get; set; }

        public virtual bool  Active     { get; set; }
        public         Guid? MerchantId { get; set; }
        public         Guid? PlatformId { get; set; }
        public         string     MerchantEmail          { get; set; }

        public MerchantStore()
        {

        }

        public MerchantStore(Guid id, string name, string code, bool active)
        {
            Id = id;
            Check.NotNull(name, nameof(name));
            Name = name;
            Code = code;
            Active = active;
        }
    }
}