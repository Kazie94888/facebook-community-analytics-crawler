using System;
using System.Collections.Generic;
using FacebookCommunityAnalytics.Crawler.NET.Client.Clients;
using FacebookCommunityAnalytics.Crawler.NET.Client.Core;
using FacebookCommunityAnalytics.Crawler.NET.Client.Entities;
using FacebookCommunityAnalytics.Crawler.NET.Client.Enums;
using FacebookCommunityAnalytics.Crawler.NET.Core.Extensions;
using RestSharp;

namespace FacebookCommunityAnalytics.Crawler.NET.Client.Consumers
{
    public class CrawlApiConsumer : BaseApiConsumer
    {
        public CrawlApiConsumer(RestClient client) : base(client)
        {
        }


        public ApiResponse<UncrawledPostsResponse> GetUncrawledPosts(UncrawledPostsRequest request)
        {
            var queryString = request.ToQueryString();
            var restRequest = new RestRequest($"/app/crawl/uncrawled?{queryString}", Method.GET);

            return HandleResponse<UncrawledPostsResponse>(Execute(restRequest));
        }
        public ApiResponse<GetUncrawledGroupApiResponse> GetUncrawledGroups(GetUncrawledGroupApiRequest request)
        {
            var queryString = request.ToQueryString();
            var restRequest = new RestRequest($"/app/crawl/uncrawled-groups?{queryString}", Method.GET);
            restRequest.AddParameter("GroupSourceType", request.GroupSourceType);

            return HandleResponse<GetUncrawledGroupApiResponse>(Execute(restRequest));
        }

        public ApiResponse<GetUncrawledGroupUserApiResponse> GetUncrawledGroupUsers(GetUncrawledGroupUserApiRequest request)
        {
            var queryString = request.ToQueryString();
            var restRequest = new RestRequest($"/app/crawl/uncrawled-group-users?{queryString}", Method.GET);

            return HandleResponse<GetUncrawledGroupUserApiResponse>(Execute(restRequest));
        }

        public void PostCrawlResult(SaveCrawlResultApiRequest request)
        {
            var restRequest = new RestRequest("/app/crawl/crawl-result", Method.POST);
            restRequest.AddJsonBody(request);

            Execute(restRequest);
        }
        
        public ApiResponse<SaveAutoCrawlResultApiResponse> PostAutoCrawlResult(SaveAutoCrawlResultApiRequest apiRequest)
        {
            var restRequest = new RestRequest("/app/crawl/auto-crawl-result", Method.POST);
            restRequest.AddJsonBody(apiRequest);
            
            return HandleResponse<SaveAutoCrawlResultApiResponse>(Execute(restRequest));
        }
        
        public ApiResponse<List<AccountProxyItem>> GetAccountProxies(GetAccountProxiesRequest request)
        {
            var queryString = request.ToQueryString();
            var req = request.AccountType == AccountType.Unknown ? new RestRequest($"/app/crawl/account-proxies", Method.GET) : new RestRequest($"/app/crawl/account-proxies?{queryString}", Method.GET);
            

            return HandleResponse<List<AccountProxyItem>>(Execute(req));
        }

        public ApiResponse<List<AccountDto>> GetAccounts(GetAccountsRequest request)
        {
            var queryString = request.ToQueryString();
            var req = new RestRequest($"/app/crawl/accounts?{queryString}", Method.GET);

            return HandleResponse<List<AccountDto>>(Execute(req));
        }

        public void ResetAccountsCrawlStatus(ResetAccountsCrawlStatusRequest resetAccountsCrawlStatusRequest)
        {
            var restRequest = new RestRequest("/app/crawl/reset-accounts-crawl-status", Method.POST);
            restRequest.AddJsonBody(resetAccountsCrawlStatusRequest);
            
            Execute(restRequest);
        }

        public ApiResponse<IList<AutoPostFacebookNotDoneDto>> GetAutoPostFacebookNotDone()
        {
            var req = new RestRequest($"/app/auto-post-facebook/get-posts-not-done", Method.GET);

            return HandleResponse<IList<AutoPostFacebookNotDoneDto>>(Execute(req));
        }

        public void UpdateLikeComment(UpdateLikeCommentDto updateLikeCommentDto)
        {
            var restRequest = new RestRequest($"/app/auto-post-facebook/update-number-like-comment", Method.POST);
            
            restRequest.AddJsonBody(updateLikeCommentDto);
            
            Execute(restRequest);
        }
    }
}