using System;
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
using Newtonsoft.Json;

namespace FacebookCommunityAnalytics.Crawler.NET.Console.Services.CrawlerServices
{
    public class CrawlGroupUserPostService : CrawlServiceBase
    {
        protected override AccountType AccountType { get; }
        protected override CrawlStopCondition CrawlStopCondition { get; }

        public CrawlGroupUserPostService(GlobalConfig globalConfig) : base(globalConfig)
        {
            AccountType = AccountType.NETFacebookGroupUserPost;
            CrawlStopCondition = CrawlStopCondition.Weekly;
        }

        private string GetSelectorPosts()
        {
            return FacebookConsts.Selector_GroupPost;
        }
        
        private async Task<List<CrawlModelBase>> CrawlGroupUser(AccountProxyItem accountProxy, List<CrawlModelBase> crawlSourceItems)
        {
            List<CrawlModelBase> crawledSourceItems = new List<CrawlModelBase>();
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
                                if (StringExtensions.IsNullOrEmpty(crawlItem.Url) )
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
                                    return crawledSourceItems;
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
                                        
                                        var response = ApiClient.Crawl.PostAutoCrawlResult(new SaveAutoCrawlResultApiRequest
                                        {
                                            Items = crawlResult.Posts,
                                            GroupFid = crawlItem.GroupFid,
                                            CrawlType = CrawlType.GroupUser
                                        });
                                        
                                        System.Console.WriteLine($"{GetType().Name}: Status:{crawlResult.Status} - CRAWLED {crawlResult.Posts?.Count ?? 0} on {DateTime.UtcNow}");
                                        
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

                                        return crawledSourceItems;
                                    }

                                    default: throw new ArgumentOutOfRangeException("crawlerResult.Status");
                                }

                                await subpage.CloseAsync();
                            } // end for/end batch

                            return crawledSourceItems;
                        }
                        else
                        {
                            if (GenericExtensions.IsIn(loginResponse.Status, CrawlStatus.AccountBanned, CrawlStatus.LoginApprovalNeeded, CrawlStatus.BlockedTemporary))
                            {
                                System.Console.WriteLine($"====================={this.GetType().Name}: REMOVE ACCOUNT: {accountProxy.account.username} STATUS: {loginResponse.Status}");
                                LogBannedAccount(accountProxy.account, loginResponse.Status);
                                return crawledSourceItems;
                            }
                            else
                            {
                                System.Console.WriteLine($"LOGIN FAILURE {CrawlStatus.UnknownFailure}====================================================================");
                                return crawledSourceItems; 
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
                        return crawledSourceItems;
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
        
        public override async Task Execute()
        {
            // 1. get source crawl data
            var crawlSourceDataResponse = ApiClient.Crawl.GetUncrawledGroupUsers(new GetUncrawledGroupUserApiRequest());
            if (!crawlSourceDataResponse.IsSuccess) return;
            var crawlItems = crawlSourceDataResponse.Resource.Items;

            // 2. Get crawl account
            var accountProxyQueue = GetAccountProxyQueue(AccountType);
            if (accountProxyQueue == null) return;
            await crawlItems.ParallelForEachAsync
            (
                async groupBatch =>
                {
                    AccountProxyItem accountProxy = null;
                    bool isFinished = false;
                    var crawlSourceItems = groupBatch.ToCrawlModelBase();
                    CRAWL:
                    
                    try
                    {
                        accountProxy = accountProxyQueue.Dequeue();
                        var crawlResult = await CrawlGroupUser(accountProxy, crawlSourceItems);

                        if (crawlSourceItems.Count == crawlResult.Count)
                        {
                            isFinished = true;
                        }
                        else
                        {
                            crawlSourceItems = crawlSourceItems.Except(crawlResult).ToList();
                        }
                    }
                    catch (Exception exception)
                    {
                        isFinished = false;
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
                        goto CRAWL;
                    }
                },
                maxDegreeOfParallelism: GlobalConfig.CrawlConfig.Crawl_MaxThread_GroupUserPost
            );
        }

        protected override async Task<CrawlResult> CanCrawl(IPage page, CrawlModelBase crawlItem)
        {
            await page.CheckDoneButton();
            
            var url = page.Url;
            if (url != crawlItem.Url)
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
            
            if (await page.IsPageNotFound()) { return new CrawlResult(CrawlStatus.GroupUnavailable); }

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
            var crawlResult = new CrawlResult();

            if (page.Url != crawlItem.Url) return crawlResult;

            try
            {
                int count = 0;
                while (true)
                {
                    var ele_Posts = await page.QuerySelectorAllAsync(GetSelectorPosts());
                    if (ele_Posts.Count == crawlResult.Posts.Count) break;

                    ele_Posts = ele_Posts.Skip(count).ToList();
                    
                    count += 1;

                    var ele_Post = ele_Posts.First();
                    var ele_ParentPostDate = await GetElementPostCreatedAt(ele_Post, page);
                    
                    if (ele_ParentPostDate == null) continue;
                    
                    await page.EvaluateAsync("ele_ParentPostDate => ele_ParentPostDate.scrollIntoViewIfNeeded(true)", ele_ParentPostDate);

                    await page.HoverAndWait(ele_ParentPostDate, FacebookConsts.Selector_ToolTip);

                    ele_ParentPostDate = await GetElementPostCreatedAt(ele_Post, page);
                    if (ele_ParentPostDate != null)
                    {
                        var createdAt = await CrawlCreatedAt(page, ele_Post);
                        if (createdAt.HasValue)
                        {
                            if (IsStopConditionMet(createdAt.Value)) return crawlResult;

                            var url = await ele_ParentPostDate.GetAttributeAsync("href");
                            if (url.IsNotNullOrEmpty() && url.Contains("?")) url = url.Split('?').FirstOrDefault();
                            if (url.IsNotNullOrEmpty() && url.Contains("//m.")) url = url.Replace("//m.", "//www.");

                            var sourcePost = await CrawlPostDetail(page, ele_Post, crawlItem, url);

                            if (sourcePost != null)
                            {
                                sourcePost.CreatedAt = createdAt.Value;
                                
                                System.Console.WriteLine($"====================={this.GetType().Name}: Post:  {JsonConvert.SerializeObject(sourcePost)}");
                                
                                crawlResult.Posts.Add(sourcePost);
                            }
                        }

                    }
                    else
                    {
                        return crawlResult;
                    }

                    if (count % 5 == 0)
                    {
                        await page.Wait(GlobalConfig.CrawlConfig.ScrollTimeout);
                    }
                }

                return crawlResult;
            }
            catch (Exception e)
            {
                System.Console.WriteLine(
                    $"{GetType().Name}: ERROR ===================================================================================");
                System.Console.WriteLine($"{e.Message}");
                throw;
            }
        }
        
        protected override async Task<IElementHandle> GetElementPostCreatedAt(IElementHandle ele_Post, IPage page)
        {
            // return await ele_Post.QuerySelectorAsync(
            //     "//ancestor::a[boolean(@href) and boolean(@aria-label) and @tabindex=0 and boolean(ancestor::a[@tabindex=-1]) = false]");
            
            return await ele_Post.QuerySelectorAsync("//*[local-name() = 'svg'][@title]//ancestor::span//ancestor::a");
            
            
        }

        // private async Task<bool> IsNotValidDate(
        //     GlobalConfig globalConfig,
        //     IElementHandle ele_PostDate,
        //     IElementHandle ele_Post,
        //     IPage page)
        // {
        //     DateTime? createAt;
        //     switch (globalConfig.CrawlConfig.CrawlStopCondition)
        //     {
        //         case CrawlStopCondition.Daily:
        //             var ele_CreatedAt = await ele_PostDate.QuerySelectorAsync("span");
        //             return ele_CreatedAt != null && FacebookHelper.IsNotToday(await ele_CreatedAt.InnerTextAsync());
        //         case CrawlStopCondition.Monthly:
        //             createAt = await CrawlCreatedAt(page, ele_Post);
        //             if (createAt.HasValue && createAt.Value.Month != DateTime.Now.Month) { return true; }
        //
        //             return false;
        //         case CrawlStopCondition.FourHour:
        //             createAt = await CrawlCreatedAt(page, ele_Post);
        //             if (!createAt.HasValue) return false;
        //             return createAt.Value < DateTime.Now.AddHours(-4);
        //         
        //         case CrawlStopCondition.TwoDay:
        //         {
        //             createAt = await CrawlCreatedAt(page, ele_Post);
        //             if (!createAt.HasValue) return false;
        //             return createAt.Value < DateTime.Now.AddDays(-2);
        //         }
        //
        //         default:
        //             return true;
        //     }
        // }
    }
}