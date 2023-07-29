namespace FacebookCommunityAnalytics.Crawler.NET.Client.Enums
{
    public enum AccountType
    {
        Unknown = 1,
        All = 10,
        NodeFacebook = 100,
        NodeInstagram = 101,

        NETFacebookGroupSelectivePost = 200,
        NETFacebookGroupPost = 201,
        NETFacebookGroupUserPost = 202,

        NETFacebookPagePost = 300,

        NETInstagram = 400,

        NETTelegram = 500,

        NETWebsite = 600,

        TestLogin = 1000,
        TestLoginComplete = 1001,

        AutoPostGroup = 2000,
        AutoPostPage = 2001,
        AutoPostInstagram = 2002
    }

    public enum AccountStatus
    {
        Unknown = 1,
        New = 10,
        Active = 20,
        Deactive = 21,
        LoginApprovalNeeded = 30,
        BlockedTemporary = 31,
        Banned = 99
    }
}
