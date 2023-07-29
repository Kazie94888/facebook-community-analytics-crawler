using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Crawler.PlaywrightConsoleApp.Models.Apis
{
    public class SchedulePost
    {
        public SchedulePost()
        {
            Images = new List<string>();
            Videos = new List<string>();
            LocalFilesDownloaded = new List<string>();
        }

        public Guid Id { get; set; }
        public string Content { get; set; }
        public bool IsAutoPost { get; set; }
        public DateTime? ScheduledPostDateTime { get; set; }
        public DateTime? PostedAt { get; set; }
        public string GroupIds { get; set; }
        public List<string> Images { get; set; }
        public List<string> Videos { get; set; }

        [JsonIgnore] public List<string> LocalFilesDownloaded { get; set; }
    }
}