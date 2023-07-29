using System;
using System.Net;
using System.Security.Authentication;
using System.Threading;
using FacebookCommunityAnalytics.Crawler.NET.Client.Core;
using FacebookCommunityAnalytics.Crawler.NET.Core.Extensions;
using Newtonsoft.Json;
using RestSharp;

namespace FacebookCommunityAnalytics.Crawler.NET.Client.Clients
{
    public abstract class BaseApiConsumer
    {
        private RestClient Client { get; set; }

        protected BaseApiConsumer(RestClient client)
        {
            Client = client;
        }

        protected ApiResponse<T> HandleResponse<T>(IRestResponse response)
        {
            var res = new ApiResponse<T>
            {
                StatusCode = response.StatusCode
            };

            var content = response.Content;
            switch (response.StatusCode)
            {
                case HttpStatusCode.OK:
                case HttpStatusCode.Created:
                    if (content.IsNotNullOrEmpty())
                    {
                        res.Resource = JsonConvert.DeserializeObject<T>(content);
                    }
                    break;
                
                case HttpStatusCode.NoContent:
                    break;
                
                case HttpStatusCode.NotFound:
                case HttpStatusCode.BadRequest:
                    res.IsSuccess = false;
                    if (content.IsNotNullOrEmpty())
                    {
                        var er = JsonConvert.DeserializeObject<ApiErrorResponse>(content);
                        res.ErrorResponse = er;
                    }
                    break;
                
                case HttpStatusCode.Unauthorized:
                    throw new AuthenticationException("Please authenticate before attempting to use the API");
                
                case HttpStatusCode.Forbidden:
                    throw new AuthenticationException("Please authenticate before attempting to use the API - Forbidden");
                
                case HttpStatusCode.InternalServerError:
                case HttpStatusCode.ServiceUnavailable:
                    throw new AuthenticationException("Can't reach API server. Please try again later");
            }

            return res;
        }

        protected IRestResponse Execute(RestRequest restRequest)
        {
            int retry = 10;
            int count = 0;
            IRestResponse restResponse = null;
            while (count < retry)
            {
                restResponse = Client.Execute(restRequest);
                if (restResponse.ResponseStatus == ResponseStatus.Error &&
                    (restResponse.ErrorMessage.Contains("No connection could be made because the target machine actively refused it") ||
                     restResponse.ErrorMessage.Contains("Can't reach API server. Please try again later")))
                {
                    count += 1;
                    if (count > retry)
                    {
                        return restResponse;
                    }
                    Console.WriteLine("Wait for API service online");
                    Wait(60000);
                }
                else
                {
                    return restResponse;
                }
            }

            return restResponse;
        }

        private void Wait(int timeInMilliseconds)
        {
            using (var autoResetEvent = new AutoResetEvent(false))
            {
                autoResetEvent.WaitOne(timeInMilliseconds);
            }
        }
    }
}