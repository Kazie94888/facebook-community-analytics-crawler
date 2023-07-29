using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace FacebookCommunityAnalytics.Crawler.NET.Core.Extensions
{
    public static class ObjectExtensions
    {
        public static T Clone<T>(this T source)
        {
            var serialized = JsonConvert.SerializeObject(source);
            return JsonConvert.DeserializeObject<T>(serialized);
        }
    }

    public static class GenericExtensions
    {
        public static bool IsIn(this string value, IEnumerable<string> candidates, StringComparison stringComparison = StringComparison.InvariantCultureIgnoreCase)
        {
            return value.IsIn(candidates.ToArray(), stringComparison);
        }
        public static bool IsIn(this string value, string[] candidates, StringComparison stringComparison = StringComparison.InvariantCultureIgnoreCase)
        {
            return candidates.Any(c => c?.Trim().Equals(value.Trim(), stringComparison) ?? false);
        }

        public static bool IsIn<T>(this T obj, params T[] candidates)
        {
            return candidates.Any(c => c == null ? obj == null : c.Equals(obj));
        }
        public static bool IsNotIn<T>(this T obj, params T[] candidates)
        {
            return !obj.IsIn(candidates);
        }

        public static bool IsIn<T>(this T obj, IEnumerable<T> candidates)
        {
            return obj != null && obj.IsIn(candidates.ToArray());
        }
        public static bool IsNotIn<T>(this T obj, IEnumerable<T> candidates)
        {
            return !obj.IsIn(candidates.ToArray());
        }

        public static bool NotIn<T>(this T source, params T[] candidates)
        {
            return !candidates.Any(c => c == null ? source == null : c.Equals(source));
        }

        public static bool NotIn<T>(this T source, IEnumerable<T> candidates)
        {
            return source.NotIn(candidates.ToArray());
        }

        public static T Or<T>(this T source, T alternateIfSourceIsDefault)
        {
            return source.Equals(default(T)) ? alternateIfSourceIsDefault : source;
        }

        public static TOut IfPoss<T, TOut>(this T? nullable, Func<T, TOut> getter, TOut valueIfNotPoss = default) where T : struct
        {
            return nullable.Cond(t => !t.HasValue, t => valueIfNotPoss, t => getter(t.Value));
        }

        public static TOut IfPoss<T, TOut>(this T obj, Func<T, TOut> getter, TOut valueIfNotPoss = default)
            where T : class
        {
            return obj.Cond(t => t == null, t => valueIfNotPoss, getter);
        }

        public static string IfNotNullOrEmpty(this string obj, Func<string, string> getter, string valueIfNotPoss = null)
        {
            return obj.IsNotNullOrEmpty() ? getter(obj) : valueIfNotPoss;
        }

        public static T Modify<T>(this T obj, Action<T> modifier)
        {
            modifier(obj);
            return obj;
        }

        public static TOut Use<T, TOut>(this T obj, Func<T, TOut> usage)
        {
            return usage(obj);
        }

        public static TOut Cond<T, TOut>(this T obj, Func<T, bool> test, Func<T, TOut> resultIf, Func<T, TOut> resultElse)
        {
            return test(obj) ? resultIf(obj) : resultElse(obj);
        }

        public static T? AsNullable<T>(this T t) where T : struct
        {
            return t;
        }

        public static T ToEnum<T>(this int i) where T : struct, IConvertible
        {
            var @enum = (T)Enum.Parse(typeof(T), i.ToString());
            return @enum;
        }
        
        public static T ToEnum<T>(this string s) where T : struct, IConvertible
        {
            var @enum = (T)Enum.Parse(typeof(T), s);
            return @enum;
        }

        public static bool InRange(this int i, int bottom, int top, bool inclusive = true)
        {
            return inclusive
                ? bottom <= i && i <= top
                : bottom < i && i < top;
        }

        public static int ToInt<T>(this T e) where T : struct, IConvertible
        {
            if (!typeof(T).IsEnum) throw new ArgumentException("T must be an enumerated type");
            return (int) (IConvertible) e;
        }

        public static int ToIntOrDefault(this int? i, int defaultValue = 0)
        {
            return i ?? 0;
        }

        public static double ToDouble(this decimal value)
        {
            return Convert.ToDouble(value);
        }

        public static void AddRange<T>(this ConcurrentBag<T> @this, IEnumerable<T> toAdd)
        {
            foreach (var element in toAdd) @this.Add(element);
        }

        public static string GetAggregateExceptionMessage(this AggregateException ae)
        {
            var exceptionMessage = string.Empty;
            foreach (var exInnerException in ae.Flatten().InnerExceptions)
            {
                var exNestedInnerException = exInnerException;
                do
                {
                    if (exNestedInnerException.Message.IsNotNullOrEmpty()) exceptionMessage += exNestedInnerException.Message + "\n";
                    exNestedInnerException = exNestedInnerException.InnerException;
                } while (exNestedInnerException != null);
            }

            return exceptionMessage;
        }
    }
}