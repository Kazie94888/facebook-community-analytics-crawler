using System;
using System.Diagnostics;
using System.Threading;
using FacebookCommunityAnalytics.Crawler.NET.Service.Models;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace FacebookCommunityAnalytics.Crawler.NET.Service.Services
{
    public static class FacebookProcessService
    {
        private const int _timeSleep = 2000;
        //private static string[] _fbPosting = new string[]{"/html[1]/body[1]/div[1]/div[1]/div[1]/div[1]/div[3]/div[1]/div[1]/div[1]/div[1]/div[2]/div[1]/div[1]/div[4]/div[2]/div[1]/div[2]/div[1]/div[1]/div[1]/div[1]/div[2]/div[1]",
        //    "/html[1]/body[1]/div[1]/div[1]/div[1]/div[1]/div[4]/div[1]/div[1]/div[1]/div[1]/div[2]/div[1]/div[1]/div[1]/form[1]/div[1]/div[1]/div[1]/div[2]/div[2]/div[1]/div[1]/div[1]/div[1]/div[1]/div[1]/div[1]/div[1]/div[2]/div[1]/div[1]/div[1]/div[1]",
        //    "/html[1]/body[1]/div[1]/div[1]/div[1]/div[1]/div[4]/div[1]/div[1]/div[1]/div[1]/div[2]/div[1]/div[1]/div[1]/form[1]/div[1]/div[1]/div[1]/div[2]/div[3]/div[4]/div[1]"};

        private static readonly string[] _fbPosting =
        {
            "/html/body/div[1]/div/div[4]/div/div[1]/div/div[3]/div/div[1]/div[2]",
            "/html/body/div[2]/div[1]/div/div[2]/div/div/div[5]/div[3]/form/div[3]/div[3]/textarea",
            "/html/body/div[2]/div[1]/div/div[2]/div/div/div[5]/div[3]/div/div/button"
        };

        private static readonly string _pathPostImages = "/html/body/div[2]/div[1]/div/div[2]/div/div/div[5]/div[3]/form/div[7]/div/button[1]";

        //private static string _inputFilePath = "/html/body/div[1]/div/div[1]/div/div[4]/div/div/div[1]/div/div[2]/div/div/div/form/div/div[1]/div/div[2]/div/div[3]/div[1]/div[2]/div[1]/input";
        public static bool PostToFacebook(ChromeDriver driver, PostModel model)
        {
            try
            {
                driver.FindElementByXPath(_fbPosting[0]).Click();

                Thread.Sleep(_timeSleep);
                var textAreaInput = driver.FindElementByXPath(_fbPosting[1]);

                PopulateElementJs(driver, textAreaInput, model.Content);

                textAreaInput.SendKeys(Keys.Enter);

                Thread.Sleep(_timeSleep);

                //Upload images & videos
                foreach (var image in model.Images)
                {
                    driver.FindElementByXPath(_pathPostImages).Click();
                    driver.SwitchTo().ActiveElement().SendKeys(Keys.Enter);
                }


                Thread.Sleep(_timeSleep);

                //Do post
                driver.FindElementByXPath(_fbPosting[2]).Click();

                Thread.Sleep(_timeSleep);

                return true;
            }
            catch (Exception e)
            {
                Debug.WriteLine(" ***************** Error: ********************");
                Debug.Write(e.Message);
                Debug.WriteLine(" *********************************************");
                return false;
            }
        }

        public static void PopulateElementJs(IWebDriver driver, IWebElement element, string text)
        {
            var script = @"var elm = arguments[0],
            txt = arguments[1]; elm.value += txt;
            elm.dispatchEvent(new Event('keydown', { bubbles: true }));
            elm.dispatchEvent(new Event('keypress', { bubbles: true }));
            elm.dispatchEvent(new Event('input', { bubbles: true }));
            elm.dispatchEvent(new Event('keyup', { bubbles: true })); ";
            ((IJavaScriptExecutor) driver).ExecuteScript(script, element, text);
        }
    }
}