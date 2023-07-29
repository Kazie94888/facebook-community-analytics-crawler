using System;
using System.Collections.Generic;
using LookOn.Core.Shared.Enums;
using LookOn.Dashboards.DashboardBase;
using LookOn.Enums;
using LookOn.Merchants;
using Volo.Abp.Application.Dtos;

namespace LookOn.Insights;

public class MetricDto : AuditedEntityDto<Guid>
{
    public Guid                MerchantId { get; set; }
    public List<MetricItemDto> Items      { get; set; }
    public DateTime            CreatedAt  { get; set; }

    public MetricDto()
    {
        CreatedAt = DateTime.UtcNow;
        Items     = new List<MetricItemDto>();
    }
}

public class MetricItemDto
{
    public MetricDataSourceType DataSourceType { get; set; }
    public TimeFrameType        TimeFrameType  { get; set; }

    //
    public EcomMetric_SummaryDto                 EcomSummary            { get; set; }
    public EcomMetric_RevenueSummaryDto          EcomRevenueSummary     { get; set; }
    public List<EcomMetric_RevenueByProductDto>  EcomRevenueByProducts  { get; set; }
    public List<EcomMetric_RevenueByLocationDto> EcomRevenueByLocations { get; set; }
    public EcomMetric_AdvancedDto                EcomAdvanced           { get; set; }

    //
    public SocialMetric_DemographicDto              SocialDemographic                 { get; set; }
    public List<SocialMetric_LocationByProvinceDto> SocialLocationByProvinces         { get; set; }
    public SocialMetric_CommunityInteractionDto     SocialCommunityInteraction        { get; set; }
    public List<string>                             SocialAboveNormalInfluencerPhones { get; set; }
    public SocialMetric_InsightDto                  SocialInsight                     { get; set; }
    public SocialMetric_ComparisionDto              SocialComparision                 { get; set; }
}

public class InsightUserDto
{
    public int EcomUserCount      { get; set; }
    public int SocialUserCount    { get; set; }
    public int IntersectUserCount { get; set; }
}