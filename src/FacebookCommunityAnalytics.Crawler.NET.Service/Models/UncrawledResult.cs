using System.Collections.Generic;

namespace FacebookCommunityAnalytics.Crawler.NET.Service.Models
{
    public class UncrawledResult
    {
        public long Total { get; set; }
        public List<string> Urls { get; set; }
    }
}