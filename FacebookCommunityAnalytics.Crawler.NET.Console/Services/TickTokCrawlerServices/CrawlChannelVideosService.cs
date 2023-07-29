using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using ClosedXML.Excel;
using Dasync.Collections;
using FacebookCommunityAnalytics.Crawler.NET.Client.Configurations;
using FacebookCommunityAnalytics.Crawler.NET.Client.Entities;
using FacebookCommunityAnalytics.Crawler.NET.Client.Enums;
using FacebookCommunityAnalytics.Crawler.NET.Console.Playwrights;
using FacebookCommunityAnalytics.Crawler.NET.Core.Extensions;
using FacebookCommunityAnalytics.Crawler.NET.Core.Helpers;
using Group = FacebookCommunityAnalytics.Crawler.NET.Client.Entities.Group;

namespace FacebookCommunityAnalytics.Crawler.NET.Console.Services.TickTokCrawlerServices
{
    public class CrawlChannelVideosService : BaseChannelVideoService
    {
        public CrawlChannelVideosService(GlobalConfig globalConfig) : base(globalConfig)
        {
        }

        public async Task Execute(CrawlStopCondition crawlStopCondition = CrawlStopCondition.Weekly)
        {
            var uncrawledChannel = ApiClient.Crawl.GetUncrawledGroups(new GetUncrawledGroupApiRequest
            {
                GroupSourceType = GroupSourceType.Tiktok,
                IgnoreTime = true
            });
            
            var crawlItems = uncrawledChannel.Resource.Groups;
            
            System.Console.WriteLine($"{GetType().Name}: found {crawlItems.Count} channels");
            await Task.Delay(3000);
            if (crawlItems.IsNullOrEmpty())
            {
                System.Console.WriteLine($"{GetType()}: exit in 5 seconds, no channels found");
                await Task.Delay(5000);
            }
            
            var accountProxyQueue = GetAccountProxyQueue(AccountType.NETFacebookGroupSelectivePost);
            if (accountProxyQueue == null) return;
            
            await crawlItems.ParallelForEachAsync(async group =>
            {
                var accountProxyItem = GetAccountProxyItem(accountProxyQueue);
                var tiktokVideos     = await Crawl(group, crawlStopCondition, accountProxyItem);
                if (tiktokVideos.Any())
                {
                    var response = ApiClient.TikTok.PostAutoCrawlChannelVideosResult
                    (
                        new SaveChannelVideoRequest
                        {
                            Videos = tiktokVideos,
                            ChannelId = group.fid
                        }
                    );
                }
            }, GlobalConfig.CrawlConfig.Crawl_MaxThread_TiktokVideo);

            if (crawlStopCondition == CrawlStopCondition.Weekly)
            {
                var startDate = DateTime.Now.AddDays(-1).Date;
                var endDate = startDate.Date.Add(new TimeSpan(23, 59, 59));
                // create excel file
                var tiktokExportRows = GetTiktokExportRows(startDate, endDate);
                string vnTimeZoneKey = "SE Asia Standard Time";
                TimeZoneInfo vnTimeZone = TimeZoneInfo.FindSystemTimeZoneById(vnTimeZoneKey);
                foreach (var tiktokExportRow in tiktokExportRows)
                {
                    tiktokExportRow.CreatedDateTime = TimeZoneInfo.ConvertTimeFromUtc(tiktokExportRow.CreatedDateTime, vnTimeZone);
                }
                
                if (tiktokExportRows.Any())
                {
                    // save to excel file
                    var subject = SaveExcelResult(tiktokExportRows);
                    // send excel file to dev email
                    var filePath = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly()?.Location),
                        $"{subject}.xlsx");

                    await EmailService.SendUsingGmail($"[Tiktok] Daily Report {DateTime.Now:yyyyMMdd}", subject, filePath, GlobalConfig.TiktokConfig.DailyReportEmails);
                    
                    // update video state
                    ApiClient.TikTok.UpdateTitokVideosState(new UpdateTiktokVideosStateRequest
                    {
                        IsNew = false,
                        VideoIds = tiktokExportRows.Select(row => row.Fid).ToList()
                    });
                }
            }
        }

        private static string SaveExcelResult(List<TiktokExportRow> tiktokExportRows)
        {
            var dt = new DataTable();
            dt.Columns.AddRange(new DataColumn[6]
            {
                new("Kênh", typeof(string)),
                new("Ngành Hàng", typeof(string)),
                new("Link", typeof(string)),
                new("UID", typeof(string)),
                new("Video Id", typeof(string)),
                new("Ngày Đăng", typeof(string)),
            });

            foreach (var tiktokExportRow in tiktokExportRows)
            {
                dt.Rows.Add(tiktokExportRow.Channel, tiktokExportRow.Category, tiktokExportRow.Url, tiktokExportRow.UID, tiktokExportRow.Fid,
                    tiktokExportRow.CreatedDateTime.ToString("MM/dd/yyyy HH:mm"));
            }

            var subject = $"Tiktok_DailyReport_{DateTime.Now:yyyyMMdd}";
            using var workbook = new XLWorkbook();
            var worksheets = workbook.Worksheets.Add(dt, "Result");
            worksheets.Columns().AdjustToContents();
            workbook.SaveAs($"{subject}.xlsx");
            
            return subject;
        }

        private List<TiktokExportRow> GetTiktokExportRows(DateTime startDate, DateTime endDate)
        {
            var apiResponse = ApiClient.TikTok.GetTiktokExportRow(new GetTiktoksInputExtend
            {
                MaxResultCount = int.MaxValue,
                CreatedDateTimeMin = startDate,
                CreatedDateTimeMax = endDate,
                Sorting = "Tiktok.ChannelId",
                ClientOffsetInMinutes = 420,
                TikTokMcnType = TikTokMCNType.MCNGdl
            });

            var tiktokExportRows = apiResponse.Resource;
            return tiktokExportRows;
        }

        
        private async Task<List<TiktokVideoDto>> Crawl(Group channel, CrawlStopCondition crawlStopCondition, AccountProxyItem accountProxy)
        {
            var browserContext = await PlaywrightHelper.InitPersistentBrowser(GlobalConfig.CrawlConfig, null, true, channel.name);
            using (browserContext.Playwright)
            {
                await using (browserContext.Browser)
                {
                    try
                    {
                        return await CrawlVideoByChannel(browserContext, channel.fid, crawlStopCondition);
                    }
                    catch (Exception e)
                    {
                        await e.Log(string.Empty, string.Empty);
                        throw;
                    }
                    finally
                    {
                        await browserContext.BrowserContext.CloseAsync();
                    }
                }
            }
        }

        private AccountProxyItem GetAccountProxyItem(List<AccountProxyItem> accountProxyQueue)
        {
            var random           = new Random();
            int index            = random.Next(accountProxyQueue.Count);
            var accountProxyItem = accountProxyQueue[index];

            return accountProxyItem;
        }
    }
}