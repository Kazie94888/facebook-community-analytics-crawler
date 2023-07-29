using FacebookCommunityAnalytics.Crawler.NET.Client.Configurations;
using FacebookCommunityAnalytics.Crawler.NET.Client.Entities;
using FacebookCommunityAnalytics.Crawler.NET.Console.Playwrights;
using Microsoft.Playwright;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FacebookCommunityAnalytics.Crawler.NET.Core.Extensions;

namespace FacebookCommunityAnalytics.Crawler.NET.Console.Services
{
    public class FacebookLoginService
    {
        private readonly string _rootPath = Directory.GetCurrentDirectory();
        private readonly CrawlConfig _crawlConfig;

        public FacebookLoginService(CrawlConfig config)
        {
            _crawlConfig = config;
        }

        #region LOGIN

        // stop if failed to enter 2 times
        public async Task<CrawlResult> EnterLoginInfo(IPage page, AccountProxyItem accountProxy)
        {
            var account = accountProxy.account;
            var elementHandleTypeOptions = new ElementHandleTypeOptions
            {
                Delay = _crawlConfig.TypingDelay
            };

            var loginEmailElement = await page.QuerySelectorAsync("input#m_login_email");
            if (loginEmailElement != null) await loginEmailElement.TypeAsync(account.username, elementHandleTypeOptions);
            var loginPasswordElement = await page.QuerySelectorAsync("input#m_login_password");
            if (loginPasswordElement != null) await loginPasswordElement.TypeAsync(account.password, elementHandleTypeOptions);
            else
            {
                loginPasswordElement = await page.QuerySelectorAsync("input[name='pass']");
                if (loginPasswordElement != null) await loginPasswordElement.TypeAsync(account.password, elementHandleTypeOptions);
            }

            await page.WaitForTimeoutAsync(_crawlConfig.ActionDelay);

            await page.Click("[name='login']");
            await page.WaitASecond();
            await page.WaitForLoadStateAsync(LoadState.NetworkIdle);

            var loginApproval = await page.QuerySelectorAsync("#checkpoint_title");
            if (loginApproval != null)
            {
                var loginApprovalText = await loginApproval.InnerTextAsync();
                if (GenericExtensions.IsIn(loginApprovalText,"Cần phê duyệt đăng nhập", "Login approval needed"))
                {
                    return new CrawlResult(CrawlStatus.LoginApprovalNeeded);
                }
            }
            
            var loginBtn = await page.QuerySelectorAsync("button[type='submit'][value='OK']");
            if (loginBtn != null)
            {
                await page.Click("button[type='submit'][value='OK']");
            }
            else
            {
                loginBtn = await page.QuerySelectorAsync("input[type='submit']");
                if (loginBtn != null)
                {
                    await page.Click("input[type='submit']");
                }
            }

            await page.WaitASecond();
            await page.WaitForLoadStateAsync(LoadState.NetworkIdle);
            
            var loginWithOneClickForm = await page.QuerySelectorAsync("form[action='/login/device-based/update-nonce/']");
            if (loginWithOneClickForm != null)
            {
                var isOKClicked = await page.Click("input[value='OK'][type='submit']");
                if (isOKClicked)
                {
                    await page.WaitASecond();
                    await page.WaitForLoadStateAsync(LoadState.NetworkIdle);
                }
            }

            return new CrawlResult(CrawlStatus.OK);
        }

        public async Task<CrawlResult> Login(IBrowserContext browserContext, IPage page, AccountProxyItem accountProxy)
        {
            var loginResult = new CrawlResult(CrawlStatus.OK);
            
            var account = accountProxy.account;
            var clickOptions = new ElementHandleClickOptions
            {
                Delay = 250,
                Timeout = 0
            };

            var elementHandleTypeOptions = new ElementHandleTypeOptions
            {
                Delay = _crawlConfig.TypingDelay
            };

            // var cookies = GetCookies(accountProxy);
            // if (cookies != null && !PlaywrightHelper.IsCookieExpired(cookies))
            // {
            //     await browserContext.AddCookiesAsync(cookies);
            //     await page.GotoAsync(_crawlConfig.RootUrl);
            //
            //     var checkPicResponse = await CheckProfilePicAndSaveCookies(browserContext, page, accountProxy);
            //     if (checkPicResponse.Success)
            //     {
            //         return checkPicResponse;
            //     }
            // }

            await page.GotoAsync(_crawlConfig.RootUrl);
            await page.WaitForLoadStateAsync(LoadState.NetworkIdle);

            var checkPicResponse2nd = await CheckProfilePicAndSaveCookies(browserContext, page, accountProxy);
            if (checkPicResponse2nd.Success || checkPicResponse2nd.Status == CrawlStatus.AccountBanned)
            {
                return checkPicResponse2nd;
            }

            var elementsRequiredLoginAgain = await page.QuerySelectorAllAsync("div[data-sigil='login_profile_form']");
            if (elementsRequiredLoginAgain.Count > 1)
            {
               await elementsRequiredLoginAgain[1].ClickAsync();
               await page.WaitASecond();
               loginResult = await EnterLoginInfo(page, accountProxy);
               if (!loginResult.Success) return loginResult; 
               
               var touchLoginButton = await page.QuerySelectorAllAsync("button[data-sigil='touchable password_login_button']");
               if ( touchLoginButton.Count > 0)
               {
                   await touchLoginButton[0].ClickAsync();

                   var touchButton = await page.QuerySelectorAsync("button[data-sigil='touchable']");
                   if (touchButton != null) await touchButton.ClickAsync();
               }
            }
            
            var ele_HiddenAccounts = await page.QuerySelectorAllAsync($"input[name='uid'][value='{account.username}']");
            if (ele_HiddenAccounts.Any())
            {
                var processLoggedFlowResponse = await ProcessLoggedFlow(browserContext, page, accountProxy, ele_HiddenAccounts.ToList());
                if (processLoggedFlowResponse.Success) return new CrawlResult();
            }

            loginResult = await EnterLoginInfo(page, accountProxy);
            if (!loginResult.Success) return loginResult; 

            int loginFailureCount = 0;
            int approvalCodeFailureCount = 0;
            // var selector_ButtonSendCode = "td#checkpointSubmitButton";
            var selector_ButtonSendCode = "#checkpointSubmitButton-actual-button";
            while (true)
            {
                continueWhile: ;
                // await page.WaitForLoadStateAsync(LoadState.NetworkIdle);
                await page.WaitForLoadStateAsync(LoadState.Load);
                await page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
                await page.WaitASecond();
                
                if (!await page.Wait(selector_ButtonSendCode, timeoutInMs: 2000))
                {
                    // xpath=html/body/div[1]/div/div[1]/div/div/div[2]/div/article
                    var ele_UnsupportedArticle = await page.QuerySelectorAsync("xpath=html/body/div[1]/div/div[1]/div/div/div[2]/div/article");
                    var ele_ButFirstMustLogin = await page.QuerySelectorAsync("xpath=/html/body/div[1]/div/div[2]/div[1]/div/div[2]/div/div[2]/span");
                    // if (ele_UnsupportedArticle != null && ele_ButFirstMustLogin != null)
                    if (ele_ButFirstMustLogin != null)
                    {
                        loginFailureCount++;
                        if (loginFailureCount >= 3) break;

                        loginResult = await EnterLoginInfo(page, accountProxy);
                        if (!loginResult.Success) return loginResult; 
                        goto continueWhile;
                    }
                    else
                    {
                        break;
                    }
                }

                if (await page.Wait("#approvals_code"))
                {
                    approvalCodeFailureCount++;
                    if (approvalCodeFailureCount >= 5) break;

                    var approvePasscodeElement = await page.QuerySelectorAsync("#approvals_code");
                    if (approvePasscodeElement != null)
                    {
                        //  var code = TwoFAService.Get(_crawlConfig.ApiGet2FACodeUrl, account.twoFactorCode);
                        var code = TwoFAService.Get(account.twoFactorCode);
                        await approvePasscodeElement.TypeAsync(code, elementHandleTypeOptions);
                    }
                }

                if (await page.Wait("input[value='save_device']"))
                {
                    var ele_save_device = await page.QuerySelectorAsync("input[value='save_device']");
                    if (ele_save_device != null)
                    {
                        await ele_save_device.ClickAsync(null);
                    }
                }

                if (await page.Wait("input[value='code_gen_instruction']"))
                {
                    var ele_code_gen_instruction = await page.QuerySelectorAsync("input[value='code_gen_instruction']");
                    if (ele_code_gen_instruction != null)
                    {
                        await ele_code_gen_instruction.ClickAsync(null);
                    }
                }

                var ele_ButtonSendCode = await page.QuerySelectorAsync(selector_ButtonSendCode);
                await ele_ButtonSendCode.ClickAsync(clickOptions);
            }

            var result = await CheckProfilePicAndSaveCookies(browserContext, page, accountProxy);
            return result;
        }

        private async Task<CrawlResult> ProcessLoggedFlow(
            IBrowserContext browserContext
            , IPage page
            , AccountProxyItem accountProxy
            , List<IElementHandle> ele_HiddenAccounts
        )
        {
            IElementHandle? currentAccountSelection = null;

            foreach (var ele in ele_HiddenAccounts)
            {
                var valueAtt = await ele.GetAttributeAsync("value");
                if (valueAtt != null && valueAtt == accountProxy.account.username)
                {
                    // get parent and click
                    currentAccountSelection = await ele.QuerySelectorAsync("xpath=..");
                    if (currentAccountSelection != null) break;
                }
            }

            var automationResponse = new CrawlResult(CrawlStatus.LoginFailed);

            // if (false)
            if (currentAccountSelection != null)
            {
                var isProfilePicClicked = await page.Click("input[type='image']");
                // var clickableSelector = "div[class='_mDeviceLoginHomepage__userNameAndBadge'][role='button']";
                // var isClickable = await page.Click("");
                if (isProfilePicClicked)
                {
                    var passwordInput = await page.QuerySelectorAsync("input[type='password'][name='pass']");
                    if (passwordInput != null)
                    {
                        return automationResponse;
                    }
                    automationResponse = await CheckProfilePicAndSaveCookies(browserContext, page, accountProxy);
                }
            }
            else
            {
                var loginIntoAnotherAccountClickable = await page.Click(@"a[href*='/login/?next'] div");
                if (loginIntoAnotherAccountClickable)
                {
                    await EnterLoginInfo(page, accountProxy);
                    automationResponse = await CheckProfilePicAndSaveCookies(browserContext, page, accountProxy);
                }
            }

            return automationResponse;
        }

        private async Task<CrawlResult> CheckProfilePicAndSaveCookies(IBrowserContext browserContext, IPage page, AccountProxyItem accountProxy)
        {
            var ele_profile_tab_jewel = await page.QuerySelectorAsync("div#profile_tab_jewel");
            var ele_basicComposer = await page.QuerySelectorAsync("form[action*='/composer']");
            if (ele_profile_tab_jewel != null || ele_basicComposer != null)
            {
                // var newCookies = await browserContext.CookiesAsync();
                // SaveCookies
                // (
                //     accountProxy,
                //     newCookies.Select
                //         (
                //             _ => new Cookie
                //             {
                //                 Domain = _.Domain,
                //                 Expires = _.Expires,
                //                 Name = _.Name,
                //                 Path = _.Path,
                //                 Secure = _.Secure,
                //                 Value = _.Value,
                //                 SameSite = _.SameSite,
                //                 HttpOnly = _.HttpOnly
                //             }
                //         )
                //         .ToList()
                // );

                return new CrawlResult(CrawlStatus.OK);
            }

            if (await IsAccountBanned(page))
            {
                return new CrawlResult(CrawlStatus.AccountBanned);
            }

            return new CrawlResult(CrawlStatus.LoginFailed);
        }

        public async Task<bool> IsAccountBanned(IPage page)
        {
            // var bannedNotiElementVi = await page.QuerySelectorAsync("//span[contains(., 'bị khóa')]");
            // var bannedNotiElementVi2 = await page.QuerySelectorAsync("//span[contains(., 'mở khóa tài khoản')]");
            // var bannedNotiElementEn = await page.QuerySelectorAsync("//span[contains(., 'Has Been Locked')]");
            // var bannedNotiElementEn2 = await page.QuerySelectorAsync("//span[contains(., 'unlock your account')]");
            // var bannedNotiElementEn3 = await page.QuerySelectorAsync("//span[contains(., 'has been locked')]");
            // var bannedNotiElementEn4 = await page.QuerySelectorAsync("//span[contains(., 'unlock your account')]");

            
            var selector_banned =
                "//span[contains(., 'bị khóa') or contains(., 'mở khóa tài khoản') or contains(., 'Has Been Locked') or contains(., 'unlock your account') or contains(., 'has been locked') or contains(., 'unlock your account')]";
            try
            {
                await page.WaitForSelectorAsync(selector_banned, new PageWaitForSelectorOptions
                {
                    Timeout = 3000
                });

                var selector_Feed = "//div[@role='feed']";
                var ele_Feed = await page.QuerySelectorAsync(selector_Feed);

                var selector_Message = "//div[@data-ad-comet-preview='message']";
                var ele_Message      = await page.QuerySelectorAsync(selector_Message);
                if (ele_Feed != null || ele_Message != null)
                {
                    return false;
                }
                // var isLocked = (bannedNotiElementVi != null && bannedNotiElementVi2 != null) || 
                //                (bannedNotiElementEn != null && bannedNotiElementEn2 != null) ||
                //                (bannedNotiElementEn3 != null && bannedNotiElementEn4 != null)
                //     ;
                // if (isLocked)
                // {
                //     return true;
                // }

                return true;
            }
            catch (Exception e)
            {
                // ignored
            }

            // account maybe is banned, update api
            var formCheckPoint = await page.QuerySelectorAsync(FacebookConsts.Selector_CheckpointForm);
            if (formCheckPoint != null)
            {
                var buttonRequestAReviewEn = await page.QuerySelectorAsync(FacebookConsts.Selector_RequestAReviewButtonEn);
                var buttonRequestAReviewVi = await page.QuerySelectorAsync(FacebookConsts.Selector_RequestAReviewButtonVi);
                if (buttonRequestAReviewEn != null || buttonRequestAReviewVi != null)
                {
                    return true;
                }
            }

            return false;
        }

        private void SaveCookies(AccountProxyItem accountProxy, IReadOnlyCollection<Cookie> allCookies)
        {
            var directory = $"{_rootPath}\\Cookies";
            if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);
            File.WriteAllText($"{_rootPath}\\Cookies\\{GetCookieName(accountProxy)}.json", JsonConvert.SerializeObject(allCookies));
        }

        private List<Cookie> GetCookies(AccountProxyItem accountProxy)
        {
            try
            {
                var content = File.ReadAllText($"{_rootPath}\\Cookies\\{GetCookieName(accountProxy)}.json");
                return JsonConvert.DeserializeObject<List<Cookie>>(content);
            }
            catch (Exception)
            {
                return null;
            }
        }

        private string GetCookieName(AccountProxyItem accountProxy)
        {
            // return $"{accountProxy.account?.username}-{accountProxy.proxy?.ip}";
            return accountProxy.account?.username;
        }

        #endregion
    }
}