using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LookOn.Core.Extensions;
using LookOn.Core.Shared.Enums;
using LookOn.Dashboards.DashboardBases;
using LookOn.Enums;
using LookOn.Integrations.Datalytis.Models.Entities;
using LookOn.Integrations.Haravan;
using LookOn.Integrations.Haravan.Models.Entities;
using LookOn.Merchants;
using LookOn.MerchantSyncInfos;
using Volo.Abp.Domain.Repositories;

namespace LookOn.Insights.Metrics;

public class MetricCalculatorManager : LookOnManager
{
    protected readonly IRepository<Metric>         MetricRepository;
    protected readonly IMerchantRepository         MerchantRepository;
    protected readonly IMerchantSyncInfoRepository MerchantSyncInfoRepo;
    protected readonly HaravanOrderManager         HaravanOrderManager;
    protected readonly HaravanCustomerManager      HaravanCustomerManager;

    public MetricCalculatorManager(IRepository<Metric>         metricRepository,
                                   IMerchantRepository         merchantRepository,
                                   IMerchantSyncInfoRepository merchantSyncInfoRepo,
                                   HaravanOrderManager         haravanOrderManager,
                                   HaravanCustomerManager      haravanCustomerManager)
    {
        MetricRepository       = metricRepository;
        MerchantRepository     = merchantRepository;
        MerchantSyncInfoRepo   = merchantSyncInfoRepo;
        HaravanOrderManager    = haravanOrderManager;
        HaravanCustomerManager = haravanCustomerManager;
    }

    #region ECOM_SUMMARY

    private List<HaravanOrder> GetOrdersByCreatedAt(List<HaravanOrder> orders, DateTime from, DateTime to)
    {
        return orders.Where(x => x.CreatedAt.HasValue && x.ConfirmedAt.HasValue && x.CreatedAt.HasValue && x.CreatedAt.Value >= from && x.CreatedAt.Value <= to).DistinctBy(o => o.Id).ToList();
    }

    protected EcomMetric_Summary GetEcomSummary(List<HaravanOrder> allOrders, List<HaravanOrder> timeFrameOrders, TimeFrameType timeFrame, DateTime from, DateTime to)
    {
        var previousRange  = timeFrame.GetDateByTimeFrameType(from, false);
        var previousOrders = GetOrdersByCreatedAt(allOrders, previousRange.Item1, previousRange.Item2);

        var beforePreviousRange  = timeFrame.GetDateByTimeFrameType(previousRange.Item1, false);
        var beforePreviousOrders = GetOrdersByCreatedAt(allOrders, beforePreviousRange.Item1, beforePreviousRange.Item2);

        var result = DoGetEcomSummary(beforePreviousOrders, previousOrders, timeFrameOrders);

        return result;
    }

    private EcomMetric_Summary DoGetEcomSummary(List<HaravanOrder> beforePreviousOrders, List<HaravanOrder> previousOrders, List<HaravanOrder> currentOrders)
    {
        var ecomSummary = new EcomMetric_Summary();

        // if no previous haravan order no need to calculate changing rate
        var hadPreviousHaravanOrder = previousOrders.IsNotNullOrEmpty();

        // 1
        ecomSummary.OrderCount = currentOrders.Count;
        if (hadPreviousHaravanOrder)
        {
            ecomSummary.OrderGrowthRate = MetricGenericCalculator.Rate(previousOrders.Count, currentOrders.Count);
        }

        // 2
        var purchasedCustomerCountCurrent = MetricGenericCalculator.PurchasedCustomerCount(currentOrders);
        ecomSummary.PurchasedCustomerCount = purchasedCustomerCountCurrent;
        if (hadPreviousHaravanOrder)
        {
            var purchasedCustomerCountPrevious = MetricGenericCalculator.PurchasedCustomerCount(previousOrders);
            var purchasedCustomerGrowthRate    = MetricGenericCalculator.Rate(purchasedCustomerCountCurrent, purchasedCustomerCountPrevious);
            ecomSummary.PurchasedCustomerGrowthRate = purchasedCustomerGrowthRate;
        }

        // 3 - AOV
        var aovCurrent = MetricGenericCalculator.AOV(currentOrders);
        ecomSummary.AOV = aovCurrent;

        if (hadPreviousHaravanOrder)
        {
            var aovPrevious   = MetricGenericCalculator.AOV(previousOrders);
            var aovGrowthRate = MetricGenericCalculator.Rate(aovPrevious, aovCurrent);
            ecomSummary.AOVGrowthRate = aovGrowthRate;
        }

        // 4- CLV
        var clvCurrent = MetricGenericCalculator.CLV(currentOrders);
        ecomSummary.CLV = clvCurrent;
        if (hadPreviousHaravanOrder)
        {
            var clvPrevious   = MetricGenericCalculator.CLV(previousOrders);
            var clvGrowthDate = MetricGenericCalculator.Rate(clvCurrent, clvPrevious);
            ecomSummary.CLVGrowthRate = clvGrowthDate;
        }

        // 5 - Returned OrderSyncInfo
        var returnedOrdersCurrent     = HaravanOrderManager.GetReturnedOrders(currentOrders);
        var returnedOrdersRateCurrent = currentOrders.Count == 0 ? 0 : (decimal)returnedOrdersCurrent.Count / currentOrders.Count;
        ecomSummary.ReturnedOrderRate = returnedOrdersRateCurrent;
        if (hadPreviousHaravanOrder)
        {
            var returnedOrdersPrevious     = HaravanOrderManager.GetReturnedOrders(previousOrders);
            var returnedOrdersRatePrevious = previousOrders.Count == 0 ? 0 : (decimal)returnedOrdersPrevious.Count / previousOrders.Count;
            var returnedOrderGrowthRate    = MetricGenericCalculator.Rate(returnedOrdersRatePrevious, returnedOrdersRateCurrent);
            ecomSummary.ReturnedOrderGrowthRate = returnedOrderGrowthRate;
        }

        // 6 - Cancelled OrderSyncInfo
        var cancelledOrdersCurrent     = HaravanOrderManager.GetCancelledOrders(currentOrders);
        var cancelledOrdersRateCurrent = currentOrders.Count == 0 ? 0 : (decimal)cancelledOrdersCurrent.Count / currentOrders.Count;
        ecomSummary.CancelledOrderRate = cancelledOrdersRateCurrent;
        if (hadPreviousHaravanOrder)
        {
            var cancelledOrdersPrevious     = HaravanOrderManager.GetCancelledOrders(previousOrders);
            var cancelledOrdersRatePrevious = previousOrders.Count == 0 ? 0 : (decimal)cancelledOrdersPrevious.Count / previousOrders.Count;
            var cancelledOrderGrowthRate    = MetricGenericCalculator.Rate(cancelledOrdersRatePrevious, cancelledOrdersRateCurrent);
            ecomSummary.CancelledOrderGrowthRate = cancelledOrderGrowthRate;
        }

        // 7 - Retention Rate
        var customerIds = currentOrders.Where(order => order.HaravanCustomerId.HasValue).Select(order => order.HaravanCustomerId.Value).Distinct().ToList();

        var customerIdsOfPreviousTimeFrame = previousOrders.Where(order => order.HaravanCustomerId.HasValue).Select(order => order.HaravanCustomerId.Value).Distinct().ToList();

        var retentionCustomerIds = customerIds.Intersect(customerIdsOfPreviousTimeFrame).ToList();

        ecomSummary.RetentionRate = retentionCustomerIds.Count == 0 ? 0 : retentionCustomerIds.Count / (decimal)customerIdsOfPreviousTimeFrame.Count;

        if (hadPreviousHaravanOrder)
        {
            var customerIdsPrevious                      = previousOrders.Where(order => order.HaravanCustomerId.HasValue).Select(order => order.HaravanCustomerId.Value).Distinct().ToList();
            var customerIdsOfPreviousOfPreviousTimeFrame = beforePreviousOrders.Where(order => order.HaravanCustomerId.HasValue).Select(order => order.HaravanCustomerId.Value).Distinct().ToList();

            retentionCustomerIds = customerIdsPrevious.Intersect(customerIdsOfPreviousOfPreviousTimeFrame).ToList();

            var retentionPreviousRate = retentionCustomerIds.Count == 0 ? 0 : retentionCustomerIds.Count / (decimal)customerIdsOfPreviousOfPreviousTimeFrame.Count;

            ecomSummary.RetentionGrowthRate = MetricGenericCalculator.Rate(retentionPreviousRate, ecomSummary.RetentionRate);
        }

        // 8 - Avg User SKU
        var productCount    = MetricGenericCalculator.ProductCount(currentOrders);
        var customerCount   = MetricGenericCalculator.CustomerCount(currentOrders);
        var avgProductCount = customerCount == 0 ? 0 : (decimal)productCount / customerCount;
        ecomSummary.AvgProductCount = avgProductCount;
        if (hadPreviousHaravanOrder)
        {
            var productCountPrevious    = MetricGenericCalculator.ProductCount(previousOrders);
            var avgProductCountPrevious = customerCount == 0 ? 0 : (decimal)productCountPrevious / customerCount;
            var avgProductCountRate     = MetricGenericCalculator.Rate(avgProductCountPrevious, avgProductCount);
            ecomSummary.AvgProductCountRate = avgProductCountRate;
        }

        return ecomSummary;
    }

    #endregion

    #region ECOM_OTHERS

    protected EcomMetric_RevenueSummary GetEcomRevenueSummary(List<HaravanOrder> allOrders, List<HaravanOrder> currentOrders, TimeFrameType timeFrame, DateTime fromDateTime, DateTime endDateTime)
    {
        fromDateTime = fromDateTime.Date;
        endDateTime  = endDateTime.Date.AddDays(-1);
        
        var previousRange  = timeFrame.GetDateByTimeFrameType(fromDateTime, false);
        var previousOrders = GetOrdersByCreatedAt(allOrders, previousRange.Item1, previousRange.Item2);

        var ecomRevenueSummary = new EcomMetric_RevenueSummary();
        var revenueCurrent     = MetricGenericCalculator.TotalSaleAmount(currentOrders);
        var revenueByDates = fromDateTime.To(endDateTime)
                                         .Select(date => new EcomMetric_RevenueByDate
                                          {
                                              Revenue  = currentOrders.Where(o => o.CreatedAt.HasValue && o.CreatedAt.Value.Date == date).Sum(o => o.TotalPrice ?? 0),
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

        if (previousOrders.IsNotNullOrEmpty())
        {
            var revenuePrevious   = MetricGenericCalculator.TotalSaleAmount(previousOrders);
            var revenueGrowthRate = MetricGenericCalculator.Rate(revenuePrevious, revenueCurrent);
            ecomRevenueSummary.RevenueGrowthRate = revenueGrowthRate;
        }

        return ecomRevenueSummary;
    }

    protected List<EcomMetric_RevenueByProduct> GetEcomRevenueByProducts(List<HaravanOrder> orders)
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

    protected List<EcomMetric_RevenueByLocation> GetEcomRevenueByLocations(List<HaravanOrder> orders)
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

            var provinceOrders = ordersHaveShippingAddress.Where(o => o.ShippingAddress.Province == province || o.ShippingAddress.City == province).ToList();

            dic[province] = provinceOrders.Sum(o => o.TotalPrice ?? 0);
        }

        var ecomRevenueByLocations = dic.OrderByDescending(entry => entry.Value).Take(EcomMetric_RevenueByLocation.Amount).Select(x => new EcomMetric_RevenueByLocation { Name = x.Key, Value = x.Value, Rate = x.Value / revenueCurrent }).ToList();
        return ecomRevenueByLocations;
    }

    #endregion

    #region SOCIAL METRICS

    protected SocialMetric_Demographic GetSocialDemographic(List<DatalytisUser> socialUsers, List<HaravanCustomer> customers)
    {
        SocialMetric_Demographic metric = new()
        {
            AgeMetrics          = MetricGenericCalculator.CalculateAgeRangeMetric(socialUsers).ToList(),
            GenderMetrics       = MetricGenericCalculator.CalculateGenderMetric(socialUsers).ToList(),
            RelationshipMetrics = MetricGenericCalculator.CalculateRelationshipMetric(socialUsers).ToList()
        };

        var customerPhones = customers.Select(customer => customer.Phone.ToInternationalPhoneNumberFromVN()).Distinct().ToList();
        var userPhones     = socialUsers.Select(user => user.Phone.ToInternationalPhoneNumberFromVN()).Distinct().ToList();

        var purchaseNoLikePhones = customerPhones.Where(_ => !userPhones.Contains(_)).ToList();
        metric.PurchaseNoLike = new SocialMetric_PurchaseNoLike
        {
            Count = purchaseNoLikePhones.Count,
            Rate  = socialUsers.Count == 0 ? 0 : purchaseNoLikePhones.Count / (decimal)socialUsers.Count
        };

        var noPurchaseLikePhones = userPhones.Where(_ => !customerPhones.Contains(_)).ToList();
        metric.NoPurchaseLikePage = new SocialMetric_NoPurchaseLikePage
        {
            Count = noPurchaseLikePhones.Count,
            Rate  = socialUsers.Count == 0 ? 0 : noPurchaseLikePhones.Count / (decimal)socialUsers.Count
        };

        return metric;

        // TODOO: Car & House
        // CarOwnerCount   = NumericExtensions.RandomInt(0, 5000),
        // CarOwnerRate    = NumericExtensions.RandomDecimal(0, 1),
        // HouseOwnerCount = NumericExtensions.RandomInt(0, 5000),
        // HouseOwnerRate  = NumericExtensions.RandomDecimal(0, 1),
    }

    protected List<SocialMetric_LocationByProvince> GetSocialLocationByProvince(List<DatalytisUser> socialUsers)
    {
        var items = MetricGenericCalculator.CalculateLocationByProvinceItems(socialUsers).OrderByDescending(x => x.Count).ToList();

        var topLocations = items.Count > SocialMetric_LocationByProvince.Amount ? items.Take(SocialMetric_LocationByProvince.Amount).ToList() : items;

        return topLocations;
    }

    protected SocialMetric_CommunityInteraction GetCommunityInteraction(List<DatalytisUser> socialUsers, List<DatalytisUserSocialInsight> socialInsights)
    {
        var topFollowerMetric  = MetricGenericCalculator.CalculateTopFollowerMetric(socialUsers).ToList();
        var topLikedPage       = MetricGenericCalculator.CalculateTopLikedPageMetric(socialInsights, socialUsers).ToList();
        var topCheckinLocation = MetricGenericCalculator.CalculateTopCheckinLocation(socialInsights, socialUsers).ToList();
        var topGroup           = MetricGenericCalculator.CalculateTopGroup(socialInsights, socialUsers).ToList();

        return new SocialMetric_CommunityInteraction
        {
            TopFollowerMetrics = topFollowerMetric, TopLikedPageMetrics = topLikedPage, TopCheckinLocations = topCheckinLocation, TopGroups = topGroup
        };
    }

    protected List<string> GetAboveNormalInfluencerPhones(List<DatalytisUser> socialUsers)
    {
        return socialUsers.Where(x => MetricGenericCalculator.MapInfluencerTypeByFollower(x.Follow + x.Friends).ToInt() >= InfluencerTypeByFollower.Normal.ToInt()).Select(x => x.Phone).ToList();
    }

    protected SocialMetric_Insight GetSocialInsightMetric(List<DatalytisUser> socialUsers, List<HaravanCustomer> customers, List<DatalytisUserSocialInsight> insights)
    {
        SocialMetric_Insight metric = new();

        var ecomCustomerPhones    = customers.Select(_ => _.Phone.ToInternationalPhoneNumberFromVN()).ToList();
        var nonPurchasedUsers = socialUsers.Where(user => !ecomCustomerPhones.Contains(user.Phone.ToInternationalPhoneNumberFromVN())).ToList();

        metric.NonPurchasedUserCount = nonPurchasedUsers.Count;
        metric.NonPurchasedUserRate  = socialUsers.Count == 0 ? 0 : nonPurchasedUsers.Count / socialUsers.Count;

        var socialCustomerPhones = socialUsers.Select(_ => _.Phone.ToInternationalPhoneNumberFromVN()).ToList();
        var nonLikedPageUsers    = customers.Where(_ => !socialCustomerPhones.Contains(_.Phone.ToInternationalPhoneNumberFromVN())).ToList();
        metric.NonLikedPageUserCount = nonLikedPageUsers.Count;
        metric.NonLikedPageUserRate  = customers.Count == 0 ? 0 : nonLikedPageUsers.Count / customers.Count;

        if (insights.IsNotNullOrEmpty())
        {
            // Calculate TopLikedPageRate Metric 
            var aboveNanoInfluencerSocialUsers = nonPurchasedUsers.Where(x => MetricGenericCalculator.MapInfluencerTypeByFollower(x.Follow + x.Friends).ToInt() >= InfluencerTypeByFollower.Nano.ToInt()).ToList();

            var aboveNanoInfluencerSocialUserIds               = aboveNanoInfluencerSocialUsers.Select(user => user.Uid).Distinct().ToList();
            var aboveNanoInfluencerSocialUserInsightsLikedPage = insights.Where(insight => aboveNanoInfluencerSocialUserIds.Contains(insight.Uid) && insight.Type.ToEnumOrDefault<SocialInsightsType>() == SocialInsightsType.Like).Select(insight => insight).ToList();

            if (aboveNanoInfluencerSocialUserInsightsLikedPage.Any())
            {
                var likedPageGroup = aboveNanoInfluencerSocialUserInsightsLikedPage.GroupBy(insight => insight.Name);
                likedPageGroup = likedPageGroup.OrderByDescending(_ => _.Count()).ToList();
                var totalLikes = likedPageGroup.Sum(_ => _.Count());

                metric.TopLikedPageRate = totalLikes == 0 ? 0 : likedPageGroup.First().Count() / (decimal)totalLikes;
                metric.TopLikedPageName = likedPageGroup.First().Key;
            }

            // Calculate Most popular segment and Less Interest SegmentRate
            var categoryGroup = insights.SelectMany(insight => insight.Category.CategoryList).GroupBy(item => item.Name).ToList();
            if (categoryGroup.Any())
            {
                categoryGroup = categoryGroup.OrderByDescending(items => items.Count()).ToList();
                var totalCategories = categoryGroup.Sum(items => items.Count());

                metric.MostPopularSegmentRate = totalCategories == 0 ? 0 : categoryGroup.First().Count() / (decimal)totalCategories;
                metric.MostPopularSegmentName = categoryGroup.First().Key;

                metric.LessInterestSegmentRate = totalCategories == 0 ? 0 : categoryGroup.Last().Count() / (decimal)totalCategories;
                metric.LessInterestSegmentName = categoryGroup.Last().Key;
            }
        }

        return metric;
    }

    #endregion

    #region ECOM x SOCIAL

    protected async Task<EcomMetric_Advanced> GetEcomAdvancedMetrics(MetricConfigs metricConfigs, List<HaravanOrder> allOrders, List<HaravanOrder> filteredOrders, List<DatalytisUser> socialUsers, int filteredCustomerCount, int allCustomerCount)
    {
        var advancedMetrics = new EcomMetric_Advanced();

        // Calculate Customers purchased only 1 order metric
        var only1OrderPurchasedCustomersCount = filteredOrders.Where(o => o.HaravanCustomerId.HasValue).GroupBy(o => o.HaravanCustomerId).Count(o => o.Key.HasValue && o.Count() == 1 && o.First().LineItems.Any());
        advancedMetrics.Only1OrderPurchasedCustomersCount = only1OrderPurchasedCustomersCount;
        advancedMetrics.Only1OrderPurchasedCustomersRate  = filteredCustomerCount == 0 ? 0 : only1OrderPurchasedCustomersCount / (decimal)filteredCustomerCount;

        // Calculate Customer no order in X months metric
        var orderGroups                    = allOrders.Where(o => o.HaravanCustomerId.HasValue).GroupBy(o => o.HaravanCustomerId.Value).ToList();
        var customerNoOrderInXMonths       = orderGroups.Where(g => g.All(o => o.CreatedAt.HasValue && o.CreatedAt < DateTime.UtcNow.AddMonths(-metricConfigs.Ecom_RetentionThresholdInMonth))).ToList();
        var ecomCustomerIdNoOrderInXMonths = customerNoOrderInXMonths.Select(x => x.Key).Count();

        advancedMetrics.EcomCustomersNoOrderInXMonthsCount = ecomCustomerIdNoOrderInXMonths;
        advancedMetrics.EcomCustomersNoOrderInXMonthsRate  = filteredCustomerCount == 0 ? 0 : ecomCustomerIdNoOrderInXMonths / (decimal)allCustomerCount;

        // Calculate AOV
        var aov = MetricGenericCalculator.AOV(filteredOrders);
        advancedMetrics.AverageOrderValueComparedToTheWorld     = aov;
        advancedMetrics.AverageOrderValueComparedToTheWorldRate = metricConfigs.OrderTotalKPI == 0 ? 0 : aov / metricConfigs.OrderTotalKPI;

        // Calculate RevenueEcomCustomerHasMoreThan1000FollowerAndFriend
        var userPhoneNosEmails = socialUsers.Where(user => user.Follow + user.Friends >= 1000).Select(user => new KeyValuePair<string, string>(user.Phone, user.Email)).Distinct().ToList();

        var haravanCustomers         = filteredOrders.Where(order => order.HaravanCustomerId.HasValue).Select(order => order.HaravanCustomerId.Value).Distinct().ToList();
        var customerPhoneNoAndEmails = await HaravanCustomerManager.GetCusPhoneNosAndEmails(haravanCustomers);

        var haravanCustomerIds = customerPhoneNoAndEmails.Where(x => userPhoneNosEmails.Any(pair => (pair.Key.TrimSafe().IsNotNullOrWhiteSpace() && pair.Key == x.PhoneNo) || (pair.Value.TrimSafe().IsNotNullOrWhiteSpace() && pair.Value == x.Email))).Select(x => x.HaravanCustomerId).Distinct().ToList();

        var haravanOrdersOfCustomerHasMoreThan1000FollowerAndFriend = filteredOrders.Where(order => order.HaravanCustomerId.HasValue && haravanCustomerIds.Contains(order.HaravanCustomerId.Value)).ToList();
        var _1kFollowerRevenue                                      = MetricGenericCalculator.TotalSaleAmount(haravanOrdersOfCustomerHasMoreThan1000FollowerAndFriend);
        var allOrdersRevenue                                        = MetricGenericCalculator.TotalSaleAmount(filteredOrders);
        advancedMetrics.RevenueEcomCustomerHasMoreThan1000FollowerAndFriend     = _1kFollowerRevenue;
        advancedMetrics.RevenueEcomCustomerHasMoreThan1000FollowerAndFriendRate = allOrdersRevenue == 0 ? 0 : _1kFollowerRevenue / allOrdersRevenue;

        return advancedMetrics;
    }

    #endregion

    #region ECOM x SOCIAL - COMPARISION

    protected SocialMetric_Comparision GetComparisionMetric(List<DatalytisUser> ecomUsers, List<DatalytisUser> socialUsers)
    {
        var comparisionMetric = new SocialMetric_Comparision
        {
            GenderComparision       = GetGenderComparision(ecomUsers, socialUsers),
            AgeComparision          = GetAgeComparision(ecomUsers, socialUsers),
            RelationshipComparision = GetRelationshipComparision(ecomUsers, socialUsers),
        };

        return comparisionMetric;
    }

    private GenderComparision GetGenderComparision(List<DatalytisUser> ecomUsers, List<DatalytisUser> socialUsers)
    {
        var genderComparision = new GenderComparision();

        if (ecomUsers.IsNotNullOrEmpty())
        {
            var ecomGenderGroups = ecomUsers.Where(user => user.Gender is not GenderType.Unknown).GroupBy(user => user.Gender).OrderByDescending(users => users.Count()).ToList();
            if (ecomGenderGroups.IsNotNullOrEmpty())
            {
                genderComparision.PurchasedGenderName = L[$"Enum:GenderType:{ecomGenderGroups.First().Key.ToInt()}"];
                genderComparision.PurchasedGenderRate = ecomUsers.Count == 0 ? 0 : ecomGenderGroups.First().Count() / (decimal)ecomUsers.Count;
            }
        }

        if (socialUsers.IsNotNullOrEmpty())
        {
            var socialGenderGroup = socialUsers.Where(user => user.Gender is not GenderType.Unknown).GroupBy(user => user.Gender).OrderByDescending(users => users.Count()).ToList();
            if (socialGenderGroup.IsNotNullOrEmpty())
            {
                genderComparision.NonPurchasedGenderName = L[$"Enum:GenderType:{socialGenderGroup.First().Key.ToInt()}"];
                genderComparision.NonPurchasedGenderRate = socialUsers.Count == 0 ? 0 : socialGenderGroup.First().Count() / (decimal)socialUsers.Count;
            }
        }

        return genderComparision;
    }

    private AgeComparision GetAgeComparision(List<DatalytisUser> ecomUsers, List<DatalytisUser> socialUsers)
    {
        var ageComparision = new AgeComparision();

        if (ecomUsers.IsNotNullOrEmpty())
        {
            var ecomAgeGroups = ecomUsers.Where(user => user.Birthday.HasValue).GroupBy(user => MetricGenericCalculator.MapAgeSegment(user.Birthday.Value)).ToList();
            if (ecomAgeGroups.IsNotNullOrEmpty())
            {
                ecomAgeGroups                   = ecomAgeGroups.OrderByDescending(users => users.Count()).ToList();
                ageComparision.PurchasedAge     = L[$"Enum:AgeSegmentEnum:{ecomAgeGroups.First().Key.ToInt()}"];
                ageComparision.PurchasedAgeRate = ecomUsers.Count == 0 ? 0 : ecomAgeGroups.First().Count() / (decimal)ecomUsers.Count;
            }
        }

        if (socialUsers.IsNotNullOrEmpty())
        {
            var socialAgeGroups = socialUsers.Where(user => user.Birthday.HasValue).GroupBy(user => MetricGenericCalculator.MapAgeSegment(user.Birthday.Value)).ToList();
            if (socialAgeGroups.IsNotNullOrEmpty())
            {
                socialAgeGroups                    = socialAgeGroups.OrderByDescending(users => users.Count()).ToList();
                ageComparision.NonPurchasedAge     = L[$"Enum:AgeSegmentEnum:{socialAgeGroups.First().Key.ToInt()}"];
                ageComparision.NonPurchasedAgeRate = socialUsers.Count == 0 ? 0 : socialAgeGroups.First().Count() / (decimal)socialUsers.Count;
            }
        }
        return ageComparision;
    }

    private RelationshipComparision GetRelationshipComparision(List<DatalytisUser> ecomUsers, List<DatalytisUser> socialUsers)
    {
        var relationshipComparision = new RelationshipComparision();
        if (ecomUsers.IsNotNullOrEmpty())
        {
            var ecomGroups = ecomUsers.Where(user => user.RelationshipStatus is not RelationshipStatus.Unknown).GroupBy(user => user.RelationshipStatus).ToList();
            if (ecomGroups.IsNotNullOrEmpty())
            {
                ecomGroups                                        = ecomGroups.OrderByDescending(users => users.Count()).ToList();
                relationshipComparision.PurchasedRelationship     = L[$"Enum:RelationshipStatus:{ecomGroups.First().Key.ToInt()}"];
                relationshipComparision.PurchasedRelationshipRate = ecomUsers.Count == 0 ? 0 : ecomGroups.First().Count() / (decimal)ecomUsers.Count;
            }
        }

        if (socialUsers.IsNotNullOrEmpty())
        {
            var socialGroups = socialUsers.Where(user => user.RelationshipStatus is not RelationshipStatus.Unknown).GroupBy(user => user.RelationshipStatus).ToList();
            if (socialGroups.IsNotNullOrEmpty())
            {
                socialGroups                                         = socialGroups.OrderByDescending(users => users.Count()).ToList();
                relationshipComparision.NonPurchasedRelationship     = L[$"Enum:RelationshipStatus:{socialGroups.First().Key.ToInt()}"];
                relationshipComparision.NonPurchasedRelationshipRate = socialUsers.Count == 0 ? 0 : socialGroups.First().Count() / (decimal)socialUsers.Count;
            }
        }
        return relationshipComparision;
    }

    #endregion
}