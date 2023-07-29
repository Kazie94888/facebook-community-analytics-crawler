using FacebookCommunityAnalytics.Crawler.NET.Client.Clients;
using FacebookCommunityAnalytics.Crawler.NET.Client.Core;
using RestSharp;

namespace FacebookCommunityAnalytics.Crawler.NET.Client.Consumers
{
    public class AnonymousConsumer : BaseApiConsumer
    {
        public AnonymousConsumer(RestClient client) : base(client)
        {
        }
        
        public ApiResponse<string> GetSecurityCode()
        {
            var restRequest = new RestRequest($"/app/anonymous/SecurityCode", Method.GET);

            return HandleResponse<string>(Execute(restRequest));
        }
    }
}