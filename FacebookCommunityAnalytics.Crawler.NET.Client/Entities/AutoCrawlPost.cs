using System;
using System.Collections.Generic;

namespace FacebookCommunityAnalytics.Crawler.NET.Client.Entities
{
    
    public class SaveAutoCrawlResultRequest
    {
        public List<AutoCrawlPost> Items { get; set; }
    }
    public class SaveAutoCrawlResultResponse
    {
        public bool Success { get; set; }
    }

    public class AutoCrawlPost
    {
        public PostSourceType PostSourceType { get; set; }
        
        public string Url { get; set; }

        /// <summary>
        /// This is used for affiliate urls (usually called short links)
        /// </summary>
        public List<string> Urls { get; set; }

        public string Content { get; set; }
        public List<string> HashTags { get; set; }
        
        public int LikeCount { get; set; }
        public int CommentCount { get; set; }
        public int ShareCount { get; set; }
        
        public DateTime CreatedAt { get; set; }
        
        public string CreatedBy { get; set; }
        public string CreateFuid { get; set; }
        
        /// <summary>
        /// No need to crawl, just copied from API
        /// </summary>
        public bool IsNotAvailable { get; set; }
        
        // TODO Vu.Nguyen: later crawl this
        public string GroupFid { get; set; }
        public string CampaignCode { get; set; }
        public string PartnerCode { get; set; }
    }
}