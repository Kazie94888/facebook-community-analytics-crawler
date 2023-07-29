namespace FacebookCommunityAnalytics.Crawler.NET.Core.Extensions
{
    public static class CurrencyExtensions
    {
        public static string ToVND(this decimal value)
        {
            if (value == 0) return "---";
            return $"{value:###,###} vnđ";
        }

        public static string ToVND(this object value)
        {
            decimal.TryParse(value.ToString(), out var number);
            return ToVND(number);
        }
    }
}