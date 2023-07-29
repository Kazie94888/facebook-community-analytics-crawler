using System.Collections.Generic;
using LookOn.Dashboards.DashboardBases;

namespace LookOn.Dashboards.Page2;

public class Page2Metric : MetricBase
{
    public Page2Metric_SocialInsight              SocialInsight       { get; set; }
    public Page2SocialMetric_Demographic          Demographic         { get; set; }
    public IList<SocialMetric_LocationByProvince> LocationByProvinces { get; set; }
    public SocialMetric_CommunityInteraction       CommunityInteraction { get; set; }
    public Page2Metric_Comparision                SocialComparision   { get; set; }
}

public class Page2SocialMetric_Demographic : SocialMetric_Demographic
{
    public int     LikedPagePurchasedOrderUserCount { get; set; }
    public decimal LikedPagePurchasedOrderUserRate  { get; set; }
}

public class Page2Metric_SocialInsight: SocialMetric_Insight
{
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

public class Page2Metric_Comparision: SocialMetric_Comparision
{
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
    public string PurchasedGenderName { get; set; }
    public decimal PurchasedGenderRate { get; set; }
    public string NonPurchasedGenderName  { get; set; }
    public decimal NonPurchasedGenderRate { get; set; }
}

public class AgeComparision
{
    public string PurchasedAge       { get; set; }
    public decimal PurchasedAgeRate   { get; set; }
    public string NonPurchasedAge { get; set; }
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
    public string  PurchasedHouseOwnerStatus { get; set; }
    public decimal PurchasedHouseOwnerRate   { get; set; }
    public string  NonPurchasedHouseOwnerStatus  { get; set; }
    public decimal NonPurchasedHouseOwnerRate    { get; set; }
}

public class RelationshipComparision
{
    public string  PurchasedRelationship     { get; set; }
    public decimal PurchasedRelationshipRate { get; set; }
    public string  NonPurchasedRelationship      { get; set; }
    public decimal NonPurchasedRelationshipRate  { get; set; }
}

public class SocialGapSummary : MetricBase
{
    public string  MerchantName              { get; set; }
    public int     TotalCustomerDoesNotOrder { get; set; }
    public decimal CustomerDoesNotOrderRate  { get; set; }
    public int     TotalLike                 { get; set; }
    public decimal LikeRate                  { get; set; }
    public int     TotalFollower             { get; set; }
    public decimal FollowerRate              { get; set; }
}



public class SocialNetworkComparision : MetricBase
{
}