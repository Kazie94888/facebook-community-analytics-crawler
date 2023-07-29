using System;
using System.Linq;
using FacebookCommunityAnalytics.Crawler.NET.Core.Extensions;
using FacebookCommunityAnalytics.Crawler.NET.Service.Models;
using Fizzler.Systems.HtmlAgilityPack;
using HtmlAgilityPack;

namespace FacebookCommunityAnalytics.Crawler.NET.Service.Services
{
    public static class HtmlExtractDataService
    {
        public static CrawlResultItemDto ExtractFromPost(HtmlDocument htmlDocument)
        {
            CrawlResultItemDto result;

            try
            {
                var element = htmlDocument.DocumentNode.QuerySelector("div[data-sigil='m-mentions-expand']");
                var commentCount = htmlDocument.DocumentNode.QuerySelectorAll("div[data-sigil*='comment']").Count();
                //var childHtmlNode = element.ChildNodes.FirstOrDefault();
                //if (childHtmlNode != null)
                //{
                //    var reactedElement = childHtmlNode.QuerySelector("div[aria-label='reactions']");
                //}

                result = new CrawlResultItemDto();
                return result;
            }
            catch (Exception e)
            {
                Console.WriteLine(" ***************** Error: ********************");
                Console.Write(e.Message);
                Console.WriteLine(" *********************************************");
                return null;
            }
        }

        public static int GetReactionCount(HtmlDocument htmlDocument)
        {
            try
            {
                var htmlNode = htmlDocument.DocumentNode.QuerySelectorAll("span[data-sigil='reaction_profile_tab_count']").FirstOrDefault();
                if (htmlNode != null) return htmlNode.InnerText.Split(' ').LastOrDefault().ToIntODefault();
                return 0;
            }
            catch (Exception e)
            {
                Console.WriteLine(" ***************** Error: ********************");
                Console.Write(e.Message);
                Console.WriteLine(" *********************************************");
                return 0;
            }
        }
    }
}