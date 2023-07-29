using LookOn.MongoDB;
using Volo.Abp.Autofac;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.Modularity;

namespace LookOn.DbMigrator;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(LookOnMongoDbModule),
    typeof(LookOnApplicationContractsModule)
)]
public class LookOnDbMigratorModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpBackgroundJobOptions>(options =>
        {
            options.IsJobExecutionEnabled = false;
        });
    }
}
