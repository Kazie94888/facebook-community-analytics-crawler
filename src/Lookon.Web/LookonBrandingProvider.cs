using Volo.Abp.Ui.Branding;
using Volo.Abp.DependencyInjection;

namespace LookOn.Web;

[Dependency(ReplaceServices = true)]
public class LookOnBrandingProvider : DefaultBrandingProvider
{
    public override string AppName => "LookOn";
}
