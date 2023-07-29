using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using LookOn.Core.Extensions;
using LookOn.Enums;
using Volo.Abp.Application.Dtos;

namespace LookOn.Merchants
{
    public class MerchantDto : FullAuditedEntityDto<Guid>
    {
        public string                           Name           { get; set; }
        public string                           Phone          { get; set; }
        public string                           Address        { get; set; }
        public string                           Email          { get; set; }
        public string                           Fax            { get; set; }
        public Guid                             OwnerAppUserId { get; set; }
        public Guid?                            CategoryId     { get; set; }
        public List<MerchantSocialCommunityDto> Communities    { get; set; }
        public MetricConfigsDto                 MetricConfigs  { get; set; }

        //Terms & Conditions
        public bool      IsTermAccepted { get; set; }
        public DateTime? TermAcceptedAt { get; set; }

        public MerchantDto()
        {
            Communities   = new List<MerchantSocialCommunityDto>();
            // Default Ecom_RetentionThresholdInMonth = 3 Months
            MetricConfigs = new MetricConfigsDto { Ecom_RetentionThresholdInMonth = 3 };
        }
        
        public List<string> GetSocialCommunityIds()
        {
            return Communities.Select(_ => _.SocialCommunityId).Distinct().Where(_ => _.IsNotNullOrSpace()).ToList();
        }
    }

    public class MetricConfigsDto
    {
        public int     Ecom_RetentionThresholdInMonth { get; set; }
        
        public decimal OrderTotalKPI                  { get; set; }
    }

    public class MerchantSocialCommunityDto
    {
        public Guid MerchantId   { get; set; }
        public string MerchantName { get; set; }
        public string SocialCommunityId { get;   set; }
        public string SocialCommunityName { get; set; }
        public string Url { get; set; }
        public string ImageUrl { get; set; }
        public string Description { get; set; }
        
        public SocialCommunityType CommunityType { get; set; }
        
        //
        public SocialCommunityVerificationStatus VerificationStatus { get; set; }
        public string                            VerificationReason { get; set; }
        public DateTime?                         VerifiedAt         { get; set; }
    }
}