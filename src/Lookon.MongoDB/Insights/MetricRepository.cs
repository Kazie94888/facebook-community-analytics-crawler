using System;
using LookOn.Insights.Metrics;
using LookOn.Merchants;
using LookOn.MongoDB;
using Volo.Abp.Domain.Repositories.MongoDB;
using Volo.Abp.MongoDB;

namespace LookOn.Insights;

public class MetricRepository : MongoDbRepository<LookOnMongoDbContext, Metric, Guid>, IMetricRepository
{
    public MetricRepository(IMongoDbContextProvider<LookOnMongoDbContext> dbContextProvider) : base(dbContextProvider)
    {
    }
}