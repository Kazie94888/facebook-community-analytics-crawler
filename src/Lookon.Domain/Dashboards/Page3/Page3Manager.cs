using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentDate;
using FluentDateTime;
using LookOn.Dashboards.DashboardBases;
using LookOn.Integrations.Datalytis.Components.Repositories.Interfaces;

namespace LookOn.Dashboards.Page3;

public class Page3Manager : LookOnManager
{
    private readonly DashboardPage3DataSourceManager _page3DataManager;
    private readonly Page3CacheManager               _page3CacheManager;
    private readonly IDatalytisUserRepository        _datalytisUserRepo;

    public Page3Manager(DashboardPage3DataSourceManager page3DataManager, Page3CacheManager page3CacheManager, IDatalytisUserRepository datalytisUserRepo)
    {
        _page3DataManager       = page3DataManager;
        _page3CacheManager      = page3CacheManager;
        _datalytisUserRepo = datalytisUserRepo;
    }

    public async Task<Page3Metric> GetMetrics(Page3DataRequest request)
    {
        // load cache collection from database first
        var existingPage3Metrics = await _page3CacheManager.Get(request.MerchantId);
        if (existingPage3Metrics.Any())
        {
            // var existingPage3Metric = existingPage3Metrics.FirstOrDefault(entity => entity.Filter == request.Filter || entity.Filter.IsMatching(request.Filter));
            var existingPage3Metric = existingPage3Metrics
               .FirstOrDefault(entity => (request.Filter == null && request.Filter == entity.Filter)
                                      || (request.Filter != null && entity.Filter != null && entity.Filter.IsMatching(request.Filter)));
            if (existingPage3Metric != null && existingPage3Metric.CreatedAt >= 1.Months().Ago())
            {
                LogDebug(GetType(), "existingPage3Metric is ready");
                return existingPage3Metric.Metric;
            }
        }
        
        var page3Data = await _page3DataManager.GetPage3Data(request);
        if (page3Data == null) return null;
        LogDebug(GetType(), "Data is not ready");
        
        var demographicMetric   = await CalculateDemographicMetric(request, page3Data);
        var communityInteraction = CalculateCommunityInteractionMetric(page3Data);
        var locationByProvince  = CalculateLocationByProvince(page3Data.SocialData);

        var page3Metric = new Page3Metric
        {
            MerchantId = request.MerchantId, Demographic = demographicMetric, CommunityInteraction = communityInteraction, LocationByProvinces = locationByProvince
        };
        
        LogDebug(GetType(), "DONE CALCULATE METRIC DATA. SAVING...");
        await _page3CacheManager.Save(page3Metric, request.Filter);

        return page3Metric;
    }

    private async Task<Page3SocialMetric_Demographic> CalculateDemographicMetric(Page3DataRequest request, Page3DataSource data)
    {
        var age           = MetricGenericCalculator.CalculateAgeRangeMetric(data.SocialData.SocialUsers).ToList();
        var genders       = MetricGenericCalculator.CalculateGenderMetric(data.SocialData.SocialUsers).ToList();
        var relationships = MetricGenericCalculator.CalculateRelationshipMetric(data.SocialData.SocialUsers).ToList();

        var allSocialUsersCount                = await _datalytisUserRepo.CountUsers(request.SocialCommunityIds);
        var likedPageNoPurchaseOrderUserCount = data.SocialData.SocialUsersNoPurchase.Count;
        var likedPageNoPurchaseOrderUserRate =
            allSocialUsersCount == 0 ? 0 : data.SocialData.SocialUsersNoPurchase.Count / (decimal)allSocialUsersCount;
        
        return new Page3SocialMetric_Demographic
        {
            GenderMetrics                      = genders,
            AgeMetrics                         = age,
            RelationshipMetrics                = relationships,
            LikedPageNoPurchasedOrderUserCount = likedPageNoPurchaseOrderUserCount,
            LikedPageNoPurchasedOrderUserRate  = likedPageNoPurchaseOrderUserRate,

            // TODOO: Car & House
        };
    }
    
    private SocialMetric_CommunityInteraction CalculateCommunityInteractionMetric(Page3DataSource data)
    {
        var topFollowerMetric = MetricGenericCalculator.CalculateTopFollowerMetric(data.SocialData.SocialUsers).ToList();
        var topLikedPage = MetricGenericCalculator.CalculateTopLikedPageMetric(data.SocialData.SocialInsights, data.SocialData.SocialUsers)
                                                  .ToList();
        var topCheckinLocation = MetricGenericCalculator
                                .CalculateTopCheckinLocation(data.SocialData.SocialInsights, data.SocialData.SocialUsers)
                                .ToList();
        var topGroup = MetricGenericCalculator.CalculateTopGroup(data.SocialData.SocialInsights, data.SocialData.SocialUsers).ToList();

        return new SocialMetric_CommunityInteraction
        {
            TopFollowerMetrics  = topFollowerMetric,
            TopLikedPageMetrics = topLikedPage,
            TopCheckinLocations = topCheckinLocation,
            TopGroups           = topGroup
        };
    }
    
    private IList<SocialMetric_LocationByProvince> CalculateLocationByProvince(Page3DataSourceSocial socialData)
    {
        var items = MetricGenericCalculator.CalculateLocationByProvinceItems(socialData.SocialUsers)
                                           .OrderByDescending(x => x.Count)
                                           .ToList();

        var topLocations = items.Count > SocialMetric_LocationByProvince.Amount ? items.Take(SocialMetric_LocationByProvince.Amount).ToList() : items;

        return topLocations;
    }
}