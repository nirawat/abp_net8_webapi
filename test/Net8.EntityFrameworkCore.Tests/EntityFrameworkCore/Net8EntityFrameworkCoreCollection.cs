using Xunit;

namespace Net8.EntityFrameworkCore;

[CollectionDefinition(Net8TestConsts.CollectionDefinitionName)]
public class Net8EntityFrameworkCoreCollection : ICollectionFixture<Net8EntityFrameworkCoreFixture>
{

}
