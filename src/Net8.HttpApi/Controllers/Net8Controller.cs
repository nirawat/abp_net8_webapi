using Net8.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace Net8.Controllers;

/* Inherit your controllers from this class.
 */
public abstract class Net8Controller : AbpControllerBase
{
    protected Net8Controller()
    {
        LocalizationResource = typeof(Net8Resource);
    }
}
