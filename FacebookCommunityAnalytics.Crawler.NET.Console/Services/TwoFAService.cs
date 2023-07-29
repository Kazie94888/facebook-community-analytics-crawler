using FacebookCommunityAnalytics.Crawler.NET.Client.Core;
using Newtonsoft.Json;
using OtpNet;
using RestSharp;
using System.Net;
using FacebookCommunityAnalytics.Crawler.NET.Core.Helpers;

namespace FacebookCommunityAnalytics.Crawler.NET.Console.Services
{
    public static class TwoFAService
    {
        public static string Get(string _2faApiUrl, string code)
        {
            var response = RestHelper.CreateClient(_2faApiUrl).Get<string>(new RestRequest(code));

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var token = JsonConvert.DeserializeObject<TwoFAToken>(response.Content);
                return token.token;
            }

            return string.Empty;
        }

        public static string Get(string privatekey)
        {
            if (string.IsNullOrWhiteSpace(privatekey))
                return null;
            privatekey = privatekey.Replace(" ", "");
            var bytes = Base32Encoding.ToBytes(privatekey);
            var totp = new Totp(bytes);
            var code = totp.ComputeTotp();
            return code;
        }

        public class TwoFAToken
        {
            public string token { get; set; }
        }
    }
    
//     public class CustomWebClient : WebClient
//     {
//         protected override WebRequest GetWebRequest(Uri address)
//         {
//             if (base.GetWebRequest(address) is not HttpWebRequest request) return null;
//             request.AutomaticDecompression = DecompressionMethods.GZip;
//             request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/71.0.3578.98 Safari/537.36";
//             return request;
//
//         }
//     }
//     public static class GetTwoFaCodeService
//     {
//         public static string Get(string urlGet2FaCode,string inputCode)
//         {
//             inputCode = inputCode.Replace(" ", string.Empty);
//             var webClient = new CustomWebClient();
//             var tokenJson = JsonConvert.DeserializeObject<dynamic>(webClient.DownloadString(new Uri(urlGet2FaCode + inputCode)));
//             return tokenJson?.token;
//         }
//     }
}