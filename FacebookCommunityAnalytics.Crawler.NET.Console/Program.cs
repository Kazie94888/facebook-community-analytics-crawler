using FacebookCommunityAnalytics.Crawler.NET.Client.Configurations;
using FacebookCommunityAnalytics.Crawler.NET.Client.Entities;
using FacebookCommunityAnalytics.Crawler.NET.Client.Enums;
using FacebookCommunityAnalytics.Crawler.NET.Console.Services.CrawlerServices;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FacebookCommunityAnalytics.Crawler.NET.Console.Services.FacebookCrawlerServices;
using FacebookCommunityAnalytics.Crawler.NET.Console.Services.HotMailServices;
using FacebookCommunityAnalytics.Crawler.NET.Console.Services.TickTokCrawlerServices;
using FacebookCommunityAnalytics.Crawler.NET.Core.Extensions;

namespace FacebookCommunityAnalytics.Crawler.NET.Console
{
    static class Program
    {
        private static GlobalConfig InitConfig()
        {
            var configRoot = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("Configurations/globalconfigs.json")
                .Build();
            var section = configRoot.GetSection(nameof(GlobalConfig));
            var config = section.Get<GlobalConfig>();

            return config;
        }

        private static CrawlType GetCrawlType(string arg)
        {
            try
            {
                var type = GenericExtensions.ToEnum<CrawlType>(arg);
                System.Console.WriteLine($"CURRENT CRAWL TYPE: {type}");
                return type;
            }
            catch (Exception)
            {
                System.Console.WriteLine($"CURRENT CRAWL TYPE: {CrawlType.None}");
                return CrawlType.None;
            }
        }

        private static AccountType GetAccountType(string arg)
        {
            try
            {
                var accountType = GenericExtensions.ToEnum<AccountType>(arg);
                System.Console.WriteLine($"CURRENT ACCOUNT TYPE: {accountType}");
                return accountType;
            }
            catch (Exception)
            {
                System.Console.WriteLine($"CURRENT ACCOUNT TYPE: N/A");
                return AccountType.Unknown;
            }
        }

        static async Task Main(string[] args)
        {
            if (args.IsNullOrEmpty())
            {
                System.Console.WriteLine($"EMPTY ARGUMENTS...Existing");
                await Task.Delay(3000);
                return;
            }

            // var command = CrawlType.GroupPost;
            var command = GetCrawlType(args[0]);
            
            await Task.Delay(3000);

            var config = InitConfig();
            if (config == null)
            {
                System.Console.WriteLine($"EMPTY CONFIG... Exiting");
                await Task.Delay(3000);
                return;
            }

            switch (command)
            {
                case CrawlType.None:
                {
                    System.Console.WriteLine($"CURRENT COMMAND: {command}... Existing");
                    await Task.Delay(3000);
                    break;
                }
                case CrawlType.GroupSelectivePost:
                {
                    var crawlPostService = new CrawlSelectivePostService(config);
                    await crawlPostService.Execute();
                    break;
                }
                case CrawlType.GroupUser:
                {
                    var crawlGroupUserPostService = new CrawlGroupUserPostService(config);
                    await crawlGroupUserPostService.Execute();
                    break;
                }

                case CrawlType.GroupPost:
                {
                    var crawlGroupPostService = new CrawlGroupPostService(config);
                    await crawlGroupPostService.Execute();
                    
                    break;
                }
                
                case CrawlType.AllGroupPosts:
                {
                    var crawlGroupPostService = new CrawlGroupPostService(config, true, CrawlStopCondition.TwoDay);
                    await crawlGroupPostService.Execute();
                    
                    break;
                }

                case CrawlType.PagePost:
                {
                    var crawlPagePostService = new CrawlPagePostService(config);
                    await crawlPagePostService.Execute();
                    break;
                }

                case CrawlType.AllPagePosts:
                {
                    var crawlPagePostService = new CrawlPagePostService(config, true, CrawlStopCondition.TwoDay);
                    await crawlPagePostService.Execute();
                    break;
                }

                case CrawlType.ByHashTag:
                {
                    var crawlByHashTagService = new CrawlByHashTagService(config);
                    await crawlByHashTagService.Execute();
                    break;                
                }

                case CrawlType.AllHashTags:
                {
                    var crawlGroupPostService = new CrawlGroupPostService(config, true, CrawlStopCondition.TwoWeek);
                    await crawlGroupPostService.Execute();
                    
                    break;
                }

                case CrawlType.JoinGroupManual:
                {
                    var crawlByHashTagService = new JoinGroupService(config);
                    await crawlByHashTagService.Execute();
                    break;
                }

                case CrawlType.LoginAuto:
                {
                    var accountType = GetAccountType(args[1]);
                    if (accountType== AccountType.Unknown) break;

                    System.Console.WriteLine($"CURRENT COMMAND: {command}... WITH ACCOUNT TYPE {accountType}");
                    await Task.Delay(3000);
                    
                    var loginService = new LoginService(config)
                    {
                        AccountTypes = new List<AccountType>
                        {
                            accountType
                        }
                    };

                    await loginService.Execute();
                    break;
                }

                case CrawlType.LoginManual:
                {
                    var accountType = GetAccountType(args[1]);
                    if (accountType== AccountType.Unknown) break;

                    System.Console.WriteLine($"CURRENT COMMAND: {command}... WITH ACCOUNT TYPE {accountType}");
                    await Task.Delay(3000);
                    
                    var loginService = new LoginService(config)
                    {
                        AccountTypes = new List<AccountType>
                        {
                            accountType
                        }
                    };
                    await loginService.ManualLogin();
                    break;
                }

                case CrawlType.TiktokVideos:
                {
                    var crawlGroupService = new CrawlChannelVideosService(config);
                    await crawlGroupService.Execute();
                    break;
                }

                case CrawlType.TiktokVideosMonthly:
                {
                    var crawlGroupService = new CrawlChannelVideosService(config);
                    await crawlGroupService.Execute(CrawlStopCondition.Monthly);
                    break;
                }

                case CrawlType.TiktokChannelStats:
                {
                    var channelFollowersService = new CrawlChannelStatsService(config);
                    await channelFollowersService.Execute();
                    break;
                }

                case CrawlType.TiktokStats:
                {
                    var crawlHashTagStatService = new CrawlHashTagStatService(config);
                    await crawlHashTagStatService.Execute();
                    break;
                }

                case CrawlType.TiktokMCNvideos:
                {
                    var crawlMcnVideoService = new CrawlMCNVideoService(config);
                    await crawlMcnVideoService.Execute();
                    break;
                }

                case CrawlType.TiktokMCNVietNamChanel:
                {
                    var crawlTopMcnVietNamService = new CrawlTopMCNVietNamService(config);
                    await crawlTopMcnVietNamService.Execute();
                    break;
                }

                case CrawlType.UnlockEmails:
                {
                    var unlockEmailService = new EmailService(config);
                    await unlockEmailService.UnlockEmails();
                    break;
                }

                case CrawlType.DeleteComment:
                {
                    var deleteCommentService = new DeleteCommentService(config);
                    await deleteCommentService.DeleteComment();
                    break;
                }

                case CrawlType.AutoLike:
                {
                    var autoLikeService = new AutoLikeService(config);
                    await autoLikeService.Execute();
                    break;
                }

                case CrawlType.AutoPost:
                {
                    var autoPost = new AutoPostService(config);
                    await autoPost.Execute();
                    break;
                }

                case CrawlType.Twitter:
                {
                    var twitter = new CrawlTwitterService(config);
                    await twitter.Crawl();
                    break;  
                }
                
                case CrawlType.Medium:
                {
                    var mediumService = new CrawlMediumService(config);
                    await mediumService.Crawl();
                    break;  
                }

                case CrawlType.Telegram:
                {
                    var telegramService = new CrawlTelegramService(config);
                    await telegramService.Execute();
                    break;
                }

                case CrawlType.TiktokTrending:
                {
                    var crawlHotTrendingService = new CrawlHotTrendingService(config);
                    crawlHotTrendingService.Execute();
                    break;
                }
            }
        }

        // private static Stack<AccountProxyItem> GetAccountProxyStack()
        // {
        //     var accountProxyStack = new Stack<AccountProxyItem>();
        //
        //     accountProxyStack.Push(new AccountProxyItem
        //     {
        //         account = new Account
        //         {
        //             username = "100069675897101",
        //             password = "thang114",
        //             twoFactorCode = "DR2ESTBIBYIIIZG5LW3KLPQYIEO5OUMB",
        //         }
        //     });
        //
        //     accountProxyStack.Push(new AccountProxyItem
        //     {
        //         account = new Account
        //         {
        //             username = "100069683036797",
        //             password = "thang114",
        //             twoFactorCode = "4GH7M4KMMDDTOJDEUCR7PBDOONIGUP7H",
        //         }
        //     });
        //
        //     accountProxyStack.Push(new AccountProxyItem
        //     {
        //         account = new Account
        //         {
        //             username = "100069899687798",
        //             password = "thang114",
        //             twoFactorCode = "SY4KE2KSVUG5IF4HA4G6WDEGZSO6EBA3",
        //         }
        //     });
        //
        //     // accountProxyStack.Push(new AccountProxyItem
        //     // {
        //     //     account = new Account
        //     //     {
        //     //         username = "100070099360511",
        //     //         password = "thang114",
        //     //         twoFactorCode = "XUFLRBFS73KXAIF6RUBBCHQ5YI52DZH7",
        //     //     }
        //     // });
        //
        //     accountProxyStack.Push(new AccountProxyItem
        //     {
        //         account = new Account
        //         {
        //             username = "100069922247823",
        //             password = "123123qq",
        //             twoFactorCode = "4YJU7SXHK6ELVNDI25L2EIBTEZEN5L3G",
        //         }
        //     });
        //
        //     // banned
        //     // accountProxyStack.Push(new AccountProxyItem
        //     // {
        //     //     account = new Account
        //     //     {
        //     //         username = "100011416433647",
        //     //         password = "123123qq",
        //     //         twoFactorCode = "BVQE JSVE IQUE F5YH IM2L S25A HGYW 3JMI",
        //     //     }
        //     // });
        //
        //     return accountProxyStack;
        // }
    }
}