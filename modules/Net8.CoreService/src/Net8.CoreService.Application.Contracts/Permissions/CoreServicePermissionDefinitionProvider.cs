using Net8.CoreService.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace Net8.CoreService.Permissions;

public class CoreServicePermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var myGroup = context.AddGroup(CoreServicePermissions.GroupName, L("Permission:CoreService"));
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<CoreServiceResource>(name);
    }
}
