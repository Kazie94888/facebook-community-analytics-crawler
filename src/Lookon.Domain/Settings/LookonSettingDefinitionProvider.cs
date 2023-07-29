using Volo.Abp.Settings;

namespace LookOn.Settings;

public class LookOnSettingDefinitionProvider : SettingDefinitionProvider
{
    public override void Define(ISettingDefinitionContext context)
    {
        //Define your own settings here. Example:
        //context.Add(new SettingDefinition(LookOnSettings.MySetting1));
    }
}
