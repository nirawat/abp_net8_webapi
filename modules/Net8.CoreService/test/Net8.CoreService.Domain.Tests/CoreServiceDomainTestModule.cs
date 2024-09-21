using Volo.Abp.Modularity;

namespace Net8.CoreService;

[DependsOn(
    typeof(CoreServiceDomainModule),
    typeof(CoreServiceTestBaseModule)
)]
public class CoreServiceDomainTestModule : AbpModule
{

}
