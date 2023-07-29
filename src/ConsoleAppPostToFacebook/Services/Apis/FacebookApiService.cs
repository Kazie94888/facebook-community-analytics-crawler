using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net;
using ConsoleAppPostToFacebook.Models.Apis;
using RestSharp;

namespace ConsoleAppPostToFacebook.Services.Apis
{
    public static class FacebookApiService
    {
        private static readonly string ApiGetSchedulePost = ConfigurationManager.AppSettings["ApiGetSchedulePost"];
        private static readonly string ApiGetAccountProxy = ConfigurationManager.AppSettings["ApiGetAccountProxy"];

        public static AccountFacebookProxy GetAccountFacebookProxy()
        {
            var client = new RestSharp.RestClient();
            var request = new RestRequest(ApiGetAccountProxy, Method.GET);
            var result = client.Execute<AccountFacebookProxy>(request);
            return result.Data;
        }

        public static List<SchedulePost> GetSchedulePosts()
        {
            var client = new RestSharp.RestClient();
            var request = new RestRequest(ApiGetSchedulePost, Method.GET);
            var result = client.Execute<List<SchedulePost>>(request);
            if (result.IsSuccessful)
            {
                var webClient = new WebClient();
                foreach (var schedulePost in result.Data)
                    try
                    {
                        foreach (var imageUrl in schedulePost.Images)
                        {
                            //var tempPath = Path.GetTempPath();
                            var tempPath = "D:\\temp";
                            var savePath = $"{tempPath}\\images";
                            if (!Directory.Exists(savePath)) Directory.CreateDirectory(savePath);

                            var ext = Path.GetExtension(imageUrl);
                            var saveFilePath = $"{savePath}\\{DateTime.Now.Ticks}{ext}";
                            webClient.DownloadFile(new Uri(imageUrl), saveFilePath);
                            schedulePost.LocalFilesDownloaded.Add(saveFilePath);
                        }
                    }
                    catch (Exception)
                    {
                        // ignored
                    }

                return result.Data;
            }

            return null;
        }
    }
}