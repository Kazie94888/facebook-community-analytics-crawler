using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FacebookCommunityAnalytics.Crawler.NET.Client.Configurations;
using FacebookCommunityAnalytics.Crawler.NET.Client.Entities;
using FacebookCommunityAnalytics.Crawler.NET.Client.Enums;
using FacebookCommunityAnalytics.Crawler.NET.Console.Models;
using FacebookCommunityAnalytics.Crawler.NET.Console.Playwrights;
using FacebookCommunityAnalytics.Crawler.NET.Core.Extensions;
using Microsoft.Playwright;

namespace FacebookCommunityAnalytics.Crawler.NET.Console.Services.CrawlerServices
{
    public class CrawlMediumService : CrawlServiceBase
    {
        private readonly GlobalConfig _globalConfig;
        private string _url = "https://multichaincapital.medium.com";
        
        public CrawlMediumService(GlobalConfig globalConfig) : base(globalConfig)
        {
            _globalConfig = globalConfig;
        }

        public async Task Crawl()
        {
            // Clean userData folder name
            var userDataDir = $"{_globalConfig.CrawlConfig.UserDataDirRoot}/Medium";
            var dir = new DirectoryInfo(userDataDir);
            if (dir.Exists)
            {
                dir.Delete(true);
            }
            
            var browserContext =
                await PlaywrightHelper.InitPersistentBrowser(_globalConfig.CrawlConfig, null, false,
                    $"Medium");
            
            var results = new List<MediumResult>();

            using (browserContext.Playwright)
            {
                await using (browserContext.Browser)
                {
                    var page = await browserContext.BrowserContext.NewPageAsync();
                    
                    await page.GotoAsync(_url);
                    await page.WaitForLoadStateAsync(LoadState.Load);
                    await page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);

                    var selectorArticles = "//article";
                    
                    var count = 0;
                    while (true)
                    {
                        BEGIN:
                        var elementsArticles = await page.QuerySelectorAllAsync(selectorArticles);
                        if (count > elementsArticles.Count)
                        {
                            await page.EvaluateAsync("() => window.scrollTo(0, document.body.scrollHeight)");
                            await page.WaitForLoadStateAsync(LoadState.Load);
                            await page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
                            await page.WaitASecond();
                            count = 0;
                            goto BEGIN;
                        }
                        
                        var articles = elementsArticles.Skip(count).ToList();
                        if (articles.Any())
                        {
                            var article = articles.First();

                            // ---------------------------- Crawl Image ---------------------------------
                            var base64String = await CrawlImage(article);

                            // ---------------------------- Crawl Url -----------------------------------
                            var url = await CrawlUrl(article, page);

                            // --------------------------- Crawl Date Time ----------------------
                            var dateTime = await CrawlDateTime(article);

                            // ------------------------- Crawl Content -----------------------
                            var content = await CrawlContent(browserContext, url);

                            results.Add(new MediumResult
                            {
                                Content = content,
                                DateTime = dateTime,
                                Image = base64String,
                                Url = url
                            });

                            count += 1;
                        }
                    }
                }
            }
        }

        private static async Task<string> CrawlContent(PlaywrightContext browserContext, string url)
        {
            var subPage = await browserContext.BrowserContext.NewPageAsync();
            await subPage.GotoAsync(url);
            await subPage.WaitForLoadStateAsync(LoadState.Load);
            await subPage.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
            var selectorArticle = "//article";
            var elementArticle = await subPage.QuerySelectorAsync(selectorArticle);
            string content = string.Empty;
            if (elementArticle != null)
            {
                content = await elementArticle.InnerHTMLAsync();
            }

            await subPage.CloseAsync();
            return content;
        }

        private static async Task<DateTime> CrawlDateTime(IElementHandle article)
        {
            DateTime dateTime = new DateTime();
            var selectorDateTime = "//div/div/div/div/div/div/div[2]/div[2]/span/a/p";
            var elementDateTime = await article.QuerySelectorAsync(selectorDateTime);
            if (elementDateTime != null)
            {
                var dateTimeText = await elementDateTime.InnerTextAsync();

                if (dateTimeText.Contains("hours ago") || dateTimeText.Contains("hour ago"))
                {
                    dateTimeText = dateTimeText.Replace("hours ago", "").Trim();
                    dateTime = DateTime.Now.AddHours(-dateTimeText.ToIntODefault());
                }
                else
                {
                    var cultureInfo = new CultureInfo("en-US");
                    DateTime.TryParse(dateTimeText, cultureInfo, DateTimeStyles.AssumeLocal, out dateTime);
                }
            }

            return dateTime;
        }

        private static async Task<string> CrawlUrl(IElementHandle article, IPage page)
        {
            string url = string.Empty;
            var selectorUrl = "//div/div/div/div/div/div/div[2]/div[2]/span/a";
            var elementUrl = await article.QuerySelectorAsync(selectorUrl);
            if (elementUrl != null)
            {
                url = await elementUrl.GetAttributeAsync("href");
                url = $"{page.Url.Trim('/')}/{url.Trim('/')}";
            }

            return url;
        }

        private static async Task<string> CrawlImage(IElementHandle article)
        {
            string base64String = string.Empty;
            var selectorArticleImage = "//div/div/div/div/div/div[2]/div/div[1]";
            var elementArticleImage = await article.QuerySelectorAsync(selectorArticleImage);
            if (elementArticleImage != null)
            {
                var screenshot = await elementArticleImage.ScreenshotAsync();
                base64String = Convert.ToBase64String(screenshot, 0, screenshot.Length);
            }

            return base64String;
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


    public class MediumResult
    {
        public string Url { get; set; }
        public string Content { get; set; }
        public string Image { get; set; }
        public DateTime? DateTime { get; set; }
    }
}