using LookOn.Integrations.Datalytis.Models.Entities;
using MongoDB.Driver;
using Volo.Abp.Data;
using Volo.Abp.MongoDB;

namespace LookOn.Integrations.Datalytis;

[ConnectionStringName("LookOnDatalytis")]
public class DatalytisDbContext : AbpMongoDbContext
{
    [MongoCollection("Users")] public IMongoCollection<DatalytisUser> Users => Collection<DatalytisUser>();
    [MongoCollection("UserSocialInsights")] public IMongoCollection<DatalytisUserSocialInsight> UserSocialInsights =>
        Collection<DatalytisUserSocialInsight>();

    protected override void CreateModel(IMongoModelBuilder modelBuilder)
    {
        base.CreateModel(modelBuilder);

        //Customize the configuration for your collections.
    }
}