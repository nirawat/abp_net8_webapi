using Net8.CoreService.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace Net8.CoreService;

public abstract class CoreServiceController : AbpControllerBase
{
    protected CoreServiceController()
    {
        LocalizationResource = typeof(CoreServiceResource);
    }
}
