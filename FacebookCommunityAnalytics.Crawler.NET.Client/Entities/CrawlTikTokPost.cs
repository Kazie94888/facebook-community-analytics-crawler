using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace FacebookCommunityAnalytics.Crawler.NET.Client.Entities
{
    public enum TikTokMCNType
    {
        FilterNoSelect = 0,
        Unknown = 1,
        MCNVietNam = 10,
        MCNGdl = 20,
    }
    
    public class SaveChannelStatApiResponse
    {
        public bool Success { get; set; }
    }
    
    public class SaveChannelStatApiRequest : ChannelStat
    {
        public DateTime UpdatedAt { get; set; }

        public SaveChannelStatApiRequest()
        {
            UpdatedAt = DateTime.UtcNow;
        }
    }

    public class SaveMCNVietNamChannelApiResponse
    {
        public bool Success { get; set; }
    }

    public class SaveMCNVietNamChannelApiRequest
    {
        public List<SaveMCNVietNamChannelDto> MCNVietNamChannels { get; set; }
    }

    public class SaveMCNVietNamChannelDto
    {
        public string   Name            { get; set; }
        public string   Link            { get; set; }
        public int      Followers       { get; set; }
        public DateTime CreatedDateTime { get; set; }

        public SaveMCNVietNamChannelDto()
        {
            CreatedDateTime = DateTime.UtcNow;
        }
    }

    public class ChannelStat
    {
        public string Title { get; set; }
        public string ChannelId { get; set; }
        public string Description { get; set; }
        public int Followers { get; set; }
        public int Likes { get; set; }
        public string ThumbnailImage { get; set; }

        public SaveChannelStatApiRequest ToSavingChannelStatApiRequest()
        {
            return new SaveChannelStatApiRequest
            {
                Title = this.Title,
                ChannelId = this.ChannelId,
                Description = this.Description,
                Followers = this.Followers,
                Likes = this.Likes,
                ThumbnailImage = this.ThumbnailImage
            };
        }

        public SaveMCNVietNamChannelDto ToSavingMCNVietNamChannelRequest()
        {
            return new SaveMCNVietNamChannelDto
            {
                Name = Title,
                Followers = Followers,
                Link = $"https://www.tiktok.com/@{ChannelId}"
            };
        }
    }

    public class SaveChannelVideoResponse
    {
        public bool Success { get; set; }
    }
    
    public class SaveChannelVideoRequest
    {
        public string ChannelId { get; set; }
        public List<TiktokVideoDto> Videos { get; set; }
        public DateTime UpdatedAt { get; set; }

        public SaveChannelVideoRequest()
        {
            UpdatedAt = DateTime.UtcNow;
        }
    }

    public class SaveSelectiveVideoRequest
    {
        public List<TiktokVideoDto> Videos { get; set; }
    }
    
    public class TiktokVideoDto
    {
        public string VideoId { get; set; }
        public string VideoUrl { get; set; }
        public DateTime CreatedAt { get; set; }
        
        public int Like { get; set; }
        public int Comment { get; set; }
        public int Share { get; set; }
        public int ViewCount { get; set; }
        public string Content { get; set; }
        public  string ThumbnailImage { get; set; }
        public List<string> HashTags { get; set; }
    }
    
    public class GetTiktokHashTagsApiResponse
    {
        public bool Success { get; set; }
        public List<string> HashTags { get; set; }
    }
    
    public class SaveTiktokStatApiRequest
    {
        public string Hashtag { get; set; }
        public long Count { get; set; }
    }
    
    public class SaveTiktokStatApiResponse
    {
        public bool Success { get; set; }
    }

    public class TiktokExportRow
    {
        public string Channel  { get; set; }
        public string Category { get; set; }
        public string Url      { get; set; }
        public string UID      { get; set; }
        public string Fid      { get; set; }
        
        public DateTime CreatedDateTime { get; set; }
    }
    
    public class GetTiktoksInputExtend
    {
        public GetTiktoksInputExtend()
        {
            MaxResultCount = int.MaxValue;
        }
        public string Search { get; set; }
        public DateTime? CreatedDateTimeMin { get; set; }
        public DateTime? CreatedDateTimeMax { get; set; }
        public int ClientOffsetInMinutes { get; set; }
        public bool SendEmail { get; set; }
        public int MaxResultCount { get; set; }
        public string Sorting { get; set; }
        public TikTokMCNType? TikTokMcnType { get; set; }
    }
    
    public class UpdateTiktokVideosStateRequest
    {
        public List<string> VideoIds { get; set; }
        public bool IsNew { get; set; }
    }

    public class TiktokTrendingRequest
    {
        public List<TiktokTrendingDetailDto> TrendingDetails { get; set; }
    }

    public class TiktokTrendingDetailDto
    {
        public int       View            { get; set; }
        public string    Description     { get; set; }
        public DateTime? CreatedDateTime { get; set; }
    }
}