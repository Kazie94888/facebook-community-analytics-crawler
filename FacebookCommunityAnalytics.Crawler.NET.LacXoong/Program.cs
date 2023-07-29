using FacebookCommunityAnalytics.Crawler.NET.Client.Configurations;
using FacebookCommunityAnalytics.Crawler.NET.Client.Entities;
using FacebookCommunityAnalytics.Crawler.NET.Client.Enums;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FacebookCommunityAnalytics.Crawler.NET.Console.Services.CrawlerServices;
using FacebookCommunityAnalytics.Crawler.NET.Console.Services.HotMailServices;
using FacebookCommunityAnalytics.Crawler.NET.Core.Extensions;
using Microsoft.Extensions.Configuration;

namespace FacebookCommunityAnalytics.Crawler.NET.LacXoong
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
            
            // var command = CrawlType.GroupSelectivePost;
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
                
                case CrawlType.LoginAuto:
                {
                    var accountType = GetAccountType(args[1]);

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
                
                case CrawlType.UnlockEmails:
                {
                    var unlockEmailService = new EmailService(config);
                    await unlockEmailService.UnlockEmails();
                    break;
                }
            }
        }
    }
}