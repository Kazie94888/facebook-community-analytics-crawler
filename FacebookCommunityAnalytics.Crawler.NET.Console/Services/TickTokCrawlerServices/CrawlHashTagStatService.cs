using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FacebookCommunityAnalytics.Crawler.NET.Client.Clients;
using FacebookCommunityAnalytics.Crawler.NET.Client.Configurations;
using FacebookCommunityAnalytics.Crawler.NET.Client.Entities;
using FacebookCommunityAnalytics.Crawler.NET.Client.Enums;
using FacebookCommunityAnalytics.Crawler.NET.Console.Playwrights;
using FacebookCommunityAnalytics.Crawler.NET.Core.Extensions;
using FacebookCommunityAnalytics.Crawler.NET.Core.Helpers;
using Microsoft.Playwright;

namespace FacebookCommunityAnalytics.Crawler.NET.Console.Services.TickTokCrawlerServices
{
    public class CrawlHashTagStatService
    {
        private readonly GlobalConfig _globalConfig;
        private readonly ApiClient _apiClient;
        
        public CrawlHashTagStatService(GlobalConfig globalConfig)
        {
            _globalConfig = globalConfig;
            _apiClient = new ApiClient(globalConfig.ApiConfig);
        }

        public async Task Execute()
        {
            var tiktokHashTags = _apiClient.TikTok.GetTiktokHashTags();
            var accountProxyItems = GetAccountProxyItems();
            AccountProxyItem accountProxyItem = null;
            // if (accountProxyItems.Any())
            // {
            //     accountProxyItem = accountProxyItems.First();
            // }
            
            var hashTags = tiktokHashTags.Resource.HashTags;
            foreach (var hashTag in hashTags)
            {
                var views = await Crawl(hashTag, accountProxyItem);
                var response = _apiClient.TikTok.SaveTiktokStat(new SaveTiktokStatApiRequest
                {
                    Count = views,
                    Hashtag = hashTag
                });
            }
        }

        private async Task<long> Crawl(string hashTag, AccountProxyItem accountProxyItem)
        {
            var browserContext = await PlaywrightHelper.InitPersistentBrowser(_globalConfig.CrawlConfig, accountProxyItem);
            using (browserContext.Playwright)
            {
                await using (browserContext.Browser)
                {
                    var page = await browserContext.BrowserContext.NewPageAsync();
                    try
                    {
                        var url = $"https://www.tiktok.com/tag/{hashTag}";
                        await page.GotoAsync(url);
                        await page.WaitForLoadStateAsync(LoadState.Load);
                        await page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);

                        var selector_Views = "//h2[@data-e2e='challenge-vvcount']/strong";
                        var ele_Views = await page.QuerySelectorAsync(selector_Views);
                        if (ele_Views == null)
                        {
                            selector_Views = "//h2[@class='share-sub-title-thin']/strong";
                            ele_Views = await page.QuerySelectorAsync(selector_Views);
                        }

                        long totalView = 0;
                        if (ele_Views != null)
                        {
                            var text = await ele_Views.InnerTextAsync();
                            text = text.Replace("views", "").Trim();
                            if (text.ToUpper().Contains("K"))
                            {
                                totalView = (long)(text.Trim('K').ToDecimalOrDefault() * 1000);
                            }

                            if (text.ToUpper().Contains("M"))
                            {

                                totalView = (long)(text.Trim('M').ToDecimalOrDefault() * 1000000);
                            }
                        
                            if (text.ToUpper().Contains("B"))
                            {

                                totalView = (long)(text.Trim('B').ToDecimalOrDefault() * 1000000000);
                            }
                        }
                        
                        System.Console.WriteLine($"{hashTag}: total views {totalView}");

                        return totalView;
                    }
                    catch (Exception e)
                    {
                        System.Console.WriteLine(e);
                        await e.Log(string.Empty, string.Empty);
                        throw;
                    }
                    finally
                    {
                        await page.CloseAsync();
                        await browserContext.BrowserContext.CloseAsync();
                    }
                }
            }
        }
        
        private List<AccountProxyItem> GetAccountProxyItems()
        {
            var res = _apiClient.Crawl.GetAccountProxies(new GetAccountProxiesRequest {AccountType = AccountType.NETFacebookGroupUserPost});
            if (!res.IsSuccess || res.Resource.IsNullOrEmpty()) return null;
            var accountProxyItems = res.Resource.Where(_ => _.account.accountStatus == AccountStatus.Active).ToList();
            accountProxyItems = accountProxyItems.OrderBy(item => item.accountProxy.CrawledAt).ToList();
            return accountProxyItems;
        }
    }
}