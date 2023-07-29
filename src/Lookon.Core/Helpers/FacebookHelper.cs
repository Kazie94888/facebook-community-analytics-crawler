using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Flurl;
using LookOn.Core.Extensions;
using Newtonsoft.Json;
using RestSharp;

namespace LookOn.Core.Helpers
{
    public static class FacebookHelper
    {
        private const string FacebookPageRegex =
            @"(?:https?:\/\/)?(?:www\.)?(mbasic.facebook|m\.facebook|facebook|fb)\.(com|me)\/(?:(?:\w\.)*#!\/)?(?:pages\/)?(?:[\w\-\.]*\/)*([\w\-\.]*)";

        public class FacebookIdentityModel
        {
            public string Id { get; set; }
        }

        public static async Task<string> GetFacebookId(string url)
        {
            try
            {
                var client  = new RestClient("https://id.traodoisub.com/api.php");
                var request = new RestRequest();
                request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
                request.AddParameter("link", url);
                var response = await client.PostAsync<FacebookIdentityModel>(request);

                return response?.Id;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public static bool IsValidUrl(string url)
        {
            try
            {
                if (url.IsNullOrEmpty()) return false;

                var u    = new Url(url);
                var host = u.Host;
                if (!host.Contains("facebook.com")) return false;
                var regexForm1 = new Regex(FacebookPageRegex);

                if (regexForm1.IsMatch(u.Path)) return regexForm1.IsMatch(u.Path);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}