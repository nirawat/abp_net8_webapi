using Volo.Abp.Ui.Branding;
using Volo.Abp.DependencyInjection;
using Microsoft.Extensions.Localization;
using Net8.Localization;

namespace Net8.Web;

[Dependency(ReplaceServices = true)]
public class Net8BrandingProvider : DefaultBrandingProvider
{
    private IStringLocalizer<Net8Resource> _localizer;

    public Net8BrandingProvider(IStringLocalizer<Net8Resource> localizer)
    {
        _localizer = localizer;
    }

    public override string AppName => _localizer["AppName"];
}
