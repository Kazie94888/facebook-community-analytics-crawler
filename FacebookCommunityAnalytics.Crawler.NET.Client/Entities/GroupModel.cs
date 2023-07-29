using System;
using System.Collections.Generic;
using FacebookCommunityAnalytics.Crawler.NET.Client.Enums;

namespace FacebookCommunityAnalytics.Crawler.NET.Client.Entities
{
    public class Group
    {
        public string title { get; set; }
        public string fid { get; set; }
        public string name { get; set; }
        public string altName { get; set; }
        public string url { get; set; }
        public bool isActive { get; set; }
        public GroupSourceType groupSourceType { get; set; }
        public GroupOwnershipType groupOwnershipType { get; set; }
        public GroupCategoryType groupCategoryType { get; set; }
        
        public bool isDeleted { get; set; }
        public object deleterId { get; set; }
        public object deletionTime { get; set; }
        public DateTime? lastModificationTime { get; set; }
        public string lastModifierId { get; set; }
        public DateTime creationTime { get; set; }
        public string creatorId { get; set; }
        public string id { get; set; }

        public List<CrawlModelBase> ToCrawlModelBase()
        {
            return new()
            {
                new CrawlModelBase
                {
                    Fuid = fid,
                    GroupFid = fid,
                    Url = $"{url}?sorting_setting=CHRONOLOGICAL"
                }
            };
        }
    }

    public class GetGroupsResponse
    {
        public int totalCount { get; set; }
        public List<Group> items { get; set; }
    }

    public class GetGroupsRequest : PagedAndSortedRequest
    {
        public string FilterText { get; set; }

        public string Title { get; set; }
        public string Fid { get; set; }
        public string Name { get; set; }
        public string AltName { get; set; }
        public string Url { get; set; }
        public bool? IsActive { get; set; }
        public GroupSourceType ? GroupSourceType { get; set; }

        public GroupOwnershipType ? GroupOwnershipType { get; set; }
        public GroupCategoryType ? GroupCategoryType { get; set; }

        public GetGroupsRequest() : base()
        {
        }
    }
}