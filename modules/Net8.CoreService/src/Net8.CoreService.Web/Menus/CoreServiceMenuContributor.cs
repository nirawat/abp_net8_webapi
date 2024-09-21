using System.Threading.Tasks;
using Volo.Abp.UI.Navigation;

namespace Net8.CoreService.Web.Menus;

public class CoreServiceMenuContributor : IMenuContributor
{
    public async Task ConfigureMenuAsync(MenuConfigurationContext context)
    {
        if (context.Menu.Name == StandardMenus.Main)
        {
            await ConfigureMainMenuAsync(context);
        }
    }

    private Task ConfigureMainMenuAsync(MenuConfigurationContext context)
    {
        //Add main menu items.
        context.Menu.AddItem(new ApplicationMenuItem(CoreServiceMenus.Prefix, displayName: "CoreService", "~/CoreService", icon: "fa fa-globe"));

        return Task.CompletedTask;
    }
}
