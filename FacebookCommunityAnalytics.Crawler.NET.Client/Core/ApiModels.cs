using System.Collections.Generic;
using System.Net;

namespace FacebookCommunityAnalytics.Crawler.NET.Client.Core
{
    public class ApiResponse<T>
    {
        public HttpStatusCode StatusCode { get; set; }
        public bool IsSuccess { get; set; } = true;
        public T Resource { get; set; }
        public ApiErrorResponse ErrorResponse { get; set; }
    }

    public class ApiErrorResponse
    {
        public ApiError error { get; set; }
    }
    
    public class ApiValidationError
    {
        public string message { get; set; }
        public List<string> members { get; set; }
    }

    public class ApiError
    {
        public string code { get; set; }
        public string message { get; set; }
        public string details { get; set; }
        public object data { get; set; }
        public List<ApiValidationError> validationErrors { get; set; }
    }
}