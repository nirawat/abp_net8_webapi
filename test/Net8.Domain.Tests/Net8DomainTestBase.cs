using Volo.Abp.Modularity;

namespace Net8;

/* Inherit from this class for your domain layer tests. */
public abstract class Net8DomainTestBase<TStartupModule> : Net8TestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
