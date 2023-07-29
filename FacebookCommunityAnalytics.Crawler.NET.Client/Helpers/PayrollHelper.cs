using System;
using System.Collections.Generic;

namespace FacebookCommunityAnalytics.Crawler.NET.Client.Helpers
{
    public static class PayrollHelper
    {
        public static KeyValuePair<DateTime, DateTime> GetDefaultPayrollDateTime()
        {
            int startDay = 26;
            int startHour = 0;
            int endDay = 26;
            int endHour = 0;
            
            
            var now = DateTime.UtcNow;
            var from = now.AddMonths(-1);
            var to = now;

            if (now.Day >= endDay)
            {
                from = now;
                to = now.AddMonths(1);
            }

            var fromDateTime = DateTime.SpecifyKind(new DateTime(@from.Year, @from.Month, startDay, startHour, 0, 0), DateTimeKind.Utc);
            var toDateTime = DateTime.SpecifyKind(new DateTime(to.Year, to.Month, endDay, endHour, 0, 0), DateTimeKind.Utc);

            return new KeyValuePair<DateTime, DateTime>(fromDateTime, toDateTime);
        } 
    }
}