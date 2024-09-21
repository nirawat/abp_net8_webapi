using Net8.CoreService.EntityFrameworkCore.Interface;
using Net8.CoreService.Models.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Net8.CoreService.AppServices
{
    public class AccountAppService : CoreServiceAppService, IAccountAppService
    {
        private readonly IAccountRepositories _iAccountRepositories;

        public AccountAppService(
            IAccountRepositories iAccountRepositories)
        {
            _iAccountRepositories = iAccountRepositories;
        }

        public async Task<DtoAccountUserProfileModel> GetAccountUserProfiles(string userId)
        {
            var result = await _iAccountRepositories.GetAccountUserProfiles(userId);

            var account = new DtoAccountUserProfileModel();

            if (result != null)
            {
                var profile = new DtoAccountUserProfilesModel()
                {
                    Id = result.FirstOrDefault().Id,
                    Name = result.FirstOrDefault().Name,
                    Email =result.FirstOrDefault().Email
                };

                account.Profile = new DtoAccountUserProfilesModel();
                account.Profile = profile;
            }

            return account;
        }
    }
}
