using System.Net;
using FacebookCommunityAnalytics.Crawler.NET.Client.Clients;
using FacebookCommunityAnalytics.Crawler.NET.Client.Core;
using FacebookCommunityAnalytics.Crawler.NET.Client.Entities;
using Newtonsoft.Json;
using RestSharp;

namespace FacebookCommunityAnalytics.Crawler.NET.Client.Consumers
{
    public class AuthApiConsumer: BaseApiConsumer
    {
        public AuthApiConsumer(RestClient client) : base(client)
        {
        }

        public ApiResponse<TokenModel.Response> PostToken(TokenModel.Request request)
        {
            var restRequest = new RestRequest("/connect/token", Method.POST);
            // req.AddJsonBody(request);
            
            restRequest.AddParameter("client_id", request.client_id);
            restRequest.AddParameter("client_secret", request.client_secret);
            restRequest.AddParameter("grant_type", request.grant_type);

            var res = Execute(restRequest);
            if (res.StatusCode == HttpStatusCode.OK)
            {
                var response = new ApiResponse<TokenModel.Response>
                {
                    StatusCode = res.StatusCode
                };
                
                var resource =  JsonConvert.DeserializeObject<TokenModel.Response>(res.Content);
                response.Resource = resource;

                return response;
            }
            else
            {
                return new ApiResponse<TokenModel.Response>
                {
                    StatusCode = res.StatusCode,
                    IsSuccess = false,
                    ErrorResponse = new ApiErrorResponse
                    {
                        error = new ApiError
                        {
                            message = JsonConvert.DeserializeObject<TokenModel.Error>(res.Content)?.error ?? "Failed to get token"
                        }
                    }
                };
            }
        }
        
        public ApiResponse<TokenModel.Response> PostTokenDefault()
        {
            return PostToken(new TokenModel.Request
            {
                client_id = "Api_App",
                client_secret = "Api_App",
                grant_type = "client_credentials"
            });
        }
    }
}