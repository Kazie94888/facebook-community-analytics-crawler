using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using FacebookCommunityAnalytics.Crawler.NET.Client.Clients;
using FacebookCommunityAnalytics.Crawler.NET.Client.Configurations;
using FacebookCommunityAnalytics.Crawler.NET.Client.Entities;
using FacebookCommunityAnalytics.Crawler.NET.Client.Enums;
using FacebookCommunityAnalytics.Crawler.NET.Console.Playwrights;
using FacebookCommunityAnalytics.Crawler.NET.Core.Helpers;
using MailKit.Net.Pop3;
using Microsoft.Playwright;

namespace FacebookCommunityAnalytics.Crawler.NET.Console.Services.HotMailServices
{
    public class EmailService
    {
        private readonly GlobalConfig _globalConfig;
        private readonly ApiClient _apiClient;
        private readonly ApiEmailClient _apiEmailClient;
        private ViotpService _viotpService;
        
        public EmailService(GlobalConfig globalConfig)
        {
            _globalConfig = globalConfig;
            _apiClient = new ApiClient(globalConfig.ApiConfig);
            _apiEmailClient = new ApiEmailClient(globalConfig.ApiConfig);
            _viotpService = new ViotpService(globalConfig);
        }

        public async Task UnlockEmails()
        {
            var accountProxies = _apiClient.Crawl.GetAccounts(new GetAccountsRequest
            {
                AccountStatus = AccountStatus.Banned
            });

            // Clean userData folder name
            // var userDataDir = $"{_globalConfig.CrawlConfig.UserDataDirRoot}/Email";
            // var dir = new DirectoryInfo(userDataDir);
            // if (dir.Exists)
            // {
            //     dir.Delete(true);
            // }

            int count = 1;
            foreach (var accountDto in accountProxies.Resource.Where(dto => !string.IsNullOrWhiteSpace(dto.Email)))
            {
                var email = $"{accountDto.Username}@gdll.vn";
                count += 1;
                var browserContext =
                    await PlaywrightHelper.InitPersistentBrowser(_globalConfig.CrawlConfig, null, false,
                        $"Email/{accountDto.Email}");
                using (browserContext.Playwright)
                {
                    await using (browserContext.Browser)
                    {
                        var page = await browserContext.BrowserContext.NewPageAsync();

                        try
                        {
                            await page.GotoAsync("https://account.microsoft.com");
                            await page.WaitForLoadStateAsync(LoadState.Load);
                            await page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);

                            try
                            {
                                var selector_SignIn = "//div[@id='mectrl_headerPicture']";
                                await page.WaitForSelector(selector_SignIn);
                                var ele_SignIn = await page.QuerySelectorAsync(selector_SignIn);
                                await ele_SignIn.ClickAsync();
                            }
                            catch (Exception e)
                            {
                                // ignored
                            }

                            await page.WaitForLoadStateAsync(LoadState.Load);
                            await page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);

                            var elementHandleTypeOptions = new ElementHandleTypeOptions
                            {
                                Delay = _globalConfig.CrawlConfig.TypingDelay
                            };

                            string selector_Email;
                            try
                            {
                                selector_Email = $"//span[text()='{accountDto.Email}']";
                                await page.WaitForSelector(selector_Email, 5000);

                                await UnlockFBAccount(page, accountDto, elementHandleTypeOptions, false);
                                continue;
                            }
                            catch (Exception e)
                            {
                                // ignored
                            }

                            string selector_Next;
                            IElementHandle ele_Next;
                            IElementHandle ele_Email;
                            try
                            {
                                selector_Email = "//input[@type='email']";
                                await page.WaitForSelector(selector_Email);
                                ele_Email = await page.QuerySelectorAsync(selector_Email);
                                await ele_Email.TypeAsync(accountDto.Email, elementHandleTypeOptions);

                                selector_Next = "//input[@type='submit']";
                                ele_Next = await page.QuerySelectorAsync(selector_Next);
                                await ele_Next.ClickAsync();
                            }
                            catch (Exception e)
                            {
                                // ignored
                            }


                            var selector_Password = "//input[@type='password']";
                            await page.WaitForSelector(selector_Password);
                            await page.Wait(1000);
                            var ele_Password = await page.QuerySelectorAsync(selector_Password);
                            await ele_Password.TypeAsync(accountDto.EmailPassword, elementHandleTypeOptions);

                            var selector_SignInBtn = "//input[@value='Sign in']";
                            await page.WaitForSelector(selector_SignInBtn);
                            var ele_SignInBtn = await page.QuerySelectorAsync(selector_SignInBtn);
                            await ele_SignInBtn.ClickAsync();
                            await page.Wait(2000);

                            var selector_Header = "//div[@Id='StartHeader']";
                            var element_Header = await page.QuerySelectorAsync(selector_Header);
                            if (element_Header != null)
                            {
                                var text = await element_Header.InnerTextAsync();
                                if (text.Contains("Your account has been locked"))
                                {
                                    selector_Next = "//input[@value='Next']";
                                    await page.WaitForSelector(selector_Next);
                                    ele_Next = await page.QuerySelectorAsync(selector_Next);
                                    await ele_Next.ClickAsync();
                                    await page.WaitForLoadStateAsync(LoadState.Load);
                                    await page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);

                                    var selector_Option = "//select[@aria-label='Country code']";
                                    await page.WaitForSelector(selector_Option);
                                    var element_Option = await page.QuerySelectorAsync(selector_Option);
                                    await element_Option.SelectOptionAsync(new[] {"VN"});

                                    var phoneNumberDetails = _viotpService.RegisterPhoneNumber();
                                    var phoneNumber = phoneNumberDetails.phone_number;
                                    // Wait 30 secs for new mobile number
                                    await page.Wait(10000);

                                    var selector_PhoneNumber = "//label[text()='Phone number']/../input";
                                    var ele_PhoneNumber = await page.QuerySelectorAsync(selector_PhoneNumber);
                                    await ele_PhoneNumber.TypeAsync(phoneNumber, elementHandleTypeOptions);

                                    var selector_SendCode = "//a[text()='Send code']";
                                    await page.WaitForSelector(selector_SendCode);
                                    var ele_SendCode = await page.QuerySelectorAsync(selector_SendCode);
                                    await ele_SendCode.ClickAsync();

                                    var selector_AccessCode = "//input[@aria-label='Enter the access code']";
                                    await page.WaitForSelector(selector_AccessCode);

                                    // Receive code from mobile phone
                                    var smsContent = _viotpService.GetPhoneCode(phoneNumberDetails.request_id);
                                    var ele_AccessCode = await page.QuerySelectorAsync(selector_AccessCode);
                                    await ele_AccessCode.TypeAsync(smsContent, elementHandleTypeOptions);

                                    var selector_Submit = "//input[@value='Submit']";
                                    var ele_Submit = await page.QuerySelectorAsync(selector_Submit);
                                    await ele_Submit.ClickAsync();

                                    var selector_UnlockAccount = "//div[@id='finishHeader']";
                                    await page.WaitForSelector(selector_UnlockAccount);

                                    var selector_Continues = "//input[@id='FinishAction']";
                                    var ele_Continues = await page.QuerySelectorAsync(selector_Continues);
                                    await ele_Continues.ClickAsync();

                                    var selector_EmailProtect = "//input[@id='EmailAddress']";
                                    await page.WaitForSelector(selector_EmailProtect);
                                    var ele_EmailProtect = await page.QuerySelectorAsync(selector_EmailProtect);
                                    await ele_EmailProtect.TypeAsync(email, elementHandleTypeOptions);

                                    var selector_NextBlock = "//input[@id='iNext']";
                                    var ele_NextBlock = await page.QuerySelectorAsync(selector_NextBlock);
                                    await ele_NextBlock.ClickAsync();

                                    // Read code from email dev@veek.vn
                                    var securityCode = GetSecurityCode();

                                    var selector_Code = "//input[@id='iOttText']";
                                    await page.WaitForSelector(selector_Code);
                                    var ele_Code = await page.QuerySelectorAsync(selector_Code);
                                    await ele_Code.TypeAsync(securityCode, elementHandleTypeOptions);

                                    selector_NextBlock = "//input[@id='iNext']";
                                    ele_NextBlock = await page.QuerySelectorAsync(selector_NextBlock);
                                    await ele_NextBlock.ClickAsync();
                                    
                                    try
                                    {
                                        var selector_emailRadio = "//input[@id='iProof0']";
                                        await page.WaitForSelector(selector_emailRadio, 5000);
                                        var ele_emailRadio = await page.QuerySelectorAsync(selector_emailRadio);
                                        await ele_emailRadio.ClickAsync();

                                        selector_Email = "//input[@id='iProofEmail']";
                                        ele_Email = await page.QuerySelectorAsync(selector_Email);
                                        await ele_Email.TypeAsync(accountDto.Username, elementHandleTypeOptions);

                                        selector_SendCode = "//input[@id='iSelectProofAction']";
                                        ele_SendCode = await page.QuerySelectorAsync(selector_SendCode);
                                        ele_SendCode.ClickAsync();
                                            
                                        await page.Wait(5000);
                                        securityCode = GetSecurityCode();

                                        selector_Code = "//input[@id='iOttText']";
                                        await page.WaitForSelector(selector_Code);
                                        ele_Code = await page.QuerySelectorAsync(selector_Code);
                                        await ele_Code.TypeAsync(securityCode, elementHandleTypeOptions);

                                        selector_Next = "//input[@id='iVerifyCodeAction']";
                                        ele_Next = await page.QuerySelectorAsync(selector_Next);
                                        await ele_Next.ClickAsync();
                                    }
                                    catch (Exception e)
                                    {
                                        // ignored
                                    }

                                    await UnlockFBAccount(page, accountDto, elementHandleTypeOptions);
                                }
                            }
                            else
                            {
                                var selector_Verify = "//input[@value='Verify']";
                                var element_Verify = await page.QuerySelectorAsync(selector_Verify);
                                if (element_Verify != null)
                                {
                                    await element_Verify.ClickAsync();
                                    await page.Wait(1000);
                                    if (await IsRecoveryManually(page))
                                    {
                                        await RecoverWithGdllEmail(page, accountDto, elementHandleTypeOptions);
                                    }
                                    else
                                    {
                                        var selector_Option = "//select[@aria-label='Country/region']";
                                        var element_Option = await page.QuerySelectorAsync(selector_Option);
                                        
                                        await element_Option.SelectOptionAsync(new[] {"VN"});

                                        var phoneNumberDetails = _viotpService.RegisterPhoneNumber();
                                        var phoneNumber = phoneNumberDetails.phone_number;
                                        // Wait 30 secs for new mobile number
                                        await page.Wait(10000);

                                        var selector_PhoneNumber = "//input[@id='DisplayPhoneNumber']";
                                        var ele_PhoneNumber = await page.QuerySelectorAsync(selector_PhoneNumber);
                                        await ele_PhoneNumber.TypeAsync(phoneNumber, elementHandleTypeOptions);

                                        selector_Next = "//input[@value='Next']";
                                        ele_Next = await page.QuerySelectorAsync(selector_Next);
                                        await ele_Next.ClickAsync();

                                        // Get Code
                                        // Receive code from mobile phone
                                        var smsContent = _viotpService.GetPhoneCode(phoneNumberDetails.request_id);
                                        // Input Code
                                        var selector_Code = "//input[@id='iOttText' and @aria-label='Code']";
                                        await page.WaitForSelector(selector_Code);
                                        var element_Code = await page.QuerySelectorAsync(selector_Code);
                                        await element_Code.TypeAsync(smsContent, elementHandleTypeOptions);
                                        
                                        selector_Next = "//input[@id='iVerifyPhoneViewAction']";
                                        ele_Next = await page.QuerySelectorAsync(selector_Next);
                                        await ele_Next.ClickAsync();

                                        selector_Password = "//input[@type='password']";
                                        await page.WaitForSelector(selector_Password);
                                        ele_Password = await page.QuerySelectorAsync(selector_Password);

                                        var password = PasswordService.Generate(8, 1);
                                        await ele_Password.TypeAsync(password, elementHandleTypeOptions);

                                        selector_Next = "//input[@id='iPasswordViewAction']";
                                        ele_Next = await page.QuerySelectorAsync(selector_Next);
                                        await ele_Next.ClickAsync();

                                        var selector_ReviewSecurity = "//div[@id='iPageTitle']";
                                        await page.WaitForSelector(selector_ReviewSecurity);

                                        // update New password
                                        accountDto.EmailPassword = password;

                                        _apiClient.Account.Update(Guid.Parse(accountDto.Id),
                                            accountDto.ToAccountUpdateDto());

                                        selector_Next = "//input[@id='iReviewProofsViewAction']";
                                        ele_Next = await page.QuerySelectorAsync(selector_Next);
                                        await ele_Next.ClickAsync();

                                        selector_Email = "//input[@placeholder='someone@example.com']";
                                        await page.WaitForSelector(selector_Email);
                                        ele_Email = await page.QuerySelectorAsync(selector_Email);
                                        await ele_Email.TypeAsync(email, elementHandleTypeOptions);

                                        selector_Next = "//input[@id='iCollectProofsViewAction']";
                                        ele_Next = await page.QuerySelectorAsync(selector_Next);
                                        await ele_Next.ClickAsync();
                                        
                                        selector_Next = "//input[@id='iFinishViewAction']";
                                        await page.WaitForSelector(selector_Next);
                                        ele_Next = await page.QuerySelectorAsync(selector_Next);
                                        await ele_Next.ClickAsync();
                                        
                                        selector_Password = "//input[@type='password']";
                                        await page.WaitForSelector(selector_Password);
                                        ele_Password = await page.QuerySelectorAsync(selector_Password);
                                        await ele_Password.TypeAsync(password, elementHandleTypeOptions);
                                        
                                        selector_SignInBtn = "//input[@value='Sign in']";
                                        await page.WaitForSelector(selector_SignInBtn);
                                        ele_SignInBtn = await page.QuerySelectorAsync(selector_SignInBtn);
                                        await ele_SignInBtn.ClickAsync();
                                        await page.Wait(2000);
                                        
                                        
                                        try
                                        {
                                            var selector_emailRadio = "//input[@id='iProof0']";
                                            await page.WaitForSelector(selector_emailRadio, 5000);
                                            var ele_emailRadio = await page.QuerySelectorAsync(selector_emailRadio);
                                            await ele_emailRadio.ClickAsync();

                                            selector_Email = "//input[@id='iProofEmail']";
                                            ele_Email = await page.QuerySelectorAsync(selector_Email);
                                            await ele_Email.TypeAsync(accountDto.Username, elementHandleTypeOptions);

                                            var selector_SendCode = "//input[@id='iSelectProofAction']";
                                            var ele_SendCode = await page.QuerySelectorAsync(selector_SendCode);
                                            await ele_SendCode.ClickAsync();
                                                
                                            await page.Wait(5000);
                                            var securityCode = GetSecurityCode();

                                            selector_Code = "//input[@id='iOttText']";
                                            await page.WaitForSelector(selector_Code);
                                            var ele_Code = await page.QuerySelectorAsync(selector_Code);
                                            await ele_Code.TypeAsync(securityCode, elementHandleTypeOptions);

                                            selector_Next = "//input[@id='iVerifyCodeAction']";
                                            ele_Next = await page.QuerySelectorAsync(selector_Next);
                                            await ele_Next.ClickAsync();
                                        }
                                        catch (Exception e)
                                        {
                                            // ignored
                                        }


                                        await UnlockFBAccount(page, accountDto, elementHandleTypeOptions);
                                    }

                                }
                                else
                                {
                                    var selector_SecurityOptions = "//select[@id='iProofOptions']";
                                    var ele_SecurityOptions = await page.QuerySelectorAsync(selector_SecurityOptions);
                                    if (ele_SecurityOptions != null)
                                    {
                                        await ele_SecurityOptions.SelectOptionAsync(new[] {"Email"});

                                        selector_Email = "//input[@type='email']";
                                        await page.WaitForSelector(selector_Email);
                                        ele_Email = await page.QuerySelectorAsync(selector_Email);
                                        await ele_Email.TypeAsync(email, elementHandleTypeOptions);

                                        selector_Next = "//input[@id='iNext']";
                                        ele_Next = await page.QuerySelectorAsync(selector_Next);
                                        await ele_Next.ClickAsync();

                                        
                                        await page.Wait(5000);
                                        var securityCode = GetSecurityCode();

                                        var selector_Code = "//input[@id='iOttText']";
                                        await page.WaitForSelector(selector_Code);
                                        var ele_Code = await page.QuerySelectorAsync(selector_Code);
                                        await ele_Code.TypeAsync(securityCode, elementHandleTypeOptions);

                                        selector_Next = "//input[@id='iNext']";
                                        ele_Next = await page.QuerySelectorAsync(selector_Next);
                                        await ele_Next.ClickAsync();

                                        try
                                        {
                                            var selector_emailRadio = "//input[@id='iProof0']";
                                            await page.WaitForSelectorAsync(selector_emailRadio);
                                            var ele_emailRadio = await page.QuerySelectorAsync(selector_emailRadio);
                                            await ele_emailRadio.ClickAsync();

                                            selector_Email = "//input[@id='iProofEmail']";
                                            ele_Email = await page.QuerySelectorAsync(selector_Email);
                                            await ele_Email.TypeAsync(accountDto.Username, elementHandleTypeOptions);

                                            var selector_SendCode = "//input[@id='iSelectProofAction']";
                                            var ele_SendCode = await page.QuerySelectorAsync(selector_SendCode);
                                            await ele_SendCode.ClickAsync();
                                                
                                            await page.Wait(5000);
                                            securityCode = GetSecurityCode();

                                            selector_Code = "//input[@id='iOttText']";
                                            await page.WaitForSelector(selector_Code);
                                            ele_Code = await page.QuerySelectorAsync(selector_Code);
                                            await ele_Code.TypeAsync(securityCode, elementHandleTypeOptions);

                                            selector_Next = "//input[@id='iVerifyCodeAction']";
                                            ele_Next = await page.QuerySelectorAsync(selector_Next);
                                            await ele_Next.ClickAsync();
                                        }
                                        catch (Exception e)
                                        {
                                            // ignored
                                        }

                                        await UnlockFBAccount(page, accountDto, elementHandleTypeOptions);

                                    }
                                    else
                                    {
                                        if (await IsRecoveryManually(page))
                                        {
                                            await RecoverWithGdllEmail(page, accountDto, elementHandleTypeOptions);
                                        }
                                        else
                                        {
                                            var selector_ResetPassword = "//a[text()='reset it now.']";
                                            var ele_ResetPassword = await page.QuerySelectorAsync(selector_ResetPassword);
                                            if (ele_ResetPassword != null)
                                            {
                                                System.Console.WriteLine(
                                                    $"Invalid password {accountDto.Email} - {accountDto.EmailPassword}");
                                            }
                                            else
                                            {
                                                await UnlockFBAccount(page, accountDto, elementHandleTypeOptions);
                                            }
                                        }
                                    }
                                }

                            }
                        }
                        catch (Exception e)
                        {
                            System.Console.WriteLine(e);
                            await e.Log(string.Empty, string.Empty);
                            System.Console.WriteLine(
                                $"Error recovery {accountDto.Email} - {accountDto.EmailPassword}");
                            // throw;
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

        private async Task RecoverWithGdllEmail(IPage page, AccountDto accountDto,
            ElementHandleTypeOptions elementHandleTypeOptions)
        {
            var selector_hiddenEmail = "//span[contains(text(),'*****@gdll.vn')]";
            var ele_hiddenEmail = await page.QuerySelectorAsync(selector_hiddenEmail);
            if (ele_hiddenEmail != null)
            {
                var selector_emailRadio = "//input[@id='iProof0']";
                var ele_emailRadio = await page.QuerySelectorAsync(selector_emailRadio);
                await ele_emailRadio.ClickAsync();

                var selector_Email = "//input[@id='iProofEmail']";
                var ele_Email = await page.QuerySelectorAsync(selector_Email);
                await ele_Email.TypeAsync(accountDto.Username, elementHandleTypeOptions);

                var selector_SendCode = "//input[@id='iSelectProofAction']";
                var ele_SendCode = await page.QuerySelectorAsync(selector_SendCode);
                await ele_SendCode.ClickAsync();

                await page.Wait(5000);
                var securityCode = GetSecurityCode();

                var selector_Code = "//input[@id='iOttText']";
                await page.WaitForSelector(selector_Code);
                var ele_Code = await page.QuerySelectorAsync(selector_Code);
                await ele_Code.TypeAsync(securityCode, elementHandleTypeOptions);

                var selector_Next = "//input[@id='iVerifyCodeAction']";
                var ele_Next = await page.QuerySelectorAsync(selector_Next);
                await ele_Next.ClickAsync();

                await UnlockFBAccount(page, accountDto, elementHandleTypeOptions);
            }
            else
            {
                System.Console.WriteLine(
                    $"Need to recovery by manually {accountDto.Email} - {accountDto.EmailPassword}");
            }
        }

        private async Task UnlockFBAccount(IPage page, AccountDto accountDto, ElementHandleTypeOptions elementHandleTypeOptions, bool isClickSubmit = true)
        {
            await page.Wait(2000);

            try
            {
                var selector_Password = "//input[@id='iPassword']";
                await page.WaitForSelector(selector_Password);

                var ele_Password = await page.QuerySelectorAsync(selector_Password);
                var emailPassword = PasswordService.Generate(8, 1);
                await ele_Password.TypeAsync(emailPassword, elementHandleTypeOptions);
                var selector_Next = "//input[@id='iPasswordViewAction']";
                var ele_Next = await page.QuerySelectorAsync(selector_Next);
                await ele_Next.ClickAsync();

                var selector_ReviewSecurity = "//div[@id='iPageTitle']";
                await page.WaitForSelector(selector_ReviewSecurity);

                // update New password
                accountDto.EmailPassword = emailPassword;

                _apiClient.Account.Update(Guid.Parse(accountDto.Id),
                    accountDto.ToAccountUpdateDto());
                
                selector_Next = "//input[@id='iReviewProofsViewAction']";
                ele_Next = await page.QuerySelectorAsync(selector_Next);
                await ele_Next.ClickAsync();
                
                var selector_Option = "//select[@aria-label='Country/region']";
                var element_Option = await page.QuerySelectorAsync(selector_Option);

                await element_Option.SelectOptionAsync(new[] {"VN"});

                var phoneNumberDetails = _viotpService.RegisterPhoneNumber();
                var phoneNumber = phoneNumberDetails.phone_number;
                // Wait 30 secs for new mobile number
                await page.Wait(10000);

                var selector_PhoneNumber = "//input[@id='DisplayPhoneNumber']";
                var ele_PhoneNumber = await page.QuerySelectorAsync(selector_PhoneNumber);
                await ele_PhoneNumber.TypeAsync(phoneNumber, elementHandleTypeOptions);

                selector_Next = "//input[@value='Next']";
                ele_Next = await page.QuerySelectorAsync(selector_Next);
                await ele_Next.ClickAsync();

                selector_Next = "//input[@id='iFinishViewAction']";
                await page.WaitForSelector(selector_Next);
                ele_Next = await page.QuerySelectorAsync(selector_Next);
                await ele_Next.ClickAsync();
                                        
                selector_Password = "//input[@type='password']";
                await page.WaitForSelector(selector_Password);
                ele_Password = await page.QuerySelectorAsync(selector_Password);
                await ele_Password.TypeAsync(emailPassword, elementHandleTypeOptions);
                                        
                var selector_SignInBtn = "//input[@value='Sign in']";
                await page.WaitForSelector(selector_SignInBtn);
                var ele_SignInBtn = await page.QuerySelectorAsync(selector_SignInBtn);
                await ele_SignInBtn.ClickAsync();
                await page.Wait(2000);

                try
                {
                    var selector_YesBtn = "//input[@id='idSIButton9']";
                    await page.WaitForSelector(selector_YesBtn);
                    var ele_YesBtn = await page.QuerySelectorAsync(selector_YesBtn);
                    await ele_YesBtn.ClickAsync();
                    await page.Wait(2000); 
                }
                catch (Exception e)
                {
                    // ignored
                }
            }
            catch (Exception e1)
            {
                if (isClickSubmit)
                {
                    // Active email service
                    var selector_Submit = "//input[@type='submit']";
                    await page.WaitForSelector(selector_Submit);
                    var ele_selector = await page.QuerySelectorAsync(selector_Submit);
                    await ele_selector.ClickAsync();
                }


                string selector_Email;
                try
                {
                    selector_Email = $"//span[text()='{accountDto.Email}']";
                    await page.WaitForSelector(selector_Email);
                }
                catch (Exception e2)
                {
                    var selector_NoThanks = "//a[@id='iCancel']";
                    await page.WaitForSelector(selector_NoThanks);
                    var ele_NoThanks = await page.QuerySelectorAsync(selector_NoThanks);
                    await ele_NoThanks.ClickAsync();

                    selector_Email = $"//span[text()='{accountDto.Email}']";
                    await page.WaitForSelector(selector_Email);
                }
            }

            await page.GotoAsync("https://outlook.live.com/mail");
            
            try
            {
                var selector_CancelPopup = $"//span[text()='Cancel']";
                await page.WaitForSelector(selector_CancelPopup, 5000);
                var ele_CancelPopUp = await page.QuerySelectorAsync(selector_CancelPopup);
                await ele_CancelPopUp.ClickAsync();
            }
            catch (Exception exception)
            {
                // ignored
            }

            var selector_Favorites = "//span[text()='Folders' or text()='Thư mục']";
            await page.WaitForSelector(selector_Favorites);

            // Recovery facebook
            await page.GotoAsync("https://mbasic.facebook.com");
            
            var selector_emailFB = "//input[@id='m_login_email']";
            try
            {
                await page.WaitForSelector(selector_emailFB);
            }
            catch (Exception e)
            {
                try
                {
                    var selector_TextArea = "//textarea[@name='xc_message']";
                    await page.WaitForSelector(selector_TextArea);
                    // login success
                    accountDto.AccountStatus = AccountStatus.Active;
                    _apiClient.Account.Update(Guid.Parse(accountDto.Id), accountDto.ToAccountUpdateDto());
                    return;
                }
                catch (Exception exception)
                {
                    try
                    {
                        var selector_Input = "//input[@type='submit']";
                        var element_Input = await page.QuerySelectorAsync(selector_Input);
                        await element_Input.ClickAsync();

                        var selector_InputPass = "//input[@type='password']";
                        var element_InputPass = await page.QuerySelectorAsync(selector_InputPass);
                        await element_InputPass.TypeAsync(accountDto.Password, elementHandleTypeOptions);
                        
                        goto GETFBCODE;
                    }
                    catch (Exception e1)
                    {
                        var selector_NextFB1 = "//a/span/..";
                        await page.WaitForSelector(selector_NextFB1);
                        var ele_NextFB1 = await page.QuerySelectorAsync(selector_NextFB1);
                        await ele_NextFB1.ClickAsync();

                        selector_NextFB1 = "//a/span/..";
                        await page.WaitForSelector(selector_NextFB1);
                        ele_NextFB1 = await page.QuerySelectorAsync(selector_NextFB1);
                        await ele_NextFB1.ClickAsync();

                        goto GETFBCODE;
                    }
                }
            }

            var ele_emailFB = await page.QuerySelectorAsync(selector_emailFB);
            await ele_emailFB.TypeAsync(accountDto.Username, elementHandleTypeOptions);
            await page.WaitASecond();
            var selector_passwordFB = "//input[@name='pass']";
            var ele_passwordFB = await page.QuerySelectorAsync(selector_passwordFB);
            await ele_passwordFB.TypeAsync(accountDto.Password, elementHandleTypeOptions);
            await page.WaitASecond();
            var selector_LoginFB = "//input[@name='login']";
            var ele_LoginFB = await page.QuerySelectorAsync(selector_LoginFB);
            await ele_LoginFB.ClickAsync();
            await page.WaitASecond();
            var selector_ApproveCode = "//input[@id='approvals_code']";
            await page.WaitForSelector(selector_ApproveCode);
            var ele_ApproveCode =
                await page.QuerySelectorAsync(selector_ApproveCode);
            var code = TwoFAService.Get(accountDto.TwoFactorCode);
            await ele_ApproveCode.TypeAsync(code, elementHandleTypeOptions);

            var selector_SubmitCode =
                "//input[@id='checkpointSubmitButton-actual-button']";
            var ele_SubmitCode = await page.QuerySelectorAsync(selector_SubmitCode);
            await ele_SubmitCode.ClickAsync();

            await page.Wait(5000);
            ele_SubmitCode = await page.QuerySelectorAsync(selector_SubmitCode);
            if (ele_SubmitCode != null)
            {
                await ele_SubmitCode.ClickAsync();
            }

            var selector_GetStarted = "(//a[contains(@href,'unauthenticated')])[2]";
            try
            {
                await page.WaitForSelector(selector_GetStarted);
            }
            catch (Exception e)
            {
                var selector_TextArea = "//textarea[@name='xc_message']";
                await page.WaitForSelector(selector_TextArea);
                // login success
                accountDto.AccountStatus = AccountStatus.Active;
                _apiClient.Account.Update(Guid.Parse(accountDto.Id), accountDto.ToAccountUpdateDto());
                return;
            }
            
            var ele_GetStarted = await page.QuerySelectorAsync(selector_GetStarted);
            await ele_GetStarted.ClickAsync();

            var selector_NextFB = "//a/span/..";
            await page.WaitForSelector(selector_NextFB);
            var ele_NextFB = await page.QuerySelectorAsync(selector_NextFB);
            await ele_NextFB.ClickAsync();

            GETFBCODE:
            selector_NextFB = "//input[@type='submit']";
            await page.WaitForSelector(selector_NextFB);
            ele_NextFB = await page.QuerySelectorAsync(selector_NextFB);
            await ele_NextFB.ClickAsync();

            selector_NextFB = "//input[@type='submit']";
            await page.WaitForSelector(selector_NextFB);
            ele_NextFB = await page.QuerySelectorAsync(selector_NextFB);
            await ele_NextFB.ClickAsync();

            // Get Code from email
            //"(?<=Your security code is:)(.*)(?=To help us confirm)"
            await page.Wait(10000);
            var facebookCode = GetFacebookCode(accountDto.Email,
                accountDto.EmailPassword);

            var selector_InputText = "//input[@type='text']";
            await page.WaitForSelector(selector_InputText);
            var ele_InputText = await page.QuerySelectorAsync(selector_InputText);
            await ele_InputText.TypeAsync(facebookCode, elementHandleTypeOptions);

            var selector_Confirm = "//input[@value='Confirm' or @value='Xác nhận']";
            var ele_Confirm = await page.QuerySelectorAsync(selector_Confirm);
            await ele_Confirm.ClickAsync();

            selector_NextFB = "//a/span/..";
            await page.WaitForSelector(selector_NextFB);
            ele_NextFB = await page.QuerySelectorAsync(selector_NextFB);
            await ele_NextFB.ClickAsync();

            selector_NextFB = "//a/span/..";
            await page.WaitForSelector(selector_NextFB);
            ele_NextFB = await page.QuerySelectorAsync(selector_NextFB);
            await ele_NextFB.ClickAsync();

            var password = PasswordService.Generate(8, 1);

            var selector_NewPassword = "//input[@name='pw']";
            await page.WaitForSelector(selector_NewPassword);
            var ele_NewPassword = await page.QuerySelectorAsync(selector_NewPassword);
            await ele_NewPassword.TypeAsync(password, elementHandleTypeOptions);

            selector_Confirm = "//input[@value='Confirm' or @value='Xác nhận']";
            ele_Confirm = await page.QuerySelectorAsync(selector_Confirm);
            await ele_Confirm.ClickAsync();

            selector_NextFB = "//a/span/..";
            await page.WaitForSelector(selector_NextFB);
            ele_NextFB = await page.QuerySelectorAsync(selector_NextFB);
            await ele_NextFB.ClickAsync();
            
            // update New password
            accountDto.Password = password;
            accountDto.AccountStatus = AccountStatus.Active;
            _apiClient.Account.Update(Guid.Parse(accountDto.Id),
                accountDto.ToAccountUpdateDto());

            selector_NextFB = "//a/span[text()='Back to Facebook' or text()='Quay lại Facebook']/..";
            await page.WaitForSelector(selector_NextFB);
            ele_NextFB = await page.QuerySelectorAsync(selector_NextFB);
            await ele_NextFB.ClickAsync();

            await page.Wait(5000);
        }
        
        private async Task<bool> IsRecoveryManually(IPage page)
        {
            var selector_hiddenEmail = "//span[contains(text(),'*****@gmail.com') or contains(text(),'*****@gdll.vn') or contains(text(),'*****@gdl.vn')]";
            var ele_hiddenEmail = await page.QuerySelectorAsync(selector_hiddenEmail);
            return ele_hiddenEmail != null;
        }

        private string GetSecurityCode()
        {
            while (true)
            {
                var securityCode = _apiEmailClient.Anonymous.GetSecurityCode();
                if (!string.IsNullOrWhiteSpace(securityCode.Resource))
                {
                    return securityCode.Resource;
                }
            }
        }

        private string GetFacebookCode(string email, string password)
        {
            Pop3Client pop3Client = new Pop3Client();

            pop3Client.Connect("pop3.live.com", 995, true);
            pop3Client.Authenticate(email, password);

            var count = pop3Client.GetMessageCount();
            var  message = pop3Client.GetMessage(count - 1);
            string messageText = message.TextBody;
            
            var regex = new Regex("(?<=Mã bảo mật của bạn là:)(.*)(?=Để xác nhận danh tính của bạn trên Facebook)|(?<=Your security code is:)(.*)(?=To help us confirm)");
            var match = regex.Match(messageText);

            var facebookCode = match.Success ? match.Groups[0].Value : string.Empty;

            return facebookCode.Trim();

            // string htmlTagPattern = "<.*?>";
            // var regexCss = new Regex("(<script(.+?))|(<style(.+?))", RegexOptions.Singleline | RegexOptions.IgnoreCase);
            // htmlString = regexCss.Replace(htmlString, string.Empty);
            // htmlString = Regex.Replace(htmlString, htmlTagPattern, string.Empty);
            // htmlString = Regex.Replace(htmlString, @"^\s+$[\r\n]*", "", RegexOptions.Multiline);

            //""
        }
        
    }
}