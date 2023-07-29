// using System;
// using System.Threading.Tasks;
// using FacebookCommunityAnalytics.Crawler.NET.Client.Configurations;
// using FacebookCommunityAnalytics.Crawler.NET.Client.Entities;
// using Microsoft.Playwright;
//
// namespace FacebookCommunityAnalytics.Crawler.NET.Console.Services.FacebookCrawlerServices
// {
//     public class JoinGroupService : CrawlPagePostBaseService
//     {
//         public JoinGroupService(CrawlConfig config) : base(config)
//         {
//         }
//         
//         public async Task<CrawlResult> Join(IPage page, CrawlModelBase crawlItem)
//         {
//             try
//             {
//                 await page.GotoAsync(crawlItem.Url);
//                 await page.WaitForLoadStateAsync(LoadState.NetworkIdle);
//
//                 if (await page.IsPageNotFound())
//                 {
//                     CrawlResult.Success = false;
//                     return CrawlResult;
//                 }
//                 
//                 if (!await page.IsJoinedGroup())
//                 {
//                     await page.JoinGroup();
//                 }
//                 
//                 CrawlResult.Success = true;
//                 return CrawlResult;
//             }
//             catch (Exception e)
//             {
//                 Debug.WriteLine(e);
//                 CrawlResult.Success = false;
//                 return CrawlResult;
//             }
//         }
//     }
// }