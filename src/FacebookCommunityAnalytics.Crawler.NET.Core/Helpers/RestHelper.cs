using RestSharp;

namespace FacebookCommunityAnalytics.Crawler.NET.Core.Helpers
{
    public static class RestHelper
    {
        public const string ApiDatetimeFormat = "yyyy-MM-ddTHH:mm:ss";

        public static RestClient CreateClient(string baseUrl, int timeoutInSecs = 90)
        {
            var client = new RestClient(baseUrl);

            // Override with Newtonsoft JSON Handler
            client.AddHandler("application/json", NewtonsoftJsonSerializer.Default);
            client.AddHandler("text/json", NewtonsoftJsonSerializer.Default);
            client.AddHandler("text/x-json", NewtonsoftJsonSerializer.Default);
            client.AddHandler("text/javascript", NewtonsoftJsonSerializer.Default);
            client.AddHandler("*+json", NewtonsoftJsonSerializer.Default);
            client.Timeout = 90000; //90 seconds
            return client;
        }
    }
}