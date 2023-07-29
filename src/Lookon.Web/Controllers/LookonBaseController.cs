using LookOn.Configs;
using LookOn.Localization;
using Microsoft.Extensions.Configuration;
using Volo.Abp.AspNetCore.Mvc;

namespace LookOn.Web.Controllers;

public abstract class LookOnBaseController : AbpController
{
    public IConfiguration Configuration { get; set; }
    public GlobalConfig GlobalConfig { get; set; }

    protected LookOnBaseController()
    {
        LocalizationResource = typeof(LookOnResource);
    }
}