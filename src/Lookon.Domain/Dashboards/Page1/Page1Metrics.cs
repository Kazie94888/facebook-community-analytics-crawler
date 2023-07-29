using System;
using System.Collections.Generic;
using LookOn.Dashboards.DashboardBases;

namespace LookOn.Dashboards.Page1;

public class Page1Metric : MetricBase
{
    public Page1Metric_Ecom            Ecom            { get; set; }
    public Page1Metric_Social          Social          { get; set; }
    public Page1Metric_Advanced        Advanced        { get; set; }
    public DateTime?                   UpdatedAt       { get; set; }
    public Page1MetricPurchaseShortTermSocial PurchaseShortTermSocial { get; set; }

    public Page1Metric()
    {
        UpdatedAt = DateTime.UtcNow;
    }
}

public class Page1MetricPurchaseShortTermSocial: SocialMetric_PurchaseNoLike
{
    public DateTime CreatedAt { get; set; }
}

public class SocialMetric_PurchaseNoLike
{
    public int      Count      { get; set; }
    public decimal  Rate       { get; set; }
    public bool     IsPositive { get; set; }

    public SocialMetric_PurchaseNoLike()
    {
        IsPositive = false;
    }
}

public class SocialMetric_NoPurchaseLikePage
{
    public int     Count  { get; set; }
    public decimal Rate       { get; set; }
    public bool    IsPositive { get; set; }

    public SocialMetric_NoPurchaseLikePage()
    {
        IsPositive = true;
    }
}

public class Page1Metric_Ecom
{
    public EcomMetric_Summary                 Summary            { get; set; }
    public EcomMetric_RevenueSummary          RevenueSummary     { get; set; }
    public List<EcomMetric_RevenueByProduct>  RevenueByProducts  { get; set; }
    public List<EcomMetric_RevenueByLocation> RevenueByLocations { get; set; }
    public DateTime                           CreatedAt          { get; set; }

    // public EcomMetric_SaleCountByProduct      SaleCountByProduct { get; set; }
    public Page1Metric_Ecom()
    {
        RevenueByProducts  = new List<EcomMetric_RevenueByProduct>();
        RevenueByLocations = new List<EcomMetric_RevenueByLocation>();
    }
}

public class Page1Metric_Social
{
    public Page1SocialMetric_Demographic         Demographic         { get; set; }
    public List<SocialMetric_LocationByProvince> LocationByProvinces { get; set; }
    public SocialMetric_CommunityInteraction      CommunityInteraction { get; set; }

    // advanced metrics
    public List<string> AboveNormalInfluencerPhoneNos { get; set; }
    public DateTime     CreatedAt                     { get; set; }

    public Page1Metric_Social()
    {
        LocationByProvinces           = new List<SocialMetric_LocationByProvince>();
        AboveNormalInfluencerPhoneNos = new List<string>();
    }
}

public class Page1SocialMetric_Demographic : SocialMetric_Demographic
{
}

public class Page1Metric_Advanced: EcomMetric_Advanced
{
}


public class EcomMetric_Advanced
{
    public long     EcomCustomersNoOrderInXMonthsCount                      { get; set; }
    public decimal  EcomCustomersNoOrderInXMonthsRate                       { get; set; }
    public bool     IsEcomCustomersNoOrderInXMonthsPositive                 { get; set; }
    public decimal  AverageOrderValueComparedToTheWorld                     { get; set; }
    public decimal  AverageOrderValueComparedToTheWorldRate                 { get; set; }
    public long     Only1OrderPurchasedCustomersCount                            { get; set; }
    public decimal  Only1OrderPurchasedCustomersRate                             { get; set; }
    public bool     IsOnly1OrderPurchasedCustomersPositive                       { get; set; }
    public decimal  RevenueEcomCustomerHasMoreThan1000FollowerAndFriend     { get; set; }
    public decimal  RevenueEcomCustomerHasMoreThan1000FollowerAndFriendRate { get; set; }
    
    public EcomMetric_Advanced()
    {
        IsEcomCustomersNoOrderInXMonthsPositive = false;
        IsOnly1OrderPurchasedCustomersPositive       = false;
    }
}