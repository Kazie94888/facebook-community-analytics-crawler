using System;
using FacebookCommunityAnalytics.Crawler.NET.Core;
using Newtonsoft.Json;

namespace ConsoleAppPostToFacebook.Services
{
    public static class GetTwoFaCodeService
    {
        private const string Url = "https://2fa.live/tok/";

        public static string Get(string inputCode)
        {
            inputCode = inputCode.Replace(" ", string.Empty);
            var webClient = new CustomWebClient();
            var tokenJson = JsonConvert.DeserializeObject<dynamic>(webClient.DownloadString(new Uri(Url + inputCode)));
            return tokenJson?.token;
        }
    }
}