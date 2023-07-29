using System;
using LookOn.Enums;

namespace LookOn.MerchantSyncInfos;

public class MerchantEcomScanDto
{
    public MerchantJobStatus RawOrderSyncStatus   { get; set; }
    public DateTime?         LastRawOrderSyncedAt { get; set; }
    public DateTime?         LastRawOrderRanAt    { get; set; }

    //
    public MerchantJobStatus CleanOrderSyncStatus   { get; set; }
    public DateTime?         LastCleanOrderRanAt    { get; set; }
    public DateTime?         LastCleanOrderSyncedAt { get; set; }

    // first sync
    public bool      IsFirstSyncCompleted { get; set; }
    public DateTime? FirstSyncCompletedAt { get; set; }
}