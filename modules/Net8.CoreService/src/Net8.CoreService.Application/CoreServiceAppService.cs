using Net8.CoreService.Localization;
using Volo.Abp.Application.Services;

namespace Net8.CoreService;

public abstract class CoreServiceAppService : ApplicationService
{
    protected CoreServiceAppService()
    {
        LocalizationResource = typeof(CoreServiceResource);
        ObjectMapperContext = typeof(CoreServiceApplicationModule);
    }
}
