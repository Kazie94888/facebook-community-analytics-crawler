using FacebookCommunityAnalytics.Crawler.NET.Client.Clients;
using FacebookCommunityAnalytics.Crawler.NET.Client.Core;
using FacebookCommunityAnalytics.Crawler.NET.Client.Entities;
using RestSharp;

namespace FacebookCommunityAnalytics.Crawler.NET.Client.Consumers
{
    public class ViotpConsumer : BaseApiConsumer
    {
        private const string ViotpToken = "15493e5c63874108a96c40d2b8e68831";

        public ViotpConsumer(RestClient client) : base(client)
        {
        }
        
        public ApiResponse<RequestPhoneNumberResponse> RequestPhoneNumber()
        {
            // service Id = 5 : Hotmail/Outlook/Azure (Microsoft)
            // network Mobile phone
            var restRequest = new RestRequest($"/request/getv2?token={ViotpToken}&serviceId=5&network=MOBIFONE|VINAPHONE|VIETTEL|VIETNAMOBILE|ITELECOM", Method.GET);
            
            return HandleResponse<RequestPhoneNumberResponse>(Execute(restRequest));
        }

        public ApiResponse<SessionResponse> GetPhoneCode(string requestId)
        {
            var restRequest = new RestRequest($"/session/getv2?requestId={requestId}&token={ViotpToken}", Method.GET);
            
            return HandleResponse<SessionResponse>(Execute(restRequest));
        }
    }
}