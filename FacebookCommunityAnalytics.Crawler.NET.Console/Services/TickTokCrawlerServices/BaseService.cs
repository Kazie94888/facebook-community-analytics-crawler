using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FacebookCommunityAnalytics.Crawler.NET.Client.Clients;
using FacebookCommunityAnalytics.Crawler.NET.Client.Configurations;
using FacebookCommunityAnalytics.Crawler.NET.Client.Entities;
using FacebookCommunityAnalytics.Crawler.NET.Client.Enums;
using FacebookCommunityAnalytics.Crawler.NET.Console.Models;
using FacebookCommunityAnalytics.Crawler.NET.Console.Playwrights;
using FacebookCommunityAnalytics.Crawler.NET.Core.Extensions;
using Microsoft.Playwright;

namespace FacebookCommunityAnalytics.Crawler.NET.Console.Services.TickTokCrawlerServices
{
    public class BaseService
    {
        protected ApiClient ApiClient { get; }
        protected GlobalConfig GlobalConfig { get; }
        
        protected BaseService(GlobalConfig globalConfig)
        {
            ApiClient = new ApiClient(globalConfig.ApiConfig);
            GlobalConfig = globalConfig;
        }
        protected static async Task<int> CrawlViewCount(IElementHandle ele_Video)
        {
            var elementHandle = await ele_Video.QuerySelectorAsync("//ancestor::strong");
            if (elementHandle == null) return 0;
            var viewCountText = await elementHandle.InnerTextAsync();
            return ConvertToTotalCount(viewCountText);
        }

        protected static async Task<string> CrawlThumbnailImage(IElementHandle ele_ThumbnailImage)
        {
            if (ele_ThumbnailImage == null) return string.Empty;
            var screenshot = await ele_ThumbnailImage.ScreenshotAsync();
            var base64String = Convert.ToBase64String(screenshot, 0, screenshot.Length);
            return base64String;
        }

        protected static int ConvertToTotalCount(string text)
        {
            if (text.ToUpper().Contains("K"))
            {
                return (int)(text.Trim('K').ToDecimalOrDefault() * 1000);
            }
            else if (text.ToUpper().Contains("M"))
            {

                return (int)(text.Trim('M').ToDecimalOrDefault() * 1000000);
            }
            else
            {
                return text.ToIntODefault();
            }
        }
        
        protected bool IsStopped(DateTime createdAt, CrawlStopCondition crawlStopCondition)
        {
            switch (crawlStopCondition)
            {
                case CrawlStopCondition.Weekly:
                    return createdAt < DateTime.UtcNow.AddDays(-7);
                case CrawlStopCondition.Monthly:
                    return createdAt < DateTime.UtcNow.AddDays(-32);
                case CrawlStopCondition.TwoWeek:
                    return createdAt < DateTime.UtcNow.AddDays(-14);
            }

            return false;
        }
        
        protected async Task CrawReaction(PlaywrightContext browserContext, string url, TiktokVideoDto tiktokVideoDto)
        {
            var subPage = await browserContext.BrowserContext.NewPageAsync();
            try
            {
                await subPage.GotoAsync(url, new PageGotoOptions{Timeout = 0});
            
                await subPage.WaitForLoadStateAsync(LoadState.Load, new PageWaitForLoadStateOptions{Timeout = 0});
                await subPage.Wait();
                
                var selector_like = "//strong[@data-e2e='like-count']";
                var ele_Like = await subPage.QuerySelectorAsync(selector_like);
                if (ele_Like == null)
                {
                    selector_like = "//strong[@data-e2e='browse-like-count']";
                    ele_Like = await subPage.QuerySelectorAsync(selector_like);
                }
                if (ele_Like != null)
                {
                    var like = await ele_Like.InnerTextAsync();

                    tiktokVideoDto.Like = ConvertToTotalCount(like);

                }
                
                var selector_Comment = "//strong[@data-e2e='comment-count']";
                var ele_Comment = await subPage.QuerySelectorAsync(selector_Comment);
                if (ele_Comment == null)
                {
                    selector_Comment = "//strong[@data-e2e='browse-comment-count']";
                    ele_Comment = await subPage.QuerySelectorAsync(selector_Comment);
                }
                if (ele_Comment != null)
                {
                    var comment = await ele_Comment.InnerTextAsync();
                    tiktokVideoDto.Comment = ConvertToTotalCount(comment);
                }
                
                var selector_Share = "//strong[@data-e2e='share-count']";
                var ele_Share = await subPage.QuerySelectorAsync(selector_Share);
                if (ele_Share == null)
                {
                    selector_Share = "//strong[@title='share']";
                    ele_Share = await subPage.QuerySelectorAsync(selector_Share);
                }
                if (ele_Share != null)
                {
                    var share = await ele_Share.InnerTextAsync();
                    tiktokVideoDto.Share = ConvertToTotalCount(share);
                }
                
                // Crawl Content
                tiktokVideoDto.Content = await CrawlContent(subPage);
                
                // Crawl Hash Tag
                tiktokVideoDto.HashTags = CrawlHashTags(tiktokVideoDto.Content);

                // Crawl Create At
                tiktokVideoDto.CreatedAt = await CrawlCreatedAt(subPage);
            }
            catch (Exception e)
            {
                System.Console.WriteLine(e.Message);
            }
            finally
            {
                await subPage.CloseAsync();
            }
        }

        private async Task<DateTime> CrawlCreatedAt(IPage subPage)
        {
            var selector_DateTime = "//span[@data-e2e='browser-nickname']/span[2]";
            var ele_DateTime = await subPage.QuerySelectorAsync(selector_DateTime);
            if (ele_DateTime == null)
            {
                selector_DateTime = "//h4[contains(@class,'author-nickname')]";
                ele_DateTime = await subPage.QuerySelectorAsync(selector_DateTime);
                if (ele_DateTime == null)
                {
                    selector_DateTime = "//button[@data-e2e='feed-follow']/../div[1]/a[2]";
                    ele_DateTime = await subPage.QuerySelectorAsync(selector_DateTime);
                }
            }

            if (ele_DateTime is not null)
            {
                var text = await ele_DateTime.InnerTextAsync();
                text = text.Trim();
                if (text.Equals("·"))
                {
                    selector_DateTime = "//span[@data-e2e='browser-nickname']/span[3]";
                    ele_DateTime      = await subPage.QuerySelectorAsync(selector_DateTime);
                }
            }
            

            if (ele_DateTime != null)
            {
                var text = await ele_DateTime.InnerTextAsync();
                if (text.Contains(" · "))
                {
                    text = text.Split(" · ")[1].Trim();
                }
                
                if (text.Contains("m ago"))
                {
                    var minutes = text.Split("m ago")[0].ToDoubleOrDefault();
                    return DateTime.UtcNow.AddMinutes(-minutes);
                }

                if (text.Contains("h ago"))
                {
                    var hours = text.Split("h ago")[0].ToDoubleOrDefault();
                    return DateTime.UtcNow.AddHours(-hours);
                }

                if (text.Contains("d ago"))
                {
                    var days = text.Split("d ago")[0].ToDoubleOrDefault();
                    return DateTime.UtcNow.AddDays(-days);
                }

                if (text.Contains("w ago"))
                {
                    var weeks = text.Split("w ago")[0].ToIntOrDefault();
                    return DateTime.UtcNow.AddDays(-weeks * 7);
                }

                if (text.Split("-").Length == 3)
                {
                    var year = text.Split("-")[0].ToIntOrDefault();
                    var month = text.Split("-")[1].ToIntOrDefault();
                    var day = text.Split("-")[2].ToIntOrDefault();
                    return new DateTime(year, month, day);
                }
                else
                {
                    var month = text.Split("-")[0].ToIntOrDefault();
                    var day = text.Split("-")[1].ToIntOrDefault();
                    if (DateTime.UtcNow.Month == 1 && month == 12)
                    {
                        return new DateTime(DateTime.UtcNow.Year - 1, month, day);
                    }

                    return new DateTime(DateTime.UtcNow.Year, month, day);
                }
            }

            return new DateTime();
        }

        private static async Task<string> CrawlContent(IPage subPage)
        {
            var select_Content = "//div[@data-e2e='browse-video-desc' or @data-e2e='video-desc']";
            var ele_Content = await subPage.QuerySelectorAsync(select_Content);
            string content = string.Empty;
            if (ele_Content == null)
            {
                select_Content = "//div[contains(@class,'tt-video-meta-caption')]";
                ele_Content = await subPage.QuerySelectorAsync(select_Content);
            }
            if (ele_Content != null)
            {
                content = await ele_Content.InnerTextAsync();
                
            }

            return content;
        }

        private static List<string> CrawlHashTags(string content)
        {
            if (!string.IsNullOrWhiteSpace(content))
            {
                return FacebookHelper.GetHashtags(content);
            }
            
            return new List<string>();
        }
        
        protected List<AccountProxyItem> GetAccountProxyQueue(AccountType accountType)
        {
            return GetAccountProxyItems(accountType);
        }
        
        private List<AccountProxyItem> GetAccountProxyItems(AccountType accountType)
        {
            var res = ApiClient.Crawl.GetAccountProxies(new GetAccountProxiesRequest {AccountType = accountType});
            if (!res.IsSuccess || res.Resource.IsNullOrEmpty()) return null;
            var accountProxyItems = res.Resource.Where(_ => _.account.accountStatus == AccountStatus.Active).ToList();
            accountProxyItems = accountProxyItems.OrderBy(item => item.accountProxy.CrawledAt).ToList();
            return accountProxyItems;
        }
    }
}