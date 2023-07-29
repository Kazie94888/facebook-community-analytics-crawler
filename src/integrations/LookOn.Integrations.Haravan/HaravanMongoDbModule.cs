using LookOn.Integrations.Haravan.Components.Repositories;
using LookOn.Integrations.Haravan.Models.Entities;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Modularity;
using Volo.Abp.MongoDB;
using Volo.Abp.Uow;

namespace LookOn.Integrations.Haravan;

[DependsOn(
    typeof(HaravanModule),
    typeof(AbpMongoDbModule)
    )]
public class HaravanMongoDbModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddMongoDbContext<HaravanDbContext>(options =>
        {
                options.AddDefaultRepositories(includeAllEntities: true);
                options.AddRepository<HaravanStore,MongoHaravanStoreRepository>();
                options.AddRepository<HaravanCustomer,MongoHaravanStoreRepository>();
                options.AddRepository<HaravanOrder,MongoHaravanStoreRepository>();
        });
        
        Configure<AbpUnitOfWorkDefaultOptions>(options =>
        {
            options.TransactionBehavior = UnitOfWorkTransactionBehavior.Disabled;
        });
    }
}
