using System.Diagnostics;
using System.Net;
using FluentDateTime;
using LookOn.Core.Extensions;
using LookOn.Integrations.Haravan.Configs;
using LookOn.Integrations.Haravan.Models.RawModels;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Authenticators.OAuth2;

namespace LookOn.Integrations.Haravan.Components.ApiConsumers
{
    public static class HaravanApiUrl
    {
        public const  string Customers                = "https://apis.haravan.com/com/customers.json";
        public static string OrdersWithDateTimeFilter = "https://apis.haravan.com/com/orders.json?page={0}&limit={1}&created_at_min={2}&created_at_max={3}";
        public static string Orders                   = "https://apis.haravan.com/com/orders.json?page={0}&limit={1}";
        public const  string Products                 = "https://apis.haravan.com/com/products.json?page={0}";
        public const  string Shop                     = "https://apis.haravan.com/com/shop.json";
        public const  string SubscribeWebhook         = "https://webhook.haravan.com/api/subscribe";
    }

    public static class HaravanApiConsumer
    {
        public static async Task<HRVOrderResponseRaw> GetOrders(string accessToken, int page, DateTime? from, DateTime? to, bool rateLimit = false)
        {
            var results    = new HRVOrderResponseRaw { Orders = new List<HRVOrderRaw>() };
            var requestUrl = string.Format(HaravanApiUrl.Orders, page, HaravanGlobalConfig.PageSize);
            if (from.HasValue && to.HasValue)
            {
                requestUrl = string.Format(HaravanApiUrl.OrdersWithDateTimeFilter, page, HaravanGlobalConfig.PageSize, from.Value.ToString("o"), to.Value.ToString("o"));
            }
            var restClient = new RestClient { Authenticator = new OAuth2AuthorizationRequestHeaderAuthenticator(accessToken, "Bearer") };
            var request    = new RestRequest(requestUrl);

            var response = await restClient.GetAsync(request);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                if (response.Content == null) return results;
                var orderResponse = JsonConvert.DeserializeObject<HRVOrderResponseRaw>(response.Content);

                if (orderResponse != null) results.Orders.AddRange(orderResponse.Orders);
            }

            if (rateLimit) await Task.Delay(HaravanGlobalConfig.RateLimitInMs);

            return results;
        }

        public static async Task<HRVBaseResponse> SubscribeWebhook(string accessToken)
        {
            var results    = new HRVBaseResponse();
            var restClient = new RestClient { Authenticator = new OAuth2AuthorizationRequestHeaderAuthenticator(accessToken, "Bearer") };
            var request    = new RestRequest(HaravanApiUrl.SubscribeWebhook);
            await Task.Delay(HaravanGlobalConfig.RateLimitInMs);

            var response = await restClient.PostAsync(request);

            if (response.StatusCode != HttpStatusCode.OK) return results;

            if (response.Content == null) return results;

            var baseResponse = JsonConvert.DeserializeObject<HRVBaseResponse>(response.Content);

            if (baseResponse != null) results = baseResponse;
            return results;
        }
        
        public static async Task<HRVShopResponseRaw> GetShopInfo(string accessToken)
        {
            var results    = new HRVShopResponseRaw();
            var restClient = new RestClient { Authenticator = new OAuth2AuthorizationRequestHeaderAuthenticator(accessToken, "Bearer") };
            var request    = new RestRequest(HaravanApiUrl.Shop);
            await Task.Delay(HaravanGlobalConfig.RateLimitInMs);

            var response = await restClient.GetAsync(request);

            if (response.StatusCode != HttpStatusCode.OK) return results;

            if (response.Content == null) return results;

            var shopResponseRaw = JsonConvert.DeserializeObject<HRVShopResponseRaw>(response.Content);

            if (shopResponseRaw != null) results = shopResponseRaw;
            return results;
        }

        public static string GetLoginUrl(string currentUserId, HaravanConfig config)
        {
            var url = $"{config.url_authorize}?response_mode=form_post"
                    + $"&response_type={config.response_type}"
                    + $"&scope={config.scope_login}"
                    + $"&client_id={config.app_id}"
                    + $"&redirect_uri={string.Format(config.login_callback_url, currentUserId)}"
                    + $"&nonce={config.nonce}";
            return url;
        }

        public static async Task<RestResponse<HRVTokenRaw>> GetAccessToken(string uid, string code, HaravanConfig config)
        {
            var client  = new RestClient();
            var request = new RestRequest(config.url_connect_token, Method.Post);

            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("content-type",  "application/x-www-form-urlencoded");
            request.AddParameter("client_id",     $"{config.app_id}");
            request.AddParameter("client_secret", $"{config.app_secret}");
            request.AddParameter("grant_type",    $"{config.grant_type}");
            request.AddParameter("code",          $"{code}");
            request.AddParameter("redirect_uri",  $"{string.Format(config.login_callback_url, uid)}");

            var response = await client.ExecuteAsync<HRVTokenRaw>(request);

            return response;
        }
    }
}