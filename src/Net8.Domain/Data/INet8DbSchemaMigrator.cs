using System.Threading.Tasks;

namespace Net8.Data;

public interface INet8DbSchemaMigrator
{
    Task MigrateAsync();
}
