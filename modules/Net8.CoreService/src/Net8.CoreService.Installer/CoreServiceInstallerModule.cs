using Volo.Abp.Modularity;
using Volo.Abp.VirtualFileSystem;

namespace Net8.CoreService;

[DependsOn(
    typeof(AbpVirtualFileSystemModule)
    )]
public class CoreServiceInstallerModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpVirtualFileSystemOptions>(options =>
        {
            options.FileSets.AddEmbedded<CoreServiceInstallerModule>();
        });
    }
}
