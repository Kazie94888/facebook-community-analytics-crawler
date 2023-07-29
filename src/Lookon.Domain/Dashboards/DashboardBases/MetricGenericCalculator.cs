using System;
using System.Collections.Generic;
using System.Linq;
using Castle.Core.Internal;
using IdentityServer4.Extensions;
using LookOn.Core.Extensions;
using LookOn.Core.Helpers;
using LookOn.Enums;
using LookOn.Integrations.Datalytis.Models.Entities;
using LookOn.Integrations.Haravan.Models.Entities;

namespace LookOn.Dashboards.DashboardBases;

public static class MetricGenericCalculator
{
    public static decimal Rate(decimal oldValue, decimal newValue)
    {
        return oldValue switch
        {
            0 when newValue > 0 => 1,
            0 when newValue == 0 => 0,
            _ when newValue == 0 => -1,
            _ => (newValue - oldValue) / oldValue 
        };
    }

    public static int OrderCount(List<HaravanOrder> orders)
    {
        return orders.Count;
    }

    /// <summary>
    /// REVENUE
    /// </summary>
    /// <param name="orders"></param>
    /// <returns></returns>
    public static decimal TotalSaleAmount(List<HaravanOrder> orders)
    {
        return orders.Where(o => o.TotalPrice.HasValue).Sum(o => o.TotalPrice.Value);
    }

    public static int PurchasedCustomerCount(List<HaravanOrder> orders)
    {
        return orders.Where(x => x.HaravanCustomerId.HasValue).Select(x => x.HaravanCustomerId).Distinct().Count();
    }

    public static decimal AOV(List<HaravanOrder> orders)
    {
        if (orders.IsNullOrEmpty()) return 0;
        var totalSaleAmount = TotalSaleAmount(orders);
        var orderCount      = OrderCount(orders);

        var aov = totalSaleAmount / orderCount;

        return aov;
    }

    public static decimal CLV(List<HaravanOrder> orders)
    {
        var totalSaleAmount        = TotalSaleAmount(orders);
        var purchasedCustomerCount = PurchasedCustomerCount(orders);
        if (purchasedCustomerCount == 0) return 0;

        var secondPurchasedCustomersCount = orders.Where(o => o.HaravanCustomerId.HasValue)
                                                  .GroupBy(o => o.HaravanCustomerId)
                                                  .Count(o => o.Key.HasValue && o.Count() >= 2);
        var churnRate       = 1 - ((decimal) secondPurchasedCustomersCount / purchasedCustomerCount);
        var averageLifeTime = churnRate == 0 ? 0 : 1 / churnRate;

        var clv = totalSaleAmount * averageLifeTime / purchasedCustomerCount;

        return clv;
    }

    public static int ProductCount(List<HaravanOrder> orders)
    {
        return orders.Sum(x => x.LineItems.Where(s=>s.Quantity.HasValue).Sum(s => s.Quantity.Value));
    }
    public static int CountSKUs(List<HaravanOrder> orders)
    {
        return DistinctSKUs(orders).Count;
    }


    public static List<string> DistinctSKUs(List<HaravanOrder> orders)
    {
        return orders.Where(o => o.LineItems.IsNotNullOrEmpty()).SelectMany(o => o.LineItems).Select(o => o.Sku.TrimSafe()).Distinct().ToList();
    }
 public static int CustomerCount(List<HaravanOrder> orders)
    {
        return DistinctCustomers(orders).Count;
    }


    public static List<long> DistinctCustomers(List<HaravanOrder> orders)
    {
        return orders.Where(x=>x.HaravanCustomerId.HasValue).Select(x=>x.HaravanCustomerId.Value).Distinct().ToList();
    }

    public static InfluencerTypeByFollower MapInfluencerTypeByFollower(int follower)
    {
        return follower switch
        {
            < 1000     => InfluencerTypeByFollower.Normal,
            <= 8000    => InfluencerTypeByFollower.Nano,
            <= 100000  => InfluencerTypeByFollower.Micro,
            <= 1000000 => InfluencerTypeByFollower.Macro,
            _          => InfluencerTypeByFollower.Mega
        };
    }

    public static IEnumerable<SocialMetric_AgeRange> CalculateAgeRangeMetric(IList<DatalytisUser> datalytisUsers)
    {
        if (datalytisUsers.Count == 0) yield break;

        var userGroups = datalytisUsers.Where(x => x.Birthday.HasValue).GroupBy(x => MapAgeSegment(x.Birthday.Value)).ToList();

        foreach (AgeSegmentEnum item in Enum.GetValues(typeof(AgeSegmentEnum)))
        {
            var userGroup = userGroups.FirstOrDefault(_ => _.Key == item);
            var rate      = userGroup is null ? 0 : (decimal) userGroup.Count() / datalytisUsers.Count;
            var metric    = new SocialMetric_AgeRange {AgeSegmentEnum = item, Rate = rate};
            yield return metric;
        }
    }

    public static AgeSegmentEnum MapAgeSegment(DateTime value)
    {
        return value.ToAge() switch
        {
            < 18  => AgeSegmentEnum.Age_1_17,
            <= 24 => AgeSegmentEnum.Age_18_24,
            <= 34 => AgeSegmentEnum.Age_25_34,
            <= 44 => AgeSegmentEnum.Age_35_44,
            <= 54 => AgeSegmentEnum.Age_45_54,
            <= 64 => AgeSegmentEnum.Age_55_64,
            _     => AgeSegmentEnum.Age_65_Above
        };
    }

    public static IEnumerable<SocialMetric_Gender> CalculateGenderMetric(IList<DatalytisUser> datalytisUsers)
    {
        if (datalytisUsers.Count == 0) yield break;

        var genderGroups = datalytisUsers.GroupBy(x => x.Gender).OrderByDescending(_ => _.Key).ToList();

        foreach (var gender in genderGroups.Select(group => new SocialMetric_Gender {Type = group.Key.ToString(), Count = group.Count()}))
        {
            yield return gender;
        }
    }

    public static IEnumerable<SocialMetric_Relationship> CalculateRelationshipMetric(IList<DatalytisUser> datalytisUsers)
    {
        if (datalytisUsers.Count == 0) yield break;

        var relationshipGroups = datalytisUsers.GroupBy(x => x.RelationshipStatus).OrderByDescending(_ => _.Key).ToList();

        foreach (var relationship in relationshipGroups.Select(@group => new SocialMetric_Relationship
            {
                Name = @group.Key.ToString(), Count = @group.Count()
            }))
        {
            yield return relationship;
        }
    }

    public static IEnumerable<SocialMetric_LocationByProvince> CalculateLocationByProvinceItems(IList<DatalytisUser> datalytisUsers)
    {
        if (datalytisUsers.Count == 0) yield break;

        var totalHasLocation = datalytisUsers.Count(x => x.City.IsNotNullOrWhiteSpace());
        var userGroups = datalytisUsers.Where(user => user.City.IsNotNullOrWhiteSpace()).GroupBy(x => MapLocationByProvince(x.City).Value).ToList();
        foreach (var user in userGroups)
        {
            var rate     = (decimal) user.Count() / totalHasLocation;
            var province = new SocialMetric_LocationByProvince {Name = user.Key, Rate = rate, Count = user.Count()};
            yield return province;
        }
    }

    public static KeyValuePair<int, string> MapLocationByProvince(string province)
    {
        try
        {
            var city =
                CityHelper.Names.FirstOrDefault(x => x.Value.Contains(_ => string.Equals(_, province, StringComparison.CurrentCultureIgnoreCase)));

            if (city.Equals(default(KeyValuePair<int, List<string>>)))
            {
                city = CityHelper.Names.FirstOrDefault();
            }

            return new KeyValuePair<int, string>(city.Key, city.Value.FirstOrDefault());
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public static IEnumerable<SocialMetric_TopFollower> CalculateTopFollowerMetric(IList<DatalytisUser> datalytisUsers)
    {
        if (datalytisUsers.IsNullOrEmpty()) yield break;

        var userGroups = datalytisUsers.GroupBy(x => MetricGenericCalculator.MapInfluencerTypeByFollower(x.Follow + x.Friends)).ToList();

        foreach (InfluencerTypeByFollower item in Enum.GetValues(typeof(InfluencerTypeByFollower)))
        {
            var userGroup = userGroups.FirstOrDefault(_ => _.Key == item);
            var rate      = userGroup is null ? 0 : (decimal) userGroup.Count() / datalytisUsers.Count;
            var follower  = new SocialMetric_TopFollower {InfluencerTypeByFollower = item, Rate = rate};
            yield return follower;
        }
    }

    public static IEnumerable<SocialMetric_TopLikedPage> CalculateTopLikedPageMetric(IList<DatalytisUserSocialInsight> datalytisUserSocialInsights,
                                                                                     IList<DatalytisUser>              datalytisUsers)
    {
        if (datalytisUserSocialInsights.IsNullOrEmpty() || datalytisUsers.IsNullOrEmpty()) yield break;

        var userLikeInsights = datalytisUserSocialInsights.Where(x => x.Type.ToEnumOrDefault<SocialInsightsType>() == SocialInsightsType.Like);
        var likeInsightGroups = userLikeInsights.GroupBy(x => x.Name)
                                                .OrderByDescending(x => x.Count())
                                                .Take(SocialMetric_TopLikedPage.Amount)
                                                .ToList();

        foreach (var likeInsight in likeInsightGroups)
        {
            var count    = likeInsight.Count();
            var rate     = (decimal) likeInsight.Count() / datalytisUsers.Count;
            var topLiked = new SocialMetric_TopLikedPage {Name = likeInsight.Key, Rate = rate, Count = count};
            yield return topLiked;
        }
    }

    public static IEnumerable<SocialMetric_TopCheckinLocation> CalculateTopCheckinLocation(
        IList<DatalytisUserSocialInsight> datalytisUserSocialInsights,
        IList<DatalytisUser>              datalytisUsers)
    {
        if (datalytisUserSocialInsights.IsNullOrEmpty() || datalytisUsers.IsNullOrEmpty()) yield break;

        var userLikeInsights = datalytisUserSocialInsights.Where(x => x.Type.ToEnumOrDefault<SocialInsightsType>() == SocialInsightsType.CheckIn);
        var userLikeInsightsGroups = userLikeInsights.GroupBy(x => x.Name)
                                                     .OrderByDescending(x => x.Count())
                                                     .Take(SocialMetric_TopCheckinLocation.Amount)
                                                     .ToList();

        foreach (var userLikeInsight in userLikeInsightsGroups)
        {
            var count      = userLikeInsight.Count();
            var rate       = (decimal) userLikeInsight.Count() / datalytisUsers.Count;
            var topCheckin = new SocialMetric_TopCheckinLocation {Name = userLikeInsight.Key, Rate = rate, Count = count};
            yield return topCheckin;
        }
    }

    public static IEnumerable<SocialMetric_TopGroup> CalculateTopGroup(IList<DatalytisUserSocialInsight> datalytisUserSocialInsights,
                                                                       IList<DatalytisUser>              datalytisUsers)
    {
        if (datalytisUserSocialInsights.IsNullOrEmpty() || datalytisUsers.IsNullOrEmpty()) yield break;

        var socialLikeInsights = datalytisUserSocialInsights.Where(x => x.Type.ToEnumOrDefault<SocialInsightsType>() == SocialInsightsType.Group);
        var socialLikeInsightGroups =
            socialLikeInsights.GroupBy(x => x.Name).OrderByDescending(x => x.Count()).Take(SocialMetric_TopGroup.Amount).ToList();

        foreach (var socialLikeInsight in socialLikeInsightGroups)
        {
            var count    = socialLikeInsight.Count();
            var rate     = (decimal) socialLikeInsight.Count() / datalytisUsers.Count;
            var topGroup = new SocialMetric_TopGroup {Name = socialLikeInsight.Key, Rate = rate, Count = count};
            yield return topGroup;
        }
    }
}