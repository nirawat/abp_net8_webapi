using Net8.Localization;
using Volo.Abp.Application.Services;

namespace Net8;

/* Inherit your application services from this class.
 */
public abstract class Net8AppService : ApplicationService
{
    protected Net8AppService()
    {
        LocalizationResource = typeof(Net8Resource);
    }
}
