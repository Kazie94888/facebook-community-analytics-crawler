using System;
using Volo.Abp.Application.Dtos;

namespace LookOn.MerchantStores
{
    public class MerchantStoreDto : FullAuditedEntityDto<Guid>
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public bool Active { get; set; }
        public Guid? MerchantId { get; set; }
        public Guid? PlatformId { get; set; }
    }
}