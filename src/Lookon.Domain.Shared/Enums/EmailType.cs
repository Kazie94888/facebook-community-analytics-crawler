namespace LookOn.Enums;

public enum EmailType
{
    Unknown,
    //MerchantSyncStatus
    MerchantSyncPending = 10,
    MerchantSyncEcomOrderCompleted = 11,
    MerchantSyncSocialUserRequestCompleted = 12,
    MerchantSyncSocialUserScanCompleted = 13,
    MerchantSyncSocialUserSyncCompleted = 14,
    
    PageMetric_MerchantSyncSocialInsightRequestInitiated = 20,
    PageMetric_MerchantSyncSocialInsightRequestCompleted = 21,
    PageMetric_MerchantSyncSocialInsightScanCompleted = 22,
    PageMetric_MerchantDashboardDataReady = 29,
    //
    // Page2_MerchantSyncSocialInsightRequestInitiated = 30,
    // Page2_MerchantSyncSocialInsightRequestCompleted = 31,
    // Page2_MerchantSyncSocialInsightScanCompleted    = 32,
    // Page2_MerchantDashboardDataReady                = 39,
    
    //Subscription
    SubscriptionAdded = 51,
    SubscriptionExtend = 52,
    SubscriptionUpgrade = 53,
    SubscriptionExpirationSoon1Month = 54,
    SubscriptionExpirationSoon1Week = 55,
    SubscriptionExpirationSoon1Day = 56,
    
    //Account
    NewAccount = 60,
    
    //Social connection
    NewCommunity = 70,
    VerifyCommunity = 71
}