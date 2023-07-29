using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Volo.Abp;
using Volo.Abp.Autofac;
using Volo.Abp.AutoMapper;
using Volo.Abp.Modularity;
using Volo.Abp.MongoDB;

namespace LookOn.Integrations.Haravan;

[DependsOn(typeof(AbpAutofacModule)
            , typeof(AbpAutoMapperModule))]
public class HaravanModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpAutoMapperOptions>(options =>
        {
            //Add all mappings defined in the assembly of the MyModule class
            options.AddMaps<HaravanModule>();
            options.AddProfile<HaravanAutoMapperProfile>();
        });
    }
}