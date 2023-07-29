namespace LookOn.Enums;

public enum SubscriptionType
{
    Trial    = 1,
    Starter    = 10,
    Growth     = 20,
    Plus       = 30,
    Enterprise = 90
}

public enum SubscriptionStatus
{
    Pending   = 10,
    Active    = 20,
    Suspended = 30,
    Expired   = 40,
    Canceled  = 50
}

public enum SubscriptionMethod
{
    Unknown = 0,
    BankWire = 10,
    Haravan = 20,
}

public class SubscriptionConfig
{
    public SubscriptionType SubscriptionType { get; set; }
    public int              MaxSocialUser    { get; set; }
    /// <summary>
    /// Condition to request API:
    /// 1. Gender
    /// 2. Relationship
    /// 3. DateOfBirth
    /// </summary>
    public int    MaxSocialUserInsight             { get; set; }
    public decimal MaxSocialUserInsightAddInPercent { get; set; }
    public int    MaxSocialPage                    { get; set; }
    public bool   HasDailyReport                   { get; set; }
    public bool   HasWeeklyReport                  { get; set; }
    public bool   HasMonthlyReport                 { get; set; }
}

public enum SubscriptionEmailStatus
{
    Added = 10,
    Extend = 20,
    Upgrade = 30,
    ExpirationSoon1Month = 40,
    ExpirationSoon1Week = 50,
    ExpirationSoon1Day = 60
}