using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace LookOn.Dashboards.Page3;

public class Page3MetricEntity : AuditedEntity<Guid>
{
    public Page3Metric Metric        { get; set; }
    public Guid        MerchantId    { get; set; }
    public string      MerchantEmail { get; set; }
    public DateTime    CreatedAt     { get; set; }
    public Page3Filter Filter        { get; set; }
}