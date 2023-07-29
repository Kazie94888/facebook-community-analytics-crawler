using System.Collections.Generic;
using LookOn.Enums;

namespace LookOn.Dashboards.DashboardBases;

public class SocialMetric_Demographic
{
    public int                             HouseOwnerCount      { get; set; }
    public decimal                         HouseOwnerRate       { get; set; }
    public int                             CarOwnerCount        { get; set; }
    public decimal                         CarOwnerRate         { get; set; }
    public List<SocialMetric_Gender>       GenderMetrics        { get; set; }
    public List<SocialMetric_AgeRange>     AgeMetrics           { get; set; }
    public List<SocialMetric_Relationship> RelationshipMetrics  { get; set; }
    
    public SocialMetric_PurchaseNoLike PurchaseNoLike { get; set; }
    public SocialMetric_NoPurchaseLikePage  NoPurchaseLikePage  { get; set; }
}


public class SocialMetric_PurchaseNoLike
{
    public int     Count      { get; set; }
    public decimal Rate       { get; set; }
    public bool    IsPositive { get; set; }

    public SocialMetric_PurchaseNoLike()
    {
        IsPositive = false;
    }
}

public class SocialMetric_NoPurchaseLikePage
{
    public int     Count      { get; set; }
    public decimal Rate       { get; set; }
    public bool    IsPositive { get; set; }

    public SocialMetric_NoPurchaseLikePage()
    {
        IsPositive = true;
    }
}


public class SocialMetric_AgeRange
{
    public AgeSegmentEnum AgeSegmentEnum { get; set; }
    public decimal        Rate           { get; set; }
}

public class SocialMetric_Gender
{
    public string  Type { get; set; }
    public int Count { get; set; }
}

public class SocialMetric_Relationship
{
    public string  Name { get; set; }
    public int Count { get; set; }
}

public class SocialMetric_LocationByProvince
{
    public        long    Count  { get; set; }
    public        string  Name   { get; set; }
    public        decimal Rate   { get; set; }
    public static int     Amount => 5;
}

public class SocialMetric_CommunityInteraction
{
    public List<SocialMetric_TopFollower>        TopFollowerMetrics  { get; set; }
    public List<SocialMetric_TopLikedPage>       TopLikedPageMetrics { get; set; }
    public List<SocialMetric_TopCheckinLocation> TopCheckinLocations { get; set; }
    public List<SocialMetric_TopGroup>           TopGroups           { get; set; }
}

public class SocialMetric_TopFollower
{
    public        InfluencerTypeByFollower InfluencerTypeByFollower { get; set; }
    public        decimal                  Rate                     { get; set; }
    public static int                      Amount                   => 5;
}

public class SocialMetric_TopLikedPage
{
    public        string  Name   { get; set; }
    public        long    Count  { get; set; }
    public        decimal Rate   { get; set; }
    public static int     Amount => 5;
}

public class SocialMetric_TopCheckinLocation
{
    public        long    Count  { get; set; }
    public        string  Name   { get; set; }
    public        decimal Rate   { get; set; }
    public static int     Amount => 5;
}

public class SocialMetric_TopGroup
{
    public        string  Name   { get; set; }
    public        long    Count  { get; set; }
    public        decimal Rate   { get; set; }
    public static int     Amount => 5;
}