using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ClosedXML.Excel;
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
    public class CrawlByHashTagService : CrawlServiceBase
    {
        public CrawlByHashTagService(GlobalConfig globalConfig) : base(globalConfig)
        {
            AccountType = AccountType.TestLoginComplete;
        }

        protected override AccountType AccountType { get; }
        protected override CrawlStopCondition CrawlStopCondition { get; }

        public override async Task Execute()
        {
            // 1. Get HashTag
            IDictionary<string, IList<string>> hashTags = new Dictionary<string, IList<string>>
            {
                { "2931569376876318", new List<string>{"tungchangduongdaimataqua"} }
            };
            
            System.Console.WriteLine($"{GetType().Name}: found {hashTags.Count} hashtags");
            await Task.Delay(3000);
            if (hashTags.IsNullOrEmpty())
            {
                System.Console.WriteLine($"{GetType()}: exit in 5 seconds, no hastags found");
                await Task.Delay(5000);
            }
            
            // 2. Get crawl account
            var accountProxyQueue = GetAccountProxyQueue(AccountType);
            if (accountProxyQueue == null) return;

            await hashTags.ParallelForEachAsync(async hashTag =>
            {
                AccountProxyItem accountProxy = null;
                try
                {
                    accountProxy = accountProxyQueue.Dequeue();
                    
                    var crawlModelBases = hashTag.Value.Select(hasTagsValue => new CrawlModelBase
                    {
                        Url = $"https://www.facebook.com/hashtag/{hasTagsValue}?__gid__={hashTag.Key}",
                        GroupFid = hashTag.Key
                    }).ToList();

                    var crawlResult = await Crawl(accountProxy, crawlModelBases);

                    if (crawlResult is { Success: true })
                    {
                        // 4. Save crawl result to Excel
                        var jsonText = JsonConvert.SerializeObject(crawlResult.Posts);
                        var dt = (DataTable)JsonConvert.DeserializeObject(jsonText, (typeof(DataTable)));

                        if (dt != null)
                        {
                            dt.Columns.Remove("Urls");
                            dt.Columns.Remove("HashTags");

                            using (var workbook = new XLWorkbook())
                            {
                                workbook.Worksheets.Add(dt, "Result");
                                workbook.SaveAs($"Result_{string.Join('-',hashTag.Value)}_{hashTag.Key}.xlsx");
                            }
                        }
                    }
                }
                finally
                {
                    if (accountProxy != null)
                    {
                        accountProxyQueue.Enqueue(accountProxy);
                    }
                }
            }, 1);
        }

        protected override async Task<CrawlResult> CanCrawl(IPage page, CrawlModelBase crawlItem)
        {
            await page.CheckDoneButton();
            
            if (await page.IsPageNotFound()) { return new CrawlResult(CrawlStatus.GroupUnavailable); }

            var ele_PermissionImage = await page.QuerySelectorAsync(FacebookHelper.Image_GroupPermission);
            if (ele_PermissionImage != null)
            {
                var postUnavailableResult = new CrawlResult(CrawlStatus.GroupUnavailable);
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
            return elementImage == null ? new CrawlResult(CrawlStatus.BlockedTemporary) : new CrawlResult();
        }

        protected override async Task<CrawlResult> DoCrawl(IPage page, CrawlModelBase crawlItem)
        {
            var crawlResult = new CrawlResult();
            int count = 0;

            while (true)
            {
                var ele_Posts = await page.QuerySelectorAllAsync(GetAllPosts());
                
                if(ele_Posts.Count == crawlResult.Posts.Count) break;
                
                ele_Posts = ele_Posts.Skip(count).ToList();
                count += 1;
                
                var ele_Post = ele_Posts.First();
                var ele_ParentPostDate = await GetElementPostCreatedAt(ele_Post, page);
                
                await page.EvaluateAsync("ele_ParentPostDate => ele_ParentPostDate.scrollIntoViewIfNeeded(true)", ele_ParentPostDate);
                await page.WaitForLoadStateAsync(LoadState.NetworkIdle);
                await page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
                
                await page.HoverAndWait(ele_ParentPostDate, FacebookConsts.Selector_ToolTip);
                ele_ParentPostDate = await GetElementPostCreatedAt(ele_Post, page);
                
                var createdAt = await CrawlCreatedAt(page, ele_Post);
                if (createdAt.HasValue)
                {
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
                    await page.Wait(GlobalConfig.CrawlConfig.ScrollTimeout);
                }
            }
            
            return crawlResult;
        }
        
        protected override async Task<IElementHandle> GetElementPostCreatedAt(IElementHandle ele_Post, IPage page)
        {
            return (await ele_Post.QuerySelectorAllAsync("//ancestor::a[boolean(@href) and boolean(@aria-label) ]"))[1];
        }
        
        protected override string GetSelectorReaction()
        {
            return FacebookConsts.Selector_PagePostReaction;
        }

        protected override string GetSelectorComment()
        {
            return "//span[@role='toolbar']/../../div[2]/div[1]";
        }

        protected override string GetSelectorShare()
        {
            return "//span[@role='toolbar']/../../div[2]/div[2]";
        }

        private string GetAllPosts()
        {
            return "//div[@role='article' and boolean(@tabindex) = false and div[boolean(@role) = false]]";
        }
    }
}