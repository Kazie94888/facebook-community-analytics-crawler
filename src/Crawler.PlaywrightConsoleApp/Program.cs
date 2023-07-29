using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ConsoleAppPostToFacebook.Services;
using Crawler.PlaywrightConsoleApp.Models;
using Crawler.PlaywrightConsoleApp.Models.Apis;
using Crawler.PlaywrightConsoleApp.Services;
using Crawler.PlaywrightConsoleApp.Services.Apis;
using Microsoft.Playwright;
using Newtonsoft.Json;

namespace Crawler.PlaywrightConsoleApp
{
    static class Program
    {
        private static readonly string RootUrl = ConfigurationManager.AppSettings["RootUrl"];
        private static readonly string RootPath = Directory.GetCurrentDirectory();

        static async Task Main(string[] args)
        {
            while (true)
            {
                var loadmoreId = "see_more_cards_id";
                var timeOut = 2000;
                var schedulePosts = FacebookApiService.GetSchedulePosts();
                //if (schedulePosts == null || !schedulePosts.Any()) return;
                var proxyAccount = FacebookApiService.GetAccountFacebookProxy();
                if (proxyAccount == null)
                {
                    proxyAccount = new AccountFacebookProxy();
                }
                //Fake account 
                proxyAccount.Proxy = new ProxyAccount
                {

                };
                proxyAccount.Account = new FacebookAccount
                {

                };

                var proxyModel = proxyAccount.Proxy;
                var proxy = new Proxy()
                {
                    Server = $"{proxyModel.Ip}:{proxyModel.Port}",
                    Username = proxyModel.Username,
                    Password = proxyModel.Password
                };
                using var playwright = await Playwright.CreateAsync();
                await using var browser = await playwright.Firefox.LaunchAsync(new BrowserTypeLaunchOptions()
                {
                    Proxy = proxy,
                    Timeout = 120000,
                    Headless = false
                });

                var browserContext = await browser.NewContextAsync();
                var page = await browserContext.NewPageAsync();

                var getCookies = GetCookies(proxyAccount.Account.Username);
                if (getCookies == null)
                {
                    await page.GotoAsync(RootUrl);
                    var loginAccount = proxyAccount.Account;
                    var loginEmailElement = await page.QuerySelectorAsync("input#m_login_email");
                    if (loginEmailElement != null) await loginEmailElement.TypeAsync(loginAccount.Username);

                    var loginPasswordElement = await page.QuerySelectorAsync("input#m_login_password");
                    if (loginPasswordElement != null) await loginPasswordElement.TypeAsync(loginAccount.Password);

                    await page.WaitForTimeoutAsync(timeOut);

                    var buttonLogin = await page.QuerySelectorAsync("button[name='login']");
                    if (buttonLogin != null) await buttonLogin.ClickAsync();

                    await page.WaitForSelectorAsync("#approvals_code");

                    var approvePasscodeElement = await page.QuerySelectorAsync("#approvals_code");

                    var code = GetTwoFaCodeService.Get(loginAccount.TwoFactorCode);

                    if (approvePasscodeElement != null) await approvePasscodeElement.TypeAsync(code);

                    await page.WaitForSelectorAsync("#checkpointSubmitButton-actual-button");

                    var buttonSendCode = await page.QuerySelectorAsync("#checkpointSubmitButton-actual-button");

                    if (buttonSendCode != null)
                    {
                        var clickOptions = new ElementHandleClickOptions()
                        {
                            Delay = 2000,
                            Timeout = 120000
                        };
                        await buttonSendCode.ClickAsync(clickOptions);
                        await page.WaitForLoadStateAsync(LoadState.Load, new PageWaitForLoadStateOptions
                        {
                            Timeout = 120000
                        });

                        while (true)
                        {
                            buttonSendCode = await page.QuerySelectorAsync("#checkpointSubmitButton-actual-button");
                            if (buttonSendCode == null) break;

                            await buttonSendCode.ClickAsync(clickOptions);

                            await page.WaitForLoadStateAsync(LoadState.Load, new PageWaitForLoadStateOptions
                            {
                                Timeout = 120000
                            });
                        }
                    }

                    var browserContextCookiesResults = await browserContext.CookiesAsync();
                    SaveCookies(proxyAccount.Account.Username, browserContextCookiesResults.Select(_=> new Cookie()
                    {
                        Domain = _.Domain,
                        Expires = _.Expires,
                        Name = _.Name,
                        Path = _.Path,
                        Secure = _.Secure,
                        Value = _.Value,
                        SameSite = _.SameSite,
                        HttpOnly = _.HttpOnly
                    }).ToList());
                }
                else
                {
                    await browserContext.AddCookiesAsync(getCookies);
                }
                //Get all link fanpage
                var postService = new FacebookPostService();
                var links = await postService.GetAllPostsFanpageVerMobile(page,"https://m.facebook.com/groups/ghiendalat");
                var listPosts = new List<SourcePostModel>();
                foreach (var link in links)
                {
                    var postModel = await postService.ExtractData(page: page, link);
                    if (postModel != null)
                    {
                        listPosts.Add(postModel);
                    }
                }
                //var linksGroups = await postService.GetAllPostsFanpageVerFull(page,"https://www.facebook.com/yannews");
                
                foreach (var schedulePost in schedulePosts)
                {
                    foreach (var groupId in schedulePost.GroupIds.Split(","))
                    {
                        //await page.GotoAsync($"https://m.facebook.com/groups/{groupId}");
                        await page.GotoAsync("https://m.facebook.com/groups/641546036270426"); // Group test

                        var model = new PostModel
                        {
                            PostId = schedulePost.Id.ToString(),
                            Content = schedulePost.Content,
                            Images = schedulePost.LocalFilesDownloaded
                        };

                        //var postService = new FacebookPostService();

                        await postService.JoinGroupAsync(page);
                        await postService.PostToFacebook(page, model);

                        await page.WaitForTimeoutAsync(timeOut);
                    }
                }

                var key = Console.ReadKey();
                if (key.Key == ConsoleKey.E)
                {
                    break;
                }

                await page.ScreenshotAsync(new PageScreenshotOptions {Path = "screenshot.png"});
            }
        }

        private static void SaveCookies(string fileName, IReadOnlyCollection<Cookie> allCookies)
        {
            var directory = $"{RootPath}\\Cookies";
            if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);
            File.WriteAllText($"{RootPath}\\Cookies\\{fileName}-fb-cookies.json", JsonConvert.SerializeObject(allCookies));
        }


        private static List<Cookie> GetCookies(string accountName)
        {
            try
            {
                var content = File.ReadAllText($"{RootPath}\\Cookies\\{accountName}-fb-cookies.json");
                return JsonConvert.DeserializeObject<List<Cookie>>(content);
            }
            catch (Exception e)
            {
                return null;
            }
        }
    }
}