using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using FacebookCommunityAnalytics.Crawler.NET.Client.Configurations;
using FacebookCommunityAnalytics.Crawler.NET.Client.Entities;
using FacebookCommunityAnalytics.Crawler.NET.Client.Enums;
using FacebookCommunityAnalytics.Crawler.NET.Console.Playwrights;
using FacebookCommunityAnalytics.Crawler.NET.Core.Extensions;
using Flurl;
using Microsoft.Playwright;
using Newtonsoft.Json;

namespace FacebookCommunityAnalytics.Crawler.NET.Console.Services.CrawlerServices;

public class CrawlReelPostService : CrawlServiceBase
{
    public CrawlReelPostService(GlobalConfig globalConfig) : base(globalConfig)
    {
    }

    protected override AccountType        AccountType        { get; }
    protected override CrawlStopCondition CrawlStopCondition { get; }

    public override Task Execute()
    {
        throw new System.NotImplementedException();
    }

    protected override Task<CrawlResult> CanCrawl(IPage page, CrawlModelBase crawlItem)
    {
        throw new System.NotImplementedException();
    }

    protected override async Task<CrawlResult> DoCrawl(IPage page, CrawlModelBase crawlItem)
    {
        throw new System.NotImplementedException();
    }

    public async Task<DateTime?> GetCreateAt(IPage page, IElementHandle ele_Post)
    {
        var ele_ParentPostDate = await GetElementPostCreatedAt(ele_Post, page);
        await page.EvaluateAsync("ele_ParentPostDate => ele_ParentPostDate.scrollIntoViewIfNeeded(true)", ele_ParentPostDate);
        await page.WaitASecond();
        await page.HoverAndWait(ele_ParentPostDate, FacebookConsts.Selector_ToolTip);
        // ele_ParentPostDate = await GetElementPostCreatedAt(ele_Post, page);
        var createdAt = await CrawlCreatedAt(page, ele_Post);
        return createdAt;
    }

    public async Task<AutoCrawledPost> CrawlReelPost(IPage page, IElementHandle ele_Post, string groupFid)
    {
        var ele_UrlPost                                         = await ele_Post.QuerySelectorAsync("//descendant::a[contains(@href,'reel')]");
        var url                                                 = await ele_UrlPost.GetAttributeAsync("href");
        if (url.IsNotNullOrEmpty() && url.Contains("?")) url    = url.Split('?').FirstOrDefault();
        if (url.IsNotNullOrEmpty() && url.Contains("//m.")) url = url.Replace("//m.", "//www.");
        if (!url.Contains("https://www.facebook.com"))
        {
            url = Url.Combine("https://www.facebook.com", url);
        }
        var sourcePost                                          = await CrawlPostDetail(page, ele_Post, groupFid, url);
        return sourcePost;
    }

    private async Task<AutoCrawledPost> CrawlPostDetail(IPage page, IElementHandle ele_Post, string groupid, string url)
    {
        var createdByPair = await CrawlCreatedBy(ele_Post);
        var sourcePost = new AutoCrawledPost
        {
            Url            = url,
            PostSourceType = PostSourceType.Group,
            CreateFuid     = createdByPair.Key,
            GroupFid       = groupid,
            CreatedBy      = createdByPair.Value,
            // TODO: get emoji/icon
            Content = await CrawlContent(ele_Post)
        };
        sourcePost.Urls         = await CrawlUrls(sourcePost.Content);
        sourcePost.HashTags     = FacebookHelper.GetHashtags(sourcePost.Content);
        sourcePost.LikeCount    = await CrawlReactionCount(page, ele_Post);
        sourcePost.CommentCount = await CrawlDefaultCount(page, ele_Post, GetSelectorComment());
        sourcePost.ShareCount   = await CrawlShareCount(page, ele_Post);
        if (sourcePost.ShareCount == 0) sourcePost.ShareCount = await CrawlDefaultCount(page, ele_Post, GetSelectorShare());
        return sourcePost;
    }

    private async Task<string> CrawlContent(IElementHandle ele_Post)
    {
        var ele_ParrentContent = await ele_Post.QuerySelectorAsync("//descendant::a");
        var ele_Content        = await ele_ParrentContent.QuerySelectorAsync("//descendant::span[boolean(text())]");
        if (ele_Content != null)
        {
            return await ele_Content.InnerTextAsync();
        }

        return string.Empty;
    }

    private static async Task<KeyValuePair<string, string>> CrawlCreatedBy(IElementHandle ele_Post)
    {
        var ele_User = await ele_Post.QuerySelectorAsync("//descendant::object/a[boolean(text()) and boolean(@aria-label) = false]");
        if (ele_User == null)
        {
            //descendant::a
            ele_User = await ele_Post.QuerySelectorAsync("//descendant::a");
            if (ele_User == null)
            {
                return default(KeyValuePair<string, string>);
            }           
        }
        var    href = await ele_User.GetAttributeAsync("href");
        string fuid = string.Empty;
        if (href.Contains("profile.php"))
        {
            fuid = new Url(href).QueryParams.FirstOrDefault().Value.ToString();
        }
        else
        {
            href = href.Replace("/groups/", string.Empty);
            fuid = new Url(href).Path.Trim('/');
            if (fuid.Contains("/user/"))
            {
                fuid = Regex.Split(fuid, "/user/")[1];
            }
        }

        return new KeyValuePair<string, string>(fuid, await ele_User.InnerTextAsync());
    }

    protected override async Task<IElementHandle> GetElementPostCreatedAt(IElementHandle ele_Post, IPage page)
    {
        var element = await ele_Post.QuerySelectorAsync("//descendant::span[text()='Reels']/span[boolean(text())]");
        return element;
    }
}