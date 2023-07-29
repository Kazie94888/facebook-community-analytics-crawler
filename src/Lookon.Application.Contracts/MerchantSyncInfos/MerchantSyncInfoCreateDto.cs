using System;
using System.ComponentModel.DataAnnotations;

namespace LookOn.MerchantSyncInfos
{
    public class MerchantSyncInfoCreateDto
    {
        [EmailAddress]
        public string MerchantEmail { get; set; }
        public Guid? MerchantId { get; set; }
    }
}