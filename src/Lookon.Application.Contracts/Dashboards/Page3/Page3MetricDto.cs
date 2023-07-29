using System;
using System.Collections.Generic;
using LookOn.Dashboards.DashboardBase;

namespace LookOn.Dashboards.Page3;

public class Page3MetricDto
{
    public Page3SocialMetric_DemographicDto          Demographic         { get; set; }
    public SocialMetric_CommunityInteractionDto       CommunityInteraction { get; set; }
    public IList<SocialMetric_LocationByProvinceDto> LocationByProvinces { get; set; }
}

public class Page3SocialMetric_DemographicDto : SocialMetric_DemographicDto
{
    public int     LikedPageNoPurchasedOrderUserCount   { get; set; }
    public decimal LikedPageNoPurchasedOrderUserRate    { get; set; }
    public decimal LikedPageNoPurchasedOrderUserPercent => Math.Round(LikedPageNoPurchasedOrderUserRate * 100, 2);
}