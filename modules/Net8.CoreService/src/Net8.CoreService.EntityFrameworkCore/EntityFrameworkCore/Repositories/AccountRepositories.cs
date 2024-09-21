using Microsoft.EntityFrameworkCore;
using Net8.CoreService.EntityFrameworkCore.Interface;
using Net8.CoreService.Models.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories.Dapper;
using Volo.Abp.EntityFrameworkCore;

namespace Net8.CoreService.EntityFrameworkCore.Repositories
{
    [ExposeServices(typeof(IAccountRepositories))]
    public class AccountRepositories : DapperRepository<CoreServiceDbContext>, ITransientDependency, IAccountRepositories
    {
        IDbContextProvider<CoreServiceDbContext> _provider;

        public AccountRepositories(IDbContextProvider<CoreServiceDbContext> provider) : base(provider)
        {
            _provider = provider;
        }

        public async Task<DbSet<DbAccountUserProfile>> GetAccountUserProfiles(string id)
        {
            var context = await _provider.GetDbContextAsync();

            return context.AccountUserProfiles;
        }
    }
}
