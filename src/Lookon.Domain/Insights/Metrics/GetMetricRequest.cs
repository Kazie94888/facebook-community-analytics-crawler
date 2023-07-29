using System;
using LookOn.Core.Shared.Enums;
using LookOn.Enums;

namespace LookOn.Insights.Metrics;

public class GetMetricRequest
{
    public Guid                 MerchantId           { get; set; }
    public MetricDataSourceType MetricDataSourceType { get; set; }
    public TimeFrameType        TimeFrame            { get; set; }
    public DateTime?            From                 { get; set; }
    public DateTime?            To                   { get; set; }
}