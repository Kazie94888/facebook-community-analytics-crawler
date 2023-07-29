using System.Collections.Generic;

namespace Crawler.PlaywrightConsoleApp.Models
{
    public class UncrawledResult
    {
        public long Total { get; set; }
        public List<string> Urls { get; set; }
    }
}