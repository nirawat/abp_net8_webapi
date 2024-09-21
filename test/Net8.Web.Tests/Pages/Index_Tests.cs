using System.Threading.Tasks;
using Shouldly;
using Xunit;

namespace Net8.Pages;

[Collection(Net8TestConsts.CollectionDefinitionName)]
public class Index_Tests : Net8WebTestBase
{
    [Fact]
    public async Task Welcome_Page()
    {
        var response = await GetResponseAsStringAsync("/");
        response.ShouldNotBeNull();
    }
}
