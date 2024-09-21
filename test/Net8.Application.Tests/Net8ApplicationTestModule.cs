using Volo.Abp.Modularity;

namespace Net8;

[DependsOn(
    typeof(Net8ApplicationModule),
    typeof(Net8DomainTestModule)
)]
public class Net8ApplicationTestModule : AbpModule
{

}
