using LookOn.Configs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Modularity;

namespace LookOn;

public class GlobalConfigurationModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        var services = context.Services;
        var configuration = context.Services.GetConfiguration();

        // global config
        var globalConfigurationSection = configuration.GetSection(nameof(GlobalConfig));
        var globalConfiguration = globalConfigurationSection.Get<GlobalConfig>();
        if (globalConfiguration != null) services.AddSingleton(globalConfiguration);
        
        // Haravan config
        // var haravanConfigurationSection = configuration.GetSection(nameof(HaravanConfiguration));
        // var haravanConfiguration        = haravanConfigurationSection.Get<HaravanConfiguration>();
        // if (haravanConfiguration != null) services.AddSingleton(haravanConfiguration);
    }
}