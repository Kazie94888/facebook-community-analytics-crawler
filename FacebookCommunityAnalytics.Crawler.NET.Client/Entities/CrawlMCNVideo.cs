using System.Collections.Generic;

namespace FacebookCommunityAnalytics.Crawler.NET.Client.Entities
{
    public class CrawlMCNVideo
    {
        public string Hashtag { get; set; }
        public List<ChannelVideo> ChannelVideos { get; set; }
    }

    public class ChannelVideo
    {
        public ChannelStat ChannelStat { get; set; }
        public List<TiktokVideoDto> TiktokVideos { get; set; }
    }

    public class VideoInformation
    {
        public string ChannelName { get; set; }
        public string VideoUrl { get; set; }
    }
}