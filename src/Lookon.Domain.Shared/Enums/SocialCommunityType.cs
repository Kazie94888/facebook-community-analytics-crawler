namespace LookOn.Enums;

public enum SocialCommunityType
{
    Unknown = 0,
    FacebookPage = 10,
    FacebookGroup = 11,
    Instagram = 20,
    TikTok = 30
}

public enum SocialCommunityVerificationStatus
{
    Pending,
    Approved,
    Rejected,
    InvalidCommunity
}