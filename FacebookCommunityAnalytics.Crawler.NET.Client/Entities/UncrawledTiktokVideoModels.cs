using System.Collections.Generic;

namespace FacebookCommunityAnalytics.Crawler.NET.Client.Entities
{
    public class UncrawledTiktokVideoResponse
    {
        public List<UncrawledTiktokVideoDto> Items { get; set; }
    }

    public class UncrawledTiktokVideoDto
    {
        public string VideoUrl { get; set; }
        public string VideoId { get; set; }
        public string ChannelId { get; set; }
        public string ChannelUrl { get; set; }
    }
}