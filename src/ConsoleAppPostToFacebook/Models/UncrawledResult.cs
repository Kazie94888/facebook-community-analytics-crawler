using System.Collections.Generic;

namespace ConsoleAppPostToFacebook.Models
{
    public class UncrawledResult
    {
        public long Total { get; set; }
        public List<string> Urls { get; set; }
    }
}