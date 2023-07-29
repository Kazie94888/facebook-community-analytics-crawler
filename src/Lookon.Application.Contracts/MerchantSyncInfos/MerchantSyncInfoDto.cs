using System;
using LookOn.Enums;
using Volo.Abp.Application.Dtos;

namespace LookOn.MerchantSyncInfos
{
    public class MerchantSyncInfoDto : FullAuditedEntityDto<Guid>
    {
        public string                    MerchantEmail   { get; set; }
        public Guid?                     MerchantId      { get; set; }
        public MerchantSocialSyncInfoDto SocialSyncInfo  { get; set; }
        public MerchantEcomScanDto       EcomScan        { get; set; }
        public MerchantSyncStatus        Page1SyncStatus { get; set; }
        public MerchantSyncStatus        Page2SyncStatus { get; set; }
        public MerchantSyncStatus        Page3SyncStatus { get; set; }
    }
}