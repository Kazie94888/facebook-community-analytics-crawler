using System.Threading.Tasks;
using LookOn.Integrations.Datalytis;
using LookOn.Integrations.Haravan;
using LookOn.MongoDB;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Volo.Abp;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;

namespace LookOn.Console.Dev;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(LookOnMongoDbModule)
)]
public class DevModule : AbpModule
{
    public override Task OnApplicationInitializationAsync(ApplicationInitializationContext context)
    {
        var logger = context.ServiceProvider.GetRequiredService<ILogger<DevModule>>();
        var configuration = context.ServiceProvider.GetRequiredService<IConfiguration>();

        var hostEnvironment = context.ServiceProvider.GetRequiredService<IHostEnvironment>();
        logger.LogInformation($"EnvironmentName => {hostEnvironment.EnvironmentName}");

        return Task.CompletedTask;
    }
}
