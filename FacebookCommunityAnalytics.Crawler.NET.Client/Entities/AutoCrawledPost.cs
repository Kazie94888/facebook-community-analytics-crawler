using System.Collections.Generic;

namespace FacebookCommunityAnalytics.Crawler.NET.Client.Entities
{
    
    public class SaveAutoCrawlResultRequest
    {
        public List<AutoCrawledPost> Items { get; set; }
    }
    public class SaveAutoCrawlResultResponse
    {
        public bool Success { get; set; }
    }
}