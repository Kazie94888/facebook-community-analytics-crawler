using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using LookOn.Core.Extensions;

namespace LookOn.Merchants
{
    public class MerchantUpdateDto
    {
        [Required] [StringLength(MerchantConsts.NameMaxLength)]  public string                           Name           { get; set; }
        [Required] [StringLength(MerchantConsts.PhoneMaxLength)] public string                           Phone          { get; set; }
        public                                                          string                           Address        { get; set; }
        [Required] [EmailAddress] public                                string                           Email          { get; set; }
        public                                                          string                           Fax            { get; set; }
        public                                                          Guid                             OwnerAppUserId { get; set; }
        public                                                          Guid?                            CategoryId     { get; set; }
        public                                                          List<MerchantSocialCommunityDto> Communities    { get; set; }
        public                                                          MetricConfigsDto           MetricConfigs  { get; set; }

        //Terms & Conditions
        public bool      IsTermAccepted { get; set; }
        public DateTime? TermAcceptedAt { get; set; }

        public MerchantUpdateDto()
        {
            Communities = new List<MerchantSocialCommunityDto>();
        }
        
        public List<string> GetSocialCommunityIds()
        {
            return Communities.Select(_ => _.SocialCommunityId).Distinct().Where(_ => _.IsNotNullOrSpace()).ToList();
        }
    }
}