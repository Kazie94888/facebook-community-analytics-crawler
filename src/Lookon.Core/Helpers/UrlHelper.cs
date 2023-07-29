using System.Linq;
using Flurl;
using LookOn.Core.Extensions;

namespace LookOn.Core.Helpers
{
    public static class UrlHelper
    {
        public static string ToHyperLinkText(this string urlString, string label = "Link")
        {
            if (urlString.IsNullOrEmpty()) return string.Empty;

            var url = new Url(urlString);

            return $"{label}: {url.Host}";
        }
        
        public static string GetShortKey(string shortLink)
        {
            return (shortLink.Contains('/') ? shortLink.Trim().Split('/').LastOrDefault() : string.Empty) ?? string.Empty;
        }
    }
}