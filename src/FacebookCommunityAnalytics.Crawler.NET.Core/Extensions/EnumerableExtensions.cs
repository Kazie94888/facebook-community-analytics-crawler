using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace FacebookCommunityAnalytics.Crawler.NET.Core.Extensions
{
    public static class EnumerableExtensions
    {
        private static readonly Random _random = new Random();

        public static IEnumerable<T> TakeLast<T>(this IEnumerable<T> source, int n)
        {
            return source.Skip(Math.Max(0, source.Count() - n));
        }

        public static IEnumerable<T> Enumerate<T>(this IEnumerable<T[]> source)
        {
            foreach (var arr in source)
            foreach (var e in arr)
                yield return e;
        }

        public static IEnumerable<T> RefineWithFallback<T>(this IEnumerable<T> source, Func<T, bool> filter, Func<T, bool> altFilter)
        {
            if (source.Any(filter))
                return source.Where(filter);

            return source.Where(altFilter);
        }

        public static void Shuffle<T>(this IList<T> list)
        {
            var n = list.Count;
            while (n > 1)
            {
                n--;
                var k = _random.Next(n + 1);
                var value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        public static void ShuffleWell<T>(this IList<T> list)
        {
            var provider = new RNGCryptoServiceProvider();
            var n = list.Count;
            while (n > 1)
            {
                var box = new byte[1];
                do
                {
                    provider.GetBytes(box);
                } while (!(box[0] < n * (byte.MaxValue / n)));

                var k = box[0] % n;
                n--;
                var value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        public static HashSet<TElement> ToHashSet<TSource, TElement>(this IEnumerable<TSource> source, Func<TSource, TElement> elementSelector, IEqualityComparer<TElement> comparer = null)
        {
            if (source == null)
                throw new ArgumentNullException("source");
            if (elementSelector == null)
                throw new ArgumentNullException("elementSelector");

            // you can unroll this into a foreach if you want efficiency gain, but for brevity...
            return new HashSet<TElement>(source.Select(elementSelector), comparer);
        }

        public static HashSet<TSource> ToHashSet<TSource>(this IEnumerable<TSource> source)
        {
            return ToHashSet(source, o => o);
        }

        public static bool IsNullOrEmpty<T>(this IEnumerable<T> source)
        {
            return source == null || !source.Any();
        }

        public static bool IsNotNullOrEmpty<T>(this IEnumerable<T> source)
        {
            return source != null && source.Any();
        }

        public static bool Contains<T>(this IEnumerable<T> source, Func<T, bool> selector)
        {
            return source != null && source.Any(selector);
        }

        public static bool ContainsAny<TValue>(this HashSet<TValue> hashset, IEnumerable<TValue> values)
        {
            return values.Any(hashset.Contains);
        }

        public static bool ContainsAnyData<TValue>(this IEnumerable<TValue> values1, IEnumerable<TValue> values2)
        {
            if (values1 == null || values1 == null)
                return false;
            foreach (var item1 in values1)
            foreach (var item2 in values2)
                if (item1.Equals(item2))
                    return true;
            return false;
        }

        public static string ToRavenQuery<T>(this IEnumerable<T> source, string fieldName, bool wrapInQuotes = true)
        {
            return "({0})".FormatWith(source.Aggregate(string.Empty, (current, id) => current + (wrapInQuotes ? "{0}:\"{1}\" OR " : "{0}:{1} OR ").FormatWith(fieldName, id)).TrimEnd(' ', 'O', 'R'));
        }

        public static IEnumerable<TSource> ConditionalWhere<TSource>(this IEnumerable<TSource> source, bool condition, Func<TSource, bool> filter)
        {
            if (condition)
                return source.Where(filter);

            return source;
        }

        public static IEnumerable<TSource> MaybeOrderBy<TSource, TKey>(this IEnumerable<TSource> source, bool condition, Func<TSource, TKey> orderBy)
        {
            if (condition)
                return source.OrderBy(orderBy);

            return source;
        }

        public static IEnumerable<TSource> UnionIf<TSource>(this IEnumerable<TSource> source, bool condition, IEnumerable<TSource> unionSet)
        {
            return condition ? source.Union(unionSet) : source;
        }

        public static IEnumerable<TSource> UnionIf<TSource>(this IEnumerable<TSource> source, bool condition, Lazy<IEnumerable<TSource>> unionSet)
        {
            return condition ? source.Union(unionSet.Value) : source;
        }

        public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            var seenKeys = new HashSet<TKey>();
            return source.Where(element => seenKeys.Add(keySelector(element)));
        }

        public static IEnumerable<T> WithoutLast<T>(this IEnumerable<T> source)
        {
            var current = default(T);
            var first = true;
            foreach (var x in source)
            {
                if (first)
                    first = false;
                else
                    yield return current;
                current = x;
            }
        }


        public static string ToLuceneInFilter<T>(this IEnumerable<T> source, string fieldName)
        {
            return "({0})".FormatWith(source.Aggregate(string.Empty, (current, id) => current + "{0}:\"{1}\" OR ".FormatWith(fieldName, id)).TrimEnd(' ', 'O', 'R'));
        }

        /// <summary>
        ///     Turn some sequence of stuff into a string.
        /// </summary>
        /// <param name="toString">How we turn some arbitrary element into a string.</param>
        /// <param name="toStringLast">How we turn the last element into a string.  If not specified uses toString</param>
        /// <param name="toStringFirst">How we turn the first element into a string.  If not specified uses toString</param>
        /// <returns></returns>
        public static string StringConcat<T>(this IEnumerable<T> source, Func<T, string> toString, Func<T, string> toStringLast = null, Func<T, string> toStringFirst = null)
        {
            toStringLast = toStringLast ?? toString;
            toStringFirst = toStringFirst ?? toString;

            var lsource = source.ToList();
            var total = lsource.Count;

            var ii = 0;
            return lsource.Aggregate(
                new StringBuilder(),
                (acc, member) =>
                {
                    ii++;
                    var ret = acc.Append(ii == total
                        ? toStringLast(member)
                        : ii == 1
                            ? toStringFirst(member)
                            : toString(member));
                    return ret;
                },
                x => x.ToString());
        }

        /// <summary>
        ///     Turn some elements into strings then concatenate.
        /// </summary>
        /// <param name="toString">Convert each element to a string.</param>
        /// <param name="delimiter">What to put between the strings.</param>
        /// <param name="trimEnd">
        ///     Do we want to not put the delimiter at the end?  Defaults to true (i.e. remove trailing
        ///     delimiter)
        /// </param>
        /// <param name="lastDelimiter">
        ///     Do we want a different last delimiter.  This is with respect to trim end so will appear before last element
        ///     if trimEnd; at the end otherwise.
        /// </param>
        /// <returns></returns>
        public static string StringConcat<T>(this IEnumerable<T> source, Func<T, string> toString, string delimiter,
            bool trimEnd = true, string lastDelimiter = null)
        {
            return source.Select(toString).StringConcat(delimiter, trimEnd, lastDelimiter);
        }

        /// <summary>
        ///     Concatenates some strings.
        /// </summary>
        /// <param name="delimiter">What to put between the strings.</param>
        /// <param name="trimEnd">
        ///     Do we want to not put the delimiter at the end?  Defaults to true (i.e. remove trailing
        ///     delimiter)
        /// </param>
        /// <param name="lastDelimiter">
        ///     Do we want a different last delimiter.  This is with respect to trim end so will appear before last element
        ///     if trimEnd; at the end otherwise.
        /// </param>
        /// <returns></returns>
        public static string StringConcat(this IEnumerable<string> source, string delimiter, bool trimEnd = true, string lastDelimiter = null)
        {
            if (source == null)
                return string.Empty;

            var sourceList = source.ToList();

            //just deal with the special case of only one item with end trimmed as it was getting too hairy trying to 
            //work it out otherwise!
            if (sourceList.Count == 1 && trimEnd)
                return sourceList.First();

            if (lastDelimiter == null) return StringConcat(sourceList, x => x + delimiter, trimEnd ? x => x : (Func<string, string>) null);

            if (trimEnd) return StringConcat(sourceList, x => delimiter + x, x => lastDelimiter + x, x => x);

            return StringConcat(sourceList, x => x + delimiter, x => x + lastDelimiter);
        }

        public static IEnumerable<T> OrIfNoneThen<T>(this IEnumerable<T> enumerable, params T[] ifNone)
        {
            return enumerable.Any() ? enumerable : ifNone;
        }

        public static IEnumerable<IEnumerable<T>> Split<T>(this IEnumerable<T> list, int parts)
        {
            var i = 0;
            var splits = from item in list
                group item by i++ % parts
                into part
                select part.AsEnumerable();
            return splits;
        }

        public static IEnumerable<IEnumerable<T>> SplitInTwo<T>(this IEnumerable<T> list)
        {
            var eachLength = list.Count() / 2;
            var remainder = list.Count() % 2;

            var set1 = list.Take(eachLength + remainder);
            var set2 = list.Skip(eachLength + remainder).Take(eachLength);

            return new List<IEnumerable<T>> {set1, set2};
        }

        /// <summary>
        ///     Method to partition an IEnumerable into chunks, courtesy of Jon Skeet
        ///     http://stackoverflow.com/questions/438188/split-a-collection-into-n-parts-with-linq/438208#438208
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public static IEnumerable<IEnumerable<T>> Partition<T>(this IEnumerable<T> source, int size)
        {
            T[] array = null;
            var count = 0;
            foreach (var item in source)
            {
                if (array == null) array = new T[size];
                array[count] = item;
                count++;
                if (count == size)
                {
                    yield return new ReadOnlyCollection<T>(array);
                    array = null;
                    count = 0;
                }
            }

            if (array != null)
            {
                Array.Resize(ref array, count);
                yield return new ReadOnlyCollection<T>(array);
            }
        }

        public static int RemoveWhere<T>(this List<T> list, Func<T, bool> filter)
        {
            var removeCount = 0;
            foreach (var match in list.Where(filter).ToList())
            {
                removeCount++;
                list.Remove(match);
            }

            return removeCount;
        }

        public static Dictionary<TK, TV> FilterDict<TK, TV>(this Dictionary<TK, TV> dict, Func<KeyValuePair<TK, TV>, bool> where)
        {
            return dict.Where(where).ToDictionary(x => x.Key, x => x.Value);
        }

        public static IEnumerable<T> WrapInEnumerable<T>(this T obj, bool emptyIfNull = true)
        {
            if (emptyIfNull && obj == null)
                yield break;

            yield return obj;
        }

        public static IEnumerable<T> UnionParams<T>(this IEnumerable<T> sequence, params T[] extras)
        {
            return sequence.Union(extras.AsEnumerable());
        }

        public static string GetName<T>(this T enumValue) where T : struct, IConvertible
        {
            return !typeof(T).IsEnum ? null : Enum.GetName(typeof(T), enumValue);
        }

        public static List<string> GetNames<T>() where T : struct
        {
            return Enum.GetValues(typeof(T))
                .OfType<T>()
                .Select(s => s.ToString()).ToList();
        }

        public static IEnumerable<IEnumerable<T>> CartesianProduct<T>(this IEnumerable<IEnumerable<T>> sequences)
        {
            IEnumerable<IEnumerable<T>> emptyProduct = new[] {Enumerable.Empty<T>()};
            return sequences.Aggregate(
                emptyProduct,
                (accumulator, sequence) =>
                    from acc in accumulator
                    from item in sequence
                    select acc.Concat(new[] {item}));
        }

        public static IEnumerable<T> TakeFactor<T>(this ICollection<T> source, double factor, bool doCeiling = true)
        {
            var takeCount = source.Count * factor;

            var count = 0;
            if (doCeiling)
                count = (int) Math.Ceiling(takeCount);
            else
                count = (int) Math.Round(takeCount);

            return source.Take(count);
        }

        public static string GetDescription<T>(this T enumValue) where T : class
        {
            if (!typeof(T).IsEnum)
                return null;

            var description = enumValue.ToString();
            var fieldInfo = enumValue.GetType().GetField(enumValue.ToString() ?? string.Empty);

            if (fieldInfo != null)
            {
                var attrs = fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), true);
                if (attrs.Length > 0) description = ((DescriptionAttribute) attrs[0]).Description;
            }

            return description;
        }

        public static string GetDescriptionOrName<T>(this T enumValue) where T : struct, IConvertible
        {
            if (!typeof(T).IsEnum) return null;

            var result = enumValue.ToString();
            var fieldInfo = enumValue.GetType().GetField(enumValue.ToString() ?? string.Empty);

            if (fieldInfo != null)
            {
                var attrs = fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), true);
                if (attrs.Length > 0) result = ((DescriptionAttribute) attrs[0]).Description;
            }
            else
            {
                result = Enum.GetName(typeof(T), enumValue);
            }

            return result;
        }
    }
}