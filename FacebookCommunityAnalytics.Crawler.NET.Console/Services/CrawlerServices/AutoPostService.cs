using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FacebookCommunityAnalytics.Crawler.NET.Client.Configurations;
using FacebookCommunityAnalytics.Crawler.NET.Client.Entities;
using FacebookCommunityAnalytics.Crawler.NET.Client.Enums;
using FacebookCommunityAnalytics.Crawler.NET.Console.Playwrights;
using FacebookCommunityAnalytics.Crawler.NET.Core.Extensions;
using FacebookCommunityAnalytics.Crawler.NET.Core.Helpers;
using Microsoft.Playwright;

namespace FacebookCommunityAnalytics.Crawler.NET.Console.Services.CrawlerServices
{
    public class AutoPostService : CrawlServiceBase
    {
        private string _content = @"CÔNG BỐ GIẢI THƯỞNG CHUNG CUỘC CONTEST “VIỆT NAM ĐÓN CHÀO” VỚI NHỮNG BÀI DỰ THI CỰC ẤN TƯỢNG!!
Contest #VietNamDonChao trong 2 tuần qua đã nhận được rất nhiều hình ảnh, những khoảnh khắc đẹp và review được lan rộng nhằm sẻ chia tới bạn bè quốc trong dịp mở cửa du lịch 15/03 vừa qua.
BTC xin công bố kết quả chung cuộc và xin chúc mừng những bài dự thi xuất sắc sau:
👉🏻1 GIẢI NHẤT: Gimbal DJI Osmo 5 trị giá 5.000.000
📌Đỗ Minh Tân với bài dự thi: https://www.facebook.com/groups/729236007949900
👉🏻 1 GIẢI NHÌ: Voucher du lịch Đà Lạt trị giá 2.500.000 (1 voucher khách sạn 1.500.000 + 1 
voucher tour HPD 1.000.000).
📌Hồng Nhung với bài dự thi: https://www.facebook.com/groups/729236007949900
👉🏻 2 GIẢI BA: Mỗi giải 1 voucher du lịch trị giá 1.300.000 (2 voucher khách sạn 800.000 + 1 voucher tour HPD 500.000).
📌Nguyễn Thị Lan Chi với bài dự thi: https://www.facebook.com/groups/729236007949900
📌Son Nguyen với bài dự thi: https://www.facebook.com/groups/729236007949900
👉🏻10 GIẢI KHUYẾN KHÍCH: Xem chi tiết tại: https://www.facebook.com/groups/729236007949900
BTC xin chân thành cảm ơn tất cả mọi người đã tham gia và luôn ủng hộ Việt Nam Ơi!
Người thắng giải xin vui lòng liên hệ trực tiếp với mod Khanh Nguyễn cung cấp những thông tin để BTC tiến hành gửi quà, tránh tình trạng bị sót cho mọi người nha
From Admin with love!
#VietNamOi #VietNamDonChao #contestVietNamOi";
        
        public AutoPostService(GlobalConfig globalConfig) : base(globalConfig)
        {
        }

        protected override AccountType AccountType { get; }
        protected override CrawlStopCondition CrawlStopCondition { get; }
        public override async Task Execute()
        {
            await AutoPost(new AccountProxyItem(), new List<string>());
        }

        private async Task<List<string>> AutoPost(AccountProxyItem accountProxy, List<string> postUrls)
        {
            // Clean userData folder name
            var userDataDir = $"{GlobalConfig.CrawlConfig.UserDataDirRoot}/DeleteComment";
            var dir = new DirectoryInfo(userDataDir);
            if (dir.Exists)
            {
                dir.Delete(true);
            }
            
            var browserContext =
                await PlaywrightHelper.InitPersistentBrowser(GlobalConfig.CrawlConfig, null, false,
                    $"DeleteComment");

            using (browserContext.Playwright)
            {
                await using (browserContext.Browser)
                {
                    var page = await browserContext.BrowserContext.NewPageAsync();
                    try
                    {
                        System.Console.WriteLine("Input FB link");
                        var url = System.Console.ReadLine();
                        if (string.IsNullOrWhiteSpace(url))
                        {
                            System.Console.WriteLine("Invalid FB link");
                        }
                        else
                        {
                            await page.GotoAsync(url);
                            await page.WaitForLoadStateAsync(LoadState.Load);
                            await page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
                            
                            System.Console.WriteLine("Please login to FB on browser");

                            while (true)
                            {
                                var selector_Menu = "//div[@aria-label='Menu']";
                                try
                                {
                                    await page.WaitForSelectorAsync(selector_Menu,
                                        new PageWaitForSelectorOptions {Timeout = 1000});
                                    break;
                                }
                                catch (Exception e)
                                {
                                    // ignored
                                }
                            }

                            await DoCrawl(page);

                            // // Delete Comment
                            // System.Console.WriteLine("Input FB ID");
                            // var fbId = System.Console.ReadLine();
                            // if (string.IsNullOrWhiteSpace(fbId))
                            // {
                            //     System.Console.WriteLine("Invalid FB ID");
                            // }
                            // else
                            // {
                            //     var ele_DisplayType = await page.QuerySelectorAsync("//span[text()='Phù hợp nhất' or text()='Most relevant']");
                            //     if (ele_DisplayType != null)
                            //     {
                            //         await ele_DisplayType.ClickAsync();
                            //         var ele_AllComments =
                            //             await page.QuerySelectorAsync("//span[text()='Tất cả bình luận' or text()='All comments']");
                            //         if (ele_AllComments != null)
                            //         {
                            //             await ele_AllComments.ClickAsync();
                            //             await page.WaitASecond();
                            //         }
                            //     }
                            //
                            //     var stopFinding = false;
                            //     while (!stopFinding)
                            //     {
                            //         var selector_popup =
                            //             $"//li//ancestor::a[contains(@href,'{fbId}')]/ancestor::div[@role='article' and @tabindex=-1]/descendant::div[@aria-haspopup='menu']";
                            //         // await page.WaitForSelectorAsync(selector_popup);
                            //         while ((await page.QuerySelectorAllAsync(selector_popup)).Any())
                            //         {
                            //             var button = await page.QuerySelectorAsync(selector_popup);
                            //             if (button == null) continue;
                            //             await button.ClickAsync();
                            //             await page.WaitASecond();
                            //             var selector_DeleteButton =
                            //                 "//div[@role='menuitem']/descendant::span[text()='Xóa' or text()='Delete' or text()='Remove comment' or text()='Gỡ bình luận']";
                            //             await page.WaitForSelectorAsync(selector_DeleteButton);
                            //             var ele_DeleteButton = await page.QuerySelectorAsync(selector_DeleteButton);
                            //             if (ele_DeleteButton == null) continue;
                            //             await ele_DeleteButton.ClickAsync();
                            //             await page.WaitASecond();
                            //
                            //             var selector_DeleteConfirm =
                            //                 "//div[@role='button' and (@aria-label='Xóa' or @aria-label='Delete' or @aria-label='Remove' or @aria-label='Xóa, gỡ bỏ')]";
                            //             await page.WaitForSelectorAsync(selector_DeleteConfirm);
                            //             var ele_DeleteConfirm = await page.QuerySelectorAsync(selector_DeleteConfirm);
                            //             if (ele_DeleteConfirm != null)
                            //             {
                            //                 await ele_DeleteConfirm.ClickAsync();
                            //                 await page.WaitASecond();
                            //             }
                            //         }
                            //
                            //         var selector_Posts =
                            //             "//li//ancestor::a[@href]/ancestor::div[@role='article' and @tabindex=-1]/descendant::div[@aria-haspopup='menu']//ancestor::div[@role='article' and @tabindex=-1]/div/span/a";
                            //         // await page.WaitForSelectorAsync(selector_Posts);
                            //         var ele_posts = await page.QuerySelectorAllAsync(selector_Posts);
                            //         if (ele_posts.Any())
                            //         {
                            //             var lastPost = ele_posts.Last();
                            //             await page.EvaluateAsync("lastPost => lastPost.scrollIntoViewIfNeeded(true)",
                            //                 lastPost);
                            //             await page.WaitASecond();
                            //         }
                            //
                            //         var ele_ExtendComment =
                            //             await page.QuerySelectorAsync("//span[text()='Xem thêm bình luận' or text()='View more comments' or contains(text(),'previous comments') or contains(text(),'bình luận trước')]");
                            //         if (ele_ExtendComment != null)
                            //         {
                            //             await ele_ExtendComment.ClickAsync();
                            //             await page.WaitASecond();
                            //         }
                            //         else
                            //         {
                            //             stopFinding = true;
                            //         }
                            //     }
                            // }
                        }
                    }
                    catch (Exception e)
                    {
                        System.Console.WriteLine(e);
                        await e.Log(string.Empty, string.Empty);
                    }
                    finally
                    {
                        await page.Wait(2000);
                        await page.CloseAsync();

                        await browserContext.BrowserContext.CloseAsync();
                    }
                }

                return new List<string>();
            }
            
            // var groupPosted = new List<string>();
            // InitLogConfig(accountProxy);
            // var browserContext = await PlaywrightHelper.InitPersistentBrowser(GlobalConfig.CrawlConfig, accountProxy);
            // using (browserContext.Playwright)
            // {
            //     await using (browserContext.Browser)
            //     {
            //         var page = await browserContext.BrowserContext.NewPageAsync();
            //         try
            //         {
            //             var facebookService = new FacebookLoginService(GlobalConfig.CrawlConfig);
            //             System.Console.WriteLine(
            //                 $"====================={this.GetType().Name}: Trying to login for {accountProxy.account.username}");
            //             var loginResponse =
            //                 await facebookService.Login(browserContext.BrowserContext, page, accountProxy);
            //             if (loginResponse.Success)
            //             {
            //                 await page.Wait(1000);
            //                 System.Console.WriteLine(
            //                     $"====================={this.GetType().Name}: Trying to Auto Post for {postUrls} posts");
            //                 foreach (var url in postUrls)
            //                 {
            //                     if (url.IsNullOrEmpty())
            //                     {
            //                         continue;
            //                     }
            //
            //                     System.Console.WriteLine(
            //                         $"====================={this.GetType().Name}: Trying to like post {url}");
            //                     var subpage = await browserContext.BrowserContext.NewPageAsync();
            //                     await subpage.GotoAsync(url);
            //                     await subpage.WaitForLoadStateAsync(LoadState.NetworkIdle,
            //                         new PageWaitForLoadStateOptions
            //                         {
            //                             Timeout = 60000
            //                         });
            //
            //                     if (await facebookService.IsAccountBanned(subpage))
            //                     {
            //                         System.Console.WriteLine(
            //                             $"====================={GetType().Name}: {CrawlStatus.AccountBanned} {accountProxy.account.username}");
            //                         await subpage.CloseAsync();
            //                         return new List<string>();
            //                     }
            //                     
            //                     System.Console.WriteLine($"====================={GetType().Name}: Check Banned Success {accountProxy.account.username}");
            //                     var canCrawl = await CanCrawl(subpage, url);
            //                     System.Console.WriteLine($"====================={GetType().Name}: Can Crawl {canCrawl} {accountProxy.account.username}");
            //                     
            //                     if (canCrawl)
            //                     {
            //                         await DoCrawl(page);
            //                         groupPosted.Add(url);
            //                     }
            //
            //                     await subpage.CloseAsync();
            //                 }
            //             }
            //         }
            //         catch (Exception e)
            //         {
            //             System.Console.WriteLine(e);
            //             throw;
            //         }
            //     }
            // }
        }
        
        private async Task<bool> CanCrawl(IPage page, string url)
        {
            await page.CheckDoneButton();
            
            if (await page.IsPageNotFound()) { return false; }

            if (await page.IsBlockedTemporary(url)) return false;

            if (await page.CanNotAccessPost(url))
            {
                var isAlreadyJoined = await page.GotoAndJoinGroup(url);
                if (isAlreadyJoined)
                {
                    return false;
                }

                return true;
            }

            var ele_BlockedTemporary = await page.QuerySelectorAsync(FacebookConsts.Selector_BlockedTemporary);
            if (ele_BlockedTemporary == null)
            {
                return true;
            }

            var elementImage = await ele_BlockedTemporary.QuerySelectorAsync("//../../../div/img");
            return elementImage != null;
        }
        
        private async Task DoCrawl(IPage page)
        {
            // ---------------- single post
            var selectorPostButton =
                "//div[@data-pagelet='GroupInlineComposer']/descendant::div[@role='button' and @tabindex='0']";

            var elementPostButton = await page.QuerySelectorAsync(selectorPostButton);
            if (elementPostButton != null)
            {
                await elementPostButton.ClickAsync();
                await page.Wait(1000);

                var selectorContent = "//form[@method='POST']/descendant::div[@role='presentation']";
                var elementContent = await page.QuerySelectorAsync(selectorContent);
                if (elementContent != null)
                {
                    await elementContent.TypeAsync(_content, new ElementHandleTypeOptions
                    {
                        Delay = 100,
                        Timeout = 0
                    });

                    var fileChooser = await page.RunAndWaitForFileChooserAsync(async () =>
                    {
                        await page.ClickAsync("//form[@method='POST']/descendant::div[@aria-label='Ảnh/Video']");
                    });

                    await fileChooser.SetFilesAsync(new List<string>
                    {
                        @"C:\Driver\277521005_520430719650889_192467150027291889_n.jpg"
                    });
                    
                    await page.Wait(1000);

                    var selectorPost = "//form[@method='POST']/descendant::div[@aria-label='Đăng']";
                    var elementPost = await page.QuerySelectorAsync(selectorPost);
                    if (elementPost != null)
                    {
                        await elementPost.ClickAsync();
                        await page.WaitForRequestFinishedAsync(new PageWaitForRequestFinishedOptions
                        {
                            Timeout = 0
                        });
                    }
                }
            }
        }

        protected override Task<CrawlResult> CanCrawl(IPage page, CrawlModelBase crawlItem)
        {
            throw new NotImplementedException();
        }

        protected override Task<CrawlResult> DoCrawl(IPage page, CrawlModelBase crawlItem)
        {
            throw new NotImplementedException();
        }
    }
}
