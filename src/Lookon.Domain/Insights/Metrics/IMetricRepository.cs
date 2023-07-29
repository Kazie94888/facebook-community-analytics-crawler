using System;
using Volo.Abp.Domain.Repositories;

namespace LookOn.Insights.Metrics;

public interface IMetricRepository : IRepository<Metric, Guid>
{
    
}