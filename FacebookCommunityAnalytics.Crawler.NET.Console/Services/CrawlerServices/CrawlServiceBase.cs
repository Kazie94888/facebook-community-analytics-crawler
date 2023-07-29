using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using FacebookCommunityAnalytics.Crawler.NET.Client.Clients;
using FacebookCommunityAnalytics.Crawler.NET.Client.Configurations;
using FacebookCommunityAnalytics.Crawler.NET.Client.Entities;
using FacebookCommunityAnalytics.Crawler.NET.Client.Enums;
using FacebookCommunityAnalytics.Crawler.NET.Client.Helpers;
using FacebookCommunityAnalytics.Crawler.NET.Console.Playwrights;
using FacebookCommunityAnalytics.Crawler.NET.Core.Extensions;
using FacebookCommunityAnalytics.Crawler.NET.Core.Helpers;
using Flurl;
using log4net;
using log4net.Config;
using Microsoft.Playwright;

namespace FacebookCommunityAnalytics.Crawler.NET.Console.Services.CrawlerServices
{
    public abstract class CrawlServiceBase
    {
        public CrawlServiceBase(GlobalConfig globalConfig)
        {
            GlobalConfig = globalConfig;
            ApiClient = new ApiClient(globalConfig.ApiConfig);
        }

        protected ApiClient ApiClient { get; }
        protected GlobalConfig GlobalConfig { get; }
        protected abstract AccountType AccountType { get; }
        protected abstract CrawlStopCondition CrawlStopCondition { get; }

        private ILog Logger { get; set; }

        protected void InitLogConfig(AccountProxyItem accountProxy)
        {
            var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
            GlobalContext.Properties["fname"] = accountProxy.account.username;
            XmlConfigurator.Configure(logRepository, new FileInfo("Configurations/log4net.config"));
                
            Logger = LogManager.GetLogger(typeof(Program));
        }

        public abstract Task Execute();

        protected Queue<AccountProxyItem> GetAccountProxyQueue(AccountType accountType)
        {
            return new Queue<AccountProxyItem>(GetAccountProxyItems(accountType));
        }

        protected void ResetAccountsCrawlStatus(List<Guid> accountProxyIds, AccountType accountType)
        {
            ApiClient.Crawl.ResetAccountsCrawlStatus(new ResetAccountsCrawlStatusRequest
            {
                AccountProxyIds = accountProxyIds,
                AccountType = accountType
            });
        }

        protected abstract Task<CrawlResult> CanCrawl(IPage page, CrawlModelBase crawlItem);
        protected abstract Task<CrawlResult> DoCrawl(IPage page, CrawlModelBase crawlItem);

        protected async Task<CrawlResult> Crawl(AccountProxyItem accountProxy, List<CrawlModelBase> crawlSourceItems)
        {
            InitLogConfig(accountProxy);
            
            var browserContext = await PlaywrightHelper.InitPersistentBrowser(GlobalConfig.CrawlConfig, accountProxy, false);
            using (browserContext.Playwright)
            {
                await using (browserContext.Browser)
                {
                    var page = await browserContext.BrowserContext.NewPageAsync();
                    try
                    {
                        var facebookService = new FacebookLoginService(GlobalConfig.CrawlConfig);
                        System.Console.WriteLine($"====================={this.GetType().Name}: Trying to login for {accountProxy.account.username}");

                        var loginResponse = await facebookService.Login(browserContext.BrowserContext, page, accountProxy);
                        if (loginResponse.Success)
                        {
                            var crawlResult = new CrawlResult(CrawlStatus.OK);

                            // GroupPost: run once
                            // GroupUser: run once
                            // PagePost: run once
                            // SelectivePosts: run for many times
                            System.Console.WriteLine($"====================={this.GetType().Name}: Trying to CRAWL for {crawlSourceItems.Count} items");
                            foreach (var crawlItem in crawlSourceItems)
                            {
                                if (StringExtensions.IsNullOrEmpty(crawlItem.Url)) continue;

                                System.Console.WriteLine($"====================={this.GetType().Name}: Trying to CRAWL url {crawlItem.Url}");
                                var subpage = await browserContext.BrowserContext.NewPageAsync();
                                await subpage.Wait(1000);
                                if (!crawlItem.Url.Contains("https://"))
                                {
                                    crawlItem.Url = $"https://{crawlItem.Url}";
                                }
                                await subpage.GotoAsync(crawlItem.Url);
                                await subpage.Wait(1000);
                                await subpage.WaitForLoadStateAsync(LoadState.NetworkIdle, new PageWaitForLoadStateOptions
                                {
                                    Timeout = 60000
                                });

                                // System.Console.WriteLine($"Check account banned {accountProxy.account.username}");
                                if (await facebookService.IsAccountBanned(subpage))
                                {
                                    System.Console.WriteLine($"====================={GetType().Name}: {CrawlStatus.AccountBanned} {accountProxy.account.username}");
                                    await subpage.CloseAsync();
                                    return new CrawlResult(CrawlStatus.AccountBanned)
                                    {
                                        Success = false
                                    };
                                }
                                
                                var canCrawl = await CanCrawl(subpage, crawlItem);
                                switch (canCrawl.Status)
                                {
                                    case CrawlStatus.OK:
                                    {
                                        if (JoinGroupUrls().Contains(crawlItem.Url))
                                        {
                                            await subpage.JoinGroup();
                                        }
                                        var itemResult = await DoCrawl(subpage, crawlItem);
                                        if (itemResult != null && itemResult.Posts.Any())
                                        {
                                            crawlResult.Posts.AddRange(itemResult.Posts);
                                        }
                                        break;
                                    }

                                    // SelectivePosts: run for many times
                                    case CrawlStatus.PostUnavailable:
                                    {
                                        //  TODOO Vu.Nguyen: CREATE a fake post to store UNAVAILABLE 
                                        break;
                                    }

                                    case CrawlStatus.AccountBanned:
                                    case CrawlStatus.BlockedTemporary:
                                    case CrawlStatus.LoginFailed:
                                    case CrawlStatus.LoginApprovalNeeded:
                                    case CrawlStatus.GroupUnavailable:
                                    case CrawlStatus.UnknownFailure:
                                    {
                                        System.Console.WriteLine($"====================={GetType().Name}: {canCrawl.Status} {accountProxy.account.username}");
                                        await subpage.CloseAsync();
                                        return new CrawlResult(canCrawl.Status)
                                        {
                                            Success = false
                                        };
                                    }

                                    default: throw new ArgumentOutOfRangeException("crawlerResult.Status");
                                }

                                await subpage.CloseAsync();
                                
                                System.Console.WriteLine($"{GetType().Name}: Status:{crawlResult.Status} - CRAWLED {crawlResult.Posts?.Count ?? 0} on {DateTime.UtcNow} url {crawlItem.Url}");
                            } // end for/end batch

                            return crawlResult;
                        }
                        else
                        {
                            if (GenericExtensions.IsIn(loginResponse.Status, CrawlStatus.AccountBanned, CrawlStatus.LoginApprovalNeeded, CrawlStatus.BlockedTemporary))
                            {
                                System.Console.WriteLine($"====================={this.GetType().Name}: REMOVE ACCOUNT: {accountProxy.account.username} STATUS: {loginResponse.Status}");
                                LogBannedAccount(accountProxy.account, loginResponse.Status);
                                return new CrawlResult(loginResponse.Status);
                            }
                            else
                            {
                                System.Console.WriteLine($"LOGIN FAILURE {CrawlStatus.UnknownFailure}====================================================================");
                                return new CrawlResult(CrawlStatus.UnknownFailure); 
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        System.Console.WriteLine($"CRAWL ERROR ====================================================================");
                        System.Console.WriteLine($"{GetType().Name}: CRAWL ERROR {accountProxy.account.username}/{accountProxy.proxy.ip}:{accountProxy.proxy.port}");
                        System.Console.WriteLine($"{e.Message}");
                        System.Console.WriteLine($"================================================================================");
                        await e.Log(accountProxy.account.username, $"{accountProxy.account.username}/{accountProxy.proxy.ip}:{accountProxy.proxy.port}");
                        throw;
                    }
                    finally
                    {
                        System.Console.WriteLine("CRAWL FINISHED. CLOSE BROWSER IN 1 SECs");
                        await Task.Delay(1000);
                        await browserContext.BrowserContext.CloseAsync();
                    }
                }
            }
        }

        protected void LogBannedAccount(Account account, CrawlStatus crawlStatus)
        {
            // var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, GlobalConfig.CrawlConfig.BannedAccountsLogPath);
            // if (!Directory.Exists(path)) { Directory.CreateDirectory(path); }
            //
            // using (FileStream fs = File.Create(Path.Combine(path, $"{account.username}.txt"))) { }

            switch (crawlStatus)
            {
                case CrawlStatus.AccountBanned:
                    ApiClient.Account.UpdateAccountInfo(Guid.Parse(account.id), null, AccountStatus.Banned);
                    break;

                case CrawlStatus.LoginApprovalNeeded:
                    ApiClient.Account.UpdateAccountInfo(Guid.Parse(account.id), null, AccountStatus.LoginApprovalNeeded);
                    break;
                
                case CrawlStatus.BlockedTemporary:
                    ApiClient.Account.UpdateAccountInfo(Guid.Parse(account.id), null, AccountStatus.BlockedTemporary);
                    break;
            }
        }

        protected async Task<bool> IsReelPost(IElementHandle ele_Post)
        {
            var element = await ele_Post.QuerySelectorAsync("//descendant::span[text()='Reels']");
            return element != null;
        }

        #region CRAWL LOGIC

        public virtual async Task<AutoCrawledPost> CrawlPostDetail(IPage page, IElementHandle ele_Post, CrawlModelBase crawlItem, string url)
        {
            var createdByPair = await CrawlCreatedBy(ele_Post);
            var sourcePost = new AutoCrawledPost
            {
                Url = url,
                PostSourceType = PostSourceType.Group,
                CreateFuid = createdByPair.Key,
                GroupFid = crawlItem.GroupFid,
                CreatedBy = createdByPair.Value,
                // TODO: get emoji/icon
                Content = await CrawlContent(page, ele_Post)
            };
            sourcePost.Urls = await CrawlUrls(sourcePost.Content);
            sourcePost.HashTags = FacebookHelper.GetHashtags(sourcePost.Content);
            
            sourcePost.LikeCount = await CrawlReactionCount(page, ele_Post);
            sourcePost.CommentCount = await CrawlDefaultCount(page, ele_Post, GetSelectorComment());
            sourcePost.ShareCount = await CrawlShareCount(page, ele_Post);
            if (sourcePost.ShareCount == 0) sourcePost.ShareCount = await CrawlDefaultCount(page, ele_Post, GetSelectorShare());
            
            return sourcePost;
        }
        
        public virtual async Task<AutoCrawledPost> CrawlVideoDetail(IPage page, IElementHandle ele_Post, CrawlModelBase crawlItem, string url)
        {
            var createdByPair = await CrawlVideoCreatedBy(ele_Post);
            var ele_Content = await ele_Post.QuerySelectorAsync("xpath=div/div/div/div[2]/div/div[2]");
            var sourcePost = new AutoCrawledPost
            {
                Url = url,
                PostSourceType = PostSourceType.Video,
                CreateFuid = createdByPair.Key,
                GroupFid = crawlItem.GroupFid,
                CreatedBy = createdByPair.Value,
                // TODO: get emoji/icon
                Content = await CrawlVideoContent(page, ele_Content)
            };
            
            sourcePost.Urls = await CrawlUrls(sourcePost.Content);
            sourcePost.HashTags = FacebookHelper.GetHashtags(sourcePost.Content);
            
            var ele_LikeButton = await page.QuerySelectorAsync("//div[@id='watch_feed']//ancestor::span[@role='toolbar']");
            sourcePost.LikeCount = await CrawlVideoReactionCount(page, ele_LikeButton);
            sourcePost.CommentCount = await CrawlDefaultCount(page, ele_LikeButton, "xpath=../../div[3]//ancestor::span[contains(text(),'Comment') or contains(text(),'comment') or contains(text(),'bình luận')]");
            
            return sourcePost;
            
        }

        protected virtual string GetSelectorComment()
        {
            return FacebookConsts.Selector_Comment;
        }

        protected virtual string GetSelectorShare()
        {
            return FacebookConsts.Selector_Share;
        }

        protected virtual async Task<IElementHandle> GetElementPostCreatedAt(IElementHandle ele_Post, IPage page)
        {
            IElementHandle element = null;
            var            count   = 0;
            while (element == null && count <= 10)
            {
                element = await ele_Post.QuerySelectorAsync("//*[local-name() = 'svg'][@title]//ancestor::span//ancestor::a");
                if (element == null) await page.Wait();
                count += 1;
            }

            return element;
        }

        protected async Task<DateTime?> CrawlVideoCreatedAt(IPage page, IElementHandle ele_Post)
        {
            var ele_PostCreatedAt = await ele_Post.QuerySelectorAsync("xpath=div/div/div/div[2]/div/div/div[2]/div/div[2]/span/span/span[2]/span");
            if (ele_PostCreatedAt == null) return null;
            
            // await ele_PostCreatedAt.HoverAsync();
            await page.HoverAndWait(ele_PostCreatedAt, FacebookConsts.Selector_ToolTip, 10000);
            
            // await page.Wait(FacebookConsts.Selector_ToolTip, timeoutInMs:2000);
            var tooltip = await page.QuerySelectorAsync(FacebookConsts.Selector_ToolTip);
            
            if (tooltip != null)
            {
                var span = await tooltip.QuerySelectorAsync("div >> span");
                
                if (span == null)
                {
                    System.Console.WriteLine("span Is NUll");
                }
                
                if (span != null)
                {
                    string sDate = await span.InnerTextAsync();
                    sDate = sDate.Trim();
                    var cultureInfo = sDate.Contains("lúc") ? new CultureInfo("vi-VN") : new CultureInfo("en-US");

                    sDate = sDate.ToLower();
                    sDate = sDate.Replace(" lúc", string.Empty)
                        .Replace(" Tháng", string.Empty)
                        .Replace(" tháng", string.Empty)
                        .Replace(" at", string.Empty)
                        .Replace("thứ hai", string.Empty)
                        .Replace("thứ ba", string.Empty)
                        .Replace("thứ tư", string.Empty)
                        .Replace("thứ năm", string.Empty)
                        .Replace("thứ sáu", string.Empty)
                        .Replace("thứ bảy", string.Empty)
                        .Replace("chủ nhật", string.Empty);
                    sDate = sDate.Trim(' ').Trim(',');
                    sDate = sDate.Trim();
                    try
                    {
                        DateTime.TryParse(sDate, cultureInfo, DateTimeStyles.AssumeLocal, out var dateTime);
                        System.Console.WriteLine($"DateTime Parse {dateTime}");
                        return dateTime;
                    }
                    catch (Exception ex)
                    {
                        System.Console.WriteLine($"{ex.Message}");
                        return null;
                    }
                }
            }

            return null;
        }

        protected async Task<DateTime?> CrawlCreatedAt(IPage page, IElementHandle ele_Post)
        {
            var ele_PostCreatedAt = await GetElementPostCreatedAt(ele_Post, page);
            if (ele_PostCreatedAt == null) return null;
    
            await ele_PostCreatedAt.HoverAsync();
            await page.Wait(FacebookConsts.Selector_ToolTip, timeoutInMs:2000);
            var tooltip = await page.QuerySelectorAsync(FacebookConsts.Selector_ToolTip);
            if (tooltip == null) return null;
            
            var span = await tooltip.QuerySelectorAsync("div >> span");
                
            if (span == null)
            {
                System.Console.WriteLine("span Is NUll");
            }

            if (span == null) return null;
            var sDate = await span.InnerTextAsync();
            sDate = sDate.Trim();
            var cultureInfo = sDate.Contains("lúc") ? new CultureInfo("vi-VN") : new CultureInfo("en-US");

            sDate = sDate.ToLower();
            sDate = sDate.Replace(" lúc", string.Empty)
                         .Replace(" Tháng", string.Empty)
                         .Replace(" tháng", string.Empty)
                         .Replace(" at", string.Empty)
                         .Replace("thứ hai", string.Empty)
                         .Replace("thứ ba", string.Empty)
                         .Replace("thứ tư", string.Empty)
                         .Replace("thứ năm", string.Empty)
                         .Replace("thứ sáu", string.Empty)
                         .Replace("thứ bảy", string.Empty)
                         .Replace("chủ nhật", string.Empty);
            sDate = sDate.Trim(' ').Trim(',');
            sDate = sDate.Trim();
            try
            {
                DateTime.TryParse(sDate, cultureInfo, DateTimeStyles.AssumeLocal, out var dateTime);
                return dateTime;
            }
            catch (Exception ex)
            {
                System.Console.WriteLine($"{ex.Message}");
                return null;
            }
        }

        protected static async Task<KeyValuePair<string, string>> CrawlCreatedBy(IElementHandle ele_Post)
        {
            var ele_PostCreatedAt = await ele_Post.QuerySelectorAsync("//ancestor::a[@tabindex=0 and boolean(@aria-label)=false]");
            if (ele_PostCreatedAt == null) return new KeyValuePair<string, string>(string.Empty, string.Empty);

            var     href = await ele_PostCreatedAt.GetAttributeAsync("href");
            string? fuid;
            if (href != null && href.Contains("profile.php"))
            {
                fuid = new Url(href).QueryParams.FirstOrDefault().Value.ToString();
            }
            else
            {
                fuid = new Url(href).Path.Trim('/');
                if (fuid.Contains("/user/"))
                {
                    fuid = Regex.Split(fuid, "/user/")[1];
                }
            }
            
            
            return new KeyValuePair<string, string>(fuid?? string.Empty, await ele_PostCreatedAt.InnerTextAsync() ) ;
        }

        private static async Task<KeyValuePair<string, string>> CrawlVideoCreatedBy(IElementHandle ele_Post)
        {
            var ele_ParentPostDate = await ele_Post.QuerySelectorAsync("xpath=div/div/div/div[2]/div/div/div[2]/div/div");
            if(ele_ParentPostDate == null) return default(KeyValuePair<string,string>);
            
            var ele_PostCreatedAt = await ele_ParentPostDate.QuerySelectorAsync("//ancestor::a");
            if (ele_PostCreatedAt == null) return new KeyValuePair<string, string>(string.Empty, string.Empty);

            var href = await ele_PostCreatedAt.GetAttributeAsync("href");
            string fuid = string.Empty;
            if (href.Contains("profile.php"))
            {
                fuid = new Url(href).QueryParams.FirstOrDefault().Value.ToString();
            }
            else
            {
                fuid = new Url(href).Path.Trim('/');
                if (fuid.Contains("/user/"))
                {
                    fuid = Regex.Split(fuid, "/user/")[1];
                }
            }
            
            
            return new KeyValuePair<string, string>(fuid, await ele_PostCreatedAt.InnerTextAsync() ) ;
        }

        protected async Task<List<string>> CrawlUrls(string content)
        {
            var linksInContent = FacebookHelper.GetLinks(content);
            return linksInContent;
        }

        protected async Task<string> CrawlContent(IPage page, IElementHandle ele_Post)
        {
            var selector_PostContent = "div[data-ad-comet-preview='message']";
            
            var ele_PostContent = await ele_Post.QuerySelectorAsync(selector_PostContent);
            if (ele_PostContent == null)
            {
                await ClickSeeMoreButton(page, ele_Post);

                selector_PostContent = "xpath=//descendant::div[not(@*)]";
                ele_PostContent = await ele_Post.QuerySelectorAsync(selector_PostContent);
                if (ele_PostContent != null)
                {
                    var ele_SubPostContent = await ele_PostContent.QuerySelectorAsync("//div[2]/div/div[3]");
                    if (ele_SubPostContent != null)
                    {
                        return await ele_PostContent.InnerTextAsync();
                    }
                }

                System.Console.WriteLine($"Content is Null {page.Url}");

                return string.Empty;
            }
            await ClickSeeMoreButton(page, ele_Post);
            return await ele_PostContent.InnerTextAsync();
        }

        protected async Task<string> CrawlVideoContent(IPage page, IElementHandle ele_Post)
        {
            var selector_PostContent = "div[data-ad-comet-preview='message']";
            
            var ele_PostContent = await ele_Post.QuerySelectorAsync(selector_PostContent);
            if (ele_PostContent == null)
            {
                await ClickSeeMoreButton(page, ele_Post);

                selector_PostContent = "xpath=//descendant::div[not(@*)]";
                ele_PostContent = await ele_Post.QuerySelectorAsync(selector_PostContent);
                if (ele_PostContent != null)
                {
                    var ele_SubPostContent = await ele_PostContent.QuerySelectorAsync("//div");
                    if (ele_SubPostContent != null)
                    {
                        return await ele_PostContent.InnerTextAsync();
                    }
                }

                System.Console.WriteLine($"Content is Null {page.Url}");

                return string.Empty;
            }
            await ClickSeeMoreButton(page, ele_Post);
            return await ele_PostContent.InnerTextAsync();
        }

        private async Task ClickSeeMoreButton(IPage page, IElementHandle ele_Post)
        {
            var selector_SeeMore_En = "//ancestor::div[@role='button']//ancestor::div[text()='See more']";
            var selector_SeeMore_En1 = "//ancestor::div[@role='button']//ancestor::div[text()='See More']";
            var selector_SeeMore_Vi = "//ancestor::div[@role='button']//ancestor::div[text()='Xem thêm']";
            var selector_SeeMore_Vi1 = "//ancestor::div[@role='button']//ancestor::div[text()='Xem Thêm']";
            var btn_SeeMore_En = await ele_Post.QuerySelectorAsync(selector_SeeMore_En);
            var btn_SeeMore_En1 = await ele_Post.QuerySelectorAsync(selector_SeeMore_En1);
            var btn_SeeMore_Vi = await ele_Post.QuerySelectorAsync(selector_SeeMore_Vi);
            var btn_SeeMore_Vi1 = await ele_Post.QuerySelectorAsync(selector_SeeMore_Vi1);
            var count = 0;
            var ele_Reaction = await ele_Post.QuerySelectorAsync("//ancestor::div[@aria-label='Thích']") ?? await ele_Post.QuerySelectorAsync("//ancestor::div[@aria-label='Like']");

            while (btn_SeeMore_En != null || btn_SeeMore_Vi != null || btn_SeeMore_En1 != null || btn_SeeMore_Vi1 != null)
            {
                if(count > 10) break;
                if (btn_SeeMore_En != null)
                {
                    await page.EvaluateAsync("btn_SeeMore_En => btn_SeeMore_En.click()", btn_SeeMore_En);
                }
                
                if (btn_SeeMore_En1 != null)
                {
                    await page.EvaluateAsync("btn_SeeMore_En1 => btn_SeeMore_En1.click()", btn_SeeMore_En1);
                }

                if (btn_SeeMore_Vi != null)
                {
                    await page.EvaluateAsync("btn_SeeMore_Vi => btn_SeeMore_Vi.click()", btn_SeeMore_Vi);
                }
                
                if (btn_SeeMore_Vi1 != null)
                {
                    await page.EvaluateAsync("btn_SeeMore_Vi1 => btn_SeeMore_Vi1.click()", btn_SeeMore_Vi1);
                }

                await page.WaitQuaterSecond();

                btn_SeeMore_En = await ele_Post.QuerySelectorAsync(selector_SeeMore_En);
                btn_SeeMore_En1 = await ele_Post.QuerySelectorAsync(selector_SeeMore_En1);
                btn_SeeMore_Vi = await ele_Post.QuerySelectorAsync(selector_SeeMore_Vi);
                btn_SeeMore_Vi1 = await ele_Post.QuerySelectorAsync(selector_SeeMore_Vi1);
                count += 1;
            }

            if (ele_Reaction != null)
            {
                await page.EvaluateAsync("ele_Reaction => ele_Reaction.scrollIntoViewIfNeeded(true)", ele_Reaction);
                await page.WaitForLoadStateAsync(LoadState.NetworkIdle);
                await page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
            }
        }

        protected async Task<int> CrawlShareCount(IPage page, IElementHandle ele_Post)
        {
            var count = 0;

            var ele_Share = await ele_Post.QuerySelectorAsync(FacebookConsts.Selector_Share);
            if (ele_Share == null) return count;

            var text = (await ele_Share.InnerTextAsync()).Trim();
            var strings = text.Split(' ');
            var textValue = strings[0];
            if (textValue.Contains("K") || textValue.Contains("M"))
            {
                await page.HoverAndWait(ele_Share, FacebookConsts.Selector_ToolTip);

                var tooltipItems = await page.QuerySelectorAllAsync(FacebookConsts.Selector_ToolTipItem);
                if (tooltipItems.Any())
                {
                    var totalShare = tooltipItems.Count - 1;
                    var tooltipItem = tooltipItems.LastOrDefault();
                    if (tooltipItem != null)
                    {
                        var innerString = await tooltipItem.InnerTextAsync();
                        var reactionString = Regex.Replace(innerString, "[^0-9]+", string.Empty);
                        var reactionCount = reactionString.ToIntOrDefault();
                        if (reactionCount == 0) { totalShare++; }
                        else { totalShare += reactionCount; }
                    }

                    count = totalShare;
                }

                if (count == 0)
                {
                    if (textValue.Contains("K"))
                    {
                        count = (int) (textValue.Trim('K').ToDecimalOrDefault() * 1000);
                    }
                    else if(textValue.Contains("M"))
                    {
                        count = (int) (textValue.Trim('M').ToDecimalOrDefault() * 1000000);
                    }
                }
            }
            else
            {
                count = textValue.ToIntOrDefault();
            }

            return count;
        }

        protected async Task<int> CrawlDefaultCount(IPage page, IElementHandle ele_Post, string selector)
        {
            var count = 0;

            var regexK = FacebookConsts.GetKRegex();
            var element = await ele_Post.QuerySelectorAsync(selector);
            if (element == null) return count;

            var text = (await element.InnerTextAsync()).Trim();
            var strings = text.Split(' ');
            var textValue = strings[0]; 
            if (textValue.Contains("K") || textValue.Contains("M"))
            {
                await element.HoverAsync();
            
                text = await element.InnerTextAsync();
                var countString = Regex.Replace(text, "[^0-9]+", string.Empty);

                var iCount = countString.ToIntOrDefault();
                var multiplier = text.Contains(".") || text.Contains(",") ? 100 : 1000;
                if (regexK.IsMatch(text)) iCount *= multiplier;

                count = iCount;

                if (count == 0)
                {
                    if (textValue.Contains("K"))
                    {
                        count = (int) (textValue.Trim('K').ToDecimalOrDefault() * 1000);
                    }
                    else if(textValue.Contains("M"))
                    {
                        count = (int) (textValue.Trim('M').ToDecimalOrDefault() * 1000000);
                    }
                    
                }
            }
            else
            {
                count = textValue.ToIntOrDefault();
            }
            
            return count;
        }

        protected virtual string GetSelectorReaction()
        {
            return FacebookConsts.Selector_Reaction;
        }

        protected async Task<int> CrawlVideoReactionCount(IPage page, IElementHandle ele_Post)
        {
            var count = 0;
            var ele_Reaction = await ele_Post.QuerySelectorAsync("../div[@role='button']");
            if (ele_Reaction == null) return count;
            
            await page.EvaluateAsync("ele_Reaction => ele_Reaction.scrollIntoViewIfNeeded(true)", ele_Reaction);
            await page.WaitForLoadStateAsync(LoadState.NetworkIdle);
            await page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
                
            var text = await ele_Reaction.InnerTextAsync();
            if (text.Contains("K"))
            {
                await page.HoverAndWait(ele_Reaction, FacebookConsts.Selector_ToolTip, 10000);

                var tooltipItems = await page.QuerySelectorAllAsync(FacebookConsts.Selector_ToolTipItem);
                if (tooltipItems.Any())
                {
                    var reactionLineCount = tooltipItems.Count - 1;
                    var tooltipItem = tooltipItems.LastOrDefault();
                    if (tooltipItem != null)
                    {
                        var innerString = await tooltipItem.InnerTextAsync();
                        var reactionString = Regex.Replace(innerString, "[^0-9]+", string.Empty);
                        var lastTooltipCount = reactionString.ToIntOrDefault();
                        if (string.IsNullOrEmpty(reactionString)) { lastTooltipCount = 0; }

                        if (lastTooltipCount == 0) { reactionLineCount++; }
                        else { reactionLineCount += lastTooltipCount; }
                    }

                    count = reactionLineCount;
                }

                if (count == 0)
                {
                    count = (int) (text.Trim('K').ToDecimalOrDefault() * 1000);
                }
            }
            else
            {
                count = text.ToIntOrDefault();
            }

            return count;
        }

        protected async Task<int> CrawlReactionCount(IPage page, IElementHandle ele_Post)
        {
            var count = 0;

            var ele_Reaction = await ele_Post.QuerySelectorAsync(GetSelectorReaction());
            if (ele_Reaction == null) return count;

            await page.EvaluateAsync("ele_Reaction => ele_Reaction.scrollIntoViewIfNeeded(true)", ele_Reaction);
            await page.WaitForLoadStateAsync(LoadState.NetworkIdle);
            await page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
                
            var text                      = await ele_Reaction.InnerTextAsync();
            if (text.Contains("\n")) text = text.Split("\n")[0];
            if (text.Contains("K"))
            {
                await page.HoverAndWait(ele_Reaction, FacebookConsts.Selector_ToolTip, 10000);

                var tooltipItems = await page.QuerySelectorAllAsync(FacebookConsts.Selector_ToolTipItem);
                if (tooltipItems.Any())
                {
                    var reactionLineCount = tooltipItems.Count - 1;
                    var tooltipItem = tooltipItems.LastOrDefault();
                    if (tooltipItem != null)
                    {
                        var innerString = await tooltipItem.InnerTextAsync();
                        var reactionString = Regex.Replace(innerString, "[^0-9]+", string.Empty);
                        var lastTooltipCount = reactionString.ToIntOrDefault();
                        if (string.IsNullOrEmpty(reactionString)) { lastTooltipCount = 0; }

                        if (lastTooltipCount == 0) { reactionLineCount++; }
                        else { reactionLineCount += lastTooltipCount; }
                    }

                    count = reactionLineCount;
                }

                if (count == 0)
                {
                    count = (int) (text.Trim('K').ToDecimalOrDefault() * 1000);
                }
            }
            else
            {
                count = text.ToIntOrDefault();
            }

            return count;
        }

        // TODOO Vu.Dao: current crawler server timezone = UTC, but facebook is GMT +7, using DateTime.UtcNow is correct?
        protected bool IsStopConditionMet(DateTime createdAt)
        {
            switch (CrawlStopCondition)
            {
                case CrawlStopCondition.PayrollCycle:
                {
                    var dateTimeRange = PayrollHelper.GetDefaultPayrollDateTime();
                    var startDateTime = dateTimeRange.Key;

                    return createdAt < startDateTime;
                }
                case CrawlStopCondition.Daily:
                {
                    return createdAt.Day < DateTime.UtcNow.Day;
                }
                case CrawlStopCondition.Monthly:
                {
                    return createdAt.Month != DateTime.UtcNow.Month;
                }
                case CrawlStopCondition.FourHour:
                {
                    // one more hour for sure
                    return createdAt < DateTime.UtcNow.AddHours(-6);
                }
                case CrawlStopCondition.Weekly:
                {
                    return createdAt < DateTime.UtcNow.AddDays(-8);
                }

                case CrawlStopCondition.TwoDay:
                {
                    return createdAt < DateTime.UtcNow.AddDays(-2).Date;
                }
                case CrawlStopCondition.ThreeDay:
                {
                    return createdAt < DateTime.UtcNow.AddDays(-3).Date;
                }
                case CrawlStopCondition.FourDay:
                {
                    return createdAt < DateTime.UtcNow.AddDays(-4).Date;
                }
                case CrawlStopCondition.TwoWeek:
                {
                    return createdAt < DateTime.UtcNow.AddDays(-15).Date;
                }
                default:
                    return true;
            }
        }

        private IList<string> JoinGroupUrls()
        {
            return new List<string>
            {
                "https://www.facebook.com/groups/chiemmacdepp?sorting_setting=CHRONOLOGICAL",
                "https://www.facebook.com/groups/292029379446797?sorting_setting=CHRONOLOGICAL",
                "https://www.facebook.com/groups/travelphotoandvideo?sorting_setting=CHRONOLOGICAL"
            };
        }

        #endregion

        #region Account Login

        private List<AccountProxyItem> GetAccountProxyItems(AccountType accountType)
        {
            var res = ApiClient.Crawl.GetAccountProxies(new GetAccountProxiesRequest {AccountType = accountType});
            if (!res.IsSuccess || res.Resource.IsNullOrEmpty()) return null;
            var accountProxyItems = res.Resource.Where(_ => _.account.accountStatus == AccountStatus.Active).ToList();
            accountProxyItems = accountProxyItems.OrderBy(item => item.accountProxy.CrawledAt).ToList();
            return accountProxyItems;
        }

        #endregion
    }
}