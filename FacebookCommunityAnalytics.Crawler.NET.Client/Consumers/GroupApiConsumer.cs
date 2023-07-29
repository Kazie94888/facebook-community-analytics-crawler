using FacebookCommunityAnalytics.Crawler.NET.Client.Clients;
using FacebookCommunityAnalytics.Crawler.NET.Client.Core;
using FacebookCommunityAnalytics.Crawler.NET.Client.Entities;
using RestSharp;

namespace FacebookCommunityAnalytics.Crawler.NET.Client.Consumers
{
    public class GroupApiConsumer : BaseApiConsumer
    {
        public GroupApiConsumer(RestClient client) : base(client)
        {
        }

        public ApiResponse<GetGroupsResponse> GetList(GetGroupsRequest request)
        {
            // var queryString = request.ToQueryString();
            var restRequest = new RestRequest($"/app/groups", Method.GET);
            
            restRequest.AddParameter("MaxResultCount", request.MaxResultCount);
            restRequest.AddParameter("Fid", request.Fid);
            // restRequest.AddParameter("client_secret", request.client_secret);
            // restRequest.AddParameter("grant_type", request.grant_type);
            
            return HandleResponse<GetGroupsResponse>(Execute(restRequest));
        }
    }
}