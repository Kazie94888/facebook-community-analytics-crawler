using System;
using LookOn.Core.Shared.Enums;
using Volo.Abp.Domain.Entities.Auditing;

namespace LookOn.Dashboards.Page1;

public class Page1MetricEntity : AuditedEntity<Guid>
{
    public Page1Metric   Metric        { get; set; }
    public Guid          MerchantId    { get; set; }
    public string        MerchantEmail { get; set; }
    public TimeFrameType TimeFrame     { get; set; }
}