using Microsoft.Playwright;

namespace FacebookCommunityAnalytics.Crawler.NET.LacXoong.Models
{
    public class PlaywrightContext
    {
        public IPlaywright Playwright { get; set; }
        public IBrowser Browser { get; set; }
        public IBrowserContext BrowserContext { get; set; }
    }

}