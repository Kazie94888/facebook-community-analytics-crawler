using FacebookCommunityAnalytics.Crawler.NET.Client.Clients;
using RestSharp;

namespace FacebookCommunityAnalytics.Crawler.NET.Client.Consumers
{
    public class ScheduledPostApiConsumer : BaseApiConsumer
    {
        public ScheduledPostApiConsumer(RestClient client) : base(client)
        {
        }
    }
}