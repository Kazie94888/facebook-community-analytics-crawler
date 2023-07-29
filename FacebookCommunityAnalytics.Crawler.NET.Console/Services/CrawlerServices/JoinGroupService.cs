using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dasync.Collections;
using FacebookCommunityAnalytics.Crawler.NET.Client.Configurations;
using FacebookCommunityAnalytics.Crawler.NET.Client.Entities;
using FacebookCommunityAnalytics.Crawler.NET.Client.Enums;
using Microsoft.Playwright;

namespace FacebookCommunityAnalytics.Crawler.NET.Console.Services.CrawlerServices
{
    public class JoinGroupService : CrawlServiceBase
    {
        public JoinGroupService(GlobalConfig globalConfig) : base(globalConfig)
        {
            AccountType = AccountType.NETFacebookGroupPost;
        }

        protected override AccountType AccountType { get; }
        protected override CrawlStopCondition CrawlStopCondition { get; }
        public override async Task Execute()
        {
            var uncrawledGroups = ApiClient.Crawl.GetUncrawledGroups(new GetUncrawledGroupApiRequest
            {
                GroupSourceType = GroupSourceType.Group,
                IgnoreTime = true
            });
            var crawlItems = uncrawledGroups.Resource.Groups;
            
            var accountProxyQueue = GetAccountProxyQueue(AccountType);
            if (accountProxyQueue == null) return;
            
            await crawlItems.ParallelForEachAsync(async @group =>
            {
                AccountProxyItem accountProxy = null;
                bool isFinished = false;
                CRAWL:
                try
                {
                    // 3. crawl
                    accountProxy = accountProxyQueue.Dequeue();

                    var crawlResult = await Crawl(accountProxy, @group.ToCrawlModelBase());

                    if (crawlResult is { Success: true })
                    {
                        isFinished = true;
                    }
                }
                catch (Exception)
                {
                    isFinished = false;
                    using var autoResetEvent = new AutoResetEvent(false);
                    autoResetEvent.WaitOne(60000);
                }
                finally
                {
                    if (accountProxy != null)
                    {
                        accountProxyQueue.Enqueue(accountProxy);
                    }
                }
                    
                // This code to make sure the don't miss post
                if (isFinished == false)
                {
                    goto  CRAWL;
                }
                    
            }, GlobalConfig.CrawlConfig.Crawl_MaxThread_GroupPost);
        }

        protected override async Task<CrawlResult> CanCrawl(IPage page, CrawlModelBase crawlItem)
        {
            return await Task.Factory.StartNew(() => new CrawlResult());
        }

        protected override async Task<CrawlResult> DoCrawl(IPage page, CrawlModelBase crawlItem)
        {
            System.Console.ReadLine();
            
            return await Task.Factory.StartNew(() => new CrawlResult
            {
                Success = true
            });
        }
    }
}