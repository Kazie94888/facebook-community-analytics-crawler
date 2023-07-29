namespace FacebookCommunityAnalytics.Crawler.NET.Client.Entities
{
    public class PagedAndSortedResultRequest
    {
        public int SkipCount { get; set; }
        public int MaxResultCount { get; set; }

        public PagedAndSortedResultRequest()
        {
            MaxResultCount = 1000;
        }
    }
}