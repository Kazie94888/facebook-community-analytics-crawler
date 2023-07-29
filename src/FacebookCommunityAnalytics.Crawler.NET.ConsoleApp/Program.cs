using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using FacebookCommunityAnalytics.Crawler.NET.Core.Enums;
using FacebookCommunityAnalytics.Crawler.NET.Core.Extensions;
using FacebookCommunityAnalytics.Crawler.NET.Service.Models;
using FacebookCommunityAnalytics.Crawler.NET.Service.Services;
using Newtonsoft.Json;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace FacebookCommunityAnalytics.Crawler.NET.ConsoleApp
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            var postType = LinkType.Page;

            var proxyModel = new ProxyModel
            {
                IP = "103.90.230.75",
                Port = 9039,
                UserName = "vunguyentran",
                Password = "a00760355003edb82"
            };
            ChromeDriver driver = null;
            WebDriverService webDriverService = new WebDriverService();
            LoginFacebookCrawlerService loginService = new LoginFacebookCrawlerService();
            try
            {
                driver = webDriverService.InitChromeDriver(proxyModel);
                driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);
            
                if (postType == LinkType.Page)
                {
                    loginService.Login(driver, proxyModel, new LoginInput
                    {
                        UserName = "100011416433647",
                        Password = "123123qq",
                        TwoFACode = "BVQE JSVE IQUE F5YH IM2L S25A HGYW 3JMI"
                    });
                
                    var documentPost = webDriverService.LoadNewTab(driver, "https://m.facebook.com/story.php?story_fbid=6942415849162578&id=154180304652867");
                    var crawlResultItemDto = HtmlExtractDataService.ExtractFromPost(documentPost);
                    var documentReaction = webDriverService.LoadNewTab(driver, "https://m.facebook.com/ufi/reaction/profile/browser/?ft_ent_identifier=6942415849162578");

                    crawlResultItemDto.LikeCount = HtmlExtractDataService.GetReactionCount(documentReaction);
                    Console.WriteLine(" ***************** Crawler DATA: ********************");
                    Console.Write(JsonConvert.SerializeObject(crawlResultItemDto));
                    Console.WriteLine(" *********************************************");
                    
                    webDriverService.CloseTabs(driver);
                }
            
                if (postType == LinkType.Group)
                {
                    loginService.Login(driver, new ProxyModel
                    {
                        IP = "103.90.230.75",
                        Port = 9039,
                        UserName = "vunguyentran",
                        Password = "a00760355003edb82"
                    }, new LoginInput
                    {
                        UserName = "100011416433647",
                        Password = "123123qq",
                        TwoFACode = "BVQE JSVE IQUE F5YH IM2L S25A HGYW 3JMI"
                    });

                    //var cookies = driver.Manage().Cookies;

                    var document = webDriverService.LoadNewTab(driver, "https://m.facebook.com/story.php?story_fbid=6942038365866993&id=154180304652867");
                }

                if (postType == LinkType.PostToFacebook)
                {
                    var proxy = new ProxyModel
                    {
                        IP = "103.90.230.75",
                        Port = 9039,
                        UserName = "vunguyentran",
                        Password = "a00760355003edb82"
                    };
                    var getCookies = GetCookies();
                    if (getCookies == null)
                    {
                        loginService.Login(driver, proxy, new LoginInput
                        {
                            UserName = "100011416433647",
                            Password = "123123qq",
                            TwoFACode = "BVQE JSVE IQUE F5YH IM2L S25A HGYW 3JMI"
                        });

                        var cookies = driver.Manage().Cookies.AllCookies;
                        SaveCookies(cookies.ToList());
                    }
                    else
                    {
                        var _rootUrl = ConfigurationManager.AppSettings["RootUrl"];
                        driver = webDriverService.InitChromeDriver(proxy);

                        driver.Navigate().GoToUrl(_rootUrl);

                        foreach (var cookie in getCookies) driver.Manage().Cookies.AddCookie(cookie);
                    }


                    //Load group
                    driver.Navigate().GoToUrl("https://m.facebook.com/groups/641546036270426");

                    var model = new PostModel
                    {
                        PostId = Guid.NewGuid().ToString(),
                        Content = @"Nơi anh đến là biển xa 😷😷😷
                                nơi anh tới ngoài đảo xa 🙈🙈🙈",
                        Images = new List<string>
                        {
                            "D:\\test\\1.jpg",
                            "D:\\test\\2.jpg"
                        }
                    };
                    FacebookProcessService.PostToFacebook(driver, model);
                }

                Console.WriteLine("Nhan E de thoat chuong trinh");
                var readkey = Console.ReadKey();
                if (readkey.Key == ConsoleKey.E)
                {
                    driver.Quit();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(" ***************** Error: ********************");
                Console.Write(e.Message);
                Console.WriteLine(" *********************************************");
                if (driver != null)
                {
                    driver.Quit();
                }
            }

            var x = 0;
        }

        private static void SaveCookies(List<Cookie> allCookies)
        {
            File.WriteAllText("D:\\fb-cookies.json", JsonConvert.SerializeObject(allCookies));
        }

        private static List<Cookie> GetCookies()
        {
            try
            {
                var content = File.ReadAllText("D:\\fb-cookies.json");
                var customCookies = JsonConvert.DeserializeObject<List<CustomCookie>>(content);
                return customCookies.Select(_ => new Cookie(_.Name, _.Value, _.Path, NumericExtensions.UnixTimeStampToDateTime(_.Expiry))).ToList();
            }
            catch (Exception e)
            {
                return null;
            }
        }
    }
}