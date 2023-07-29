using System;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;
using JetBrains.Annotations;
using Volo.Abp;

namespace LookOn.Categories
{
    public class Category : FullAuditedEntity<Guid>
    {
        [NotNull]
        public virtual string Name { get; set; }

        [CanBeNull]
        public virtual string Code { get; set; }

        [CanBeNull]
        public virtual string Description { get; set; }

        public virtual int Order { get; set; }

        public Category()
        {

        }

        public Category(Guid id, string name, string code, string description, int order)
        {
            Id = id;
            Check.NotNull(name, nameof(name));
            Name = name;
            Code = code;
            Description = description;
            Order = order;
        }
    }
}