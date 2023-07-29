using Volo.Abp.DependencyInjection;
using Volo.Abp.Ui.Branding;

namespace LookOn;

[Dependency(ReplaceServices = true)]
public class LookOnBrandingProvider : DefaultBrandingProvider
{
    public override string AppName => "LookOn";
}
