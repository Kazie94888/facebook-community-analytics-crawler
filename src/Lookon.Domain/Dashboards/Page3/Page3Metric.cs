using System.Collections.Generic;
using LookOn.Dashboards.DashboardBases;

namespace LookOn.Dashboards.Page3;

public class Page3Metric : MetricBase
{
    public Page3SocialMetric_Demographic          Demographic         { get; set; }
    public SocialMetric_CommunityInteraction       CommunityInteraction { get; set; }
    public IList<SocialMetric_LocationByProvince> LocationByProvinces { get; set; }
}

public class Page3SocialMetric_Demographic : SocialMetric_Demographic
{
    public int     LikedPageNoPurchasedOrderUserCount { get; set; }
    public decimal LikedPageNoPurchasedOrderUserRate  { get; set; }
}