using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    public class CrawlGroupPostService : CrawlServiceBase
    {
        protected override AccountType        AccountType        { get; }
        protected override CrawlStopCondition CrawlStopCondition { get; }
        private readonly   bool               _ignoreTime;

        public CrawlGroupPostService(GlobalConfig globalConfig, bool ignoreTime = false, CrawlStopCondition crawlStopCondition = CrawlStopCondition.FourHour) : base(globalConfig)
        {
            AccountType        = AccountType.NETFacebookGroupPost;
            CrawlStopCondition = crawlStopCondition;
            _ignoreTime        = ignoreTime;
        }

        private string GetSelectorPost()
        {
            return FacebookConsts.Selector_GroupPosts;
        }

        /// <summary>
        /// Share same selector (group post and page post)
        /// </summary>
        protected override string GetSelectorReaction()
        {
            return FacebookConsts.Selector_PagePostReaction;
        }

        public override async Task Execute()
        {
            // 1. get source crawl data
            // var groupsResponse = ApiClient.Group.GetList(new GetGroupsRequest {GroupSourceType = GroupSourceType.Group});
            // if (!groupsResponse.IsSuccess) return;
            // var crawlItems = groupsResponse.Resource.items.Where(group => group.groupSourceType == GroupSourceType.Group).ToList();

            // 1. get source crawl data
            var uncrawledGroups = ApiClient.Crawl.GetUncrawledGroups(new GetUncrawledGroupApiRequest {GroupSourceType = GroupSourceType.Group, IgnoreTime = _ignoreTime});
            var crawlItems      = uncrawledGroups.Resource.Groups;
            // crawlItems = crawlItems.Where(group => group.fid is "2931569376876318" or "956982674510656" or "615968292219372").ToList();
            // crawlItems = crawlItems.Where(group => group.fid is "956982674510656" ).ToList();
            crawlItems = crawlItems.Where(group => !string.IsNullOrWhiteSpace(group.url)).ToList();
            System.Console.WriteLine($"{GetType().Name}: found {crawlItems.Count} groups");
            await Task.Delay(3000);
            if (crawlItems.IsNullOrEmpty())
            {
                System.Console.WriteLine($"{GetType()}: exit in 5 seconds, no groups found");
                await Task.Delay(5000);
            }

            // for now, only HDP group
            // crawlItems = crawlItems.Where(_ => _.groupOwnershipType == GroupOwnershipType.HappyDay).ToList();

            // 2. Get crawl account
            var accountProxyQueue = GetAccountProxyQueue(AccountType);
            if (accountProxyQueue == null) return;
            await crawlItems.ParallelForEachAsync(async @group =>
            {
                AccountProxyItem accountProxy = null;
                bool             isFinished   = false;
                CRAWL:
                try
                {
                    // 3. crawl
                    accountProxy = accountProxyQueue.Dequeue();
                    var crawlResult = await Crawl(accountProxy, @group.ToCrawlModelBase());
                    if (crawlResult.Posts.Any())
                    {
                        // 4. Save crawl result
                        var response = ApiClient.Crawl.PostAutoCrawlResult(new SaveAutoCrawlResultApiRequest
                        {
                            Items = crawlResult.Posts, GroupFid = @group.fid, CrawlType = CrawlType.GroupPost
                        });
                        isFinished = true;
                    }

                    ResetAccountsCrawlStatus(new List<Guid> {new Guid(accountProxy.accountProxy.id)}, AccountType);
                }
                catch (Exception exception)
                {
                    System.Console.ForegroundColor = ConsoleColor.Red;
                    System.Console.WriteLine(exception.Message);
                    System.Console.ResetColor();
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
            },
            GlobalConfig.CrawlConfig.Crawl_MaxThread_GroupPost);
        }

        protected override async Task<CrawlResult> CanCrawl(IPage page, CrawlModelBase crawlItem)
        {
            await page.CheckDoneButton();
            if (await page.IsPageNotFound())
            {
                return new CrawlResult(CrawlStatus.GroupUnavailable);
            }

            var ele_PermissionImage = await page.QuerySelectorAsync(FacebookHelper.Image_GroupPermission);
            if (ele_PermissionImage != null)
            {
                var postUnavailableResult = new CrawlResult(CrawlStatus.GroupUnavailable);
                postUnavailableResult.Posts.Add(new AutoCrawledPost {Url = crawlItem.Url, GroupFid = crawlItem.GroupFid, CreateFuid = crawlItem.Fuid, IsNotAvailable = true});
                return postUnavailableResult;
            }

            if (await page.CanNotAccessPost(crawlItem.Url))
            {
                var isAlreadyJoined = await page.GotoAndJoinGroup(crawlItem.Url);
                if (isAlreadyJoined)
                {
                    var postUnavailableResult = new CrawlResult(CrawlStatus.PostUnavailable);
                    postUnavailableResult.Posts.Add(new AutoCrawledPost {Url = crawlItem.Url, GroupFid = crawlItem.GroupFid, CreateFuid = crawlItem.Fuid, IsNotAvailable = true});
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
            var  crawlResult      = new CrawlResult();
            int  count            = 0;
            bool correctDateTime  = false;
            int  countCorrectTime = 0;
            int  height           = 100;
            bool makeSureNoPost   = false;
            while (true)
            {
                RUN_AGAIN:
                var ele_Posts = await page.QuerySelectorAllAsync(GetSelectorPost());
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

                makeSureNoPost =  false;
                ele_Posts      =  ele_Posts.Skip(count).ToList();
                count          += 1;
                var             ele_Post   = ele_Posts.First();
                var             isReelPost = await IsReelPost(ele_Post);
                AutoCrawledPost sourcePost = null;
                DateTime?       createdAt  = null;
                if (isReelPost)
                {
                    Debug:
                    try
                    {
                        var crawlReelPostService = new CrawlReelPostService(GlobalConfig);
                        createdAt = await crawlReelPostService.GetCreateAt(page, ele_Post);
                        if (createdAt.HasValue)
                        {
                            if (IsStopConditionMet(createdAt.Value))
                            {
                                if (correctDateTime)
                                {
                                    return crawlResult;
                                }

                                if (countCorrectTime == 5)
                                {
                                    correctDateTime = true;
                                }
                                else
                                {
                                    countCorrectTime += 1;
                                }
                            }

                            sourcePost = await crawlReelPostService.CrawlReelPost(page, ele_Post, crawlItem.GroupFid);
                        }
                    }
                    catch (Exception e)
                    {
                        System.Console.WriteLine(e.Message);
                        goto Debug;
                    }
                }
                else
                {
                    var ele_ParentPostDate = await GetElementPostCreatedAt(ele_Post, page);
                    if (ele_ParentPostDate != null)
                    {
                        await page.EvaluateAsync("ele_ParentPostDate => ele_ParentPostDate.scrollIntoViewIfNeeded(true)", ele_ParentPostDate);
                        await page.WaitASecond();
                        await page.HoverAndWait(ele_ParentPostDate, FacebookConsts.Selector_ToolTip);
                        ele_ParentPostDate = await GetElementPostCreatedAt(ele_Post, page);
                        createdAt          = await CrawlCreatedAt(page, ele_Post);
                        if (createdAt.HasValue)
                        {
                            if (IsStopConditionMet(createdAt.Value))
                            {
                                if (correctDateTime)
                                {
                                    return crawlResult;
                                }

                                if (countCorrectTime == 5)
                                {
                                    correctDateTime = true;
                                }
                                else
                                {
                                    countCorrectTime += 1;
                                }
                            }

                            var url                                                 = await ele_ParentPostDate.GetAttributeAsync("href");
                            if (url.IsNotNullOrEmpty() && url.Contains("?")) url    = url.Split('?').FirstOrDefault();
                            if (url.IsNotNullOrEmpty() && url.Contains("//m.")) url = url.Replace("//m.", "//www.");
                            sourcePost = await CrawlPostDetail(page, ele_Post, crawlItem, url);
                        }
                    
                    }
                }

                if (sourcePost != null)
                {
                    sourcePost.CreatedAt = createdAt.Value;
                    System.Console.WriteLine($"====================={this.GetType().Name}: Post:  {JsonConvert.SerializeObject(sourcePost)}");
                    crawlResult.Posts.Add(sourcePost);
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
            IElementHandle element = null;
            var            count   = 0;
            while (element == null && count <= 10)
            {
                element = await ele_Post.QuerySelectorAsync("//*[local-name() = 'svg'][@title]//ancestor::span//ancestor::a");
                if (element == null) await page.Wait(1000);
                count += 1;
            }

            return element;
        }
    }
}