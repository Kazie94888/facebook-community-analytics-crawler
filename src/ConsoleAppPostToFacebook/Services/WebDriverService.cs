using System.Linq;
using ConsoleAppPostToFacebook.Models.Apis;
using HtmlAgilityPack;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace ConsoleAppPostToFacebook.Services
{
    public static class WebDriverService
    {
        public static IWebDriver InitDriver(ProxyAccount proxy = null)
        {
            ChromeDriver driver;
            var service = ChromeDriverService.CreateDefaultService();
            service.HideCommandPromptWindow = true;
            var options = new ChromeOptions();


            //options.AddArguments("headless");
            //options.AddArgument("--start-maximized");
            options.AddArgument("--ignore-ssl-errors=yes");
            options.AddArgument("--ignore-certificate-errors");
            options.AddArgument("--disable-web-security");
            options.AddArgument("--allow-running-insecure-content");
            options.AcceptInsecureCertificates = true;
            options.AddArgument("--disable-blink-features=AutomationControlled");
            options.AddArgument("user-agent=Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Safari/537.36");
            //options.AddArgument("user-agent=Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/89.0.4389.128 Safari/537.36");

            //options.AddArgument("--remote-debugging-port=9222");
            //options.AddArgument(@"user-data-dir=C:\users\campi\AppData\Local\Google\Chrome\User Data");
            //options.AddArgument(@"user-data-dir=F:\ChromeUserData");

            //options.AddArgument("--disable-popup-blocking");
            //options.AddArgument("--log-level=3"); // to shut the logging

            // disable

            options.AddExcludedArgument("remote-debugging-port");
            options.AddExcludedArgument("--remote-debugging-port");
            options.AddExcludedArgument("enable-automation");
            options.AddExcludedArgument("--enable-automation");
            options.AddAdditionalCapability("useAutomationExtension", false);

            if (proxy != null)
            {
                //var proxyAuth = new ProxyAuth(proxy.IP, proxy.Port, proxy.UserName, proxy.Password);
                //var proxyServer = new SeleniumProxyServer();
                //var localPort = proxyServer.AddEndpoint(proxyAuth);

                //// Configure the driver's proxy server to the local endpoint port
                //options.AddArgument($"--proxy-server=127.0.0.1:{localPort}");

                //options.AddExtension("proxy.zip");
                options.AddArgument($"--proxy-server={proxy.Ip}:{proxy.Port}");

                driver = new ChromeDriver(service, options);
            }
            else
            {
                driver = new ChromeDriver(service);
            }

            return driver;
        }

        public static HtmlDocument LoadNewTab(ChromeDriver driver, string url)
        {
            ((IJavaScriptExecutor) driver).ExecuteScript("window.open();");
            var tabs = driver.WindowHandles.ToList();
            driver.SwitchTo().Window(tabs.LastOrDefault());

            driver.Navigate().GoToUrl(url);

            var document = new HtmlDocument();
            //Load trang web, nạp html vào document
            document.LoadHtml(driver.PageSource);

            return document;
        }
    }
}