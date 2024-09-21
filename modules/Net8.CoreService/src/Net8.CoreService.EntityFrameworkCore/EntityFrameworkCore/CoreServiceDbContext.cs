using Microsoft.EntityFrameworkCore;
using Net8.CoreService.Models.Repositories;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;

namespace Net8.CoreService.EntityFrameworkCore;

[ConnectionStringName(CoreServiceDbProperties.ConnectionStringName)]
public class CoreServiceDbContext : AbpDbContext<CoreServiceDbContext>, ICoreServiceDbContext
{
    /* Add DbSet for each Aggregate Root here. Example:
     * public DbSet<Question> Questions { get; set; }
     */

    public DbSet<DbAccountUserProfile> AccountUserProfiles { get; set; }
    public CoreServiceDbContext(DbContextOptions<CoreServiceDbContext> options)
        : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        string schema = "abp_net8_business";

        base.OnModelCreating(builder);

        var entityAccountUserProfile = builder.Entity<DbAccountUserProfile>();
        entityAccountUserProfile.ToTable("Account_User_Profile", schema);

        builder.ConfigureCoreService();
    }
}
