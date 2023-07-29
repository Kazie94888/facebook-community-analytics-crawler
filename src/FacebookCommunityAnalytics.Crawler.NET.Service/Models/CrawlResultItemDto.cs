using System.Collections.Generic;

namespace FacebookCommunityAnalytics.Crawler.NET.Service.Models
{
    public class CrawlResultItemDto
    {
        public string Url { get; set; }

        /// <summary>
        ///     This is used for affiliate urls (usually called short links)
        /// </summary>
        public List<string> Urls { get; set; }

        public int LikeCount { get; set; }
        public int CommentCount { get; set; }
        public int ShareCount { get; set; }
        public bool IsNotAvailable { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedFuid { get; set; }
        public string CreatedAt { get; set; }
    }
}