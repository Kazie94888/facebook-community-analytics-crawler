using System;
using System.Collections.Concurrent;
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
using Microsoft.Playwright;
using Newtonsoft.Json;

namespace FacebookCommunityAnalytics.Crawler.NET.Console.Services.CrawlerServices
{
    public class CrawlPagePostService : CrawlServiceBase
    {
        protected override AccountType AccountType { get; }
        protected override CrawlStopCondition CrawlStopCondition { get; }
        private bool _ignoreTime;

        public CrawlPagePostService(GlobalConfig globalConfig, bool ignoreTime = false, CrawlStopCondition crawlStopCondition = CrawlStopCondition.FourHour) : base(globalConfig)
        {
            AccountType = AccountType.NETFacebookPagePost;
            CrawlStopCondition = crawlStopCondition;
            _ignoreTime = ignoreTime;
        }

        // TODOO Vu.Dao: can we remove 'pageName'
        private string GetSelectorPostsMultipleFeeds(string pageName)
        {
            return string.Format(FacebookConsts.Selector_PagePostsMultipleFeeds, pageName);
        }
        
        private string GetSelectorPosts(string pageName)
        {
            return string.Format(FacebookConsts.Selector_PagePosts, pageName);
        }

        private string GetSelectorFeed(string pageName)
        {
            return string.Format(FacebookConsts.Selector_PageFeeds, pageName);
        }

        protected override string GetSelectorReaction()
        {
            return FacebookConsts.Selector_PagePostReaction;
        }

        public override async Task Execute()
        {
            // 1. get source crawl data
            var uncrawledPages = ApiClient.Crawl.GetUncrawledGroups(new GetUncrawledGroupApiRequest
            {
                GroupSourceType = GroupSourceType.Page,
                IgnoreTime = _ignoreTime
            });
            var crawlItems = uncrawledPages.Resource.Groups;
            System.Console.WriteLine($"{GetType().Name}: found {crawlItems.Count} pages");
            await Task.Delay(3000);
            if (crawlItems.IsNullOrEmpty())
            {
                System.Console.WriteLine($"{GetType()}: exit in 5 seconds, no pages found");
                await Task.Delay(5000);
            }

            // 2. Get crawl account
            var accountProxyQueue = GetAccountProxyQueue(AccountType);
            if (accountProxyQueue == null) return;
            
            // 3. Crawl
            await crawlItems.ParallelForEachAsync
                (
                    async page =>
                    {
                        AccountProxyItem accountProxy = null;
                        bool isFinished = false;
                        CRAWL:
                        try
                        {
                            accountProxy = accountProxyQueue.Dequeue();

                            var crawlResult = await Crawl(accountProxy, page.ToCrawlModelBase());

                            if (crawlResult is { Success: true })
                            {
                                // 4. Save crawl result
                                var response = ApiClient.Crawl.PostAutoCrawlResult
                                (
                                    new SaveAutoCrawlResultApiRequest
                                    {
                                        Items = crawlResult.Posts,
                                        GroupFid = @page.fid,
                                        CrawlType = CrawlType.PagePost
                                    }
                                );
                                
                                isFinished = true;
                            }
                            ResetAccountsCrawlStatus(new List<Guid> { new Guid(accountProxy.accountProxy.id) }, AccountType);
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
                            goto CRAWL;
                        }
                        
                    }, GlobalConfig.CrawlConfig.Crawl_MaxThread_PagePost
                );
            
        }

        protected override async Task<CrawlResult> CanCrawl(IPage page, CrawlModelBase crawlItem)
        {
            await page.CheckDoneButton();
            
            if (await page.IsPageNotFound()) { return new CrawlResult(CrawlStatus.GroupUnavailable); }

            if (await page.CanNotAccessPost(crawlItem.Url))
            {
                await page.GotoAndJoinGroup(crawlItem.Url);
                return new CrawlResult(CrawlStatus.PostUnavailable);
            }

            return new CrawlResult();
        }


        protected override async Task<CrawlResult> DoCrawl(IPage page, CrawlModelBase crawlItem)
        {
            var crawlResult = new CrawlResult();

            var ele_PageName = await page.QuerySelectorAsync(FacebookConsts.Selector_PageName) ?? await page.QuerySelectorAsync("//div[@role='main']//h2/span");
            if (ele_PageName == null)
            {
                System.Console.WriteLine("Page tach {0}", page.Url);
                return crawlResult;
            }
            
            var pageName = await ele_PageName.InnerTextAsync();

            int count = 0;
            bool correctDateTime = false;
            int height = 100;
            bool makeSureNoPost = false;
            while (true)
            {
                RUN_AGAIN:
                var ele_feeds = await page.QuerySelectorAllAsync(GetSelectorFeed(pageName));
                IReadOnlyList<IElementHandle> ele_Posts;
                if (ele_feeds.Count == 2)
                {
                    ele_Posts = await ele_feeds[1].QuerySelectorAllAsync(GetSelectorPostsMultipleFeeds(pageName));
                }
                else
                {
                    ele_Posts = await page.QuerySelectorAllAsync(GetSelectorPosts(pageName));
                }
                
                if (ele_Posts.Count == 0)
                {
                    // This code for case header of page or group is high
                    await page.EvaluateAsync("height => window.scrollTo(0, height)", height);
                    height += 100;
                    await page.Wait(1000);
                    goto RUN_AGAIN;
                }
                
                if (ele_Posts.Count == crawlResult.Posts.Count)
                {
                    // This code for case page is loaded slowly
                    if (makeSureNoPost == false)
                    {
                        await page.Wait(5000);
                        makeSureNoPost = true;
                        goto RUN_AGAIN;
                    }
                    break;
                }

                makeSureNoPost = false;

                ele_Posts = ele_Posts.Skip(count).ToList();
                count += 1;
                
                var ele_Post = ele_Posts.First();

                // var ele_ParentPostDate = await ele_Post.QuerySelectorAsync("//ancestor::a[@href= '#']");
                var ele_ParentPostDate = await GetElementPostCreatedAt(ele_Post, page);

                await page.EvaluateAsync("ele_ParentPostDate => ele_ParentPostDate.scrollIntoViewIfNeeded(true)", ele_ParentPostDate);

                await page.HoverAndWait(ele_ParentPostDate, FacebookConsts.Selector_ToolTip);

                ele_ParentPostDate = await GetElementPostCreatedAt(ele_Post, page);
                var createdAt = await CrawlCreatedAt(page, ele_Post);
                if (createdAt.HasValue)
                {
                    if (IsStopConditionMet(createdAt.Value))
                    {
                        if (correctDateTime)
                        {
                            return crawlResult;
                        }

                        correctDateTime = true;
                    }

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

                if (count % 5 == 0)
                {
                    await page.Wait(GlobalConfig.CrawlConfig.ScrollTimeout - 1000);
                }
            }

            return crawlResult;
        }
        
        protected override async Task<IElementHandle> GetElementPostCreatedAt(IElementHandle ele_Post, IPage page)
        {
            return (await ele_Post.QuerySelectorAllAsync("//ancestor::a[boolean(@href)]"))[3];
        }

        public override async Task<AutoCrawledPost> CrawlPostDetail(
            IPage page,
            IElementHandle ele_Post,
            CrawlModelBase crawlItem,
            string url)
        {
            var createdByPair = await CrawlCreatedBy(ele_Post);
            var sourcePost = new AutoCrawledPost
            {
                Url = url,
                PostSourceType = PostSourceType.Page,
                GroupFid = crawlItem.GroupFid,
                // CreateFuid = crawlItem.Fuid,
                CreateFuid = createdByPair.Key,
                CreatedBy = createdByPair.Value,
                // TODO: get emoji/icon
                Content = await CrawlContent(page, ele_Post)
            };

            sourcePost.Urls = await CrawlUrls(sourcePost.Content);
            sourcePost.HashTags = FacebookHelper.GetHashtags(sourcePost.Content);

            sourcePost.LikeCount = await CrawlReactionCount(page, ele_Post);
            sourcePost.CommentCount = await CrawlDefaultCount(page, ele_Post, FacebookConsts.Selector_Comment);
            sourcePost.ShareCount = await CrawlShareCount(page, ele_Post);
            if (sourcePost.ShareCount == 0) sourcePost.ShareCount = await CrawlDefaultCount(page, ele_Post, FacebookConsts.Selector_Share);

            return sourcePost;
        }
    }
}