using System;
using System.Collections.Generic;
using LookOn.Core.Shared.Enums;
using LookOn.Dashboards.DashboardBases;
using LookOn.Enums;
using LookOn.Integrations.Datalytis.Models.Entities;
using LookOn.Integrations.Haravan.Models.Entities;
using LookOn.Merchants;
using Volo.Abp.Domain.Entities.Auditing;

namespace LookOn.Insights;

public class Metric : AuditedEntity<Guid>
{
    public Guid             MerchantId { get; set; }
    public List<MetricItem> Items      { get; set; }
    public DateTime         CreatedAt  { get; set; }
    public Metric()
    {
        CreatedAt = DateTime.UtcNow;
        Items     = new List<MetricItem>();
    }
}

public class MetricItem
{
    public MetricDataSourceType DataSourceType { get; set; }
    public TimeFrameType        TimeFrameType  { get; set; }
    //
    public EcomMetric_Summary                 EcomSummary            { get; set; }
    public EcomMetric_RevenueSummary          EcomRevenueSummary     { get; set; }
    public List<EcomMetric_RevenueByProduct>  EcomRevenueByProducts  { get; set; }
    public List<EcomMetric_RevenueByLocation> EcomRevenueByLocations { get; set; }
    public EcomMetric_Advanced                EcomAdvanced           { get; set; }

    //
    public SocialMetric_Demographic              SocialDemographic                 { get; set; }
    public List<SocialMetric_LocationByProvince> SocialLocationByProvinces         { get; set; }
    public SocialMetric_CommunityInteraction     SocialCommunityInteraction        { get; set; }
    public List<string>                          SocialAboveNormalInfluencerPhones { get; set; }
    public SocialMetric_Insight                  SocialInsight                     { get; set; }
    public SocialMetric_Comparision              SocialComparision                 { get; set; }
}

public class SocialMetric_Comparision
{
    
    public GenderComparision       GenderComparision       { get; set; }
    public AgeComparision          AgeComparision          { get; set; }
    public CarOwnerComparision     CarOwnerComparision     { get; set; }
    public HouseOwnerComparision   HouseOwnerComparision   { get; set; }
    public RelationshipComparision RelationshipComparision { get; set; }
}


public class GenderComparision
{
    public string  PurchasedGenderName    { get; set; }
    public decimal PurchasedGenderRate    { get; set; }
    public string  NonPurchasedGenderName { get; set; }
    public decimal NonPurchasedGenderRate { get; set; }
}

public class AgeComparision
{
    public string  PurchasedAge        { get; set; }
    public decimal PurchasedAgeRate    { get; set; }
    public string  NonPurchasedAge     { get; set; }
    public decimal NonPurchasedAgeRate { get; set; }
}

public class CarOwnerComparision
{
    public string  PurchasedCarOwnerStatus    { get; set; }
    public decimal PurchasedCarOwnerRate      { get; set; }
    public string  NonPurchasedCarOwnerStatus { get; set; }
    public decimal NonPurchasedCarOwnerRate   { get; set; }
}

public class HouseOwnerComparision
{
    public string  PurchasedHouseOwnerStatus    { get; set; }
    public decimal PurchasedHouseOwnerRate      { get; set; }
    public string  NonPurchasedHouseOwnerStatus { get; set; }
    public decimal NonPurchasedHouseOwnerRate   { get; set; }
}

public class RelationshipComparision
{
    public string  PurchasedRelationship        { get; set; }
    public decimal PurchasedRelationshipRate    { get; set; }
    public string  NonPurchasedRelationship     { get; set; }
    public decimal NonPurchasedRelationshipRate { get; set; }
}

public class SocialMetric_Insight
{
    
    public long    NonPurchasedUserCount   { get; set; }
    public decimal NonPurchasedUserRate    { get; set; }
    public long    NonLikedPageUserCount   { get; set; }
    public decimal NonLikedPageUserRate    { get; set; }
    public decimal TopLikedPageRate        { get; set; }
    public string  TopLikedPageName        { get; set; }
    public decimal MostPopularSegmentRate  { get; set; }
    public string  MostPopularSegmentName  { get; set; }
    public decimal LessInterestSegmentRate { get; set; }
    public string  LessInterestSegmentName { get; set; }
}


public class EcomMetric_Advanced
{
    public long    EcomCustomersNoOrderInXMonthsCount                      { get; set; }
    public decimal EcomCustomersNoOrderInXMonthsRate                       { get; set; }
    public bool    IsEcomCustomersNoOrderInXMonthsPositive                 { get; set; }
    public decimal AverageOrderValueComparedToTheWorld                     { get; set; }
    public decimal AverageOrderValueComparedToTheWorldRate                 { get; set; }
    public long    Only1OrderPurchasedCustomersCount                       { get; set; }
    public decimal Only1OrderPurchasedCustomersRate                        { get; set; }
    public bool    IsOnly1OrderPurchasedCustomersPositive                  { get; set; }
    public decimal RevenueEcomCustomerHasMoreThan1000FollowerAndFriend     { get; set; }
    public decimal RevenueEcomCustomerHasMoreThan1000FollowerAndFriendRate { get; set; }
    
    public EcomMetric_Advanced()
    {
        IsEcomCustomersNoOrderInXMonthsPositive = false;
        IsOnly1OrderPurchasedCustomersPositive  = false;
    }
}

public class MetricDataSource
{
    public List<HaravanOrder>    AllOrders         { get; set; } = new();
    public List<HaravanOrder>    FilteredOrders      { get; set; } = new();
    //
    public List<HaravanCustomer> AllCustomers      { get; set; } = new();
    public List<HaravanCustomer> FilteredCustomers   { get; set; } = new();
    //
    public List<DatalytisUser>   AllEcomUsers      { get; set; } = new();
    public List<DatalytisUser>   FilteredEcomUsers   { get; set; } = new();
    //
    public List<DatalytisUser>   AllSocialUsers    { get; set; } = new();
    public List<DatalytisUser>   FilteredSocialUsers { get; set; } = new();
    //
    public List<DatalytisUserSocialInsight> Insights           { get; set; } = new();
    //
    public MetricConfigs                    MetricConfigs      { get; set; }
}

public class InsightUser
{
    public int EcomUserCount   { get; set; }
    public int SocialUserCount { get; set; }
    public int IntersectUserCount { get; set; }
}