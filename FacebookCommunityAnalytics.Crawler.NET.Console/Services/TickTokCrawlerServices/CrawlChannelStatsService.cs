using System;
using System.Threading.Tasks;
using Dasync.Collections;
using FacebookCommunityAnalytics.Crawler.NET.Client.Configurations;
using FacebookCommunityAnalytics.Crawler.NET.Client.Entities;
using FacebookCommunityAnalytics.Crawler.NET.Client.Enums;
using FacebookCommunityAnalytics.Crawler.NET.Console.Playwrights;
using FacebookCommunityAnalytics.Crawler.NET.Core.Extensions;
using FacebookCommunityAnalytics.Crawler.NET.Core.Helpers;

namespace FacebookCommunityAnalytics.Crawler.NET.Console.Services.TickTokCrawlerServices
{
    public class CrawlChannelStatsService : BaseChannelStatsService
    {
        
        public CrawlChannelStatsService(GlobalConfig globalConfig) : base(globalConfig)
        {
        }
        
        public async Task Execute()
        {
            var uncrawledChannel = ApiClient.Crawl.GetUncrawledGroups(new GetUncrawledGroupApiRequest
            {
                GroupSourceType = GroupSourceType.Tiktok,
                IgnoreTime = true
            });
            
            var crawlItems = uncrawledChannel.Resource.Groups;
            
            System.Console.WriteLine($"{GetType().Name}: found {crawlItems.Count} channels");
            await Task.Delay(3000);
            if (crawlItems.IsNullOrEmpty())
            {
                System.Console.WriteLine($"{GetType()}: exit in 5 seconds, no channels found");
                await Task.Delay(5000);
            }

            await crawlItems.ParallelForEachAsync(async channel =>
            {
                var crawlResult = await Crawl(channel);
                if (crawlResult is { Success: true })
                {
                    var response = ApiClient.TikTok.PostAutoCrawlChannelStatsResult
                    (
                        crawlResult.ChannelStat.ToSavingChannelStatApiRequest()
                    );
                }
                
            }, GlobalConfig.CrawlConfig.Crawl_MaxThread_GroupPost);
        }

        private async Task<CrawlTikTokResult> Crawl(Group channel)
        {
            var browserContext = await PlaywrightHelper.InitPersistentBrowser(GlobalConfig.CrawlConfig, null, userDataFolderName:channel.name);
            using (browserContext.Playwright)
            {
                await using (browserContext.Browser)
                {
                    var page = await browserContext.BrowserContext.NewPageAsync();
                    try
                    {
                        var channelStat = await CrawlChannel(page, channel.url, channel.fid);
                        var crawlTikTokResult = new CrawlTikTokResult
                        {
                            ChannelStat = channelStat
                        };
                        
                        return crawlTikTokResult;

                    }
                    catch (Exception e)
                    {
                        System.Console.WriteLine(e);
                        await e.Log(string.Empty, string.Empty);
                        throw;
                    }
                    finally
                    {
                        try
                        {
                            await page.CloseAsync();

                            await browserContext.BrowserContext.CloseAsync();
                        }
                        catch(Exception exception)
                        {
                            System.Console.WriteLine(exception.Message);
                        }
                    }
                }
            }
        }
    }
}