using Net8.CoreService.Localization;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;

namespace Net8.CoreService.Web.Pages;

/* Inherit your PageModel classes from this class.
 */
public abstract class CoreServicePageModel : AbpPageModel
{
    protected CoreServicePageModel()
    {
        LocalizationResourceType = typeof(CoreServiceResource);
        ObjectMapperContext = typeof(CoreServiceWebModule);
    }
}
