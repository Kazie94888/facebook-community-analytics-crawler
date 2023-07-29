using System;
using System.ComponentModel.DataAnnotations;

namespace LookOn.MerchantStores
{
    public class MerchantStoreCreateDto
    {
        [Required]
        public string Name { get; set; }
        public string Code { get; set; }
        public bool Active { get; set; }
        public Guid? MerchantId { get; set; }
        public Guid? PlatformId { get; set; }
    }
}