using System;
using System.Collections.Generic;
using LookOn.Core.Shared.Enums;

namespace LookOn.Core.Extensions
{
    public static class DateTimeExtensions
    {
        private static readonly DateTime _epoch = new DateTime(1970,
                                                               1,
                                                               1,
                                                               0,
                                                               0,
                                                               0,
                                                               DateTimeKind.Utc);

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
            if (date.IsNullOrEmpty()) return null;

            if (DateTime.TryParse(date, out var dt)) return dt;

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

        public static IEnumerable<DateTime> EachDay(this DateTime from, DateTime thru)
        {
            for (var day = from.Date; day.Date <= thru.Date; day = day.AddDays(1)) yield return day;
        }

        public static IEnumerable<DateTime> EachMonth(this DateTime from, DateTime thru)
        {
            for (var day = from.Date; day.Date <= thru.Date; day = day.AddMonths(1)) yield return day;
        }

        public static IEnumerable<DateTime> EachYear(this DateTime from, DateTime thru)
        {
            for (var day = from.Date; day.Date <= thru.Date; day = day.AddYears(1)) yield return day;
        }

        public static DateTimeOffset UtcToOffsetOrDefault(this DateTime utcDateTime, int offset = 0)
        {
            return new DateTimeOffset(utcDateTime, TimeSpan.FromHours(offset));
        }

        public static DateTimeOffset UtcToOffsetOrDefault(this DateTimeOffset utcDateTimeOffset, int offset = 0)
        {
            return new DateTimeOffset(utcDateTimeOffset.DateTime, TimeSpan.FromHours(offset));
        }

        public static DateTime EndOfDay(this DateTime input)
        {
            return input.Date.AddDays(1).AddTicks(-1);
        }

        /// <summary>
        /// Get age (int) by birth date
        /// </summary>
        /// <param name="birthDate"></param>
        /// <returns></returns>
        public static int ToAge(this DateTime birthDate)
        {
            var now = DateTime.UtcNow;
            int age = now.Year - birthDate.Year;

            if (now.Month < birthDate.Month || (now.Month == birthDate.Month && now.Day < birthDate.Day)) age--;

            return age;
        }

        public static (DateTime, DateTime) GetDateByTimeFrameType(this TimeFrameType timeFrameType, DateTime? fromDateTime, bool isCurrentTimeFrame)
        {
            fromDateTime ??= DateTime.UtcNow;
            
            var to                                  = fromDateTime.Value;
            var from                                = fromDateTime.Value;

            to = from.AddDays(timeFrameType.ToInt());
            if (isCurrentTimeFrame) return (from, to);
            
            from = from.AddDays(-timeFrameType.ToInt());
            to   = to.AddDays(-timeFrameType.ToInt());

            return (from, to);

        }
    }
}