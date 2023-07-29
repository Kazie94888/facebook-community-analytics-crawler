using System;
using System.Configuration;
using System.Threading;
using ConsoleAppPostToFacebook.Models.Apis;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace ConsoleAppPostToFacebook.Services
{
    public static class LoginFacebookCrawlerService
    {
        private static readonly string RootUrl = ConfigurationManager.AppSettings["RootUrl"];

        public static IWebDriver Login(ChromeDriver driver, FacebookAccount input)
        {
            try
            {
                var loginEmailElement = driver.FindElementById("m_login_email");
                loginEmailElement.SendKeys(input.Username);

                var loginPasswordElement = driver.FindElementById("m_login_password");
                loginPasswordElement.SendKeys(input.Password);

                Thread.Sleep(2000);
                var buttonLogin = driver.FindElementByXPath("//button[contains(@data-sigil,'m_login_button')]");
                buttonLogin.Click();

                Thread.Sleep(2000);

                var approvePasscodeElement = driver.FindElementById("approvals_code");

                var code = GetTwoFaCodeService.Get(input.TwoFactorCode);

                approvePasscodeElement.SendKeys(code);

                Thread.Sleep(2000);
                var buttonSendCode = driver.FindElementById("checkpointSubmitButton-actual-button");

                buttonSendCode.Click();
                Thread.Sleep(2000);


                while (driver.FindElementById("checkpointSubmitButton-actual-button") != null)
                {
                    buttonSendCode = driver.FindElementById("checkpointSubmitButton-actual-button");
                    buttonSendCode.Click();
                    Thread.Sleep(2000);
                }

                return driver;
            }
            catch (Exception e)
            {
                Console.WriteLine(" ***************** Error: ********************");
                Console.Write(e.Message);
                Console.WriteLine(" *********************************************");
                return driver;
            }
        }
    }
}