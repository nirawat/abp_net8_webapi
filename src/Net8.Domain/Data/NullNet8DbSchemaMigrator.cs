using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace Net8.Data;

/* This is used if database provider does't define
 * INet8DbSchemaMigrator implementation.
 */
public class NullNet8DbSchemaMigrator : INet8DbSchemaMigrator, ITransientDependency
{
    public Task MigrateAsync()
    {
        return Task.CompletedTask;
    }
}
