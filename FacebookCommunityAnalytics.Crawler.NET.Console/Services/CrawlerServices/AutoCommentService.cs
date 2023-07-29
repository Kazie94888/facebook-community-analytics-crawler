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

namespace FacebookCommunityAnalytics.Crawler.NET.Console.Services.CrawlerServices
{
    public class AutoCommentService : CrawlServiceBase
    {
        public AutoCommentService(GlobalConfig globalConfig) : base(globalConfig)
        {
            AccountType = AccountType.NETFacebookGroupUserPost;
        }

        protected override AccountType AccountType { get; }
        protected override CrawlStopCondition CrawlStopCondition { get; }
        
        public override async Task Execute()
        {
            try
            {
                // 1. get comment Post
                var autoPostFacebookNotDoneResponse = ApiClient.Crawl.GetAutoPostFacebookNotDone();
                IList<AutoPostFacebookNotDoneDto> autoPostFacebookNotDone = new List<AutoPostFacebookNotDoneDto>();
                var accountProxyQueue = new Queue<AccountProxyItem>();
                if (autoPostFacebookNotDoneResponse.IsSuccess)
                {
                    autoPostFacebookNotDone = autoPostFacebookNotDoneResponse.Resource;
                    accountProxyQueue = GetAccountProxyQueue(AccountType);
                }

                if(!autoPostFacebookNotDone.Any()) return;
                
                // 2. Get crawl account
                if (!accountProxyQueue.Any()) return;
                
                //3. Do auto comment
                var batchSize = GlobalConfig.CrawlConfig.BatchSize_Max_AutoComment;
                var threadCount = GlobalConfig.CrawlConfig.Crawl_MaxThread_AutoComment;
                if (threadCount > accountProxyQueue.Count)
                {
                    threadCount = accountProxyQueue.Count - 1;
                }

                if (threadCount <= 0)
                {
                    System.Console.WriteLine($"{GetType().Name}: EXITING with THREADCOUNT = {threadCount}");
                    return;
                }
                
                System.Console.WriteLine($"{GetType().Name}: FOUND {autoPostFacebookNotDone.Count} comment posts");
                System.Console.WriteLine(
                    $"{GetType().Name}: FOUND ACCOUNTS {accountProxyQueue.Count}. CRAWL with {threadCount}");
                
                await autoPostFacebookNotDone.Partition(batchSize).ParallelForEachAsync(async batch =>
                {
                    AccountProxyItem accountProxy = null;
                    var isFinished = false;
                    var count = 0;
                    var crawlSource = batch.ToList();
                    if (crawlSource.IsNullOrEmpty()) return;
                    var autoPosts = crawlSource.Where(dto => dto.NeedComment != dto.CurrentComment).ToList();
                    CRAWL:
                    try
                    {
                        accountProxy = accountProxyQueue.Dequeue();
                        var crawlResult = await AutoComment(accountProxy, autoPosts);
                        if (!crawlResult.Any() || crawlResult.Count != autoPosts.Count)
                        {
                            autoPosts = autoPosts.Except(crawlResult).ToList();
                        }
                        else
                        {
                            isFinished = true;
                        }
                        
                        if (crawlResult.Any())
                        {
                            foreach (var autoPostFacebookNotDoneDto in crawlResult)
                            {
                                ApiClient.Crawl.UpdateLikeComment(new UpdateLikeCommentDto
                                {
                                    AutoPostFacebookId = autoPostFacebookNotDoneDto.Id,
                                    NumberLike = autoPostFacebookNotDoneDto.NeedLike + 1
                                });
                            }
                        }
                    }
                    catch (Exception e)
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
                    
                }, maxDegreeOfParallelism: threadCount);
            }
            finally
            {
                Thread.Sleep(5000);
            }
        }

        private async Task<List<AutoPostFacebookNotDoneDto>> AutoComment(AccountProxyItem accountProxy, List<AutoPostFacebookNotDoneDto> autoPosts)
        {
            var commentedPost = new List<AutoPostFacebookNotDoneDto>();
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
                        System.Console.WriteLine(
                            $"====================={this.GetType().Name}: Trying to login for {accountProxy.account.username}");
                        var loginResponse =
                            await facebookService.Login(browserContext.BrowserContext, page, accountProxy);
                        if (loginResponse.Success)
                        {
                            await page.Wait(1000);
                            System.Console.WriteLine(
                                $"====================={this.GetType().Name}: Trying to Auto Comment for {autoPosts.Count} posts");
                            foreach (var autoPost in autoPosts)
                            {
                                if (StringExtensions.IsNullOrEmpty(autoPost.Url))
                                {
                                    continue;
                                }

                                System.Console.WriteLine(
                                    $"====================={this.GetType().Name}: Trying to like post {autoPost.Url}");
                                var subpage = await browserContext.BrowserContext.NewPageAsync();
                                await subpage.GotoAsync(autoPost.Url);
                                await subpage.WaitForLoadStateAsync(LoadState.NetworkIdle,
                                    new PageWaitForLoadStateOptions
                                    {
                                        Timeout = 60000
                                    });

                                if (await facebookService.IsAccountBanned(subpage))
                                {
                                    System.Console.WriteLine(
                                        $"====================={GetType().Name}: {CrawlStatus.AccountBanned} {accountProxy.account.username}");
                                    await subpage.CloseAsync();
                                    return new List<AutoPostFacebookNotDoneDto>();
                                }

                                System.Console.WriteLine(
                                    $"====================={GetType().Name}: Check Banned Success {accountProxy.account.username}");
                                var canCrawl = await CanCrawl(subpage, autoPost.Url);
                                System.Console.WriteLine(
                                    $"====================={GetType().Name}: Can Crawl {canCrawl} {accountProxy.account.username}");
                                if (canCrawl)
                                {
                                    await DoCrawl(page);
                                    commentedPost.Add(autoPost);
                                }

                                await subpage.CloseAsync();
                            }
                        }
                        
                        return commentedPost;
                    }
                    catch (Exception e)
                    {
                        System.Console.WriteLine(
                            $"CRAWL ERROR ====================================================================");
                        System.Console.WriteLine(
                            $"{GetType().Name}: CRAWL ERROR {accountProxy.account.username}/{accountProxy.proxy.ip}:{accountProxy.proxy.port}");
                        System.Console.WriteLine($"{e.Message}");
                        System.Console.WriteLine(
                            $"================================================================================");
                        await e.Log(accountProxy.account.username,
                            $"{accountProxy.account.username}/{accountProxy.proxy.ip}:{accountProxy.proxy.port}");
                        return commentedPost;
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
        
        private async Task<bool> CanCrawl(IPage page, string url)
        {
            await page.CheckDoneButton();
            
            if (await page.IsPageNotFound()) { return false; }

            if (await page.IsBlockedTemporary(url)) return false;

            if (await page.CanNotAccessPost(url))
            {
                var isAlreadyJoined = await page.GotoAndJoinGroup(url);
                if (isAlreadyJoined)
                {
                    return false;
                }

                return true;
            }

            var ele_BlockedTemporary = await page.QuerySelectorAsync(FacebookConsts.Selector_BlockedTemporary);
            if (ele_BlockedTemporary == null)
            {
                return true;
            }

            var elementImage = await ele_BlockedTemporary.QuerySelectorAsync("//../../../div/img");
            return elementImage != null;
        }

        private async Task DoCrawl(IPage page)
        {
            var selectorComment =
                "//descendant::div[(@aria-label='Viết bình luận' or @aria-label='Write a comment') and @role='textbox']";
            var eleComment = await page.QuerySelectorAsync(selectorComment);
            if (eleComment != null)
            {
                await page.EvaluateAsync("eleComment => eleComment.scrollIntoViewIfNeeded(true)", eleComment);
                await page.WaitASecond();

                await eleComment.TypeAsync("very beautiful", new ElementHandleTypeOptions
                {
                    Delay = 100
                });
                await eleComment.PressAsync("Enter");
            }

            await page.WaitASecond();
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
}