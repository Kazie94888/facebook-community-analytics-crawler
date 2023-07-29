using System.Collections.Generic;

namespace FacebookCommunityAnalytics.Crawler.NET.Client.Entities
{
    public class CrawlResult
    {
        public CrawlResult()
        {
            Success = true;
            Posts = new List<AutoCrawledPost>();
            Status = CrawlStatus.OK;
        }

        public CrawlResult(CrawlStatus status)
        {
            Posts = new List<AutoCrawledPost>();
            Status = status;
            Success = status == CrawlStatus.OK || 
                      status == CrawlStatus.PostUnavailable;
        }
        
        public bool Success { get; set; }
        public CrawlStatus Status { get; set; }
        public List<AutoCrawledPost> Posts { get; set; }
    }

    public enum CrawlStatus
    {
        OK = 100,
        PostUnavailable = 101,
        GroupUnavailable = 102,
        
        AccountBanned = 200,
        BlockedTemporary = 201,
        
        LoginFailed = 400,
        LoginApprovalNeeded = 401,
        
        UnknownFailure = 999
    }
}