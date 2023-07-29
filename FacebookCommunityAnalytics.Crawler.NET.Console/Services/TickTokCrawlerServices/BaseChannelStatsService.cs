using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FacebookCommunityAnalytics.Crawler.NET.Client.Configurations;
using FacebookCommunityAnalytics.Crawler.NET.Client.Entities;
using FacebookCommunityAnalytics.Crawler.NET.Console.Playwrights;
using FacebookCommunityAnalytics.Crawler.NET.Core.Extensions;
using Microsoft.Playwright;

namespace FacebookCommunityAnalytics.Crawler.NET.Console.Services.TickTokCrawlerServices
{
    public class BaseChannelStatsService : BaseService
    {
        protected BaseChannelStatsService(GlobalConfig globalConfig) : base(globalConfig)
        {
        }
        
        protected async Task<ChannelStat> CrawlChannel(IPage page, string url, string channelId)
        {
            await page.GotoAsync(url);
            await page.WaitForLoadStateAsync(LoadState.Load);
            await page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
                        
            await page.Wait(2000);

            // ThumbnailImage
            var thumbnailImage = await CrawlThumbnailImage(page);
            
            // Followers
            var followers = await CrawlFollowers(page);

            // Title
            var title = await CrawlTitle(page);
                        
            // Likes
            var likes = await CrawlLikes(page);
                        
            // Description
            var description = await CrawlDescription(page);


            var channelStat = new ChannelStat
            {
                ChannelId = channelId,
                Description = description,
                Followers = followers,
                Likes = likes,
                Title = title,
                ThumbnailImage = thumbnailImage
            };
            System.Console.WriteLine($"=======================Channel {channelId}====================================");
            System.Console.WriteLine($" Description {description} - Follower {followers} - Likes {likes} - Title {title}");
            System.Console.WriteLine($"=================================================================================");
            return channelStat;
        }
        
        private async Task<int> CrawlFollowers(IPage page)
        {
            var selector_Followers = "//strong[@data-e2e='followers-count']";
            var ele_Followers = await page.QuerySelectorAsync(selector_Followers);
            if (ele_Followers == null)
            {
                selector_Followers = "//strong[@title='Followers']";
                ele_Followers = await page.QuerySelectorAsync(selector_Followers);
            }
            var text = await ele_Followers.InnerTextAsync();
            if (text.ToUpper().Contains("K"))
            {
                return (int)(text.Trim('K').ToDecimalOrDefault() * 1000);
            }

            if (text.ToUpper().Contains("M"))
            {

                return (int)(text.Trim('M').ToDecimalOrDefault() * 1000000);
            }

            return text.ToIntOrDefault();
        }

        private async Task<string> CrawlTitle(IPage page)
        {
            var selector_Profile = "//h1[@data-e2e='user-subtitle']";
            var ele_Profile = await page.QuerySelectorAsync(selector_Profile);
            if (ele_Profile == null)
            {
                selector_Profile = "//span[@class='profile']";
                ele_Profile = await page.QuerySelectorAsync(selector_Profile);
            }
            if (ele_Profile != null)
            {
                return await ele_Profile.InnerTextAsync();
            }

            return string.Empty;
        }

        private async Task<int> CrawlLikes(IPage page)
        {
            var selector_Likes = "//strong[@title='Likes']";
            var ele_Likes = await page.QuerySelectorAsync(selector_Likes);
            var text = await ele_Likes.InnerTextAsync();
            if (text.ToUpper().Contains("K"))
            {
                return (int)(text.Trim('K').ToDecimalOrDefault() * 1000);
            }

            if (text.ToUpper().Contains("M"))
            {

                return (int)(text.Trim('M').ToDecimalOrDefault() * 1000000);
            }

            return text.ToIntOrDefault();
        }

        private async Task<string> CrawlDescription(IPage page)
        {
            try
            {
                var selector_Description = "//h2[@data-e2e='user-bio']";
                var ele_Description = await page.QuerySelectorAsync(selector_Description);
                if (ele_Description == null)
                {
                    selector_Description = "//h2[contains(@class,'share-desc')]";
                    ele_Description = await page.QuerySelectorAsync(selector_Description);
                }
                if (ele_Description != null)
                {
                    var description = await ele_Description.InnerTextAsync();
                    var links = await CrawlLinks(page);
                    if (links.Any())
                    {
                        var linksString = string.Join("\r\n", links);

                        description = $"{description}\r\n{linksString}";
                    }

                    return description;
                }

                return string.Empty;
            }
            catch (Exception e)
            {
                return string.Empty;
            }
        }

        private async Task<string> CrawlThumbnailImage(IPage page)
        {
            var selector = "//div[@data-e2e='user-page']/div[1]/div[1]";
            var element = await page.QuerySelectorAsync(selector);
            if (element == null) return string.Empty;
            var screenshot = await element.ScreenshotAsync();
            var base64String = Convert.ToBase64String(screenshot, 0, screenshot.Length);
            return base64String;
        }

        private async Task<IList<string>> CrawlLinks(IPage page)
        {
            var selector_Links = "//div[contains(@class,'ShareLinks')]/a";
            var ele_Links = await page.QuerySelectorAllAsync(selector_Links);
            if (!ele_Links.Any())
            {
                selector_Links = "//div[@class='share-links']/a";
                ele_Links = await page.QuerySelectorAllAsync(selector_Links);
            }
            IList<string> links = new List<string>();
            foreach (var elementHandle in ele_Links)
            {
                links.Add(await elementHandle.GetAttributeAsync("href"));
            }

            return links;
        }

        
    }
}