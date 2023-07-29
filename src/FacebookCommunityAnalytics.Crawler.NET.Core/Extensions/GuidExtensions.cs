using System;

namespace FacebookCommunityAnalytics.Crawler.NET.Core.Extensions
{
    public static class GuidExtensions
    {
        public static bool IsNullOrEmpty(this Guid? input)
        {
            return input == null || input.Value == Guid.Empty;
        }

        public static bool IsNotNullOrEmpty(this Guid? input)
        {
            return !IsNullOrEmpty(input);
        }
    }
}