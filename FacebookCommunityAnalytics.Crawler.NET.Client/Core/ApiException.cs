using System;

namespace FacebookCommunityAnalytics.Crawler.NET.Client.Core
{
    public class ApiException<T> : Exception
    { 
        public ApiResponse<T> Response { get; set; }

        public ApiException(ApiResponse<T> response, string message)
            : base(message)
        {
            Response = response;
        }
    }
}