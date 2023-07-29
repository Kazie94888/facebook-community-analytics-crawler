using System;
using System.Collections.Generic;
using LookOn.Dashboards.Page2;
using LookOn.Enums;

namespace LookOn.Dashboards.DashboardBase;

public class SocialMetric_DemographicDto
{
    public int                                 HouseOwnerCount     { get; set; }
    public decimal                             HouseOwnerRate      { get; set; }
    public int                                 CarOwnerCount       { get; set; }
    public decimal                             CarOwnerRate        { get; set; }
    public IList<SocialMetric_GenderDto>       GenderMetrics       { get; set; }
    public IList<SocialMetric_AgeRangeDto>     AgeMetrics          { get; set; }
    public IList<SocialMetric_RelationshipDto> RelationshipMetrics { get; set; }
    public decimal                             HouseOwnerPercent   => Math.Round(HouseOwnerRate * 100, 2);
    public decimal                             CarOwnerPercent     => Math.Round(CarOwnerRate   * 100, 2);
}

public class SocialMetric_GenderDto
{
    public string Type  { get; set; }
    public int    Count { get; set; }
}

public class SocialMetric_AgeRangeDto
{
    public AgeSegmentEnum AgeSegmentEnum { get; set; }
    public decimal        Rate           { get; set; }
    public decimal        Percent        => Math.Round(Rate * 100, 2);
}

public class SocialMetric_RelationshipDto
{
    public string Name  { get; set; }
    public int    Count { get; set; }
}

public class SocialMetric_LocationByProvinceDto
{
    public string  Name    { get; set; }
    public decimal Rate    { get; set; }
    public decimal Percent => Math.Round(Rate * 100, 2);
}

public class SocialMetric_CommunityInteractionDto
{
    public List<SocialMetric_TopFollowerDto>        TopFollowerMetrics  { get; set; }
    public List<SocialMetric_TopLikedPageDto>       TopLikedPageMetrics { get; set; }
    public List<SocialMetric_TopCheckinLocationDto> TopCheckinLocations { get; set; }
    public List<SocialMetric_TopGroupDto>           TopGroups           { get; set; }
}

public class SocialMetric_TopFollowerDto
{
    public InfluencerTypeByFollower InfluencerTypeByFollower { get; set; }
    public decimal                  Rate                     { get; set; }
    public decimal                  Percent                  => Math.Round(Rate * 100, 2);
}

public class SocialMetric_TopLikedPageDto
{
    public string  Name    { get; set; }
    public long    Count   { get; set; }
    public decimal Rate    { get; set; }
    public decimal Percent => Math.Round(Rate * 100, 2);
}

public class SocialMetric_TopCheckinLocationDto
{
    public long    Count   { get; set; }
    public string  Name    { get; set; }
    public decimal Rate    { get; set; }
    public decimal Percent => Math.Round(Rate * 100, 2);
}

public class SocialMetric_TopGroupDto
{
    public string  Name    { get; set; }
    public long    Count   { get; set; }
    public decimal Rate    { get; set; }
    public decimal Percent => Math.Round(Rate * 100, 2);
}

public class SocialMetric_InsightDto
{
    public long    NonPurchasedUserCount      { get; set; }
    public decimal NonPurchasedUserRate       { get; set; }
    public long    NonLikePageUserCount       { get; set; }
    public decimal NonLikePageUserRate        { get; set; }
    public decimal TopLikedPageRate           { get; set; }
    public string  TopLikedPageName           { get; set; }
    public decimal MostPopularSegmentRate     { get; set; }
    public string  MostPopularSegmentName     { get; set; }
    public decimal LessInterestSegmentRate    { get; set; }
    public string  LessInterestSegmentName    { get; set; }
    public decimal NonPurchasedUserPercent    => Math.Round(NonPurchasedUserRate    * 100, 2);
    public decimal NonLikePageUserPercent        => Math.Round(NonLikePageUserRate     * 100, 2);
    public decimal TopLikedPagePercent        => Math.Round(TopLikedPageRate        * 100, 2);
    public decimal MostPopularSegmentPercent  => Math.Round(MostPopularSegmentRate  * 100, 2);
    public decimal LessInterestSegmentPercent => Math.Round(LessInterestSegmentRate * 100, 2);
}

public class SocialMetric_ComparisionDto
{
    public GenderComparisionDto       GenderComparision       { get; set; }
    public AgeComparisionDto          AgeComparision          { get; set; }
    public CarOwnerComparisionDto     CarOwnerComparision     { get; set; }
    public HouseOwnerComparisionDto   HouseOwnerComparision   { get; set; }
    public RelationshipComparisionDto RelationshipComparision { get; set; }
}