namespace LookOn.Enums;

public enum MerchantSyncStatus
{
    Pending                           = 0,
    SyncEcomOrderCompleted            = 10,
    SyncSocialUserRequestCompleted    = 20,
    SyncSocialUserScanCompleted       = 21,
    SyncSocialUserSyncCompleted       = 22,
    SyncSocialInsightRequestInitiated = 30,
    SyncSocialInsightRequestCompleted = 31,
    SyncSocialInsightScanCompleted    = 32,
    /// <summary>
    /// sync completed = data ready
    /// </summary>
    DashboardDataReady = 90
}