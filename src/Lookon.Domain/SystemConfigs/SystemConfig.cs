using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace LookOn.SystemConfigs;

public class SystemConfig : FullAuditedEntity<Guid>
{
    public DataAccumulationConfig DataAccumulationConfig { get; set; }
}

public class DataAccumulationConfig
{
    // public bool Page1_FetchAllEcomOrders    { get; set; }
    // public bool Page1_FetchAllSocialUsers   { get; set; }
    // public bool Page1_FetchValidSocialUsers { get; set; }
    // public bool Page2_FetchAllSocialUsers   { get; set; }
    // public bool Page2_FetchValidSocialUsers { get; set; }
    // public bool Page3_FetchAllSocialUsers   { get; set; }
    // public bool Page3_FetchValidSocialUsers { get; set; }
}