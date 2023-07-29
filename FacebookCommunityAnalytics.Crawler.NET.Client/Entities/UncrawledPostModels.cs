using System;
using System.Collections.Generic;
using FacebookCommunityAnalytics.Crawler.NET.Client.Enums;

namespace FacebookCommunityAnalytics.Crawler.NET.Client.Entities
{
    public class UncrawledPostsRequest
    {
        public PostSourceType? PostSourceType { get; set; }
        public DateTime? FromDateTime { get; set; }
        public DateTime? ToDateTime { get; set; }
        public AccountType AccountType { get; set; }
    }

    public class UncrawledPostsResponse
    {
        public long Count { get; set; }
        public List<UncrawledItemDto> Items { get; set; }
        
        public List<AccountProxyItem> AccountProxies { get; set; }
    }

    public class UncrawledItemDto : CrawlModelBase
    {
        public PostSourceType PostSourceType { get; set; }
    }

    // public class CrawlResult
    // {
    //     public List<CrawlResultItem> Items { get; set; }
    // }
    //
    // public class CrawlResultItem
    // {
    //     public string Url { get; set; }
    //
    //     /// <summary>
    //     /// This is used for affiliate urls (usually called short links)
    //     /// </summary>
    //     public List<string> Urls { get; set; }
    //
    //     public int LikeCount { get; set; }
    //     public int CommentCount { get; set; }
    //     public int ShareCount { get; set; }
    //     public bool IsNotAvailable { get; set; }
    //     public string CreatedBy { get; set; }
    //     public string CreatedFuid { get; set; }
    //     public string CreatedAt { get; set; }
    // }
}