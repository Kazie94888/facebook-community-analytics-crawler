using LookOn.Users;
using LookOn.Categories;
using System;
using System.Collections.Generic;
using System.Linq;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;
using JetBrains.Annotations;
using LookOn.Core.Extensions;
using LookOn.Enums;
using Volo.Abp;

namespace LookOn.Merchants;

public class MerchantSocialCommunity
{
    public string              SocialCommunityId   { get; set; }
    public string              SocialCommunityName { get; set; }
    public SocialCommunityType CommunityType       { get; set; }
    public string              Url                 { get; set; }
    public string              ImageUrl            { get; set; }
    public string              Description         { get; set; }

    //
    public SocialCommunityVerificationStatus VerificationStatus { get; set; }
    public string                            VerificationReason { get; set; }
    public DateTime?                         VerifiedAt         { get; set; }
}

public class MetricConfigs
{
    public int     Ecom_RetentionThresholdInMonth { get; set; }
    public decimal OrderTotalKPI                  { get; set; }
}

public class Merchant : FullAuditedEntity<Guid>, IMultiTenant
{
    public virtual             Guid?                         TenantId       { get; set; }
    [NotNull]   public virtual string                        Name           { get; set; }
    [NotNull]   public virtual string                        Phone          { get; set; }
    [CanBeNull] public virtual string                        Address        { get; set; }
    [NotNull]   public virtual string                        Email          { get; set; }
    [CanBeNull] public virtual string                        Fax            { get; set; }
    public                     Guid                          OwnerAppUserId { get; set; }
    public                     Guid?                         CategoryId     { get; set; }
    public                     List<MerchantSocialCommunity> Communities    { get; set; }
    public                     MetricConfigs                 MetricConfigs  { get; set; }

    //Terms & Conditions
    public bool      IsTermAccepted { get; set; }
    public DateTime? TermAcceptedAt { get; set; }

    public Merchant()
    {
        Communities   = new List<MerchantSocialCommunity>();
        // Default Ecom_RetentionThresholdInMonth = 3 Months
        MetricConfigs = new MetricConfigs { Ecom_RetentionThresholdInMonth = 3 };
    }

    public Merchant(Guid   id,
                    Guid   ownerAppUserId,
                    string name,
                    string phone,
                    string address,
                    string email,
                    string fax)
    {
        Id = id;
        Check.NotNull(name, nameof(name));
        Check.Length(name,
                     nameof(name),
                     MerchantConsts.NameMaxLength,
                     0);
        Check.NotNull(phone, nameof(phone));
        Check.Length(phone,
                     nameof(phone),
                     MerchantConsts.PhoneMaxLength,
                     0);
        Check.NotNull(email, nameof(email));
        Name           = name;
        Phone          = phone;
        Address        = address;
        Email          = email;
        Fax            = fax;
        OwnerAppUserId = ownerAppUserId;
    }

    public List<string> GetSocialCommunityIds()
    {
        return Communities.Select(_ => _.SocialCommunityId).Distinct().Where(_ => _.IsNotNullOrSpace()).ToList();
    }
}