using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LookOn.Merchants
{
    public class MerchantCreateDto
    {
        [Required] [StringLength(MerchantConsts.NameMaxLength)]  public string                           Name           { get; set; }
        [Required] [StringLength(MerchantConsts.PhoneMaxLength)] public string                           Phone          { get; set; }
        [Required] [EmailAddress]                                public string                           Email          { get; set; }
        public                                                          string                           Address        { get; set; }
        public                                                          string                           Fax            { get; set; }
        public                                                          Guid                             OwnerAppUserId { get; set; }
        public                                                          Guid?                            CategoryId     { get; set; }
        public                                                          List<MerchantSocialCommunityDto> Communities    { get; set; }

        //Terms & Conditions
        public bool      IsTermAccepted { get; set; }
        public DateTime? TermAcceptedAt { get; set; }

        public MerchantCreateDto()
        {
            Communities = new List<MerchantSocialCommunityDto>();
        }
    }
}