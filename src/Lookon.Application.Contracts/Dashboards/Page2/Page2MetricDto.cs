using System;
using System.Collections.Generic;
using LookOn.Dashboards.DashboardBase;

namespace LookOn.Dashboards.Page2;

public class Page2MetricDto
{
    public Page2Metric_SocialInsightDto              SocialInsight       { get; set; }
    public Page2SocialMetric_DemographicDto          Demographic         { get; set; }
    public IList<SocialMetric_LocationByProvinceDto> LocationByProvinces { get; set; }
    public SocialMetric_CommunityInteractionDto       CommunityInteraction { get; set; }
    public Page2Metric_ComparisionDto                SocialComparision   { get; set; }
}

public class Page2Metric_SocialInsightDto
{
    public long    NonPurchasedUserCount      { get; set; }
    public decimal NonPurchasedUserRate       { get; set; }
    public decimal TopLikedPageRate           { get; set; }
    public string  TopLikedPageName           { get; set; }
    public decimal MostPopularSegmentRate     { get; set; }
    public string  MostPopularSegmentName     { get; set; }
    public decimal LessInterestSegmentRate    { get; set; }
    public string  LessInterestSegmentName    { get; set; }
    
    public decimal NonPurchasedUserPercent    => Math.Round(NonPurchasedUserRate    * 100, 2);
    public decimal TopLikedPagePercent        => Math.Round(TopLikedPageRate        * 100, 2);
    public decimal MostPopularSegmentPercent  => Math.Round(MostPopularSegmentRate  * 100, 2);
    public decimal LessInterestSegmentPercent => Math.Round(LessInterestSegmentRate * 100, 2);
}

public class Page2SocialMetric_DemographicDto : SocialMetric_DemographicDto
{
    public int     LikedPagePurchasedOrderUserCount   { get; set; }
    public decimal LikedPagePurchasedOrderUserRate    { get; set; }
    public decimal LikedPagePurchasedOrderUserPercent => Math.Round(LikedPagePurchasedOrderUserRate * 100, 2);
}

public class Page2Metric_ComparisionDto
{
    public GenderComparisionDto       GenderComparision       { get; set; }
    public AgeComparisionDto          AgeComparision          { get; set; }
    public CarOwnerComparisionDto     CarOwnerComparision     { get; set; }
    public HouseOwnerComparisionDto   HouseOwnerComparision   { get; set; }
    public RelationshipComparisionDto RelationshipComparision { get; set; }
}

public class GenderComparisionDto
{
    public string  PurchasedGenderName          { get; set; }
    public decimal PurchasedGenderRate          { get; set; }
    public string  NonPurchasedGenderName       { get; set; }
    public decimal NonPurchasedGenderRate       { get; set; }
    public decimal PurchaseGenderPercent => Math.Round(PurchasedGenderRate    * 100, 2);
    public decimal NonPurchaseGenderPercent  => Math.Round(NonPurchasedGenderRate * 100, 2);
}

public class AgeComparisionDto
{
    public string  PurchasedAge           { get; set; }
    public decimal PurchasedAgeRate       { get; set; }
    public string  NonPurchasedAge        { get; set; }
    public decimal NonPurchasedAgeRate    { get; set; }
    public decimal PurchasedAgePercent => Math.Round(PurchasedAgeRate    * 100, 2);
    public decimal NonPurchasedAgePercent => Math.Round(NonPurchasedAgeRate * 100, 2);
}

public class CarOwnerComparisionDto
{
    public string  PurchasedCarOwnerStatus         { get; set; }
    public decimal PurchasedCarOwnerRate           { get; set; }
    public string  NonPurchasedCarOwnerStatus      { get; set; }
    public decimal NonPurchasedCarOwnerRate        { get; set; }
    public decimal PurchasedCarOwnerPercent        => Math.Round(PurchasedCarOwnerRate    * 100, 2);
    public decimal NonPurchasedCarOwnerPercent => Math.Round(NonPurchasedCarOwnerRate * 100, 2);
}

public class HouseOwnerComparisionDto
{
    public string  PurchasedHouseOwnerStatus      { get; set; }
    public decimal PurchasedHouseOwnerRate        { get; set; }
    public string  NonPurchasedHouseOwnerStatus   { get; set; }
    public decimal NonPurchasedHouseOwnerRate     { get; set; }
    public decimal PurchasedHouseOwnerPercent => Math.Round(PurchasedHouseOwnerRate    * 100, 2);
    public decimal NonPurchasedHouseOwnerPercent  => Math.Round(NonPurchasedHouseOwnerRate * 100, 2);
}

public class RelationshipComparisionDto
{
    public string  PurchasedRelationship        { get; set; }
    public decimal PurchasedRelationshipRate    { get; set; }
    public string  NonPurchasedRelationship     { get; set; }
    public decimal NonPurchasedRelationshipRate { get; set; }
    public decimal PurchasedRelationshipPercent => Math.Round(PurchasedRelationshipRate * 100, 2);
    public decimal NonPurchasedRelationshipPercent => Math.Round(NonPurchasedRelationshipRate * 100, 2);
}