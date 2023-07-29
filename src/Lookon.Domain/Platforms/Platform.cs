using System;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;
using JetBrains.Annotations;
using Volo.Abp;

namespace LookOn.Platforms
{
    public class Platform : FullAuditedEntity<Guid>
    {
        [CanBeNull]
        public virtual string Name { get; set; }

        [CanBeNull]
        public virtual string Description { get; set; }

        [CanBeNull]
        public virtual string Url { get; set; }

        [CanBeNull]
        public virtual string LogoUrl { get; set; }

        public Platform()
        {

        }

        public Platform(Guid id, string name, string description, string url, string logoUrl)
        {
            Id = id;
            Name = name;
            Description = description;
            Url = url;
            LogoUrl = logoUrl;
        }
    }
}