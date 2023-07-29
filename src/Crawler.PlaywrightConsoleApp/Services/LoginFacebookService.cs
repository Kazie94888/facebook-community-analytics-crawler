using System;
using System.Configuration;
using System.Threading;
using System.Threading.Tasks;
using Crawler.PlaywrightConsoleApp.Models.Apis;
using Crawler.PlaywrightConsoleApp.Services;
using Microsoft.Playwright;

namespace ConsoleAppPostToFacebook.Services
{
    public static class LoginFacebookCrawlerService
    {
        private static readonly string RootUrl = ConfigurationManager.AppSettings["RootUrl"];

        public static async Task Login(IPage page, FacebookAccount input)
        {
            try
            {
                
                
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