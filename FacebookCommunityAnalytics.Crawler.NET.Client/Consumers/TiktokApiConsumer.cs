using System.Collections.Generic;
using FacebookCommunityAnalytics.Crawler.NET.Client.Clients;
using FacebookCommunityAnalytics.Crawler.NET.Client.Core;
using FacebookCommunityAnalytics.Crawler.NET.Client.Entities;
using FacebookCommunityAnalytics.Crawler.NET.Core.Extensions;
using RestSharp;

namespace FacebookCommunityAnalytics.Crawler.NET.Client.Consumers
{
    public class TiktokApiConsumer : BaseApiConsumer
    {
        public TiktokApiConsumer(RestClient client) : base(client)
        {
        }

        public ApiResponse<SaveChannelStatApiResponse> PostAutoCrawlChannelStatsResult(SaveChannelStatApiRequest apiRequest)
        {
            var restRequest = new RestRequest("/app/tiktok/channel-stats",
            Method.POST);
            restRequest.AddJsonBody(apiRequest);
            return HandleResponse<SaveChannelStatApiResponse>(Execute(restRequest));
        }

        public ApiResponse<List<SaveMCNVietNamChannelDto>> PostAutoCrawlMCNVietNamChannelStatsResult(SaveMCNVietNamChannelApiRequest apiRequest)
        {
            var restRequest = new RestRequest("/app/tiktok/save-mcn-vietnam-channel",
            Method.POST);
            restRequest.AddJsonBody(apiRequest);
            return HandleResponse<List<SaveMCNVietNamChannelDto>>(Execute(restRequest));
        }

        public ApiResponse<SaveChannelVideoResponse> PostAutoCrawlChannelVideosResult(SaveChannelVideoRequest apiRequest)
        {
            var restRequest = new RestRequest("/app/tiktok/channel-videos",
            Method.POST);
            restRequest.AddJsonBody(apiRequest);
            return HandleResponse<SaveChannelVideoResponse>(Execute(restRequest));
        }

        public ApiResponse<GetTiktokHashTagsApiResponse> GetTiktokHashTags()
        {
            var restRequest = new RestRequest("/app/tiktok/hashtags",
            Method.GET);
            return HandleResponse<GetTiktokHashTagsApiResponse>(Execute(restRequest));
        }

        public ApiResponse<SaveTiktokStatApiResponse> SaveTiktokStat(SaveTiktokStatApiRequest apiRequest)
        {
            var restRequest = new RestRequest("/app/tiktok/tiktok-stat",
            Method.POST);
            restRequest.AddJsonBody(apiRequest);
            return HandleResponse<SaveTiktokStatApiResponse>(Execute(restRequest));
        }

        public ApiResponse<List<TiktokExportRow>> GetTiktokExportRow(GetTiktoksInputExtend getTiktoksInputExtend)
        {
            var queryString = getTiktoksInputExtend.ToQueryString();
            var restRequest = new RestRequest($"/app/tiktok-stats/get-export-rows?{queryString}", Method.GET);
            return HandleResponse<List<TiktokExportRow>>(Execute(restRequest));
        }

        public void UpdateTitokVideosState(UpdateTiktokVideosStateRequest apiRequest)
        {
            var restRequest = new RestRequest("/app/tiktok/update-channel-videos-state", Method.POST);
            restRequest.AddJsonBody(apiRequest);
            Execute(restRequest);
        }

        public ApiResponse<List<string>> GetTiktokMCNs()
        {
            var restRequest = new RestRequest("/app/tiktokmcns/get-list-hashtags", Method.GET);
            return HandleResponse<List<string>>(Execute(restRequest));
        }

        public void SaveTiktokMCNVideo(CrawlMCNVideo crawlMcnVideo)
        {
            var restRequest = new RestRequest("/app/tiktoks/save-tiktok-videos-stats",
            Method.POST);
            restRequest.AddJsonBody(crawlMcnVideo);
            Execute(restRequest);
        }

        public void SaveTiktokTrending(TiktokTrendingRequest tiktokTrendingRequest)
        {
            var restRequest = new RestRequest("/app/tiktok/save-trending-details", Method.POST);
            restRequest.AddJsonBody(tiktokTrendingRequest);
            Execute(restRequest);
        }
    }
}