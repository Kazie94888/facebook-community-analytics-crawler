using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dasync.Collections;
using FacebookCommunityAnalytics.Crawler.NET.Client.Configurations;
using FacebookCommunityAnalytics.Crawler.NET.Client.Entities;
using FacebookCommunityAnalytics.Crawler.NET.Console.Playwrights;
using FacebookCommunityAnalytics.Crawler.NET.Core.Extensions;
using FacebookCommunityAnalytics.Crawler.NET.Core.Helpers;

namespace FacebookCommunityAnalytics.Crawler.NET.Console.Services.TickTokCrawlerServices
{
    public class CrawlChannelSelectiveVideoService : BaseSelectiveVideoService
    {


        public CrawlChannelSelectiveVideoService(GlobalConfig globalConfig) : base(globalConfig)
        {
            
        }

        public async Task Execute()
        {
            try
            {
                // TODO: get uncrawled channel video

                var a = new UncrawledTiktokVideoResponse();
                var crawlItems = a.Items;

                System.Console.WriteLine($"{GetType().Name}: found {crawlItems.Count} channels");
                await Task.Delay(3000);
                if (crawlItems.IsNullOrEmpty())
                {
                    System.Console.WriteLine($"{GetType()}: exit in 5 seconds, no channels found");
                    await Task.Delay(5000);
                }

                var batchSize = GlobalConfig.CrawlConfig.BatchSize_Max_TiktokSelectiveVideo;
                var threadCount = GlobalConfig.CrawlConfig.Crawl_MaxThread_TiktokSelectiveVideo;

                if (threadCount <= 0)
                {
                    System.Console.WriteLine($"{GetType().Name}: EXITING with THREADCOUNT = {threadCount}");
                    return;
                }

                System.Console.WriteLine($"{GetType().Name}: FOUND {crawlItems.Count} selective posts");
                
                await crawlItems.Partition(batchSize)
                    .ParallelForEachAsync
                    (
                        async batch =>
                        {
                            AccountProxyItem accountProxy = null;
                            try
                            {
                                var crawlSource = batch.ToList();
                                if (crawlSource.IsNullOrEmpty()) return;

                                var crawlResult = await Crawl(crawlItems);

                                if (crawlResult.Any())
                                {
                                    // 4. Save crawl result
                                    // if (crawlResult.Posts.IsNotNullOrEmpty())
                                    // {
                                    //     _apiClient.Crawl.PostCrawlResult
                                    //     (
                                    //         new SaveCrawlResultApiRequest {Items = crawlResult.Posts}
                                    //     );
                                    // }
                                }
                            }
                            finally
                            {
                                
                            }
                        },
                        maxDegreeOfParallelism: threadCount
                    );
            }
            finally
            {
                Thread.Sleep(5000);
            }
        }

        private async Task<IList<TiktokVideoDto>>  Crawl(List<UncrawledTiktokVideoDto> tiktokVideos)
        {
            var tiktokVideosResult = new List<TiktokVideoDto>();
            var firstVideo = tiktokVideos.First();
            var browserContext = await PlaywrightHelper.InitPersistentBrowser(GlobalConfig.CrawlConfig, null, false, firstVideo.ChannelId);
            using (browserContext.Playwright)
            {
                await using (browserContext.Browser)
                {
                    try
                    {
                        foreach (var uncrawledTiktokVideoDto in tiktokVideos)
                        {
                            var tiktokVideo = await CrawlVideo(browserContext, uncrawledTiktokVideoDto.ChannelUrl,
                                uncrawledTiktokVideoDto.ChannelId, uncrawledTiktokVideoDto.VideoUrl,
                                uncrawledTiktokVideoDto.VideoId);
                            
                            tiktokVideosResult.Add(tiktokVideo);

                        }
                    }
                    catch (Exception e)
                    {
                        System.Console.WriteLine(e);
                        await e.Log(string.Empty, string.Empty);
                        throw;
                    }
                    finally
                    {
                        await browserContext.BrowserContext.CloseAsync();
                    }
                }
            }

            return tiktokVideosResult;
        }
    }
}