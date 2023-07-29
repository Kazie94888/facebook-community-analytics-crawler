using System;
using Volo.Abp.Application.Dtos;

namespace LookOn.Platforms
{
    public class PlatformDto : FullAuditedEntityDto<Guid>
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Url { get; set; }
        public string LogoUrl { get; set; }
    }
}