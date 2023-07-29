using System;
using Volo.Abp.Application.Dtos;

namespace LookOn.Categories
{
    public class CategoryDto : FullAuditedEntityDto<Guid>
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public int Order { get; set; }
    }
}