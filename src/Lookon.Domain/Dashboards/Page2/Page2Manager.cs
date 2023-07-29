using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentDate;
using FluentDateTime;
using LookOn.Core.Extensions;
using LookOn.Dashboards.DashboardBases;
using LookOn.Enums;
using LookOn.Integrations.Datalytis.Components.Repositories.Interfaces;

namespace LookOn.Dashboards.Page2;

public class Page2Manager : LookOnManager
{
    private readonly Page2DataSourceManager _page2DataManager;
    private readonly Page2CacheManager               _page2CacheManager;

    // Ecom 

    // Social
    private          long                     _allSocialUsersCount;
    private readonly IDatalytisUserRepository _datalytisUserRepo;

    public Page2Manager(Page2CacheManager               page2CacheManager,
                        IDatalytisUserRepository        datalytisUserRepo,
                        Page2DataSourceManager page2DataManager)
    {
        _page2CacheManager = page2CacheManager;
        _datalytisUserRepo = datalytisUserRepo;
        _page2DataManager  = page2DataManager;
    }

    public async Task<Page2Metric> GetMetrics(Page2DataRequest request)
    {
        // load cache collection from database first
        var existingPage2Metric = await _page2CacheManager.Get(request.MerchantId);
        if (existingPage2Metric?.Metric != null && existingPage2Metric.CreatedAt >= 1.Months().Ago())
        {
            LogDebug(GetType(), "existingPage2Metric is ready");
            return existingPage2Metric.Metric;
        }

        var page2Data = await _page2DataManager.GetPage2Data(request);
        if (page2Data == null) return null;
        LogDebug(GetType(), "Data is not ready");

        var insightMetric       = await GetSocialInsightMetric(request, page2Data);
        var demographicMetric   = CalculateDemographicMetric(page2Data.SocialData);
        var locationByProvince  = CalculateLocationByProvince(page2Data.SocialData);
        var communityInteraction = CalculateCommunityInteractionMetric(page2Data.SocialData);
        var comparisionMetric   = GetComparisionMetric(page2Data.SocialData);

        var page2Metric = new Page2Metric
        {
            MerchantId          = request.MerchantId,
            SocialInsight       = insightMetric,
            Demographic         = demographicMetric,
            LocationByProvinces = locationByProvince,
            CommunityInteraction = communityInteraction,
            SocialComparision = comparisionMetric
        };

        LogDebug(GetType(), "DONE CALCULATE METRIC DATA. SAVING...");
        await _page2CacheManager.Save(page2Metric);

        return page2Metric;
    }

    private async Task<Page2Metric_SocialInsight> GetSocialInsightMetric(Page2DataRequest request, Page2DataSource data)
    {
        _allSocialUsersCount = await _datalytisUserRepo.CountUsers(request.SocialCommunityIds);

        var socialInsightMetric = new Page2Metric_SocialInsight
        {
            NonPurchasedUserCount = data.SocialData.SocialUsersNoPurchase.Count,
            NonPurchasedUserRate  = _allSocialUsersCount == 0 ? 0 : data.SocialData.SocialUsersNoPurchase.Count / (decimal)_allSocialUsersCount
        };

        if (data.SocialData.SocialInsights.Any())
        {
            // Calculate TopLikedPageRate Metric 
            var aboveNanoInfluencerSocialUsers = data.SocialData.SocialUsersNoPurchase
                                                     .Where(x => MetricGenericCalculator.MapInfluencerTypeByFollower(x.Follow + x.Friends).ToInt()
                                                              >= InfluencerTypeByFollower.Nano.ToInt())
                                                     .ToList();

            var aboveNanoInfluencerSocialUserIds = aboveNanoInfluencerSocialUsers.Select(user => user.Uid).Distinct().ToList();
            var aboveNanoInfluencerSocialUserInsightsLikedPage = data.SocialData.SocialInsights
                                                                     .Where(insight => aboveNanoInfluencerSocialUserIds.Contains(insight.Uid)
                                                                                    && insight.Type.ToEnumOrDefault<SocialInsightsType>()
                                                                                    == SocialInsightsType.Like)
                                                                     .Select(insight => insight)
                                                                     .ToList();

            if (aboveNanoInfluencerSocialUserInsightsLikedPage.Any())
            {
                var likedPageGroup = aboveNanoInfluencerSocialUserInsightsLikedPage.GroupBy(insight => insight.Name);
                likedPageGroup = likedPageGroup.OrderByDescending(insights => insights.Count()).ToList();
                var totalLikes = likedPageGroup.Sum(insights => insights.Count());

                socialInsightMetric.TopLikedPageRate = likedPageGroup.First().Count() / (decimal)totalLikes;
                socialInsightMetric.TopLikedPageName = likedPageGroup.First().Key;
            }

            // Calculate Most popular segment and Less Interest SegmentRate
            var categoryGroup = data.SocialData.SocialInsights.SelectMany(insight => insight.CategoryList).GroupBy(item => item.Name);
            categoryGroup = categoryGroup.OrderByDescending(items => items.Count()).ToList();
            var totalCategories = categoryGroup.Sum(items => items.Count());

            socialInsightMetric.MostPopularSegmentRate = categoryGroup.First().Count() / (decimal)totalCategories;
            socialInsightMetric.MostPopularSegmentName = categoryGroup.First().Key;

            socialInsightMetric.LessInterestSegmentRate = categoryGroup.Last().Count() / (decimal)totalCategories;
            socialInsightMetric.LessInterestSegmentName = categoryGroup.Last().Key;
        }

        return socialInsightMetric;
    }

    private Page2SocialMetric_Demographic CalculateDemographicMetric(Page2DataSourceSocial socialData)
    {
        var age           = MetricGenericCalculator.CalculateAgeRangeMetric(socialData.SocialUsersNoPurchase).ToList();
        var genders       = MetricGenericCalculator.CalculateGenderMetric(socialData.SocialUsersNoPurchase).ToList();
        var relationships = MetricGenericCalculator.CalculateRelationshipMetric(socialData.SocialUsersNoPurchase).ToList();

        var likedPagePurchasedOrderUserCount = socialData.SocialUsersHasPurchase.Count;
        var likedPagePurchasedOrderUserRate =
            _allSocialUsersCount == 0 ? 0 : socialData.SocialUsersHasPurchase.Count / (decimal)_allSocialUsersCount;

        return new Page2SocialMetric_Demographic
        {
            GenderMetrics                    = genders,
            AgeMetrics                       = age,
            RelationshipMetrics              = relationships,
            LikedPagePurchasedOrderUserCount = likedPagePurchasedOrderUserCount,
            LikedPagePurchasedOrderUserRate  = likedPagePurchasedOrderUserRate,

            // TODOO: Car & House
        };
    }

    private IList<SocialMetric_LocationByProvince> CalculateLocationByProvince(Page2DataSourceSocial socialData)
    {
        var items = MetricGenericCalculator.CalculateLocationByProvinceItems(socialData.SocialUsersNoPurchase)
                                           .OrderByDescending(x => x.Count)
                                           .ToList();

        var topLocations = items.Count > SocialMetric_LocationByProvince.Amount ? items.Take(SocialMetric_LocationByProvince.Amount).ToList() : items;

        return topLocations;
    }

    private SocialMetric_CommunityInteraction CalculateCommunityInteractionMetric(Page2DataSourceSocial socialData)
    {
        var topFollowerMetric = MetricGenericCalculator.CalculateTopFollowerMetric(socialData.SocialUsersNoPurchase).ToList();
        var topLikedPage = MetricGenericCalculator.CalculateTopLikedPageMetric(socialData.SocialInsights, socialData.SocialUsersNoPurchase)
                                                  .ToList();
        var topCheckinLocation = MetricGenericCalculator
                                .CalculateTopCheckinLocation(socialData.SocialInsights, socialData.SocialUsersNoPurchase)
                                .ToList();
        var topGroup = MetricGenericCalculator.CalculateTopGroup(socialData.SocialInsights, socialData.SocialUsersNoPurchase).ToList();

        return new SocialMetric_CommunityInteraction
        {
            TopFollowerMetrics  = topFollowerMetric,
            TopLikedPageMetrics = topLikedPage,
            TopCheckinLocations = topCheckinLocation,
            TopGroups           = topGroup
        };
    }

    private Page2Metric_Comparision GetComparisionMetric(Page2DataSourceSocial socialData)
    {
        var comparisionMetric = new Page2Metric_Comparision
        {
            GenderComparision       = GetGenderComparision(socialData),
            AgeComparision          = GetAgeComparision(socialData),
            RelationshipComparision = GetRelationshipComparision(socialData)
        };

        return comparisionMetric;
    }

    private GenderComparision GetGenderComparision(Page2DataSourceSocial socialData)
    {
        var genderComparision = new GenderComparision();

        var socialUserHasPurchaseGenderGroup = socialData.SocialUsersHasPurchase.Where(user => user.Gender is not GenderType.Unknown).GroupBy(user => user.Gender).ToList();
        socialUserHasPurchaseGenderGroup            = socialUserHasPurchaseGenderGroup.OrderByDescending(users => users.Count()).ToList();
        genderComparision.PurchasedGenderName = L[$"Enum:GenderType:{socialUserHasPurchaseGenderGroup.First().Key.ToInt()}"];
        genderComparision.PurchasedGenderRate =
            socialUserHasPurchaseGenderGroup.First().Count() / (decimal)socialData.SocialUsersHasPurchase.Count;

        var socialUsersNoPurchaseGenderGroup = socialData.SocialUsersNoPurchase.Where(user => user.Gender is not GenderType.Unknown).GroupBy(user => user.Gender).ToList();
        socialUsersNoPurchaseGenderGroup           = socialUsersNoPurchaseGenderGroup.OrderByDescending(users => users.Count()).ToList();
        genderComparision.NonPurchasedGenderName = L[$"Enum:GenderType:{socialUsersNoPurchaseGenderGroup.First().Key.ToInt()}"];
        genderComparision.NonPurchasedGenderRate =
            socialUsersNoPurchaseGenderGroup.First().Count() / (decimal)socialData.SocialUsersNoPurchase.Count;

        return genderComparision;
    }

    private AgeComparision GetAgeComparision(Page2DataSourceSocial socialData)
    {
        var ageComparision = new AgeComparision();

        var socialUserHasPurchaseAgeGroup = socialData.SocialUsersHasPurchase.Where(user => user.Birthday.HasValue)
                                                .GroupBy(user => MetricGenericCalculator.MapAgeSegment(user.Birthday.Value))
                                                .ToList();
        socialUserHasPurchaseAgeGroup         = socialUserHasPurchaseAgeGroup.OrderByDescending(users => users.Count()).ToList();
        ageComparision.PurchasedAge     = L[$"Enum:AgeSegmentEnum:{socialUserHasPurchaseAgeGroup.First().Key.ToInt()}"];
        ageComparision.PurchasedAgeRate = socialUserHasPurchaseAgeGroup.First().Count() / (decimal)socialData.SocialUsersHasPurchase.Count;

        var socialUserNoPurchaseAgeGroup = socialData.SocialUsersNoPurchase.Where(user => user.Birthday.HasValue)
                                               .GroupBy(user => MetricGenericCalculator.MapAgeSegment(user.Birthday.Value))
                                               .ToList();
        socialUserNoPurchaseAgeGroup         = socialUserNoPurchaseAgeGroup.OrderByDescending(users => users.Count()).ToList();
        ageComparision.NonPurchasedAge     = L[$"Enum:AgeSegmentEnum:{socialUserNoPurchaseAgeGroup.First().Key.ToInt()}"];
        ageComparision.NonPurchasedAgeRate = socialUserNoPurchaseAgeGroup.First().Count() / (decimal)socialData.SocialUsersNoPurchase.Count;

        return ageComparision;
    }

    private RelationshipComparision GetRelationshipComparision(Page2DataSourceSocial socialData)
    {
        var relationshipComparision = new RelationshipComparision();

        var socialUserHasPurchaseRelationshipGroup = socialData.SocialUsersHasPurchase.Where(user => user.RelationshipStatus is not RelationshipStatus.Unknown).GroupBy(user => user.RelationshipStatus).ToList();
        socialUserHasPurchaseRelationshipGroup = socialUserHasPurchaseRelationshipGroup.OrderByDescending(users => users.Count()).ToList();
        relationshipComparision.PurchasedRelationship = L[$"Enum:RelationshipStatus:{socialUserHasPurchaseRelationshipGroup.First().Key.ToInt()}"];
        relationshipComparision.PurchasedRelationshipRate =
            socialUserHasPurchaseRelationshipGroup.First().Count() / (decimal)socialData.SocialUsersHasPurchase.Count;

        var socialUserNoPurchaseRelationshipGroup = socialData.SocialUsersNoPurchase.Where(user => user.RelationshipStatus is not RelationshipStatus.Unknown).GroupBy(user => user.RelationshipStatus).ToList();
        socialUserNoPurchaseRelationshipGroup              = socialUserNoPurchaseRelationshipGroup.OrderByDescending(users => users.Count()).ToList();
        relationshipComparision.NonPurchasedRelationship = L[$"Enum:RelationshipStatus:{socialUserNoPurchaseRelationshipGroup.First().Key.ToInt()}"];
        relationshipComparision.NonPurchasedRelationshipRate =
            socialUserNoPurchaseRelationshipGroup.First().Count() / (decimal)socialData.SocialUsersNoPurchase.Count;

        return relationshipComparision;
    }
}