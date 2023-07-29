using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dasync.Collections;
using FacebookCommunityAnalytics.Crawler.NET.Client.Configurations;
using FacebookCommunityAnalytics.Crawler.NET.Client.Entities;
using FacebookCommunityAnalytics.Crawler.NET.Console.Playwrights;
using FacebookCommunityAnalytics.Crawler.NET.Core.Helpers;

namespace FacebookCommunityAnalytics.Crawler.NET.Console.Services.TickTokCrawlerServices
{
    public class CrawlTopMCNVietNamService : BaseChannelStatsService
    {
        public CrawlTopMCNVietNamService(GlobalConfig globalConfig) : base(globalConfig)
        {
        }

        public async Task Execute()
        {
            var channelIds = new List<string>
            {
                "linhbarbie",
                "tra.dang.904",
                "tai2701",
                "cindymiicute",
                "hoa_2309",
                "_b.t.v",
                "thanhyvo",
                "reencyngo",
                "vangiau.07",
                "tuongvyy",
                "vietphuongthoa98",
                "lykio.98",
                "gamkami",
                "lebong95",
                "quocthanh0308",
                "zaheun",
                "quocthanh0308",
                "hoangvinhhh",
                "vtkh2004",
                "lethikhanhhuyen2004",
                "datvilla94",
                "vienvibi_899",
                "bongtim96"
            };

            ConcurrentBag<SaveMCNVietNamChannelDto> saveMcnVietNamChannelApiRequests = new ConcurrentBag<SaveMCNVietNamChannelDto>();
            await channelIds.ParallelForEachAsync(async channel =>
            {
                var crawlResult = await Crawl(channel);
                if (crawlResult is { Success: true })
                {
                    saveMcnVietNamChannelApiRequests.Add(crawlResult.ChannelStat.ToSavingMCNVietNamChannelRequest());
                }
            },
            GlobalConfig.CrawlConfig.Crawl_MaxThread_GroupPost);

            if (saveMcnVietNamChannelApiRequests.Any())
            {
                ApiClient.TikTok.PostAutoCrawlMCNVietNamChannelStatsResult(new SaveMCNVietNamChannelApiRequest
                {
                    MCNVietNamChannels = saveMcnVietNamChannelApiRequests.ToList()
                });
            }
        }

        private async Task<CrawlTikTokResult> Crawl(string channelId)
        {
            var browserContext = await PlaywrightHelper.InitPersistentBrowser(GlobalConfig.CrawlConfig,
            null,
            userDataFolderName: channelId);
            var url = $"https://www.tiktok.com/@{channelId}";
            using (browserContext.Playwright)
            {
                await using (browserContext.Browser)
                {
                    var page = await browserContext.BrowserContext.NewPageAsync();
                    try
                    {
                        var channelStat = await CrawlChannel(page,
                        url,
                        channelId);
                        var crawlTikTokResult = new CrawlTikTokResult
                        {
                            ChannelStat = channelStat
                        };
                        return crawlTikTokResult;
                    }
                    catch (Exception e)
                    {
                        System.Console.WriteLine(e);
                        await e.Log(string.Empty,
                        string.Empty);
                        throw;
                    }
                    finally
                    {
                        try
                        {
                            await page.CloseAsync();
                            await browserContext.BrowserContext.CloseAsync();
                        }
                        catch (Exception exception)
                        {
                            System.Console.WriteLine(exception.Message);
                        }
                    }
                }
            }
        }
    }
}