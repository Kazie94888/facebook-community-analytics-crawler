using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace FacebookCommunityAnalytics.Crawler.NET.Client.Entities
{
    public class SaveAutoCrawlResultApiResponse
    {
        public bool Success { get; set; }
    }
    
    public class SaveAutoCrawlResultApiRequest
    {
        [JsonProperty("crawlType")]
        public CrawlType CrawlType { get; set; }
        
        public string GroupFid { get; set; }
        
        [JsonProperty("items")]
        public List<AutoCrawledPost> Items { get; set; }
    }
    
    public class SaveCrawlResultApiRequest
    {
        public List<AutoCrawledPost> Items { get; set; }
    }
    
    public class AutoCrawledPost: CrawledPost
    {
        public PostSourceType PostSourceType { get; set; }
    }
    
    public class CrawledPost
    {
        public string Url { get; set; }

        /// <summary>
        /// This is used for affiliate urls (usually called short links)
        /// </summary>
        public List<string> Urls { get; set; }

        public string Content { get; set; }
        public List<string> HashTags { get; set; }
        
        public int LikeCount { get; set; }
        public int CommentCount { get; set; }
        public int ShareCount { get; set; }
        
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public string CreateFuid { get; set; }
        
        /// <summary>
        /// No need to crawl, just copied from API
        /// </summary>
        public bool IsNotAvailable { get; set; }
        
        // TODO Vu.Nguyen: later crawl this
        public string GroupFid { get; set; }
        public string CampaignCode { get; set; }
        public string PartnerCode { get; set; }

        public string HashTagsString
        {
            get
            {
                if (HashTags.Any())
                {
                    return string.Join("\r\n", HashTags);
                }

                return string.Empty;
            }
        }

        public string UrlsString
        {
            get
            {
                if (Urls.Any())
                {
                    return string.Join("\r\n", Urls);
                }

                return string.Empty;
            }
        }
    }
}