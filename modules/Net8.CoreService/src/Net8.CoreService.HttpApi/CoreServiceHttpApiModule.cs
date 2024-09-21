using Localization.Resources.AbpUi;
using Net8.CoreService.Localization;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Localization;
using Volo.Abp.Modularity;
using Microsoft.Extensions.DependencyInjection;

namespace Net8.CoreService;

[DependsOn(
    typeof(CoreServiceApplicationContractsModule),
    typeof(AbpAspNetCoreMvcModule))]
public class CoreServiceHttpApiModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        PreConfigure<IMvcBuilder>(mvcBuilder =>
        {
            mvcBuilder.AddApplicationPartIfNotExists(typeof(CoreServiceHttpApiModule).Assembly);
        });
    }

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpLocalizationOptions>(options =>
        {
            options.Resources
                .Get<CoreServiceResource>()
                .AddBaseTypes(typeof(AbpUiResource));
        });
    }
}
