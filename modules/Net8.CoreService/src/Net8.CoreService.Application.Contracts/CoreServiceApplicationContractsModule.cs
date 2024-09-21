using Volo.Abp.Application;
using Volo.Abp.Modularity;
using Volo.Abp.Authorization;

namespace Net8.CoreService;

[DependsOn(
    typeof(CoreServiceDomainSharedModule),
    typeof(AbpDddApplicationContractsModule),
    typeof(AbpAuthorizationModule)
    )]
public class CoreServiceApplicationContractsModule : AbpModule
{

}
