namespace FacebookCommunityAnalytics.Crawler.NET.Client.Enums
{
    public enum GroupSourceType
    {
        Group,
        Page,
        Website,
        Instagram,
        Tiktok = 50
    }

    public enum GroupOwnershipType
    {
        Unknown = 1,
        GDL = 2,
        DAN = 3,
        YAN = 4,
        HappyDay = 5,
        Shopiness = 6,
    }
    
    public enum GroupCategoryType
    {
        Unknown = 1,
        Architecture = 10,
        Beauty = 20,
        FnB = 30,
        TravelAndSport = 40,
        General = 90,
        Dalat = 100,
        HappyDay = 900
    }
}