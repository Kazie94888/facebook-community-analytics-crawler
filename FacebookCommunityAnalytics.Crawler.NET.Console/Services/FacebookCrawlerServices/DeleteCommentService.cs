using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FacebookCommunityAnalytics.Crawler.NET.Client.Clients;
using FacebookCommunityAnalytics.Crawler.NET.Client.Configurations;
using FacebookCommunityAnalytics.Crawler.NET.Console.Playwrights;
using FacebookCommunityAnalytics.Crawler.NET.Console.Services.HotMailServices;
using FacebookCommunityAnalytics.Crawler.NET.Core.Helpers;
using Microsoft.Playwright;

namespace FacebookCommunityAnalytics.Crawler.NET.Console.Services.FacebookCrawlerServices
{
    public class DeleteCommentService
    {
        private readonly GlobalConfig _globalConfig;
        public DeleteCommentService(GlobalConfig globalConfig)
        {
            _globalConfig = globalConfig;
        }
        
        public async Task DeleteComment()
        {
            // Clean userData folder name
            var userDataDir = $"{_globalConfig.CrawlConfig.UserDataDirRoot}/DeleteComment";
            var dir = new DirectoryInfo(userDataDir);
            if (dir.Exists)
            {
                dir.Delete(true);
            }
            
            var browserContext =
                await PlaywrightHelper.InitPersistentBrowser(_globalConfig.CrawlConfig, null, false,
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
                            
                            // Delete Comment
                            System.Console.WriteLine("Input FB ID");
                            var fbId = System.Console.ReadLine();
                            if (string.IsNullOrWhiteSpace(fbId))
                            {
                                System.Console.WriteLine("Invalid FB ID");
                            }
                            else
                            {
                                var ele_DisplayType = await page.QuerySelectorAsync("//span[text()='Phù hợp nhất' or text()='Most relevant']");
                                if (ele_DisplayType != null)
                                {
                                    await ele_DisplayType.ClickAsync();
                                    var ele_AllComments =
                                        await page.QuerySelectorAsync("//span[text()='Tất cả bình luận' or text()='All comments']");
                                    if (ele_AllComments != null)
                                    {
                                        await ele_AllComments.ClickAsync();
                                        await page.WaitASecond();
                                    }
                                }

                                var stopFinding = false;
                                while (!stopFinding)
                                {
                                    var selector_popup =
                                        $"//li//ancestor::a[contains(@href,'{fbId}')]/ancestor::div[@role='article' and @tabindex=-1]/descendant::div[@aria-haspopup='menu']";
                                    // await page.WaitForSelectorAsync(selector_popup);
                                    while ((await page.QuerySelectorAllAsync(selector_popup)).Any())
                                    {
                                        var button = await page.QuerySelectorAsync(selector_popup);
                                        if (button == null) continue;
                                        await button.ClickAsync();
                                        await page.WaitASecond();
                                        var selector_DeleteButton =
                                            "//div[@role='menuitem']/descendant::span[text()='Xóa' or text()='Delete' or text()='Remove comment' or text()='Gỡ bình luận']";
                                        await page.WaitForSelectorAsync(selector_DeleteButton);
                                        var ele_DeleteButton = await page.QuerySelectorAsync(selector_DeleteButton);
                                        if (ele_DeleteButton == null) continue;
                                        await ele_DeleteButton.ClickAsync();
                                        await page.WaitASecond();

                                        var selector_DeleteConfirm =
                                            "//div[@role='button' and (@aria-label='Xóa' or @aria-label='Delete' or @aria-label='Remove' or @aria-label='Xóa, gỡ bỏ')]";
                                        await page.WaitForSelectorAsync(selector_DeleteConfirm);
                                        var ele_DeleteConfirm = await page.QuerySelectorAsync(selector_DeleteConfirm);
                                        if (ele_DeleteConfirm != null)
                                        {
                                            await ele_DeleteConfirm.ClickAsync();
                                            await page.WaitASecond();
                                        }
                                    }

                                    var selector_Posts =
                                        "//li//ancestor::a[@href]/ancestor::div[@role='article' and @tabindex=-1]/descendant::div[@aria-haspopup='menu']//ancestor::div[@role='article' and @tabindex=-1]/div/span/a";
                                    // await page.WaitForSelectorAsync(selector_Posts);
                                    var ele_posts = await page.QuerySelectorAllAsync(selector_Posts);
                                    if (ele_posts.Any())
                                    {
                                        var lastPost = ele_posts.Last();
                                        await page.EvaluateAsync("lastPost => lastPost.scrollIntoViewIfNeeded(true)",
                                            lastPost);
                                        await page.WaitASecond();
                                    }

                                    var ele_ExtendComment =
                                        await page.QuerySelectorAsync("//span[text()='Xem thêm bình luận' or text()='View more comments' or contains(text(),'previous comments') or contains(text(),'bình luận trước')]");
                                    if (ele_ExtendComment != null)
                                    {
                                        await ele_ExtendComment.ClickAsync();
                                        await page.WaitASecond();
                                    }
                                    else
                                    {
                                        stopFinding = true;
                                    }
                                }
                            }
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
            }
        }
    }
}