using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dasync.Collections;
using FacebookCommunityAnalytics.Crawler.NET.Client.Configurations;
using FacebookCommunityAnalytics.Crawler.NET.Client.Entities;
using FacebookCommunityAnalytics.Crawler.NET.Client.Enums;
using FacebookCommunityAnalytics.Crawler.NET.Console.Playwrights;
using FacebookCommunityAnalytics.Crawler.NET.Core.Extensions;
using FacebookCommunityAnalytics.Crawler.NET.Core.Helpers;
using Microsoft.Playwright;
using Newtonsoft.Json;

namespace FacebookCommunityAnalytics.Crawler.NET.Console.Services.CrawlerServices
{
    public class CrawlSelectivePostService : CrawlServiceBase
    {
        protected override AccountType AccountType { get; }
        protected override CrawlStopCondition CrawlStopCondition { get; }

        public CrawlSelectivePostService(GlobalConfig globalConfig) : base(globalConfig)
        {
            AccountType = AccountType.NETFacebookGroupSelectivePost;
            CrawlStopCondition = CrawlStopCondition.None;
        }

        public override async Task Execute()
        {
            var accountProxyItemsResponse = new List<AccountProxyItem>();
            var proxies = new List<AccountProxy>();

            try
            {
                // 1. get source crawl data
                var getUncrawledPostsResponse = ApiClient.Crawl.GetUncrawledPosts(new UncrawledPostsRequest
                {
                    AccountType = AccountType
                });
                
                IList<UncrawledItemDto> uncrawledPosts = new List<UncrawledItemDto>();
                if (getUncrawledPostsResponse.IsSuccess)
                {
                    uncrawledPosts = getUncrawledPostsResponse.Resource.Items;
                    proxies = getUncrawledPostsResponse.Resource.AccountProxies.Select(item => item.accountProxy)
                        .ToList();
                }
                
                if(!uncrawledPosts.Any()) return;

                // 2. Get crawl account
                if (getUncrawledPostsResponse.Resource.AccountProxies != null)
                {
                    accountProxyItemsResponse.AddRange(getUncrawledPostsResponse.Resource.AccountProxies);
                }

                if (!accountProxyItemsResponse.Any()) return;

                var accountProxyQueue = new Queue<AccountProxyItem>(accountProxyItemsResponse);

                // 3. Crawl
                // Use ConcurrentQueue to enable safe enqueueing from multiple threads.
                var batchSize = GlobalConfig.CrawlConfig.BatchSize_Max_SelectiveGroupPost;
                var threadCount = GlobalConfig.CrawlConfig.Crawl_MaxThread_SelectiveGroupPost;
                if (threadCount > accountProxyQueue.Count)
                {
                    threadCount = accountProxyQueue.Count - 1;
                }

                if (threadCount <= 0)
                {
                    System.Console.WriteLine($"{GetType().Name}: EXITING with THREADCOUNT = {threadCount}");
                    return;
                }

                System.Console.WriteLine($"{GetType().Name}: FOUND {uncrawledPosts.Count} selective posts");
                System.Console.WriteLine(
                    $"{GetType().Name}: FOUND ACCOUNTS {accountProxyQueue.Count}. CRAWL with {threadCount}");

                await uncrawledPosts.Partition(batchSize)
                    .ParallelForEachAsync
                    (
                        async batch =>
                        {
                            AccountProxyItem accountProxy = null;
                            var isFinished = false;
                            var count = 0;
                            var crawlSource = batch.ToList();
                            if (crawlSource.IsNullOrEmpty()) return;

                            var crawlItems = crawlSource.Select
                                (
                                    uncrawledItemDto => new CrawlModelBase
                                    {
                                        Url = uncrawledItemDto.Url,
                                        Fuid = uncrawledItemDto.Fuid,
                                        GroupFid = uncrawledItemDto.GroupFid
                                    }
                                )
                                .ToList();
                            CRAWL:
                            try
                            {
                                accountProxy = accountProxyQueue.Dequeue();
                                var crawlResult = await CrawlSelectivePosts(accountProxy, crawlItems);
                                
                                // 4. Save crawl result
                                if (crawlResult.Item2.IsNotNullOrEmpty())
                                {
                                    ApiClient.Crawl.PostCrawlResult
                                    (
                                        new SaveCrawlResultApiRequest {Items = crawlResult.Item2}
                                    );
                                }
                                        
                                System.Console.WriteLine($"{GetType().Name}: CRAWLED {crawlResult.Item2?.Count ?? 0} on {DateTime.UtcNow}");

                                if (crawlItems.Count == crawlResult.Item1.Count)
                                {
                                    isFinished = true;
                                }
                                else
                                {
                                    crawlItems = crawlItems.Except(crawlResult.Item1).ToList();
                                }
                            }
                            catch (Exception)
                            {
                                isFinished = false;
                                count += 1;
                            }
                            finally
                            {
                                if (accountProxy != null)
                                {
                                    accountProxyQueue.Enqueue(accountProxy);
                                }
                            }
                            
                            // This code to make sure the don't miss post
                            if (isFinished == false && count <= 5)
                            {
                                goto CRAWL;
                            }
                        },
                        maxDegreeOfParallelism: threadCount
                    );
            }
            finally
            {
                ResetAccountsCrawlStatus(proxies.Select(proxy => new Guid(proxy.id)).ToList(), AccountType);
                
                Thread.Sleep(5000);
            }
        }

        private async Task<Tuple<List<CrawlModelBase>, List<AutoCrawledPost>>> CrawlSelectivePosts(AccountProxyItem accountProxy,
            List<CrawlModelBase> crawlSourceItems)
        {
            var crawledSourceItems = new List<CrawlModelBase>();
            var autoCrawledPosts = new List<AutoCrawledPost>();
            InitLogConfig(accountProxy);
            var browserContext = await PlaywrightHelper.InitPersistentBrowser(GlobalConfig.CrawlConfig, accountProxy);
            using (browserContext.Playwright)
            {
                await using (browserContext.Browser)
                {
                    var page = await browserContext.BrowserContext.NewPageAsync();
                    try
                    {
                        var facebookService = new FacebookLoginService(GlobalConfig.CrawlConfig);
                        System.Console.WriteLine($"====================={this.GetType().Name}: Trying to login for {accountProxy.account.username}");

                        var loginResponse = await facebookService.Login(browserContext.BrowserContext, page, accountProxy);
                        if (loginResponse.Success)
                        {
                            await page.Wait(1000);
                            
                            // GroupPost: run once
                            // GroupUser: run once
                            // PagePost: run once
                            // SelectivePosts: run for many times
                            System.Console.WriteLine($"====================={this.GetType().Name}: Trying to CRAWL for {crawlSourceItems.Count} items");
                            foreach (var crawlItem in crawlSourceItems)
                            {
                                var crawlResult = new CrawlResult(CrawlStatus.OK);
                                if (StringExtensions.IsNullOrEmpty(crawlItem.Url))
                                {
                                    crawledSourceItems.Add(crawlItem);
                                    continue;
                                }
                                
                                System.Console.WriteLine($"====================={this.GetType().Name}: Trying to CRAWL url {crawlItem.Url}");
                                var subpage = await browserContext.BrowserContext.NewPageAsync();
                                await subpage.GotoAsync(crawlItem.Url);
                                await subpage.WaitForLoadStateAsync(LoadState.NetworkIdle, new PageWaitForLoadStateOptions
                                {
                                    Timeout = 60000
                                });
                                
                                if (await facebookService.IsAccountBanned(subpage))
                                {
                                    System.Console.WriteLine($"====================={GetType().Name}: {CrawlStatus.AccountBanned} {accountProxy.account.username}");
                                    await subpage.CloseAsync();
                                    return new Tuple<List<CrawlModelBase>, List<AutoCrawledPost>>(crawledSourceItems, autoCrawledPosts);
                                }

                                System.Console.WriteLine($"====================={GetType().Name}: Check Banned Success {accountProxy.account.username}");
                                var canCrawl = await CanCrawl(subpage, crawlItem);
                                System.Console.WriteLine($"====================={GetType().Name}: {canCrawl.Status} {accountProxy.account.username}");
                                switch (canCrawl.Status)
                                {
                                    case CrawlStatus.OK:
                                    {
                                        var itemResult = await DoCrawl(subpage, crawlItem);
                                        if (itemResult != null && itemResult.Posts.Any())
                                        {
                                            crawlResult.Posts.AddRange(itemResult.Posts);
                                        }
                                        
                                        // 4. Save crawl result
                                        if (crawlResult.Posts.IsNotNullOrEmpty())
                                        {
                                            autoCrawledPosts.AddRange(crawlResult.Posts);
                                        }
                                        
                                        crawledSourceItems.Add(crawlItem);
                                        break;
                                    }
                                    
                                    // SelectivePosts: run for many times
                                    case CrawlStatus.PostUnavailable:
                                    {
                                        //  TODOO Vu.Nguyen: CREATE a fake post to store UNAVAILABLE 
                                        crawledSourceItems.Add(crawlItem);
                                        break;
                                    }

                                    case CrawlStatus.AccountBanned:
                                    case CrawlStatus.BlockedTemporary:
                                    case CrawlStatus.LoginFailed:
                                    case CrawlStatus.LoginApprovalNeeded:
                                    case CrawlStatus.GroupUnavailable:
                                    case CrawlStatus.UnknownFailure:
                                    {
                                        System.Console.WriteLine($"====================={GetType().Name}: {canCrawl.Status} {accountProxy.account.username}");
                                        await subpage.CloseAsync();

                                        return new Tuple<List<CrawlModelBase>, List<AutoCrawledPost>>(crawledSourceItems, autoCrawledPosts);
                                    }

                                    default: throw new ArgumentOutOfRangeException("crawlerResult.Status");
                                }
                                
                                await subpage.CloseAsync();
                            }
                            
                            return new Tuple<List<CrawlModelBase>, List<AutoCrawledPost>>(crawledSourceItems, autoCrawledPosts);
                        }
                        else
                        {
                            if (GenericExtensions.IsIn(loginResponse.Status,CrawlStatus.AccountBanned, CrawlStatus.LoginApprovalNeeded, CrawlStatus.BlockedTemporary))
                            {
                                System.Console.WriteLine($"====================={this.GetType().Name}: REMOVE ACCOUNT: {accountProxy.account.username} STATUS: {loginResponse.Status}");
                                LogBannedAccount(accountProxy.account, loginResponse.Status);
                                return new Tuple<List<CrawlModelBase>, List<AutoCrawledPost>>(crawledSourceItems, autoCrawledPosts);
                            }
                            else
                            {
                                System.Console.WriteLine($"LOGIN FAILURE {CrawlStatus.UnknownFailure}====================================================================");
                                return new Tuple<List<CrawlModelBase>, List<AutoCrawledPost>>(crawledSourceItems, autoCrawledPosts);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        System.Console.WriteLine($"CRAWL ERROR ====================================================================");
                        System.Console.WriteLine($"{GetType().Name}: CRAWL ERROR {accountProxy.account.username}/{accountProxy.proxy.ip}:{accountProxy.proxy.port}");
                        System.Console.WriteLine($"{e.Message}");
                        System.Console.WriteLine($"================================================================================");
                        await e.Log(accountProxy.account.username, $"{accountProxy.account.username}/{accountProxy.proxy.ip}:{accountProxy.proxy.port}");
                        return new Tuple<List<CrawlModelBase>, List<AutoCrawledPost>>(crawledSourceItems, autoCrawledPosts);
                    }
                    finally
                    {
                        System.Console.WriteLine("CRAWL FINISHED. CLOSE BROWSER IN 1 SECs");
                        await Task.Delay(1000);
                        await browserContext.BrowserContext.CloseAsync();
                    }
                }
            }
        }

        protected override async Task<CrawlResult> CanCrawl(IPage page, CrawlModelBase crawlItem)
        {
            await page.CheckDoneButton();
            
            if (await page.IsPageNotFound()) { return new CrawlResult(CrawlStatus.GroupUnavailable); }

            if (await page.IsBlockedTemporary(crawlItem.Url)) return new CrawlResult(CrawlStatus.BlockedTemporary);

            if (await page.CanNotAccessPost(crawlItem.Url))
            {
                var isAlreadyJoined = await page.GotoAndJoinGroup(crawlItem.Url);
                if (isAlreadyJoined)
                {
                    var postUnavailableResult = new CrawlResult(CrawlStatus.PostUnavailable);
                    postUnavailableResult.Posts.Add
                    (
                        new AutoCrawledPost
                        {
                            Url = crawlItem.Url,
                            GroupFid = crawlItem.GroupFid,
                            CreateFuid = crawlItem.Fuid,
                            IsNotAvailable = true
                        }
                    );

                    return postUnavailableResult;
                }

                return new CrawlResult();
            }

            // var isJoinedGroup = await page.IsJoinedGroup();
            // if (!isJoinedGroup) { await page.JoinGroup(); }

            var ele_BlockedTemporary = await page.QuerySelectorAsync(FacebookConsts.Selector_BlockedTemporary);
            if (ele_BlockedTemporary == null)
            {
                return new CrawlResult();
            }

            var elementImage = await ele_BlockedTemporary.QuerySelectorAsync("//../../../div/img");
            return elementImage != null ? new CrawlResult() : new CrawlResult(CrawlStatus.BlockedTemporary);
        }


        protected override async Task<CrawlResult> DoCrawl(IPage page, CrawlModelBase crawlItem)
        {
            if (page.Url.Replace("www.","") != crawlItem.Url.Replace("www.","")) return new CrawlResult();
            AutoCrawledPost crawledPost;
            DateTime? createdAt;
            if (page.Url.ToLower().Contains("watch") || page.Url.ToLower().Contains("video"))
            {
                // var ele_video = await page.QuerySelectorAsync("//div[@id='watch_feed']//ancestor::span[@role='toolbar']");
                var ele_video = await page.QuerySelectorAsync("//div[@id='watch_feed']/div/div");
                if (ele_video != null)
                {
                    var crawlGroupUserPostService = new CrawlGroupUserPostService(GlobalConfig);
                    crawledPost = await crawlGroupUserPostService.CrawlVideoDetail(page, ele_video, crawlItem, crawlItem.Url);
                    createdAt = await CrawlVideoCreatedAt(page, ele_video);
                    
                    if (crawledPost == null) return new CrawlResult(CrawlStatus.UnknownFailure);
                    
                    if (createdAt.HasValue) { crawledPost.CreatedAt = createdAt.Value; }

                    var result = new CrawlResult();
                    
                    System.Console.WriteLine($"====================={this.GetType().Name}: Post:  {JsonConvert.SerializeObject(crawledPost)}");
                    
                    result.Posts.Add(crawledPost);

                    return result;
                }
            }
            else
            {
                // detect link is page post of user post
                var ele_Post = await page.QuerySelectorAsync(FacebookConsts.Selector_GroupPost);

                if (ele_Post != null)
                {
                    createdAt = await CrawlCreatedAt(page, ele_Post);
                    
                    if (crawlItem.Url.Contains("groups"))
                    {
                        // Crawl group user post
                        var crawlGroupUserPostService = new CrawlGroupUserPostService(GlobalConfig);
                        crawledPost = await crawlGroupUserPostService.CrawlPostDetail(page, ele_Post, crawlItem, crawlItem.Url);
                    }
                    else
                    {
                        // Crawl page post
                        var crawlPagePostService = new CrawlPagePostService(GlobalConfig);
                        crawledPost = await crawlPagePostService.CrawlPostDetail(page, ele_Post, crawlItem, crawlItem.Url);
                    }

                    if (crawledPost == null) return new CrawlResult(CrawlStatus.UnknownFailure);
                    
                    if (createdAt.HasValue) { crawledPost.CreatedAt = createdAt.Value; }

                    var result = new CrawlResult();
                    
                    System.Console.WriteLine($"====================={this.GetType().Name}: Post:  {JsonConvert.SerializeObject(crawledPost)}");
                    
                    result.Posts.Add(crawledPost);

                    return result;
                }
            }
            
            var ele_BlockedTemporary = await page.QuerySelectorAsync(FacebookConsts.Selector_BlockedTemporary);
            if (ele_BlockedTemporary == null)
            {
                return new CrawlResult(CrawlStatus.PostUnavailable);
            }

            var elementImage = await ele_BlockedTemporary.QuerySelectorAsync("//../../../div/img");
            return elementImage != null ? new CrawlResult(CrawlStatus.PostUnavailable) : new CrawlResult(CrawlStatus.BlockedTemporary);
        }
    }
}