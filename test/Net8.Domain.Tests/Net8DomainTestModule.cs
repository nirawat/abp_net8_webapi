using Volo.Abp.Modularity;

namespace Net8;

[DependsOn(
    typeof(Net8DomainModule),
    typeof(Net8TestBaseModule)
)]
public class Net8DomainTestModule : AbpModule
{

}
