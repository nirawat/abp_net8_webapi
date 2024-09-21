using Volo.Abp.Domain;
using Volo.Abp.Modularity;

namespace Net8.CoreService;

[DependsOn(
    typeof(AbpDddDomainModule),
    typeof(CoreServiceDomainSharedModule)
)]
public class CoreServiceDomainModule : AbpModule
{

}
