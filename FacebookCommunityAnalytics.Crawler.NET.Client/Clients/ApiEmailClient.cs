using FacebookCommunityAnalytics.Crawler.NET.Client.Configurations;
using FacebookCommunityAnalytics.Crawler.NET.Client.Consumers;
using FacebookCommunityAnalytics.Crawler.NET.Core.Helpers;
using RestSharp;

namespace FacebookCommunityAnalytics.Crawler.NET.Client.Clients
{
    public class ApiEmailClient
    {
        private readonly RestClient _client;
        private string _apiUrl;

        public ApiEmailClient(ApiConfig config)
        {
            _apiUrl = config.ApiEmailUrl;
            _client = RestHelper.CreateClient(_apiUrl);
            _anonymousConsumer = new AnonymousConsumer(_client);
        }
        
        private readonly AnonymousConsumer _anonymousConsumer;
        
        public AnonymousConsumer Anonymous
        {
            get
            {
                return _anonymousConsumer;
            }
        }
    }
}