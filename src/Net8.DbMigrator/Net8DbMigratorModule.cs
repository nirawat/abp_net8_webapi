using Net8.EntityFrameworkCore;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;

namespace Net8.DbMigrator;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(Net8EntityFrameworkCoreModule),
    typeof(Net8ApplicationContractsModule)
)]
public class Net8DbMigratorModule : AbpModule
{
}
