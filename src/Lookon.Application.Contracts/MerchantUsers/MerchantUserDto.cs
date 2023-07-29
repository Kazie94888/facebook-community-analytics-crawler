using System;
using Volo.Abp.Application.Dtos;

namespace LookOn.MerchantUsers
{
    public class MerchantUserDto : FullAuditedEntityDto<Guid>
    {
        public bool IsActive { get; set; }
        public Guid AppUserId { get; set; }
        public Guid MerchantId { get; set; }
    }
}