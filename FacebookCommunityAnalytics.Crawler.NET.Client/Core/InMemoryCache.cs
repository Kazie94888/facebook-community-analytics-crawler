// using System;
// using Microsoft.Extensions.Caching.Memory;
//
// namespace FacebookCommunityAnalytics.Crawler.NET.Client.Core
// {
//     
//     public interface ICacheService
//     {
//         T Get<T>(string cacheKey) where T : class;
//         void Set(string cacheKey, object item, int minutes);
//     }
//     
//     //https://stackoverflow.com/questions/41505612/memory-cache-in-dotnet-core
//     public class InMemoryCache : ICacheService
//     {
//         public T Get<T>(string cacheKey) where T : class
//         {
//             return MemoryCache.Default.Get(cacheKey) as T;
//         }
//         public void Set(string cacheKey, object item)
//         {
//             if (item != null)
//             {
//                 MemoryCache.Default.Add(cacheKey, item, DateTime.Now.AddMinutes(30));
//             }
//         }
//     }
// }