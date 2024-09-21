using Volo.Abp.Reflection;

namespace Net8.CoreService.Permissions;

public class CoreServicePermissions
{
    public const string GroupName = "CoreService";

    public static string[] GetAll()
    {
        return ReflectionHelper.GetPublicConstantsRecursively(typeof(CoreServicePermissions));
    }
}
