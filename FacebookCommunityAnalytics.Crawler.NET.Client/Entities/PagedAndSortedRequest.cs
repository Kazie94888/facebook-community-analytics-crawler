namespace FacebookCommunityAnalytics.Crawler.NET.Client.Entities
{
    public class PagedAndSortedRequest
    {
        public string Sorting { get; set; }
        public int SkipCount { get; set; }
        public int MaxResultCount { get; set; }

        public PagedAndSortedRequest()
        {
            MaxResultCount = 1000;
        }
    }
}