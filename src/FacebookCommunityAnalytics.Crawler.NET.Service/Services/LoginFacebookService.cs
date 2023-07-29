using System;
using System.Configuration;
using System.Threading;
using FacebookCommunityAnalytics.Crawler.NET.Core;
using FacebookCommunityAnalytics.Crawler.NET.Service.Models;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace FacebookCommunityAnalytics.Crawler.NET.Service.Services
{
    public class LoginFacebookCrawlerService
    {
        private readonly string _rootUrl = ConfigurationManager.AppSettings["RootUrl"];

        public void Login(ChromeDriver driver, ProxyModel proxyModel, LoginInput input)
        {
            var millisecondsTimeout = 1000;
            
            try
            {
                driver.Navigate().GoToUrl(_rootUrl);

                var loginEmailElement = driver.FindElementById("m_login_email");
                loginEmailElement.SendKeys(input.UserName);

                var loginPasswordElement = driver.FindElementById("m_login_password");
                loginPasswordElement.SendKeys(input.Password);

                var buttonLogin = driver.FindElementByXPath("//button[contains(@data-sigil,'m_login_button')]");
                buttonLogin.Click();

                var approvePasscodeElement = driver.FindElementById("approvals_code");

                var code = GetTwoFACodeService.Get(input.TwoFACode);
                approvePasscodeElement.SendKeys(code);

                var buttonSendCode = driver.FindElementById("checkpointSubmitButton-actual-button");

                buttonSendCode.Click();
                 	
                while (driver.FindElementById("checkpointSubmitButton-actual-button") != null)
                {
                    buttonSendCode = driver.FindElementById("checkpointSubmitButton-actual-button");
                    buttonSendCode.Click();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(" ***************** Error: ********************");
                Console.Write(e.Message);
                Console.WriteLine(" *********************************************");
            }
        }
    }
}