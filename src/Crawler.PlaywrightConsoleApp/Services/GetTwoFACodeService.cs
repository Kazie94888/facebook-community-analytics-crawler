using System;
using System.Configuration;
using System.Net;
using Newtonsoft.Json;

namespace Crawler.PlaywrightConsoleApp.Services
{
    public static class GetTwoFaCodeService
    {
        private static readonly string UrlGet2FaCode = ConfigurationManager.AppSettings["ApiGet2FACode"];

        public static string Get(string inputCode)
        {
            inputCode = inputCode.Replace(" ", string.Empty);
            var webClient = new CustomWebClient();
            var tokenJson = JsonConvert.DeserializeObject<dynamic>(webClient.DownloadString(new Uri(UrlGet2FaCode + inputCode)));
            return tokenJson?.token;
        }
    }
}