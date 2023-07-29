using LookOn.Configs;
using LookOn.Localization;
using Microsoft.Extensions.Localization;
using Volo.Abp.Application.Services;
using Volo.Abp.Localization;

namespace LookOn;

/* Inherit your application services from this class.
 */
public abstract class LookOnAppService : ApplicationService
{
    public GlobalConfig                          GlobalConfig { get; set; }
    public IStringLocalizer<LookOnErrorResource> Err          { get; set; }

    protected LookOnAppService()
    {
        LocalizationResource = typeof(LookOnResource);
    }
}