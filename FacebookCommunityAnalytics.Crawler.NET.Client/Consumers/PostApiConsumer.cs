using FacebookCommunityAnalytics.Crawler.NET.Client.Clients;
using RestSharp;

namespace FacebookCommunityAnalytics.Crawler.NET.Client.Consumers
{
    public class PostApiConsumer : BaseApiConsumer
    {
        public PostApiConsumer(RestClient client) : base(client)
        {
        }
    }
}