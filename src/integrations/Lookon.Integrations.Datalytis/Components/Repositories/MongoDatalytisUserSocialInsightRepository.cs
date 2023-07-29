using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LookOn.Enums;
using LookOn.Integrations.Datalytis.Components.Repositories.Interfaces;
using LookOn.Integrations.Datalytis.Models.Entities;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Volo.Abp.Domain.Repositories.MongoDB;
using Volo.Abp.MongoDB;

namespace LookOn.Integrations.Datalytis.Components.Repositories;

public class MongoDatalytisUserSocialInsightRepository : MongoDbRepository<DatalytisDbContext, DatalytisUserSocialInsight, Guid>, IDatalytisUserSocialInsightRepository
{
    public MongoDatalytisUserSocialInsightRepository(IMongoDbContextProvider<DatalytisDbContext> dbContextProvider) : base(dbContextProvider)
    {
    }

    public async Task<List<DatalytisUserSocialInsight>> Get(List<string> communityUids)
    {
        var queryable = await GetMongoQueryableAsync();
        return await queryable.Where(insight => communityUids.Contains(insight.Uid)).ToListAsync();
    }
}