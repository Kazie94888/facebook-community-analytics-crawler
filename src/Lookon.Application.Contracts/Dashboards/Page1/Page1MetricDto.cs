using System;
using System.Collections.Generic;
using LookOn.Dashboards.DashboardBase;

namespace LookOn.Dashboards.Page1;

public class Page1MetricDto
{
    public Page1Metric_EcomDto            Ecom            { get; set; }
    public Page1Metric_SocialDto          Social          { get; set; }
    public Page1Metric_AdvancedDto        Advanced        { get; set; }
    public Page1Metric_ShortTermSocialDto ShortTermSocial { get; set; }
    public DateTime?                      UpdatedAt       { get; set; }
}

public class Page1Metric_EcomDto
{
    public EcomMetric_SummaryDto                  Summary            { get; set; }
    public EcomMetric_RevenueSummaryDto           RevenueSummary     { get; set; }
    public IList<EcomMetric_RevenueByProductDto>  RevenueByProducts  { get; set; }
    public IList<EcomMetric_RevenueByLocationDto> RevenueByLocations { get; set; }
    public EcomMetric_SaleCountByProductDto       SaleCountByProduct { get; set; }
}

public class Page1Metric_SocialDto
{
    public Page1SocialMetric_DemographicDto          Demographic         { get; set; }
    public IList<SocialMetric_LocationByProvinceDto> LocationByProvinces { get; set; }
    public SocialMetric_CommunityInteractionDto       CommunityInteraction { get; set; }

    // advanced metrics
    public List<string> AboveNormalInfluencerPhoneNos { get; set; }
}

public class Page1SocialMetric_DemographicDto : SocialMetric_DemographicDto
{
}

public class Page1Metric_ShortTermSocialDto
{
    public int     NonLikePageUserCount      { get; set; }
    public decimal NonLikePageUserRate       { get; set; }
    public bool    IsNonLikePageUserPositive { get; set; }
    public decimal NonLikePageUserPercent    => Math.Round(NonLikePageUserRate * 100, 2);

    public Page1Metric_ShortTermSocialDto()
    {
        IsNonLikePageUserPositive = false;
    }
}

public class Page1Metric_AdvancedDto
{
    public long    EcomCustomersNoOrderInXMonthsCount                      { get; set; }
    public decimal EcomCustomersNoOrderInXMonthsRate                       { get; set; }
    public bool    IsEcomCustomersNoOrderInXMonthsPositive                 { get; set; }
    public decimal AverageOrderValueComparedToTheWorld                     { get; set; }
    public decimal AverageOrderValueComparedToTheWorldRate                 { get; set; }
    public long    FirstPurchasedCustomersCount                            { get; set; }
    public decimal FirstPurchasedCustomersRate                             { get; set; }
    public bool    IsFirstPurchasedCustomerPositive                        { get; set; }
    public decimal RevenueEcomCustomerHasMoreThan1000FollowerAndFriend     { get; set; }
    public decimal RevenueEcomCustomerHasMoreThan1000FollowerAndFriendRate { get; set; }
    public decimal EcomCustomersNoOrderInXMonthsPercent                    => Math.Round(EcomCustomersNoOrderInXMonthsRate       * 100, 2);
    public decimal AverageOrderValueComparedToTheWorldPercent              => Math.Round(AverageOrderValueComparedToTheWorldRate * 100, 2);
    public decimal FirstPurchasedCustomersPercent                          => Math.Round(FirstPurchasedCustomersRate             * 100, 2);
    public decimal RevenueEcomCustomerHasMoreThan1000FollowerAndFriendPercent =>
        Math.Round(RevenueEcomCustomerHasMoreThan1000FollowerAndFriendRate * 100, 2);

    public Page1Metric_AdvancedDto()
    {
        IsEcomCustomersNoOrderInXMonthsPositive = false;
        IsFirstPurchasedCustomerPositive        = false;
    }
}