using Net8.Localization;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;

namespace Net8.Web.Pages;

public abstract class Net8PageModel : AbpPageModel
{
    protected Net8PageModel()
    {
        LocalizationResourceType = typeof(Net8Resource);
    }
}
