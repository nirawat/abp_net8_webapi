using Volo.Abp.Settings;

namespace Net8.Settings;

public class Net8SettingDefinitionProvider : SettingDefinitionProvider
{
    public override void Define(ISettingDefinitionContext context)
    {
        //Define your own settings here. Example:
        //context.Add(new SettingDefinition(Net8Settings.MySetting1));
    }
}
