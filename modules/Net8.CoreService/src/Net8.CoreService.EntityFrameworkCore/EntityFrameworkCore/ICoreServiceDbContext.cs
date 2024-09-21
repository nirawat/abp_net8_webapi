using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;

namespace Net8.CoreService.EntityFrameworkCore;

[ConnectionStringName(CoreServiceDbProperties.ConnectionStringName)]
public interface ICoreServiceDbContext : IEfCoreDbContext
{
    /* Add DbSet for each Aggregate Root here. Example:
     * DbSet<Question> Questions { get; }
     */
}
