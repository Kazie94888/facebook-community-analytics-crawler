using System;
using Newtonsoft.Json;

namespace FacebookCommunityAnalytics.Crawler.NET.Core
{
    public static class GetTwoFACodeService
    {
        private const string Url = "http://2fa.live/tok/";

        public static string Get(string inputCode)
        {
            inputCode = inputCode.Replace(" ", string.Empty);
            var webClient = new CustomWebClient();
            var tokenJson = JsonConvert.DeserializeObject<dynamic>(webClient.DownloadString(new Uri(Url + inputCode)));
            return tokenJson.token;
        }
    }
}