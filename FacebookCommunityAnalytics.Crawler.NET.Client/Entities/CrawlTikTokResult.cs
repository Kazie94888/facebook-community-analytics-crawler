using System.Collections.Generic;

namespace FacebookCommunityAnalytics.Crawler.NET.Client.Entities
{
    public class CrawlTikTokResult
    {
        public CrawlTikTokResult()
        {
            Success = true;
        }
        
        public bool Success { get; set; }

        public ChannelStat ChannelStat { get; set; }
        public List<TiktokVideoDto> TiktokVideos { get; set; }
    }
}