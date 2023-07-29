using System;
using LookOn.Core.Shared.Enums;
using LookOn.Enums;

namespace LookOn.Insights;

public class GetMetricInput
{
    public Guid                 MerchantId           { get; set; }
    public MetricDataSourceType MetricDataSourceType { get; set; }
    public TimeFrameType        TimeFrame            { get; set; }
    public DateTime?            EcomFromDateTime     { get; set; }
    public DateTime?            EcomToDateTime      { get; set; }
}

public class GetInsightUserInput
{
    public Guid MerchantId { get; set; }
}