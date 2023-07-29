using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ConsoleAppPostToFacebook.Models;
using ConsoleAppPostToFacebook.Models.Apis;
using ConsoleAppPostToFacebook.Services;
using ConsoleAppPostToFacebook.Services.Apis;
using FacebookCommunityAnalytics.Crawler.NET.Service.Models;
using Newtonsoft.Json;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace ConsoleAppPostToFacebook
{
    internal static class Program
    {
        private static readonly string _rootPath = "D:\\temp";

        private static async Task Main(string[] args)
        {
            ChromeDriver driver = null;
            try
            {
                var sleepTime = 2000;
                var rootUrl = ConfigurationManager.AppSettings["RootUrl"];
                Console.WriteLine("Hello World!");
                var schedulePosts = FacebookApiService.GetSchedulePosts();
                if (schedulePosts == null || !schedulePosts.Any()) return;
                var proxyAccount = FacebookApiService.GetAccountFacebookProxy();
                if (proxyAccount == null) return;

                //Fake account 
                proxyAccount.Proxy = new ProxyAccount
                {
                    Ip = "103.90.230.75",
                    Port = 9039,
                    Username = "vunguyentran",
                    Password = "a00760355003edb82"
                };
                proxyAccount.Account = new FacebookAccount
                {
                    Username = "100011416433647",
                    Password = "123123qq",
                    TwoFactorCode = "BVQE JSVE IQUE F5YH IM2L S25A HGYW 3JMI"
                };

                driver = (ChromeDriver) WebDriverService.InitDriver(proxyAccount.Proxy);

                driver.Navigate().GoToUrl(rootUrl);

                var autoIt = new AutoItX3.Interop.AutoItX3();

                autoIt.WinWait(rootUrl, "", 10);
                if (autoIt.WinExists(rootUrl) > 0)
                {
                    autoIt.WinActivate(rootUrl);
                    autoIt.Send(proxyAccount.Proxy.Username + "{TAB}");
                    autoIt.Send(proxyAccount.Proxy.Password + "{ENTER}");
                }

                Thread.Sleep(sleepTime);

                var getCookies = GetCookies(proxyAccount.Account.Username);
                if (getCookies == null)
                    driver = (ChromeDriver) LoginFacebookCrawlerService.Login(driver, proxyAccount.Account);
                //var cookies = driver.Manage().Cookies.AllCookies;
                //SaveCookies(proxyAccount.Account.Username,cookies.ToList());
                else
                    foreach (var cookie in getCookies)
                        driver.Manage().Cookies.AddCookie(cookie);

                foreach (var post in schedulePosts)
                {
                    var groupIds = post.GroupIds.Split(',').ToList();
                    foreach (var groupId in groupIds)
                    {
                        //Load group
                        driver.Navigate().GoToUrl($"https://m.facebook.com/groups/{groupId}");
                        Thread.Sleep(sleepTime);
                        var model = new PostModel
                        {
                            PostId = string.Empty,
                            Content = post.Content,
                            Images = post.LocalFilesDownloaded
                        };
                        await FacebookPostService.PostToFacebook(driver, model);

                        Thread.Sleep(sleepTime);
                    }
                }

                Console.WriteLine("Nhan phat coi nao");
                Console.ReadKey();
                driver.Close();
                driver.Dispose();
            }
            catch (Exception e)
            {
                if (driver != null)
                {
                    driver.Close();
                    driver.Dispose();
                }

                Console.WriteLine(e);
                throw;
            }
        }

        private static void SaveCookies(string fileName, IReadOnlyCollection<Cookie> allCookies)
        {
            var directory = $"{_rootPath}\\Cookies";
            if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);
            File.WriteAllText($"{_rootPath}\\Cookies\\{fileName}-fb-cookies.json", JsonConvert.SerializeObject(allCookies));
        }

        private static List<Cookie> GetCookies(string fileName)
        {
            try
            {
                var content = File.ReadAllText($"{_rootPath}\\Cookies\\{fileName}-fb-cookies.json");
                var customCookies = JsonConvert.DeserializeObject<List<CustomCookie>>(content);

                //return customCookies?.Select(_ => new Cookie(_.Name, _.Value, _.Path, NumericExtensions.UnixTimeStampToDateTime(_.Expiry))).ToList();
                return customCookies?.Select(_ => new Cookie(_.Name, _.Value, _.Path, _.Expiry)).ToList();
            }
            catch (Exception e)
            {
                return null;
            }
        }
    }
}