using System.Collections.Generic;
using FacebookCommunityAnalytics.Crawler.NET.Client.Enums;

namespace FacebookCommunityAnalytics.Crawler.NET.Client.Entities
{
    public class GetUncrawledGroupApiRequest
    {
        public GroupSourceType GroupSourceType { get; set; }
        public bool IgnoreTime { get; set; }
    }

    public class GetUncrawledGroupApiResponse
    {
        public int Count { get; set; }
        public List<Group> Groups { get; set; }
    }
}