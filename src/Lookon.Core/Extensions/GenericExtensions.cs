#nullable enable
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using Newtonsoft.Json;
using PhoneNumbers;

namespace LookOn.Core.Extensions
{
    public static class ObjectExtensions
    {
        public static T? Clone<T>(this T source)
        {
            var serialized = JsonConvert.SerializeObject(source);
            return JsonConvert.DeserializeObject<T>(serialized);
        }
    }

    public static class GenericExtensions
    {
        //public static bool IsIn(this string value, IEnumerable<string> candidates, StringComparison stringComparison = StringComparison.InvariantCultureIgnoreCase)
        //{
        //    return value.IsIn(candidates.ToArray(), stringComparison);
        //}
        //public static bool IsIn(this string value, string[] candidates, StringComparison stringComparison = StringComparison.InvariantCultureIgnoreCase)
        //{
        //    return candidates.Any(c => c?.Trim().Equals(value.Trim(), stringComparison) ?? false);
        //}

        //public static bool IsIn<T>(this T obj, params T[] candidates)
        //{
        //    return candidates.Any(c => c == null ? obj == null : c.Equals(obj));
        //}
        //public static bool IsNotIn<T>(this T obj, params T[] candidates)
        //{
        //    return !obj.IsIn(candidates);
        //}

        //public static bool IsIn<T>(this T obj, IEnumerable<T> candidates)
        //{
        //    return obj != null && obj.IsIn(candidates.ToArray());
        //}
        //public static bool IsNotIn<T>(this T obj, IEnumerable<T> candidates)
        //{
        //    return !obj.IsIn(candidates.ToArray());
        //}

        //public static bool NotIn<T>(this T source, params T[] candidates)
        //{
        //    return !candidates.Any(c => c == null ? source == null : c.Equals(source));
        //}

        //public static bool NotIn<T>(this T source, IEnumerable<T> candidates)
        //{
        //    return source.NotIn(candidates.ToArray());
        //}

        public static string ToInternationalPhoneNumber(this string? phone, string defaultRegion = "vn")
        {
            if (phone.IsNullOrSpace()) return string.Empty;
            phone = phone?.Trim();
            var pn                       = PhoneNumberUtil.GetInstance().Parse(phone, defaultRegion.ToUpper());
            var internationalPhoneNumber = PhoneNumberUtil.GetInstance().Format(pn, PhoneNumberFormat.INTERNATIONAL);

            internationalPhoneNumber = internationalPhoneNumber.Replace(" ", "");
            return internationalPhoneNumber;
        }
        
        public static string ToInternationalPhoneNumberFromVN(this string? phone)
        {
            return phone.ToInternationalPhoneNumber("VN");
        }

        public static string IfNotNullOrEmpty(this string obj, Func<string, string> getter, string? valueIfNotPoss = null)
        {
            return (obj.IsNotNullOrEmpty() ? getter(obj) : valueIfNotPoss) ?? string.Empty;
        }

        public static T? AsNullable<T>(this T t) where T : struct
        {
            return t;
        }

        public static T ToEnumOrDefault<T>(this int i) where T : struct, IConvertible
        {
            var canParse = Enum.TryParse<T>(i.ToString(), true, out var @enum);
            return canParse ? @enum : default(T);
        }

        public static T ToEnumOrDefault<T>(this string s) where T : struct, IConvertible
        {
            if (!s.IsNotNullOrWhiteSpace()) return default;
            s = s.Trim();
            var canParse = Enum.TryParse<T>(s, true, out var enumValue);
            if (canParse)
            {
                return enumValue;
            }

            var type = typeof(T);
            foreach (var field in type.GetFields())
            {
                if (Attribute.GetCustomAttribute(field, typeof(DisplayAttribute)) is DisplayAttribute attribute)
                {
                    if (attribute.Name == s)
                    {
                        return (T)field.GetValue(null);
                    }
                }

                if (field.Name == s)
                {
                    return (T)field.GetValue(null);
                }
            }

            return default;
        }
        public static T ToEnumOrDefaultIgnoreCase<T>(this string s) where T : struct, IConvertible
        {
            if (!s.IsNotNullOrWhiteSpace()) return default;
            s = s.Trim();
            var canParse = Enum.TryParse<T>(s, true, out var enumValue);
            if (canParse)
            {
                return enumValue;
            }

            var type = typeof(T);
            foreach (var field in type.GetFields())
            {
                if (Attribute.GetCustomAttribute(field, typeof(DisplayAttribute)) is DisplayAttribute attribute)
                {
                    if (attribute.Name.Equals(s,StringComparison.InvariantCultureIgnoreCase))
                    {
                        return (T)field.GetValue(null);
                    }
                }

                if (field.Name == s)
                {
                    return (T)field.GetValue(null);
                }
            }

            return default;
        }

        public static bool InRange(this int i, int bottom, int top, bool inclusive = true)
        {
            return inclusive ? bottom <= i && i <= top : bottom < i && i < top;
        }

        public static int ToInt<T>(this T e) where T : struct, IConvertible
        {
            if (!typeof(T).IsEnum) throw new ArgumentException("T must be an enumerated type");
            return (int)(IConvertible)e;
        }

        public static int ToIntOrDefault(this int? i, int defaultValue = 0)
        {
            return i ?? 0;
        }

        public static double ToDouble(this decimal value)
        {
            return Convert.ToDouble(value);
        }

        public static decimal ToDecimal(this double value)
        {
            return Convert.ToDecimal(value);
        }

        public static void AddRange<T>(this ConcurrentBag<T> @this, IEnumerable<T> toAdd)
        {
            foreach (var element in toAdd)
            {
                @this.Add(element);
            }
        }

        public static string GetAggregateExceptionMessage(this AggregateException ae)
        {
            var exceptionMessage = string.Empty;
            foreach (Exception? exInnerException in ae.Flatten().InnerExceptions)
            {
                Exception? exNestedInnerException = exInnerException;
                do
                {
                    if (exNestedInnerException.Message.IsNotNullOrEmpty())
                    {
                        exceptionMessage += exNestedInnerException.Message + "\n";
                    }

                    exNestedInnerException = exNestedInnerException.InnerException;
                } while (exNestedInnerException != null);
            }

            return exceptionMessage;
        }
        
        public static string NumberFormatForCss(this double value)
        {
            return value.ToString(CultureInfo.InvariantCulture);
        }
    }
}