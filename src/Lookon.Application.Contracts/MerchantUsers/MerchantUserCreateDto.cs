using System;
using System.ComponentModel.DataAnnotations;

namespace LookOn.MerchantUsers
{
    public class MerchantUserCreateDto
    {
        public bool IsActive { get; set; }
        public Guid AppUserId { get; set; }
        public Guid MerchantId { get; set; }
    }
}