using Volo.Abp.Modularity;

namespace Net8.CoreService;

[DependsOn(
    typeof(CoreServiceApplicationModule),
    typeof(CoreServiceDomainTestModule)
    )]
public class CoreServiceApplicationTestModule : AbpModule
{

}
