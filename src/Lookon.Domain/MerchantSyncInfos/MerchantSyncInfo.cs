using System;
using System.Collections.Generic;
using System.Linq;
using FluentDate;
using FluentDateTime;
using Volo.Abp.Domain.Entities.Auditing;
using JetBrains.Annotations;
using LookOn.Configs;
using LookOn.Enums;
using LookOn.Integrations.Datalytis.Configs;
using LookOn.Integrations.Datalytis.Models.Enums;

namespace LookOn.MerchantSyncInfos
{
    public class MerchantSyncInfo : FullAuditedEntity<Guid>
    {
        public                     Guid?              MerchantId         { get; set; }
        [CanBeNull] public virtual string             MerchantEmail      { get; set; }
        public                     MerchantSyncStatus MerchantSyncStatus { get; set; }
        //public                     MerchantSyncStatus Page2SyncStatus { get; set; }

        // scan info
        public MerchantEcomScan   EcomScan   { get; set; }
        public MerchantSocialScan SocialScan { get; set; }

        public MerchantSyncInfo()
        {
            EcomScan   = new MerchantEcomScan();
            SocialScan = new MerchantSocialScan();
        }
    }

    public class MerchantEcomScan
    {
        public MerchantJobStatus RawOrderSyncStatus   { get; set; }
        public DateTime?         LastRawOrderSyncedAt { get; set; }
        public DateTime?         LastRawOrderRanAt    { get; set; }
        public bool ShouldSyncRawOrder
        {
            get
            {
                {
                    var isObsolete = LastRawOrderRanAt.HasValue && LastRawOrderRanAt.Value.IsBefore(GlobalConfig.JobTimeOutInHours.Hours().Ago());
                    return RawOrderSyncStatus != MerchantJobStatus.InProgress || isObsolete;
                }
            }
        }

        //
        public MerchantJobStatus CleanOrderSyncStatus   { get; set; }
        public DateTime?         LastCleanOrderRanAt    { get; set; }
        public DateTime?         LastCleanOrderSyncedAt { get; set; }

        public bool ShouldSyncCleanOrder
        {
            get
            {
                {
                    var isObsolete = LastCleanOrderRanAt.HasValue && LastCleanOrderRanAt.Value.IsBefore(GlobalConfig.JobTimeOutInHours.Hours().Ago());
                    return CleanOrderSyncStatus != MerchantJobStatus.InProgress || isObsolete;
                }
            }
        }
        //
        public double OrderSyncIntervalInDays { get; set; }

        // first sync
        public bool      IsFirstSyncCompleted { get; set; }
        public DateTime? FirstSyncCompletedAt { get; set; }

        public MerchantEcomScan()
        {
            CleanOrderSyncStatus = MerchantJobStatus.Pending;
            RawOrderSyncStatus   = MerchantJobStatus.Pending;
        }
    }

    public class MerchantSocialScan
    {
        public List<MerchantUserScan> UserScans { get; set; }
        public MerchantInsightScan InsightScan { get; set; }

        public MerchantSocialScan()
        {
            UserScans   = new List<MerchantUserScan>();
            InsightScan = new MerchantInsightScan();
        }
    }

    public class MerchantUserScan
    {
        public string                            SocialCommunityId   { get; set; }
        // REQUEST SCAN
        public bool      IsRequestCompleted { get; set; }
        public int?      RequestId          { get; set; }
        public DateTime? RequestedAt        { get; set; }

        // CHECK SCAN STATUS
        public bool      IsScanCompleted { get; set; }
        public DateTime? ScanCompletedAt { get; set; }
        public DateTime? ScanCheckedAt   { get; set; }

        // SYNC (call API and store to DB)
        // public bool      IsSyncCompleted { get; set; }
        public MerchantJobStatus SyncStatus { get; set; }
        public DateTime?         SyncedAt   { get; set; }
        public bool ShouldSync =>
            SyncedAt is null
         || SyncedAt.Value.IsBefore(DatalytisGlobalConfig.SocialUserSyncAfterDays.Days().Ago());

        public MerchantUserScan()
        {
            SyncStatus = MerchantJobStatus.Pending;
        }
    }
    public class MerchantInsightScan
    {
        // Page 1
        /// <summary>
        /// 5000 sample user of datalytis
        /// </summary>
        // public List<Guid> DatalytisUserIds { get; set; }
    
        //INSIGHT
        // 2000 sample insight of users of datalytis
        public List<string>                     InsightSocialUids   { get; set; }
        public List<MerchantInsightScanRequest> InsightScanRequests { get; set; }

        public MerchantInsightScan()
        {
            InsightSocialUids             = new List<string>();
            InsightScanRequests           = new List<MerchantInsightScanRequest>();
        }
    }
    
    

    public class MerchantInsightScanRequest
    {
        // request
        public List<string> Uids        { get; set; }
        public bool         IsRequested { get; set; }
        public string       RequestId   { get; set; }
        public DateTime?    RequestedAt { get; set; }

        // scan
        public bool      IsScanned { get; set; }
        public DateTime? ScannedAt { get; set; }

        // sync
        // public bool      IsSynced { get; set; }
        public MerchantJobStatus SyncStatus { get; set; }
        public DateTime?         SyncedAt   { get; set; }

        // status
        public DatalytisScanStatus DataStatus { get; set; }

        public MerchantInsightScanRequest()
        {
            SyncStatus = MerchantJobStatus.Pending;
            Uids       = new List<string>();
        }
    }
}