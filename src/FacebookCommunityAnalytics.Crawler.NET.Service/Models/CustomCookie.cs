using Newtonsoft.Json;

namespace FacebookCommunityAnalytics.Crawler.NET.Service.Models
{
    public class CustomCookie
    {
        [JsonProperty("name")] public string Name { get; set; }

        //
        // Summary:
        //     Gets the value of the cookie.
        [JsonProperty("value")] public string Value { get; set; }

        //
        // Summary:
        //     Gets the domain of the cookie.
        [JsonProperty("domain", NullValueHandling = NullValueHandling.Ignore)]
        public string Domain { get; set; }

        //
        // Summary:
        //     Gets the path of the cookie.
        [JsonProperty("path", NullValueHandling = NullValueHandling.Ignore)]
        public virtual string Path { get; set; }

        //
        // Summary:
        //     Gets a value indicating whether the cookie is secure.
        [JsonProperty("secure")] public virtual bool Secure { get; set; }

        //
        // Summary:
        //     Gets a value indicating whether the cookie is an HTTP-only cookie.
        [JsonProperty("httpOnly")] public virtual bool IsHttpOnly { get; set; }

        //
        // Summary:
        //     Gets the expiration date of the cookie.
        [JsonProperty("expiry", NullValueHandling = NullValueHandling.Ignore)]
        public double? Expiry { get; set; }
    }
}