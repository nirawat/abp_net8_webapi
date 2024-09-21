using Volo.Abp.Autofac;
using Volo.Abp.Http.Client.IdentityModel;
using Volo.Abp.Modularity;

namespace Net8.CoreService;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(CoreServiceHttpApiClientModule),
    typeof(AbpHttpClientIdentityModelModule)
    )]
public class CoreServiceConsoleApiClientModule : AbpModule
{

}
