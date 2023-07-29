using System.Collections.Generic;

namespace Crawler.PlaywrightConsoleApp.Models
{
    public class PostModel
    {
        public string PostId { get; set; }
        public string Content { get; set; }
        public List<string> Images { get; set; }
        public List<string> Videos { get; set; }
    }
}