using System;
using LookOn.Core.Shared.Enums;

namespace LookOn.Dashboards.DashboardBases;

public class MetricBase
{
    public Guid          MerchantId    { get; set; }
    public TimeFrameType TimeFrameType { get; set; }
    public DateTime      From      { get; set; }
    public DateTime      To       { get; set; }
}