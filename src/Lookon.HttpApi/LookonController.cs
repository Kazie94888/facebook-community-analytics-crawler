using LookOn.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace LookOn;

/* Inherit your controllers from this class.
 */
public abstract class LookOnController : AbpControllerBase
{
    protected LookOnController()
    {
        LocalizationResource = typeof(LookOnResource);
    }
}
