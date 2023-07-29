using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FacebookCommunityAnalytics.Crawler.NET.Client.Configurations;
using FacebookCommunityAnalytics.Crawler.NET.Client.Entities;
using FacebookCommunityAnalytics.Crawler.NET.Client.Enums;
using FacebookCommunityAnalytics.Crawler.NET.Console.Playwrights;
using Microsoft.Playwright;

namespace FacebookCommunityAnalytics.Crawler.NET.Console.Services.CrawlerServices
{
    public class LoginService : CrawlServiceBase
    {
        public List<AccountType> AccountTypes { get; set; }
        protected override AccountType AccountType { get; }
        protected override CrawlStopCondition CrawlStopCondition { get; }

        public LoginService(GlobalConfig globalConfig) : base(globalConfig)
        {
            AccountTypes = new List<AccountType>();
            AccountType = AccountType.Unknown;
            CrawlStopCondition = CrawlStopCondition.None;
        }


        public override async Task Execute()
        {
            var accountProxyItems = new List<AccountProxyItem>();
            foreach (var type in AccountTypes)
            {
                var accountProxyQueue = GetAccountProxyQueue(type);
                if (accountProxyQueue!=null)
                {
                    accountProxyItems.AddRange(accountProxyQueue);
                }
            }

            var queue = new Queue<AccountProxyItem>(accountProxyItems);
            Debug.WriteLine($"====================={GetType().Name}: loaded {accountProxyItems.Count} account proxy");

            while (queue.Any())
            {
                var accountProxyItem = queue.Dequeue();
                var crawlResult = await Crawl(accountProxyItem, new List<CrawlModelBase>());
            }
        }

        public async Task ManualLogin()
        {
            var accountProxyItems = new List<AccountProxyItem>();
            foreach (var type in AccountTypes)
            {
                var accountProxyQueue = GetAccountProxyQueue(type);
                if (accountProxyQueue!=null)
                {
                    accountProxyItems.AddRange(accountProxyQueue);
                }
            }
            
            var queue = new Queue<AccountProxyItem>(accountProxyItems);
            Debug.WriteLine($"====================={GetType().Name}: loaded {accountProxyItems.Count} account proxy");
            while (queue.Any())
            {
                var accountProxyItem = queue.Dequeue();
                // var crawlResult = await Crawl(accountProxyItem, new List<CrawlModelBase>());
                System.Console.WriteLine($"================================================MANUAL LOGIN: {accountProxyItem.account.username}");
                
                var browserContext = await PlaywrightHelper.InitPersistentBrowser(GlobalConfig.CrawlConfig, accountProxyItem);
                using (browserContext.Playwright)
                {
                    await using (browserContext.Browser)
                    {
                        var page = await browserContext.BrowserContext.NewPageAsync();
                        await page.GotoAsync("https://m.facebook.com");
                        
                        System.Console.ReadKey();

                        await browserContext.BrowserContext.CloseAsync();
                    }
                }
            }
        }

        protected override async Task<CrawlResult> CanCrawl(IPage page, CrawlModelBase crawlItem)
        {
            return new CrawlResult(CrawlStatus.OK);
        }

        protected override async Task<CrawlResult> DoCrawl(IPage page, CrawlModelBase crawlItem)
        {
            return new CrawlResult(CrawlStatus.OK);
        }
    }
}