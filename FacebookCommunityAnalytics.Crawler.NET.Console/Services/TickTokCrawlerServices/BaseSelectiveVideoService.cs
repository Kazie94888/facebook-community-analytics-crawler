using System;
using System.Linq;
using System.Threading.Tasks;
using FacebookCommunityAnalytics.Crawler.NET.Client.Configurations;
using FacebookCommunityAnalytics.Crawler.NET.Client.Entities;
using FacebookCommunityAnalytics.Crawler.NET.Console.Models;
using FacebookCommunityAnalytics.Crawler.NET.Core.Helpers;
using Microsoft.Playwright;
using Newtonsoft.Json;

namespace FacebookCommunityAnalytics.Crawler.NET.Console.Services.TickTokCrawlerServices
{
    public class BaseSelectiveVideoService : BaseChannelStatsService
    {
        protected BaseSelectiveVideoService(GlobalConfig globalConfig) : base(globalConfig)
        {
        }

        protected async Task<TiktokVideoDto>  CrawlVideo(PlaywrightContext browserContext, string channelUrl, string channelId, string videoUrl, string videoId)
        {
            var page = await browserContext.BrowserContext.NewPageAsync();
            try
            {
                await page.GotoAsync(channelUrl);
                await page.WaitForLoadStateAsync(LoadState.Load);
                await page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
                await page.WaitForLoadStateAsync(LoadState.NetworkIdle);
                
                var selector_videos = "//div[@data-e2e='user-post-item-list']//ancestor::div[@data-e2e='user-post-item']//ancestor::a";
                var selector_video =
                    $"//div[@data-e2e='user-post-item-list']//ancestor::div[@data-e2e='user-post-item']//ancestor::a[@href='{videoUrl}']";
                var selector_VideoThumbnailImage =
                    $"//div[@data-e2e='user-post-item-list']//ancestor::div[@data-e2e='user-post-item']//ancestor::a[@href='{videoUrl}']/..";
                while ((await page.QuerySelectorAsync(selector_video)) == null)
                {
                    var videos = await page.QuerySelectorAllAsync(selector_videos);
                    var lastVideo = videos.Last();
                    
                    await page.EvaluateAsync("lastVideo => lastVideo.scrollIntoViewIfNeeded(true)", lastVideo);
                    await page.EvaluateAsync("lastVideo => lastVideo.scrollTo(0, 1000)", lastVideo);
                    
                    await lastVideo.ScrollIntoViewIfNeededAsync(new ElementHandleScrollIntoViewIfNeededOptions
                    {
                        Timeout = 1000
                    });
                    
                    await page.WaitForLoadStateAsync(LoadState.Load);
                    await page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
                    // await page.WaitForLoadStateAsync(LoadState.NetworkIdle);
                }

                var video = await page.QuerySelectorAsync(selector_video);
                var videoThumbnailImage = await page.QuerySelectorAsync(selector_VideoThumbnailImage);
                
                var tiktokVideoDto = new TiktokVideoDto
                {
                    VideoId = videoId,
                    VideoUrl = videoUrl,
                    ViewCount = await CrawlViewCount(video),
                    ThumbnailImage = await CrawlThumbnailImage(videoThumbnailImage)
                };
                
                await CrawReaction(browserContext, videoUrl, tiktokVideoDto);
                
                System.Console.WriteLine($"=======================Channel {channelId}====================================");
                var jsonData = JsonConvert.SerializeObject(tiktokVideoDto, new JsonSerializerSettings
                    {ContractResolver = new IgnorePropertiesResolver(new[] {"ThumbnailImage"})});
                System.Console.WriteLine(jsonData);
                System.Console.WriteLine($"=================================================================================");

                return tiktokVideoDto;
            }
            catch (Exception e)
            {
                System.Console.WriteLine(e);
                await e.Log(string.Empty, string.Empty);
                throw;
            }
            finally
            {
                await page.CloseAsync();
            }
        }

        
    }
}