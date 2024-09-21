using Volo.Abp.Modularity;

namespace Net8;

public abstract class Net8ApplicationTestBase<TStartupModule> : Net8TestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
