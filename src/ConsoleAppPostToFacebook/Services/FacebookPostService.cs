using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ConsoleAppPostToFacebook.Models;
using Fizzler.Systems.HtmlAgilityPack;
using HtmlAgilityPack;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace ConsoleAppPostToFacebook.Services
{
    public static class FacebookPostService
    {
        private const int TimeSleep = 2000;
        //private static string[] _fbPosting = new string[]{"/html[1]/body[1]/div[1]/div[1]/div[1]/div[1]/div[3]/div[1]/div[1]/div[1]/div[1]/div[2]/div[1]/div[1]/div[4]/div[2]/div[1]/div[2]/div[1]/div[1]/div[1]/div[1]/div[2]/div[1]",
        //    "/html[1]/body[1]/div[1]/div[1]/div[1]/div[1]/div[4]/div[1]/div[1]/div[1]/div[1]/div[2]/div[1]/div[1]/div[1]/form[1]/div[1]/div[1]/div[1]/div[2]/div[2]/div[1]/div[1]/div[1]/div[1]/div[1]/div[1]/div[1]/div[1]/div[2]/div[1]/div[1]/div[1]/div[1]",
        //    "/html[1]/body[1]/div[1]/div[1]/div[1]/div[1]/div[4]/div[1]/div[1]/div[1]/div[1]/div[2]/div[1]/div[1]/div[1]/form[1]/div[1]/div[1]/div[1]/div[2]/div[3]/div[4]/div[1]"};

        private static readonly string[] FbPosting =
        {
            "/html/body/div[1]/div/div[4]/div/div[1]/div/div[3]/div/div[1]/div[2]",
            "/html/body/div[2]/div[1]/div/div[2]/div/div/div[5]/div[3]/form/div[3]/div[3]/textarea",
            "/html/body/div[2]/div[1]/div/div[2]/div/div/div[5]/div[3]/div/div/button"
        };

        private static readonly string _pathPostImages = "/html/body/div[2]/div[1]/div/div[2]/div/div/div[5]/div[3]/form/div[7]/div/button[1]";

        public static async Task<bool> PostToFacebook(ChromeDriver driver, PostModel model)
        {
            try
            {
                driver.FindElementByXPath(FbPosting[0]).Click();

                Thread.Sleep(TimeSleep);
                var textAreaInput = driver.FindElementByXPath(FbPosting[1]);

                PopulateElementJs(driver, textAreaInput, model.Content);

                textAreaInput.SendKeys(Keys.Enter);

                Thread.Sleep(TimeSleep);

                driver.FindElementByXPath(_pathPostImages).Click();

                Thread.Sleep(TimeSleep);

                // khởi tạo đối tượng autoIT để dùng cho C# -> nhờ nó send key click chuột dùm cái ở ngoài web browser
                var autoIt = new AutoItX3.Interop.AutoItX3();
                var processNumber = 1;
                foreach (var image in model.Images)
                {
                    var uploaded = false;
                    var trycount = 1;
                    do
                    {
                        // đưa title của cửa sổ File upload vào chuỗi. 
                        // Cửa sổ hiện ra có thể có tiêu đề là File Upload hoặc Tải lên một tập tin
                        // lấy ra cửa sổ active có tiêu đề như dưới
                        autoIt.WinActivate("File Upload");
                        // file data nằm trong thư mục debug
                        // gửi link vào ô đường dẫn
                        autoIt.Send(image);
                        // gửi phím enter sau khi truyền link vào
                        autoIt.Send("{ENTER}");

                        Thread.Sleep(TimeSleep);

                        var document = new HtmlDocument();
                        //Load trang web, nạp html vào document
                        document.LoadHtml(driver.PageSource);
                        var loadAll = document.DocumentNode.QuerySelectorAll("input[type='hidden']").ToList();
                        if (loadAll.Count(_ => _.Attributes.Any(a => a.Value.Contains("photo_ids"))) == processNumber)
                        {
                            uploaded = true;
                            processNumber++;
                        }

                        trycount++;
                        Thread.Sleep(TimeSleep);
                    } while (!uploaded && trycount < 5);
                }

                Thread.Sleep(TimeSleep);
                //Do post
                driver.FindElementByXPath(FbPosting[2]).Click();
                Thread.Sleep(TimeSleep);

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

        private static void PopulateElementJs(IWebDriver driver, IWebElement element, string text)
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