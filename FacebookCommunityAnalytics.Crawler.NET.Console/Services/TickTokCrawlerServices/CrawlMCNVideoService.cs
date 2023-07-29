using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dasync.Collections;
using FacebookCommunityAnalytics.Crawler.NET.Client.Configurations;
using FacebookCommunityAnalytics.Crawler.NET.Client.Entities;
using FacebookCommunityAnalytics.Crawler.NET.Client.Enums;
using FacebookCommunityAnalytics.Crawler.NET.Console.Playwrights;
using FacebookCommunityAnalytics.Crawler.NET.Core.Extensions;
using FacebookCommunityAnalytics.Crawler.NET.Core.Helpers;
using Microsoft.Playwright;

namespace FacebookCommunityAnalytics.Crawler.NET.Console.Services.TickTokCrawlerServices
{
    public class CrawlMCNVideoService : BaseChannelVideoService
    {
        private readonly AccountType _accountType;
        
        public CrawlMCNVideoService(GlobalConfig globalConfig) : base(globalConfig)
        {
            _accountType = AccountType.NETFacebookGroupSelectivePost;
        }

        public async Task Execute()
        {
            var tiktokMCNs = ApiClient.TikTok.GetTiktokMCNs();

            var hashtags = tiktokMCNs.Resource;
            
            System.Console.WriteLine($"{GetType().Name}: found {hashtags.Count} hashtags");
            await Task.Delay(3000);
            if (hashtags.IsNullOrEmpty())
            {
                System.Console.WriteLine($"{GetType()}: exit in 5 seconds, no hashtags found");
                await Task.Delay(5000);
            }

            hashtags = hashtags.Where(s => s == "gdlfamily").ToList();
            
            foreach (var hashtag in hashtags)
            {
                var tiktokVideos = await Crawl(hashtag);
                if (tiktokVideos.ChannelVideos.Any())
                {
                    ApiClient.TikTok.SaveTiktokMCNVideo(tiktokVideos);
                }
            }
        }

        private async Task<CrawlMCNVideo> Crawl(string hashtag)
        {
            Crawl:
            var accountProxyQueue = GetAccountProxyQueue(_accountType);
            if (!accountProxyQueue.Any()) return new CrawlMCNVideo();
            
            var accountProxyItem = GetAccountProxyItem(accountProxyQueue);
            
            var totalVideo = 100;
            
            var browserContext = await PlaywrightHelper.InitPersistentBrowser(GlobalConfig.CrawlConfig, accountProxyItem, true, hashtag);
            using (browserContext.Playwright)
            {
                await using (browserContext.Browser)
                {
                    var page = await browserContext.BrowserContext.NewPageAsync();
                    try
                    {
                        var url = $"https://www.tiktok.com/tag/{hashtag}";
                        await page.GotoAsync(url);
                        await page.WaitForLoadStateAsync(LoadState.Load);
                        await page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);

                        var tiktokVideos = new CrawlMCNVideo
                        {
                            Hashtag = hashtag,
                            ChannelVideos = new List<ChannelVideo>()
                        };
                        
                        var videoInformation = await GetVideoInformation(page, totalVideo);

                        if (videoInformation.Any())
                        {
                            var videoDic = await CrawlVideo(videoInformation);

                            var channelNames = videoInformation.Select(information => information.ChannelName).Distinct().ToList();

                            var channelStats = await CrawlChannelStats(channelNames);

                            foreach (var channelStat in channelStats)
                            {
                                if (videoDic.ContainsKey(channelStat.ChannelId))
                                {
                                    videoDic.TryGetValue(channelStat.ChannelId, out var videos);
                                    if (videos != null && videos.Any())
                                    {
                                        tiktokVideos.ChannelVideos.Add(new ChannelVideo
                                        {
                                            ChannelStat = channelStat,
                                            TiktokVideos = videos.ToList()
                                        });
                                    }
                                }
                            }
                        }

                        return tiktokVideos;

                    }
                    catch (Exception e)
                    {
                        System.Console.WriteLine(e);
                        await e.Log(string.Empty, string.Empty);
                        goto Crawl;
                    }
                    finally
                    {
                        await page.CloseAsync();
                        await browserContext.BrowserContext.CloseAsync();
                    }
                }
            }
        }

        private async Task<ConcurrentBag<ChannelStat>> CrawlChannelStats(List<string> channelNames)
        {
            ConcurrentBag<ChannelStat> channelStats = new ConcurrentBag<ChannelStat>();
            await channelNames.ParallelForEachAsync(async channelName =>
            {
                var channelStat = await PerformCrawlingChannel(channelName);
                channelStats.Add(channelStat);
            }, GlobalConfig.CrawlConfig.Crawl_MaxThread_TiktokMCNVideo);
            return channelStats;
        }

        private async Task<ConcurrentDictionary<string, ConcurrentBag<TiktokVideoDto>>> CrawlVideo(List<VideoInformation> videoInformation)
        {
            var videoGroups = videoInformation.GroupBy(information => information.ChannelName)
                .Select(grouping => new {grouping.Key, Videos = grouping.ToList()}).ToList();

            ConcurrentDictionary<string, ConcurrentBag<TiktokVideoDto>> videoDic =
                new ConcurrentDictionary<string, ConcurrentBag<TiktokVideoDto>>();
            await videoGroups.ParallelForEachAsync(async group =>
            {
                var tiktokVideos = await PerformCrawlingVideo(@group.Key);

                foreach (var tiktokVideoDto in tiktokVideos)
                {
                    videoDic.AddOrUpdate(@group.Key, new ConcurrentBag<TiktokVideoDto> {tiktokVideoDto},
                        (key, value) =>
                        {
                            value.Add(tiktokVideoDto);
                            return value;
                        });
                }
            }, GlobalConfig.CrawlConfig.Crawl_MaxThread_TiktokMCNVideo);
            return videoDic;
        }

        private static async Task<List<VideoInformation>> GetVideoInformation(IPage page, int totalVideo)
        {
            var videoInformation = new List<VideoInformation>();
            var selector_videos =
                "//div[@data-e2e='challenge-item-list']//ancestor::div[@data-e2e='challenge-item']//ancestor::a[boolean(@data-e2e) = false and boolean(@title) = false]";
            var selector_titles =
                "//div[@data-e2e='challenge-item-list']//ancestor::div[@data-e2e='challenge-item']//ancestor::a[boolean(@title)]";
            var count = 0;

            while (true)
            {
                await page.Wait(100);
                var ele_videos = await page.QuerySelectorAllAsync(selector_videos);
                var ele_titles = await page.QuerySelectorAllAsync(selector_titles);

                if (ele_videos.Any())
                {
                    var elementHandle = ele_videos.Skip(count).First();
                    var elementTitle = ele_titles.Skip(count).First();
                    count += 1;

                    await page.EvaluateAsync("() => window.scrollTo(0, document.body.scrollHeight)");
                    await page.WaitForLoadStateAsync(LoadState.Load);
                    await page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);

                    var videoUrl = await elementHandle.GetAttributeAsync("href");
                    var channelName = await elementTitle.GetAttributeAsync("title");

                    if (count > totalVideo)
                    {
                        break;
                    }
                    
                    System.Console.WriteLine($"Url: {videoUrl}");

                    videoInformation.Add(new VideoInformation
                    {
                        ChannelName = channelName,
                        VideoUrl = videoUrl
                    });
                }
                else
                {
                    break;
                }
            }

            return videoInformation;
        }

        private async Task<IList<TiktokVideoDto>> PerformCrawlingVideo(string channelId)
        {
            var accountProxyQueue = GetAccountProxyQueue(_accountType);
            Crawl:
            var accountProxy = GetAccountProxyItem(accountProxyQueue);
            
            var browserContext = await PlaywrightHelper.InitPersistentBrowser(GlobalConfig.CrawlConfig, accountProxy, true, channelId);
            using (browserContext.Playwright)
            {
                await using (browserContext.Browser)
                {
                    try
                    {
                        IList<TiktokVideoDto> tiktokVideoDtos = await CrawlVideoByChannel(browserContext, channelId, CrawlStopCondition.TwoWeek);

                        return tiktokVideoDtos;
                    }
                    catch (Exception e)
                    {
                        await e.Log(string.Empty, string.Empty);
                        goto Crawl;
                    }
                    finally
                    {
                        try
                        {
                            await browserContext.BrowserContext.CloseAsync();
                        }
                        catch (Exception e)
                        {
                            System.Console.WriteLine(e);
                        }
                    }
                }
            }
        }

        private async Task<ChannelStat> PerformCrawlingChannel(string channelName)
        {
            var accountProxyQueue = GetAccountProxyQueue(_accountType);
            Crawl:
            var accountProxy = GetAccountProxyItem(accountProxyQueue);
            
            var browserContext = await PlaywrightHelper.InitPersistentBrowser(GlobalConfig.CrawlConfig, accountProxy, true, channelName);
            using (browserContext.Playwright)
            {
                await using (browserContext.Browser)
                {
                    var page = await browserContext.BrowserContext.NewPageAsync();
                    try
                    {
                        var channelStat = await CrawlChannel(page, $"https://www.tiktok.com/@{channelName}", channelName);

                        return channelStat;
                    }
                    catch (Exception e)
                    {
                        await e.Log(string.Empty, string.Empty);
                        goto Crawl;
                    }
                    finally
                    {
                        try
                        {
                            await browserContext.BrowserContext.CloseAsync();
                        }
                        catch (Exception e)
                        {
                            System.Console.WriteLine(e);
                        }
                    }
                }
            }
        }

        private AccountProxyItem GetAccountProxyItem(List<AccountProxyItem> accountProxyQueue)
        {
            var random = new Random();
            int index = random.Next(accountProxyQueue.Count);
            var accountProxyItem = accountProxyQueue[index];

            return accountProxyItem;
        }
    }
}