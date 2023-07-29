using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using FacebookCommunityAnalytics.Crawler.NET.Client.Configurations;
using FacebookCommunityAnalytics.Crawler.NET.Client.Entities;
using FacebookCommunityAnalytics.Crawler.NET.Core.Extensions;
using FacebookCommunityAnalytics.Crawler.NET.LacXoong.Models;
using Microsoft.Playwright;

namespace FacebookCommunityAnalytics.Crawler.NET.LacXoong.Playwrights
{
    public static class PlaywrightExtensions
    {
        /// <summary>
        /// Wait for a selector. Can be DOM selector or xpath start with xpath=
        /// </summary>
        /// <param name="page"></param>
        /// <param name="selector"></param>
        /// <param name="state"></param>
        /// <param name="timeoutInMs"></param>
        /// <returns> true if selector exists, false if not</returns>
        public static async Task<bool> Wait(this IPage page, string selector, WaitForSelectorState state = WaitForSelectorState.Visible, int timeoutInMs = 250)
        {
            try
            {
                await page.WaitForSelectorAsync(selector, new PageWaitForSelectorOptions
                {
                    State = state,
                    Timeout = timeoutInMs
                });
                return true;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                return false;
            }
        }

        public static async Task HoverAndWait(this IPage page, IElementHandle elementHandle, string waitSelector, int timeoutInMiliseconds = 1500)
        {
            await elementHandle.HoverAsync();
            await page.WaitASecond();
            await page.Wait(waitSelector, WaitForSelectorState.Visible, timeoutInMiliseconds);
            await page.WaitForLoadStateAsync(LoadState.NetworkIdle);
            await page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
        }

        public static async Task WaitLoad(this IPage page, LoadState loadState = LoadState.NetworkIdle)
        {
            await page.WaitForLoadStateAsync(loadState);
        }

        public static async Task Wait(this IPage page, int timeoutInMiliseconds = 1000)
        {
            await page.WaitForTimeoutAsync(timeoutInMiliseconds);
        }

        public static async Task WaitASecond(this IPage page)
        {
            await page.Wait(1000);
        }

        public static async Task WaitMillisecond(this IPage page, int millisecond)
        {
            await page.Wait(millisecond);
        }

        public static async Task WaitHalfSecond(this IPage page)
        {
            await page.Wait(500);
        }

        public static async Task WaitQuaterSecond(this IPage page)
        {
            await page.Wait(250);
        }

        public static async Task<byte[]> Screenshot(this IPage page, string name)
        {
            if (StringExtensions.IsNullOrEmpty(name)) name = string.Empty;

            var path = $"/Screenshots/{name}-{DateTime.UtcNow:yyyy.MM.ddThh:mm:ss}";
            return await page.ScreenshotAsync(new PageScreenshotOptions
            {
                Type = ScreenshotType.Jpeg,
                Path = path,
                Quality = 100,
                // FullPage = true,
            });
        }

        public static async Task<bool> Click(this IPage page, string selector)
        {
            try
            {
                var clickableEle = await page.QuerySelectorAsync(selector);
                if (clickableEle != null)
                {
                    await page.DispatchEventAsync(selector, "click");
                    await page.WaitQuaterSecond();
                    // await page.ClickAsync(selector, new PageClickOptions
                    // {
                    //     Timeout = 3000
                    // });
                    return true;
                }
                else
                {
                    return false;
                }

            }
            catch (PlaywrightException pwe)
            {
                if (pwe.Message.Contains("Element is not visible") || pwe.Message.Contains("destroy"))
                {
                    await page.DispatchEventAsync(selector, "click");
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public static async Task<bool> Click(this IElementHandle element)
        {
            try
            {
                await element.ClickAsync(new ElementHandleClickOptions
                {
                    Force = true,
                    Timeout = 3000
                });

                return true;
            }
            catch (PlaywrightException pwe)
            {
                if (pwe.Message.Contains("Element is not visible") || pwe.Message.Contains("destroy"))
                {
                    await element.DispatchEventAsync("click");
                    return true;
                }

                throw pwe;
            }
        }

        public static async Task WaitForSelector(this IPage page, string selector, int timeout = 10000)
        {
            await page.WaitForSelectorAsync(selector, new PageWaitForSelectorOptions
            {
                Timeout = timeout
            });
        }
    }

    public static class PlaywrightHelper
    {
        public static async Task<PlaywrightContext> InitPersistentBrowser(CrawlConfig crawlConfig, AccountProxyItem accountProxy, bool headless = true, string userDataFolderName = "Test")
        {
            var browserLaunchOptions = new BrowserTypeLaunchPersistentContextOptions
            {
                Channel = "chrome",
                Timeout = 0,
                Headless = headless,
                // https://peter.sh/experiments/chromium-command-line-switches/
                Args = new List<string>
                {
                    "--start-maximized",
                    "--ignore-ssl-errors=yes",
                    "--ignore-certificate-errors",
                    "--disable-web-security",
                    "--allow-running-insecure-content",
                    "--disable-blink-features=AutomationControlled",
                    // "--auto-open-devtools-for-tabs",
                    "--disable-popup-blocking",
                    "--log-level=3",
                    "--disable-notifications"
                },
                IgnoreDefaultArgs = new List<string>
                {
                    "--enable-automation",
                },
                // SlowMo = 100,
                Locale = "en-US",
                ViewportSize = ViewportSize.NoViewport,
            };

            if (accountProxy is { proxy: { } })
            {
                browserLaunchOptions.Proxy = new Proxy
                {
                    Server = $"{accountProxy.proxy.ip}:{accountProxy.proxy.port}",
                    Username = accountProxy.proxy.username,
                    Password = accountProxy.proxy.password,
                };
            }

            var playwright = await Playwright.CreateAsync();
            string userDataDir;
            if (accountProxy != null)
            {
                userDataDir = $"{crawlConfig.UserDataDirRoot}/{accountProxy.account.username}";
            }
            else
            {
                userDataDir = $"{crawlConfig.UserDataDirRoot}/{userDataFolderName}";
            }
            var browserContext = await playwright.Chromium.LaunchPersistentContextAsync(userDataDir, browserLaunchOptions);

            return new PlaywrightContext
            {
                Playwright = playwright,
                // Browser = browserContext.Browser,
                BrowserContext = browserContext
            };
        }

        public static async Task<PlaywrightContext> InitBrowser(ApiProxy proxy = null)
        {
            var browserLaunchOptions = new BrowserTypeLaunchOptions
            {
                Channel = "chrome",
                Timeout = 0,
                Headless = false,
                // https://peter.sh/experiments/chromium-command-line-switches/
                // Args = new List<string>
                // {
                //     "--start-maximized",
                //     "--ignore-ssl-errors=yes",
                //     "--ignore-certificate-errors",
                //     "--disable-web-security",
                //     "--allow-running-insecure-content",
                //     "--disable-blink-features=AutomationControlled",
                //     "--auto-open-devtools-for-tabs",
                //     "--disable-popup-blocking",
                //     "--log-level=3",
                // },
                // IgnoreDefaultArgs = new List<string>
                // {
                //     "--enable-automation",
                // },
                // SlowMo = 50,
            };
            if (proxy != null)
            {
                browserLaunchOptions.Proxy = new Proxy
                {
                    Server = $"{proxy.ip}:{proxy.port}",
                    Username = proxy.username,
                    Password = proxy.password,
                };
            }

            var playwright = await Playwright.CreateAsync();
            var browser = await playwright.Chromium.LaunchAsync(browserLaunchOptions);
            var browserContext = await browser.NewContextAsync(new BrowserNewContextOptions
            {
                Locale = "en-US",
                // UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/74.0.3729.169 Safari/537.36"
                // UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Safari/537.36"
                // UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/92.0.4515.131 Safari/537.36"
            });

            return new PlaywrightContext
            {
                Playwright = playwright,
                Browser = browser,
                BrowserContext = browserContext
            };
        }

        public static bool IsCookieExpired(List<Cookie> cookies)
        {
            if (cookies.IsNullOrEmpty()) return true;

            var latestExpires = cookies.Where(_ => _.Expires.HasValue && _.Expires.Value > 0).Min(_ => _.Expires);
            var expirationDateTimeOffset = latestExpires.UnixTimeStampToDateTime();

            var isExpired = expirationDateTimeOffset <= DateTimeOffset.Now;
            return isExpired;
        }
    }
}