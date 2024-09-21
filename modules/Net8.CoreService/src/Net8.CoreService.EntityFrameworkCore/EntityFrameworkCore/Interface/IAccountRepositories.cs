using Microsoft.EntityFrameworkCore;
using Net8.CoreService.Models.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Net8.CoreService.EntityFrameworkCore.Interface
{
    public interface IAccountRepositories
    {
        Task<DbSet<DbAccountUserProfile>> GetAccountUserProfiles(string id);
    }
}
