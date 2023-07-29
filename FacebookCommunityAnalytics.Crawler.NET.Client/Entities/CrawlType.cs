namespace FacebookCommunityAnalytics.Crawler.NET.Client.Entities
{
    public enum CrawlType
    {
        None = 0,
        
        GroupInfo = 10,
        GroupUser = 11,
        GroupPost = 12,
        GroupSelectivePost = 13,
        AllGroupPosts = 14,
        AutoLike = 15,
        AutoPost = 16,
        
        PageInfo = 20,
        PagePost = 21,
        PageSelectivePost = 22,
        AllPagePosts = 23,
        
        ByHashTag = 30,
        AllHashTags = 31,
        
        TiktokChannelStats = 40,
        TiktokVideos = 41,
        TiktokStats = 42,
        TiktokVideosMonthly = 43,
        TiktokMCNvideos = 44,
        TiktokTrending = 45,
        TiktokMCNVietNamChanel = 46,
        
        UnlockEmails = 50,

        LoginManual = 900,
        LoginAuto = 901,
        JoinGroupManual = 902,
        DeleteComment = 903,
        Twitter = 904,
        Medium = 905,
        Telegram = 906
    }
}