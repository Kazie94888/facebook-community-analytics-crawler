using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using FacebookCommunityAnalytics.Crawler.NET.Client.Configurations;
using FacebookCommunityAnalytics.Crawler.NET.Client.Entities;
using FacebookCommunityAnalytics.Crawler.NET.Client.Enums;
using FacebookCommunityAnalytics.Crawler.NET.Console.Models;
using FacebookCommunityAnalytics.Crawler.NET.Core.Helpers;
using Microsoft.Playwright;
using Newtonsoft.Json;

namespace FacebookCommunityAnalytics.Crawler.NET.Console.Services.TickTokCrawlerServices
{
    public class BaseChannelVideoService : BaseChannelStatsService
    {
        protected BaseChannelVideoService(GlobalConfig globalConfig) : base(globalConfig)
        {
        }
        
        protected async Task<List<TiktokVideoDto>> CrawlVideoByChannel(PlaywrightContext browserContext, 
            string channelId, CrawlStopCondition crawlStopCondition)
        {
            var channelUrl = $"https://www.tiktok.com/@{channelId}";
            System.Console.WriteLine($"ChannelUrl: {channelUrl}");
            var page = await browserContext.BrowserContext.NewPageAsync();
            try
            {
                await page.GotoAsync(channelUrl, new PageGotoOptions
                {
                    Timeout = 0
                });
                await page.WaitForLoadStateAsync(LoadState.Load, new PageWaitForLoadStateOptions
                {
                    Timeout = 0
                });
                await page.WaitForLoadStateAsync(LoadState.DOMContentLoaded, new PageWaitForLoadStateOptions
                {
                    Timeout = 0
                });
                // await page.WaitForLoadStateAsync(LoadState.NetworkIdle, new PageWaitForLoadStateOptions
                // {
                //     Timeout = 0
                // });
                
                var tiktokVideos = new List<TiktokVideoDto>();
                var selector_videos = "//div[@data-e2e='user-post-item-list']//ancestor::div[@data-e2e='user-post-item']//ancestor::a";
                var selector_VideoThumbnailImage =
                    "//div[@data-e2e='user-post-item-list']//ancestor::div[@data-e2e='user-post-item']//ancestor::a/..";
                
                int count = 0;
                while (true)
                {
                    var ele_videos = await page.QuerySelectorAllAsync(selector_videos);
                    var ele_VideoThumbnailImage =
                        await page.QuerySelectorAllAsync(selector_VideoThumbnailImage);
                    if (!ele_videos.Any())
                    {
                        selector_videos = "//main/div[@class='tt-feed']//ancestor::a";
                        selector_VideoThumbnailImage = "//main/div[@class='tt-feed']//ancestor::a/..";
                        ele_videos = await page.QuerySelectorAllAsync(selector_videos);
                    }
                    
                    if (tiktokVideos.Count == ele_videos.Count)
                    {
                        break;
                    }

                    var elementHandle = ele_videos.Skip(count).First();
                    var elementVideoThumbnailImage = ele_VideoThumbnailImage.Skip(count).First();
                    count += 1;
                    
                    await page.EvaluateAsync("() => window.scrollTo(0, document.body.scrollHeight)");
                    await page.WaitForLoadStateAsync(LoadState.Load, new PageWaitForLoadStateOptions
                    {
                        Timeout = 0
                    });
                    await page.WaitForLoadStateAsync(LoadState.DOMContentLoaded, new PageWaitForLoadStateOptions
                    {
                        Timeout = 0
                    });
                    // await page.WaitForLoadStateAsync(LoadState.NetworkIdle, new PageWaitForLoadStateOptions
                    // {
                    //     Timeout = 0
                    // });
                    
                    var videoUrl = await elementHandle.GetAttributeAsync("href");
                    
                    if (!string.IsNullOrWhiteSpace(videoUrl))
                    {
                        var videoId = Regex.Split(videoUrl, "/video/")[1];

                        var tiktokVideoDto = new TiktokVideoDto
                        {
                            VideoId = videoId,
                            VideoUrl = videoUrl,
                            ViewCount = await CrawlViewCount(elementHandle),
                            ThumbnailImage = await CrawlThumbnailImage(elementVideoThumbnailImage)
                        };

                        await CrawReaction(browserContext, videoUrl, tiktokVideoDto);
                        
                        if(IsStopped(tiktokVideoDto.CreatedAt, crawlStopCondition))
                        {
                            System.Console.WriteLine($"=======================Channel {channelId}====================================");
                            System.Console.WriteLine($"{tiktokVideos.Count} videos");
                            System.Console.WriteLine($"=================================================================================");
                            return tiktokVideos;
                        };
                        
                        
                        
                        System.Console.WriteLine($"=======================Channel {channelId}====================================");
                        var jsonData = JsonConvert.SerializeObject(tiktokVideoDto, new JsonSerializerSettings
                            {ContractResolver = new IgnorePropertiesResolver(new[] {"ThumbnailImage"})});
                        System.Console.WriteLine(jsonData);
                        System.Console.WriteLine($"=================================================================================");
                        
                        tiktokVideos.Add(tiktokVideoDto);
                    }
                }
                
                return tiktokVideos;
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