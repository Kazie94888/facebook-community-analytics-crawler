using System;
using System.Collections.Generic;

namespace FacebookCommunityAnalytics.Crawler.NET.Core.Extensions
{
    public static class DateTimeExtensions
    {
        private static readonly DateTime _epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public static double ConvertToUnixTimestamp(this DateTime date)
        {
            var d2 = date.ToUniversalTime();
            var ts = new TimeSpan(d2.Ticks - _epoch.Ticks);
            return ts.TotalMilliseconds;
        }

        public static bool IsBetween(this DateTime utc, DateTime start, DateTime end)
        {
            return start <= utc && end >= utc;
        }

        public static DateTime DateFromTimestamp(this long timestamp)
        {
            return new DateTime(timestamp);
        }

        public static DateTime SecondsFromEpoch(this long seconds)
        {
            return _epoch.AddSeconds(seconds);
        }

        public static DateTime GetDateFromYYYYMMDD(this string input)
        {
            if (input == null || input.Length != 8)
                throw new InvalidOperationException($"Incorrect format {input ?? string.Empty}, YYYYMMDD expected");

            return new DateTime(int.Parse(input.Substring(0, 4)), int.Parse(input.Substring(4, 2)), int.Parse(input.Substring(6, 2)));
        }

        public static DateTime? TryParse(this string date)
        {
            if (date.IsNullOrEmpty())
                return null;

            if (DateTime.TryParse(date, out var dt))
                return dt;

            return null;
        }

        public static string ToFolder(this DateTime datetime)
        {
            return datetime.ToString("dd-MM-yyyy");
        }


        public static string ToDateTimeFormatted(this DateTime datetime)
        {
            return datetime.ToString("d MMM yyyy HH:mm:ss");
        }

        public static string ToDateFormatted(this DateTime datetime)
        {
            return datetime.ToString("dd/MM/yyyy");
        }

        public static string ToLocalDateStringFormatted(this DateTime dateTime)
        {
            return dateTime.ToLocalTime().ToDateFormatted();
        }

        public static string ToTimeFormat(this DateTime datetimeUtc)
        {
            return datetimeUtc.ToString("HH:mm");
        }

        public static IEnumerable<DateTime> To(this DateTime from, DateTime to)
        {
            for (var day = from.Date; day.Date <= to.Date; day = day.AddDays(1))
            {
                yield return day;
            }
        }

        public static IEnumerable<DateTime> ToNow(this DateTime from)
        {
            return from.To(DateTime.UtcNow);
        }
    }
}