using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace LookOn.Dashboards.Page2;

public class Page2MetricEntity : AuditedEntity<Guid>
{
    public Page2Metric Metric        { get; set; }
    public Guid        MerchantId    { get; set; }
    public string      MerchantEmail { get; set; }
    public DateTime    CreatedAt     { get; set; }
}