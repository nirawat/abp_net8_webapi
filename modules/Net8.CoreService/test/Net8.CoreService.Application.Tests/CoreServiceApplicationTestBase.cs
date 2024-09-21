using Volo.Abp.Modularity;

namespace Net8.CoreService;

/* Inherit from this class for your application layer tests.
 * See SampleAppService_Tests for example.
 */
public abstract class CoreServiceApplicationTestBase<TStartupModule> : CoreServiceTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
