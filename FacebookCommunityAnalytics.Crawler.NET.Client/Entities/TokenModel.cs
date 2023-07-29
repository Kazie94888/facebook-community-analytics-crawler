namespace FacebookCommunityAnalytics.Crawler.NET.Client.Entities
{
    public class TokenModel
    {
        public class Request
        {
            public string client_id { get; set; }
            public string client_secret { get; set; }
            public string grant_type { get; set; }
        }

        public class Response
        {
            public string access_token { get; set; }
            public long expires_in { get; set; }
            public string token_type { get; set; }
            public string scope { get; set; }
        }

        public class Error
        {
            public string error { get; set; }
        }
    }
}