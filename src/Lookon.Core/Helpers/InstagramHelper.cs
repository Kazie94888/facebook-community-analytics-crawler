using System;
using System.Text.RegularExpressions;
using Flurl;
using LookOn.Core.Extensions;

namespace LookOn.Core.Helpers
{
    public static class InstagramHelper
    {
        private const string CaseInsensitive = "(?i)";
        private const string Prefix = @"^" + CaseInsensitive;
        
        // https://www.instagram.com/p/CSwR6LHFOfb/
        private const string InstagramPostUrlForm = Prefix + @"(/p/[a-zA-Z0-9\.])";

        public static bool IsNotValidUrl(string url)
        {
            return !IsValidUrl(url);
        }
        
        public static bool IsValidUrl(string url)
        {
            try
            {
                if (url.IsNullOrEmpty()) return false;

                var u = new Url(url);
                var host = u.Host;
                if (!host.Contains("instagram.com")) return false;
                var regexForm1 = new Regex(InstagramPostUrlForm);

                if (regexForm1.IsMatch(u.Path)) return regexForm1.IsMatch(u.Path);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public static Url GetCleanUrl(string url)
        {
            url = url
                .Replace("https://///", "https://")
                .Replace("https:////", "https://")
                .Replace("https:///", "https://")
                .Trim().Trim('/').Trim('/');
            return url;
        }
    }
}