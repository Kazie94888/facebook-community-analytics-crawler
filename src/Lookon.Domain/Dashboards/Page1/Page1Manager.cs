using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentDate;
using FluentDateTime;
using Hangfire.Dashboard;
using LookOn.Core.Extensions;
using LookOn.Dashboards.DashboardBases;
using LookOn.Enums;
using LookOn.Integrations.Haravan;
using LookOn.Integrations.Haravan.Models.Entities;
using LookOn.Integrations.Haravan.Models.ValueObjects;
using LookOn.Merchants;

namespace LookOn.Dashboards.Page1;

public class Page1Manager : LookOnManager
{
    private readonly Page1DataSourceManager _page1DataManager;

    // social fields
    // private List<DatalytisUser>              _users;
    // private int                              _userCount;
    // private List<DatalytisUserSocialInsight> _insights;
    // private int                              _insightsCount;
    private readonly MerchantManager _merchantManager;

    // cache manager
    private readonly Page1CacheManager _page1CacheManager;

    // ecom
    private readonly HaravanOrderManager    _haravanOrderManager;
    private readonly HaravanCustomerManager _haravanCustomerManager;

    // ecom fields
    // private List<HaravanOrder>                 _ecomOrders;
    // private List<HaravanOrder>                 orders;
    // private int                                currentHaravanOrders.Count;
    // private List<HaravanOrder>                 previousHaravanOrders;
    // private int                                previousHaravanOrders.Count;
    // private List<HaravanCustomerPhoneNoAndEmail> _allHaravanCustomerPhoneNoEmails;

    public Page1Manager(HaravanOrderManager    haravanOrderManager,
                        HaravanCustomerManager haravanCustomerManager,
                        Page1CacheManager      page1CacheManager,
                        Page1DataSourceManager page1DataManager,
                        MerchantManager        merchantManager)
    {
        _haravanOrderManager    = haravanOrderManager;
        _haravanCustomerManager = haravanCustomerManager;
        _page1CacheManager      = page1CacheManager;
        _page1DataManager       = page1DataManager;
        _merchantManager        = merchantManager;
    }

    public async Task<Page1Metric> GetMetrics(Page1DataRequest request)
    {
        await _page1DataManager.InitMerchantSyncInfo(request.MerchantId);

        // load cache collection from database first
        var metric = await GetExistingMetric(request);
        if (metric is { Advanced: { }, Ecom: { }, Social: { }, PurchaseShortTermSocial: { } }) return metric;

        metric = new Page1Metric { MerchantId = request.MerchantId, TimeFrameType = request.TimeFrame, From = request.From, To = request.From };

        // get ecom metrics
        var page1DataSource = await GetPage1DataSource(request, metric);
        var ecomData        = page1DataSource.Item1;
        var socialData      = page1DataSource.Item2;

        metric.Social ??= GetSocialMetrics(socialData);

        var currentDateTime = DateTime.UtcNow;

        metric.Ecom ??= GetEcomMetrics(request, ecomData, currentDateTime);

        metric.Advanced ??= await GetAdvancedMetrics(request,
                                                     ecomData,
                                                     socialData,
                                                     currentDateTime);

        metric.PurchaseShortTermSocial ??= GetShortTermSocialMetric(ecomData, socialData, currentDateTime);
        metric.UpdatedAt       =   DateTime.UtcNow;

        await _page1CacheManager.Save(metric);

        return metric;
    }

    private async Task<Tuple<Page1DataSourceEcom, Page1DataSourceSocial>> GetPage1DataSource(Page1DataRequest request, Page1Metric metric)
    {
        Page1DataSourceEcom   ecomData   = null;
        Page1DataSourceSocial socialData = null;
        if (metric.Ecom is null)
        {
            ecomData = await _page1DataManager.GetPage1DataEcom(request);
        }

        if (metric.Advanced is null || metric.PurchaseShortTermSocial is null)
        {
            socialData =   await _page1DataManager.GetPage1DataSocial(request);
            ecomData   ??= await _page1DataManager.GetPage1DataEcom(request);
        }

        if (metric.Social is null)
        {
            socialData ??= await _page1DataManager.GetPage1DataSocial(request);
        }

        return new Tuple<Page1DataSourceEcom, Page1DataSourceSocial>(ecomData, socialData);
    }

    private async Task<Page1Metric> GetExistingMetric(Page1DataRequest request)
    {
        var existingPage1Metric = await _page1CacheManager.Get(request.MerchantId, request.TimeFrame);
        if (existingPage1Metric == null) return null;

        var metric = new Page1Metric { MerchantId = request.MerchantId, TimeFrameType = request.TimeFrame, From = request.From, To = request.From };

        if (existingPage1Metric.Metric.Social != null && existingPage1Metric.Metric.Social.CreatedAt >= 1.Months().Ago())
        {
            LogDebug(GetType(), "existingPage1SocialMetric is ready");
            metric.Social = existingPage1Metric.Metric.Social;
        }

        /// TODOO CHECK THIS
        // if (existingPage1Metric.Metric.Advanced != null && existingPage1Metric.Metric.Advanced.CreatedAt >= 1.Days().Ago())
        // {
        //     LogDebug(GetType(), "existingPage1AdvanceMetric is ready");
        //     metric.Advanced = existingPage1Metric.Metric.Advanced;
        // }

        // TODOO Vu Nguyen improve this logic
        // if (existingPage1Metric.Metric.Ecom != null && existingPage1Metric.Metric.Ecom.CreatedAt >= DateTime.UtcNow.AddHours(-12))
        if (existingPage1Metric.Metric.Ecom != null)
        {
            LogDebug(GetType(), "existingPage1EcomMetric is ready");
            metric.Ecom = existingPage1Metric.Metric.Ecom;
        }

        if (existingPage1Metric.Metric.PurchaseShortTermSocial != null && existingPage1Metric.Metric.PurchaseShortTermSocial.CreatedAt >= 1.Days().Ago())
        {
            LogDebug(GetType(), "existingPage1ShortTermSocialMetric is ready");
            metric.PurchaseShortTermSocial = existingPage1Metric.Metric.PurchaseShortTermSocial;
        }

        metric.UpdatedAt = existingPage1Metric.Metric.UpdatedAt ?? DateTime.UtcNow;

        return metric;
    }

    #region ECOM

    private List<HaravanOrder> GetCurrentTimeFrameOrders(Page1DataRequest request, Page1DataSourceEcom page1DataSourceEcom)
    {
        return page1DataSourceEcom.EcomOrders.Where(x => x.CreatedAt.HasValue
                                                      && x.ConfirmedAt.HasValue
                                                      && x.CreatedAt.HasValue
                                                      && x.CreatedAt.Value >= request.From
                                                      && x.CreatedAt.Value <= request.To)
                                  .DistinctBy(o => o.Id)
                                  .ToList();
    }

    private List<long> GetCurrentTimeFrameCustomerIds(Page1DataRequest request, Page1DataSourceEcom page1DataSourceEcom)
    {
        return GetCurrentTimeFrameOrders(request, page1DataSourceEcom)
              .Where(o => o.HaravanCustomerId != null)
              .Select(o => o.HaravanCustomerId.Value)
              .Distinct()
              .ToList();
    }

    private int CountCurrentTimeFrameCustomers(Page1DataRequest request, Page1DataSourceEcom page1DataSourceEcom)
    {
        return GetCurrentTimeFrameCustomerIds(request, page1DataSourceEcom).Count;
    }

    private Page1Metric_Ecom GetEcomMetrics(Page1DataRequest request, Page1DataSourceEcom ecomData, DateTime currentDateTime)
    {
        if (ecomData is null) return new Page1Metric_Ecom { Summary = new EcomMetric_Summary(), RevenueSummary = new EcomMetric_RevenueSummary(), CreatedAt = currentDateTime };

        // current orders
        var currentHaravanOrders = GetCurrentTimeFrameOrders(request, ecomData);

        // previous orders
        var previousDateTime = request.TimeFrame.GetDateByTimeFrameType(request.From, false);
        var previousFromDate = previousDateTime.Item1;
        var previousEndDate  = previousDateTime.Item2;
        var previousHaravanOrders = ecomData.EcomOrders
                                            .Where(x => x.CreatedAt.HasValue
                                                     && x.CreatedAt.Value >= previousFromDate
                                                     && x.CreatedAt.Value <= previousEndDate)
                                            .DistinctBy(o => o.Id)
                                            .ToList();

        var previousDateTimeOfPreviousDateTime = request.TimeFrame.GetDateByTimeFrameType(previousFromDate, false);
        var fromDate                           = previousDateTimeOfPreviousDateTime.Item1;
        var endDate                            = previousDateTimeOfPreviousDateTime.Item2;
        var previousOfPreviousHaravanOrders = ecomData.AllEcomOrders
                                                      .Where(x => x.CreatedAt.HasValue
                                                               && x.CreatedAt.Value >= fromDate
                                                               && x.CreatedAt.Value <= endDate)
                                                      .DistinctBy(o => o.Id)
                                                      .ToList();

        if (currentHaravanOrders.Count is 0)
            return new Page1Metric_Ecom { Summary = new EcomMetric_Summary(), RevenueSummary = new EcomMetric_RevenueSummary(), CreatedAt = currentDateTime };

        // METRICS - ECOM SUMMARY
        var ecomSummary = GetEcomSummary(ecomData,
                                         previousHaravanOrders,
                                         previousOfPreviousHaravanOrders,
                                         currentHaravanOrders,
                                         request.From,
                                         previousFromDate);

        // EcomMetric_RevenueSummary
        var ecomRevenueSummary = GetEcomRevenueSummary(previousHaravanOrders,
                                                       currentHaravanOrders,
                                                       request.From,
                                                       request.To);

        // EcomMetric_RevenueByProduct
        var ecomRevenueByProducts = GetEcomRevenueByProducts(currentHaravanOrders);

        // EcomMetric_RevenueByLocation
        var ecomRevenueByLocations = GetEcomRevenueByLocations(currentHaravanOrders);

        var metrics = new Page1Metric_Ecom
        {
            Summary            = ecomSummary,
            RevenueSummary     = ecomRevenueSummary,
            RevenueByProducts  = ecomRevenueByProducts,
            RevenueByLocations = ecomRevenueByLocations,
            CreatedAt          = currentDateTime
        };

        return metrics;
    }

    private List<EcomMetric_RevenueByLocation> GetEcomRevenueByLocations(List<HaravanOrder> orders)
    {
        var revenueCurrent            = MetricGenericCalculator.TotalSaleAmount(orders);
        var ordersHaveShippingAddress = orders.Where(o => o.LineItems.IsNotNullOrEmpty() && o.ShippingAddress != null).ToList();
        var dic                       = new Dictionary<string, decimal>();

        foreach (var item in ordersHaveShippingAddress)
        {
            var province = item.ShippingAddress.Province.IsNotNullOrWhiteSpace() ? item.ShippingAddress.Province : item.ShippingAddress.City;
            province = province.TrimSafe();
            if (province.IsNullOrSpace()) continue;

            if (!dic.ContainsKey(province)) dic.Add(province, 0);

            var provinceOrders = ordersHaveShippingAddress.Where(o => o.ShippingAddress.Province == province || o.ShippingAddress.City == province)
                                                          .ToList();

            dic[province] = provinceOrders.Sum(o => o.TotalPrice ?? 0);
        }

        var ecomRevenueByLocations = dic.OrderByDescending(entry => entry.Value)
                                        .Take(EcomMetric_RevenueByLocation.Amount)
                                        .Select(x => new EcomMetric_RevenueByLocation { Name = x.Key, Value = x.Value, Rate = x.Value / revenueCurrent })
                                        .ToList();
        return ecomRevenueByLocations;
    }

    private List<EcomMetric_RevenueByProduct> GetEcomRevenueByProducts(List<HaravanOrder> orders)
    {
        var revenueCurrent = MetricGenericCalculator.TotalSaleAmount(orders);
        var ecomRevenueByProducts = orders.Where(o => o.LineItems.IsNotNullOrEmpty())
                                          .SelectMany(o => o.LineItems)
                                          .GroupBy(item => item.Name)
                                          .Select(g => new EcomMetric_RevenueByProduct
                                           {
                                               ProductName = g.Key,
                                               SaleAmount  = g.Sum(item => item.Price ?? (item.Price ?? 0)),
                                               Rate        = g.Sum(item => item.Price ?? (item.Price ?? 0)) / revenueCurrent
                                           })
                                          .OrderByDescending(x => x.Rate)
                                          .Take(EcomMetric_RevenueByProduct.Amount)
                                          .ToList();
        return ecomRevenueByProducts;
    }

    private EcomMetric_RevenueSummary GetEcomRevenueSummary(List<HaravanOrder> previousHaravanOrders,
                                                            List<HaravanOrder> currentHaravanOrders,
                                                            DateTime           fromDateTime,
                                                            DateTime           endDateTime)
    {
        fromDateTime = fromDateTime.Date;
        endDateTime  = endDateTime.Date.AddDays(1);
        
        var ecomRevenueSummary = new EcomMetric_RevenueSummary();
        var revenueCurrent     = MetricGenericCalculator.TotalSaleAmount(currentHaravanOrders);
        var revenueByDates = fromDateTime.To(endDateTime)
                                         .Select(date => new EcomMetric_RevenueByDate
                                          {
                                              Revenue = currentHaravanOrders
                                                       .Where(o => o.CreatedAt.HasValue && o.CreatedAt.Value.Date == date)
                                                       .Sum(o => o.TotalPrice ?? 0),
                                              DateTime = date
                                          })
                                         .OrderBy(_ => _.DateTime)
                                         .ToList();

        var revenueToday           = revenueByDates.LastOrDefault()?.Revenue ?? 0m;
        var revenueADayBeforeToday = revenueByDates.SecondLast()?.Revenue    ?? 0m;
        var revenueTodayGrowthRate = MetricGenericCalculator.Rate(revenueADayBeforeToday, revenueToday);

        ecomRevenueSummary.RevenueToday           = revenueToday;
        ecomRevenueSummary.RevenueTodayGrowthRate = revenueTodayGrowthRate;
        ecomRevenueSummary.Revenue                = revenueCurrent;
        ecomRevenueSummary.RevenueByDates         = revenueByDates;

        if (previousHaravanOrders.IsNotNullOrEmpty())
        {
            var revenuePrevious   = MetricGenericCalculator.TotalSaleAmount(previousHaravanOrders);
            var revenueGrowthRate = MetricGenericCalculator.Rate(revenuePrevious, revenueCurrent);
            ecomRevenueSummary.RevenueGrowthRate = revenueGrowthRate;
        }

        return ecomRevenueSummary;
    }

    private EcomMetric_Summary GetEcomSummary(Page1DataSourceEcom ecomData,
                                              List<HaravanOrder>  previousHaravanOrders,
                                              List<HaravanOrder>  previousOfPreviousHaravanOrders,
                                              List<HaravanOrder>  currentHaravanOrders,
                                              DateTime            fromDateTime,
                                              DateTime            previousFromDate)
    {
        var ecomSummary = new EcomMetric_Summary();

        // if no previous haravan order no need to calculate changing rate
        var hadPreviousHaravanOrder = previousHaravanOrders.IsNotNullOrEmpty();

        // 1
        ecomSummary.OrderCount = currentHaravanOrders.Count;
        if (hadPreviousHaravanOrder)
        {
            ecomSummary.OrderGrowthRate = MetricGenericCalculator.Rate(previousHaravanOrders.Count, currentHaravanOrders.Count);
        }

        // 2
        var purchasedCustomerCountCurrent = MetricGenericCalculator.PurchasedCustomerCount(currentHaravanOrders);
        ecomSummary.PurchasedCustomerCount = purchasedCustomerCountCurrent;
        if (hadPreviousHaravanOrder)
        {
            var purchasedCustomerCountPrevious = MetricGenericCalculator.PurchasedCustomerCount(previousHaravanOrders);
            var purchasedCustomerGrowthRate    = MetricGenericCalculator.Rate(purchasedCustomerCountCurrent, purchasedCustomerCountPrevious);
            ecomSummary.PurchasedCustomerGrowthRate = purchasedCustomerGrowthRate;
        }

        // 3 - AOV
        var aovCurrent = MetricGenericCalculator.AOV(currentHaravanOrders);
        ecomSummary.AOV = aovCurrent;

        if (hadPreviousHaravanOrder)
        {
            var aovPrevious   = MetricGenericCalculator.AOV(previousHaravanOrders);
            var aovGrowthRate = MetricGenericCalculator.Rate(aovPrevious, aovCurrent);
            ecomSummary.AOVGrowthRate = aovGrowthRate;
        }

        // 4- CLV
        var clvCurrent = MetricGenericCalculator.CLV(currentHaravanOrders);
        ecomSummary.CLV = clvCurrent;
        if (hadPreviousHaravanOrder)
        {
            var clvPrevious   = MetricGenericCalculator.CLV(previousHaravanOrders);
            var clvGrowthDate = MetricGenericCalculator.Rate(clvCurrent, clvPrevious);
            ecomSummary.CLVGrowthRate = clvGrowthDate;
        }

        // 5 - Returned OrderSyncInfo
        var returnedOrdersCurrent     = _haravanOrderManager.GetReturnedOrders(currentHaravanOrders);
        var returnedOrdersRateCurrent = (decimal)returnedOrdersCurrent.Count / currentHaravanOrders.Count;
        ecomSummary.ReturnedOrderRate = returnedOrdersRateCurrent;
        if (hadPreviousHaravanOrder)
        {
            var returnedOrdersPrevious     = _haravanOrderManager.GetReturnedOrders(previousHaravanOrders);
            var returnedOrdersRatePrevious = (decimal)returnedOrdersPrevious.Count / previousHaravanOrders.Count;
            var returnedOrderGrowthRate    = MetricGenericCalculator.Rate(returnedOrdersRatePrevious, returnedOrdersRateCurrent);
            ecomSummary.ReturnedOrderGrowthRate = returnedOrderGrowthRate;
        }

        // 6 - Cancelled OrderSyncInfo
        var cancelledOrdersCurrent     = _haravanOrderManager.GetCancelledOrders(currentHaravanOrders);
        var cancelledOrdersRateCurrent = (decimal)cancelledOrdersCurrent.Count / currentHaravanOrders.Count;
        ecomSummary.CancelledOrderRate = cancelledOrdersRateCurrent;
        if (hadPreviousHaravanOrder)
        {
            var cancelledOrdersPrevious     = _haravanOrderManager.GetCancelledOrders(previousHaravanOrders);
            var cancelledOrdersRatePrevious = (decimal)cancelledOrdersPrevious.Count / previousHaravanOrders.Count;
            var cancelledOrderGrowthRate    = MetricGenericCalculator.Rate(cancelledOrdersRatePrevious, cancelledOrdersRateCurrent);
            ecomSummary.CancelledOrderGrowthRate = cancelledOrderGrowthRate;
        }

        // 7 - Retention Rate
        var customerIds = currentHaravanOrders.Where(order => order.HaravanCustomerId.HasValue)
                                              .Select(order => order.HaravanCustomerId.Value)
                                              .Distinct()
                                              .ToList();

        var customerIdsOfPreviousTimeFrame = previousHaravanOrders.Where(order => order.HaravanCustomerId.HasValue)
                                                                  .Select(order => order.HaravanCustomerId.Value)
                                                                  .Distinct()
                                                                  .ToList();

        var retentionCustomerIds = customerIds.Intersect(customerIdsOfPreviousTimeFrame).ToList();

        ecomSummary.RetentionRate = retentionCustomerIds.Count == 0 ? 0 : retentionCustomerIds.Count / (decimal)customerIdsOfPreviousTimeFrame.Count;

        if (hadPreviousHaravanOrder)
        {
            var customerIdsPrevious = previousHaravanOrders.Where(order => order.HaravanCustomerId.HasValue)
                                                           .Select(order => order.HaravanCustomerId.Value)
                                                           .Distinct()
                                                           .ToList();
            var customerIdsOfPreviousOfPreviousTimeFrame = previousOfPreviousHaravanOrders.Where(order => order.HaravanCustomerId.HasValue)
                                                                                          .Select(order => order.HaravanCustomerId.Value)
                                                                                          .Distinct()
                                                                                          .ToList();

            retentionCustomerIds = customerIdsPrevious.Intersect(customerIdsOfPreviousOfPreviousTimeFrame).ToList();

            var retentionPreviousRate = retentionCustomerIds.Count == 0
                                            ? 0
                                            : retentionCustomerIds.Count / (decimal)customerIdsOfPreviousOfPreviousTimeFrame.Count;

            ecomSummary.RetentionGrowthRate = MetricGenericCalculator.Rate(retentionPreviousRate, ecomSummary.RetentionRate);
        }

        // 8 - Avg User SKU
        var productCount    = MetricGenericCalculator.ProductCount(currentHaravanOrders);
        var customerCount   = MetricGenericCalculator.CustomerCount(currentHaravanOrders);
        var avgProductCount = (decimal)productCount / customerCount;
        ecomSummary.AvgProductCount = avgProductCount;
        if (hadPreviousHaravanOrder)
        {
            var productCountPrevious    = MetricGenericCalculator.ProductCount(previousHaravanOrders);
            var avgProductCountPrevious = (decimal)productCountPrevious / customerCount;
            var avgProductCountRate     = MetricGenericCalculator.Rate(avgProductCountPrevious, avgProductCount);
            ecomSummary.AvgProductCountRate = avgProductCountRate;
        }

        return ecomSummary;
    }

    #endregion

    #region SOCIAL

    private Page1Metric_Social GetSocialMetrics(Page1DataSourceSocial socialData)
    {
        if (socialData is null)
            return new Page1Metric_Social
            {
                Demographic         = new Page1SocialMetric_Demographic(),
                CommunityInteraction = new SocialMetric_CommunityInteraction(),
                LocationByProvinces = new List<SocialMetric_LocationByProvince>(),
                CreatedAt           = DateTime.UtcNow
            };

        var demographicMetric             = CalculateDemographicMetric(socialData);
        var locationByProvince            = CalculateLocationByProvince(socialData);
        var communityInteraction           = CalculateCommunityInteractionMetric(socialData);
        var aboveNormalInfluencerPhoneNos = GetAboveNormalInfluencerPhoneNos(socialData);

        return new Page1Metric_Social
        {
            Demographic                   = demographicMetric,
            LocationByProvinces           = locationByProvince,
            CommunityInteraction           = communityInteraction,
            AboveNormalInfluencerPhoneNos = aboveNormalInfluencerPhoneNos,
            CreatedAt                     = DateTime.UtcNow
        };
    }

    private Page1SocialMetric_Demographic CalculateDemographicMetric(Page1DataSourceSocial socialData)
    {
        var age           = MetricGenericCalculator.CalculateAgeRangeMetric(socialData.SocialUsers).ToList();
        var genders       = MetricGenericCalculator.CalculateGenderMetric(socialData.SocialUsers).ToList();
        var relationships = MetricGenericCalculator.CalculateRelationshipMetric(socialData.SocialUsers).ToList();

        return new Page1SocialMetric_Demographic
        {
            GenderMetrics = genders, AgeMetrics = age, RelationshipMetrics = relationships,

            // TODOO: Car & House
            // CarOwnerCount   = NumericExtensions.RandomInt(0, 5000),
            // CarOwnerRate    = NumericExtensions.RandomDecimal(0, 1),
            // HouseOwnerCount = NumericExtensions.RandomInt(0, 5000),
            // HouseOwnerRate  = NumericExtensions.RandomDecimal(0, 1),
        };
    }

    private List<SocialMetric_LocationByProvince> CalculateLocationByProvince(Page1DataSourceSocial socialData)
    {
        var items = MetricGenericCalculator.CalculateLocationByProvinceItems(socialData.SocialUsers).OrderByDescending(x => x.Count).ToList();

        var topLocations = items.Count > SocialMetric_LocationByProvince.Amount ? items.Take(SocialMetric_LocationByProvince.Amount).ToList() : items;

        return topLocations;
    }

    private SocialMetric_CommunityInteraction CalculateCommunityInteractionMetric(Page1DataSourceSocial socialData)
    {
        var topFollowerMetric  = MetricGenericCalculator.CalculateTopFollowerMetric(socialData.SocialUsers).ToList();
        var topLikedPage       = MetricGenericCalculator.CalculateTopLikedPageMetric(socialData.SocialInsights, socialData.SocialUsers).ToList();
        var topCheckinLocation = MetricGenericCalculator.CalculateTopCheckinLocation(socialData.SocialInsights, socialData.SocialUsers).ToList();
        var topGroup           = MetricGenericCalculator.CalculateTopGroup(socialData.SocialInsights, socialData.SocialUsers).ToList();

        return new SocialMetric_CommunityInteraction
        {
            TopFollowerMetrics = topFollowerMetric, TopLikedPageMetrics = topLikedPage, TopCheckinLocations = topCheckinLocation, TopGroups = topGroup
        };
    }

    private List<string> GetAboveNormalInfluencerPhoneNos(Page1DataSourceSocial socialData)
    {
        return socialData.SocialUsers
                         .Where(x => MetricGenericCalculator.MapInfluencerTypeByFollower(x.Follow + x.Friends).ToInt()
                                  >= InfluencerTypeByFollower.Normal.ToInt())
                         .Select(x => x.Phone)
                         .ToList();
    }

    #endregion

    #region SHORT TERM SOCIAL

    private Page1MetricPurchaseShortTermSocial GetShortTermSocialMetric(Page1DataSourceEcom   ecomData,
                                                                 Page1DataSourceSocial socialData,
                                                                 DateTime              currentDateTime)
    {
        var     nonLikePageUserCount = 0;
        decimal nonLikePageUserRate  = 0;
        if (socialData != null && ecomData != null)
        {
            var ecomCusPhoneNosEmails = ecomData.EcomCustomers.Select(customer => new KeyValuePair<string, string>(customer.Phone, customer.Email))
                                                .Distinct()
                                                .ToList();
            var socialUserPhoneNosEmails = socialData.SocialUsers.Select(user => new KeyValuePair<string, string>(user.Phone, user.Email))
                                                     .Distinct()
                                                     .ToList();

            var nonLikePageUser = ecomCusPhoneNosEmails.Where(pair => socialUserPhoneNosEmails.All(valuePair => valuePair.Key   != pair.Key)
                                                                   || socialUserPhoneNosEmails.All(valuePair => valuePair.Value != pair.Value));

            nonLikePageUserCount = nonLikePageUser.Count();
            nonLikePageUserRate  = socialData.SocialUsers.Count == 0 ? 0 : nonLikePageUserCount / (decimal)socialData.SocialUsers.Count;
        }

        return new Page1MetricPurchaseShortTermSocial { Count = nonLikePageUserCount, Rate = nonLikePageUserRate, CreatedAt = currentDateTime };
    }

    #endregion

    #region ADVANCE

    private async Task<Page1Metric_Advanced> GetAdvancedMetrics(Page1DataRequest      request,
                                                                Page1DataSourceEcom   ecom,
                                                                Page1DataSourceSocial social,
                                                                DateTime              currentDateTime)
    {
        var merchant       = await _merchantManager.GetMerchant(request.MerchantId);
        var merchantConfig = merchant.MetricConfigs;
        if (ecom is null || social is null) return new Page1Metric_Advanced { };

        var totalCustomers = ecom.AllEcomOrders.Where(order => order.HaravanCustomerId.HasValue)
                                 .Select(order => order.HaravanCustomerId.Value)
                                 .Distinct()
                                 .Count();
        var advancedMetrics = new Page1Metric_Advanced();

        // Calculate firstPurchasedCustomers metric
        var firstPurchasedCustomersCount = ecom.AllEcomOrders.Where(o => o.HaravanCustomerId.HasValue)
                                               .GroupBy(o => o.HaravanCustomerId)
                                               .Count(o => o.Key.HasValue
                                                        && o.Count() == 1
                                                        && o.First().LineItems.Any()
                                                        && o.First().LineItems.Count == 1);
        advancedMetrics.Only1OrderPurchasedCustomersCount = firstPurchasedCustomersCount;
        advancedMetrics.Only1OrderPurchasedCustomersRate  = totalCustomers == 0 ? 0 : firstPurchasedCustomersCount / (decimal)totalCustomers;

        // Calculate EcomCustomersNoOrderInXMonthsCount metric
        var orderGroups = ecom.AllEcomOrders.Where(o => o.HaravanCustomerId.HasValue).GroupBy(o => o.HaravanCustomerId.Value).ToList();
        var customerNoOrderInXMonths = orderGroups
                                      .Where(g => g.All(o => o.CreatedAt.HasValue
                                                          && o.CreatedAt
                                                           < DateTime.UtcNow.AddMonths(merchantConfig.Ecom_RetentionThresholdInMonth)))
                                      .ToList();
        var ecomCustomerIdNoOrderInXMonths = customerNoOrderInXMonths.Select(x => x.Key).Count();

        advancedMetrics.EcomCustomersNoOrderInXMonthsCount = ecomCustomerIdNoOrderInXMonths;
        advancedMetrics.EcomCustomersNoOrderInXMonthsRate  = totalCustomers == 0 ? 0 : ecomCustomerIdNoOrderInXMonths / (decimal)totalCustomers;

        // Calculate AOV
        var aov = MetricGenericCalculator.AOV(ecom.AllEcomOrders);
        advancedMetrics.AverageOrderValueComparedToTheWorld = aov;
        if (merchantConfig.OrderTotalKPI == 0)
        {
            advancedMetrics.AverageOrderValueComparedToTheWorldRate = 0;
        }
        else
        {
            advancedMetrics.AverageOrderValueComparedToTheWorldRate = aov / merchantConfig.OrderTotalKPI;
        }

        // Calculate RevenueEcomCustomerHasMoreThan1000FollowerAndFriend
        var userPhoneNosEmails = social.SocialUsers.Where(user => user.Follow + user.Friends >= 1000)
                                       .Select(user => new KeyValuePair<string, string>(user.Phone, user.Email))
                                       .Distinct()
                                       .ToList();

        var haravanCustomerIds = (await GetHaravanCustomerPhoneNoAndEmails(ecom))
                                .Where(x => userPhoneNosEmails.Any(pair => (pair.Key.TrimSafe().IsNotNullOrWhiteSpace()   && pair.Key   == x.PhoneNo)
                                                                        || (pair.Value.TrimSafe().IsNotNullOrWhiteSpace() && pair.Value == x.Email)))
                                .Select(x => x.HaravanCustomerId)
                                .Distinct()
                                .ToList();

        var haravanOrdersOfCustomerHasMoreThan1000FollowerAndFriend = ecom.AllEcomOrders
                                                                          .Where(order => order.HaravanCustomerId.HasValue
                                                                                       && haravanCustomerIds.Contains(order.HaravanCustomerId.Value))
                                                                          .ToList();
        var _1kFollowerRevenue = MetricGenericCalculator.TotalSaleAmount(haravanOrdersOfCustomerHasMoreThan1000FollowerAndFriend);
        var allOrdersRevenue   = MetricGenericCalculator.TotalSaleAmount(ecom.AllEcomOrders);
        advancedMetrics.RevenueEcomCustomerHasMoreThan1000FollowerAndFriend     = _1kFollowerRevenue;
        advancedMetrics.RevenueEcomCustomerHasMoreThan1000FollowerAndFriendRate = _1kFollowerRevenue / allOrdersRevenue;

        return advancedMetrics;
    }

    private async Task<List<HaravanCustomerPhoneNoAndEmail>> GetHaravanCustomerPhoneNoAndEmails(Page1DataSourceEcom page1DataSourceEcom)
    {
        var haravanCustomers = page1DataSourceEcom.AllEcomOrders.Where(order => order.HaravanCustomerId.HasValue)
                                                  .Select(order => order.HaravanCustomerId.Value)
                                                  .Distinct()
                                                  .ToList();
        var customerPhoneNoAndEmails = await _haravanCustomerManager.GetCusPhoneNosAndEmails(haravanCustomers);

        return customerPhoneNoAndEmails;
    }

    #endregion

    public async Task<List<Merchant>> GetMerchants()
    {
        return await _merchantManager.GetMerchants();
    }
}