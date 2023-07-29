using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FacebookCommunityAnalytics.Crawler.NET.Client.Configurations;
using FacebookCommunityAnalytics.Crawler.NET.Client.Entities;
using FacebookCommunityAnalytics.Crawler.NET.Client.Enums;
using FacebookCommunityAnalytics.Crawler.NET.Console.Playwrights;
using Microsoft.Playwright;

namespace FacebookCommunityAnalytics.Crawler.NET.Console.Services.CrawlerServices
{
    public class CrawlTwitterService : CrawlServiceBase
    {
        private readonly GlobalConfig _globalConfig;
        public CrawlTwitterService(GlobalConfig globalConfig) : base(globalConfig)
        {
            _globalConfig = globalConfig;
        }
        
        public async Task Crawl()
        {
            var crawlTwitterId = "elonmusk";
            var url = $"https://twitter.com/{crawlTwitterId}";
            // Clean userData folder name
            var userDataDir = $"{_globalConfig.CrawlConfig.UserDataDirRoot}/Twitter";
            var dir = new DirectoryInfo(userDataDir);
            if (dir.Exists)
            {
                dir.Delete(true);
            }
            
            // var accountProxyQueue = GetAccountProxyQueue(AccountType.NETFacebookGroupUserPost);
            // var accountProxyItem = GetAccountProxyItem(accountProxyQueue.ToList());
            
            var browserContext =
                await PlaywrightHelper.InitPersistentBrowser(_globalConfig.CrawlConfig, null, false,
                    $"Twitter");

            var results = new List<TwitterResult>();

            using (browserContext.Playwright)
            {
                await using (browserContext.Browser)
                {
                    var page = await browserContext.BrowserContext.NewPageAsync();
                    
                    await page.GotoAsync(url);
                    await page.WaitForLoadStateAsync(LoadState.Load);
                    await page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);

                    var selectorPosts = "//div[contains(@aria-label,'Timeline') and contains(@aria-label,'Tweets')]/div/div";
                    
                    var count = 0;
                    while (true)
                    {
                        BEGIN:
                        var elementsPosts = await page.QuerySelectorAllAsync(selectorPosts);
                        if (count > elementsPosts.Count)
                        {
                            await page.EvaluateAsync("() => window.scrollTo(0, document.body.scrollHeight)");
                            await page.WaitForLoadStateAsync(LoadState.Load);
                            await page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
                            await page.WaitASecond();
                            count = 0;
                            goto BEGIN;
                        }
                        var posts = elementsPosts.Skip(count).ToList();
                        if (posts.Any())
                        {
                            var post = posts.First();
                            var selectorLink = "//ancestor::a[@href]";
                            var elementLinks = await post.QuerySelectorAllAsync(selectorLink);
                            if (elementLinks.Any() && elementLinks.Count >= 4)
                            {
                                var dateTimeLink = elementLinks[3];
                                await page.EvaluateAsync("dateTimeLink => dateTimeLink.scrollIntoViewIfNeeded(true)", dateTimeLink);
                                await page.WaitASecond();

                                var keyValue = await GetTwitterId(dateTimeLink, elementLinks);
                                var twitterId = keyValue.Item1;
                                var tweetId = keyValue.Item2;
                                dateTimeLink = keyValue.Item3;

                                if ((twitterId == string.Empty) || (results.Any(result => result.TwitterId == twitterId && result.TweetId == tweetId)))
                                {
                                    count += 1;
                                    continue;
                                }
                                
                                var tweetType = TweetType.Tweeted;
                                if (twitterId != crawlTwitterId)
                                {
                                    tweetType = TweetType.Retweeted;
                                }
                                
                                var elementDateTime = await dateTimeLink.QuerySelectorAsync("//time");
                                DateTime? dateTime = null;
                                if (elementDateTime != null)
                                {
                                    var dateTimeString = await elementDateTime.GetAttributeAsync("datetime");
                                    if (dateTimeString != null)
                                    {
                                        dateTime = DateTime.ParseExact(dateTimeString, "yyyy-MM-dd'T'HH:mm:ss.fff'Z'",
                                            CultureInfo.InvariantCulture);
                                    }

                                    System.Console.WriteLine(await post.InnerHTMLAsync());
                                }
                                
                                results.Add(new  TwitterResult
                                {
                                    Content = await post.InnerHTMLAsync(),
                                    DateTime = dateTime,
                                    TweetId = tweetId,
                                    TwitterId = twitterId,
                                    TweetType = tweetType
                                });
                            }
                        }

                        count += 1;
                    }
                }
            }
        }

        private async Task<Tuple<string, string, IElementHandle>> GetTwitterId(IElementHandle dateTimeLink, IReadOnlyList<IElementHandle> elementLinks)
        {
            var url = await dateTimeLink.GetAttributeAsync("href");
            if (string.IsNullOrWhiteSpace(url)) return new Tuple<string, string, IElementHandle>(string.Empty, string.Empty, dateTimeLink);
            
            var strings = url.Trim('/').Split("/");
            if (strings.Length < 3)
            {
                dateTimeLink = elementLinks[4];
                    
                return await GetTwitterId(dateTimeLink, elementLinks);
            }
            var twitterId = strings.First();
            var twitId = strings.Last();

            return new Tuple<string, string, IElementHandle>(twitterId, twitId, dateTimeLink);

        }

        private AccountProxyItem GetAccountProxyItem(List<AccountProxyItem> accountProxyQueue)
        {
            var random = new Random();
            int index = random.Next(accountProxyQueue.Count);
            var accountProxyItem = accountProxyQueue[index];

            return accountProxyItem;
        }

        protected override AccountType AccountType { get; }
        protected override CrawlStopCondition CrawlStopCondition { get; }
        public override Task Execute()
        {
            throw new System.NotImplementedException();
        }

        protected override Task<CrawlResult> CanCrawl(IPage page, CrawlModelBase crawlItem)
        {
            throw new System.NotImplementedException();
        }

        protected override Task<CrawlResult> DoCrawl(IPage page, CrawlModelBase crawlItem)
        {
            throw new System.NotImplementedException();
        }
    }

    public class TwitterResult
    {
        public string TwitterId { get; set; }
        public string TweetId { get; set; }
        public DateTime? DateTime { get; set; }
        public string Content { get; set; }
        public TweetType TweetType { get; set; }
    }

    public enum TweetType
    {
        Retweeted,
        Tweeted
    }
}