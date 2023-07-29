using LookOn.Integrations.Datalytis.Components.Repositories;
using LookOn.Integrations.Datalytis.Models.Entities;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Modularity;
using Volo.Abp.MongoDB;
using Volo.Abp.Uow;

namespace LookOn.Integrations.Datalytis;

[DependsOn(typeof(DatalytisModule), typeof(AbpMongoDbModule))]
public class DatalytisMongoDbModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddMongoDbContext<DatalytisDbContext>(options =>
        {
            options.AddDefaultRepositories(includeAllEntities: true);
            options.AddRepository<DatalytisUser, MongoDatalytisUserRepository>();
            options.AddRepository<DatalytisUserSocialInsight, MongoDatalytisUserSocialInsightRepository>();
        });

        Configure<AbpUnitOfWorkDefaultOptions>(options => { options.TransactionBehavior = UnitOfWorkTransactionBehavior.Disabled; });
    }
}