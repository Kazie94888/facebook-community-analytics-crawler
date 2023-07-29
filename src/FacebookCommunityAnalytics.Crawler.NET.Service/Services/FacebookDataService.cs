using System;
using System.Net;
using FacebookCommunityAnalytics.Crawler.NET.Service.Models;
using Newtonsoft.Json;

namespace FacebookCommunityAnalytics.Crawler.NET.Service.Services
{
    public class FacebookDataService
    {
        public UncrawledResult GetUncrawlers(string url)
        {
            using (var webClient = new WebClient())
            {
                var dataResult = webClient.DownloadString(new Uri(url));
                if (!string.IsNullOrEmpty(dataResult)) return JsonConvert.DeserializeObject<UncrawledResult>(dataResult);

                return null;
            }
        }
    }
}