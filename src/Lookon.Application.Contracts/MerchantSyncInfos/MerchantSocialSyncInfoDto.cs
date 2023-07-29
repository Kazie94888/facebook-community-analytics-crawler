using System;
using System.Collections.Generic;
using LookOn.Enums;

namespace LookOn.MerchantSyncInfos;

public class MerchantSocialSyncInfoDto
{
    public List<MerchantSocialUserSyncInfo> Communities { get; set; }
}

public class MerchantSocialUserSyncInfo
{
    public Guid? MerchantSyncInfoId         { get; set; }
    public string                            MerchantName       { get; set; }
    //
    public string                            SocialCommunityName  { get; set; }
    public string                            SocialCommunityId  { get; set; }
    public string                            Url  { get; set; }
    public SocialCommunityType               CommunityType      { get; set; }
    public SocialCommunityVerificationStatus VerificationStatus { get; set; }
    public string                            VerificationReason { get; set; }
    public DateTime?                         VerifiedAt         { get; set; }
}