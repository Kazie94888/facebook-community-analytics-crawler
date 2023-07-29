using System;
using System.Collections.Generic;

namespace FacebookCommunityAnalytics.Crawler.NET.Client.Entities
{
    public class AutoPostFacebook
    {
        public string Url { get; set; }
        public int TotalLike { get; set; }
        public int CurrentLike { get; set; }
        
        public int TotalComment { get; set; }
        public int CurrentComment { get; set; }
        
        public List<string> Comments { get; set; }
        public bool IsDone { get; set; }
        public Guid Id { get; set; }
    }
    
    public class AutoPostFacebookNotDoneDto : AutoPostFacebook
    {
        public int NeedLike { get; set; }
        public int NeedComment { get; set; }
    }

    public class UpdateLikeCommentDto
    {
        public Guid AutoPostFacebookId { get; set; }
        public int NumberLike { get; set; }
        public int NumberComment { get; set; }
    }
}