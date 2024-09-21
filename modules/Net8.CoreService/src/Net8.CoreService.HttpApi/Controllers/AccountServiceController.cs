using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Net8.CoreService.AppServices;
using Net8.CoreService.Models.Dto;
using Net8.CoreService.Samples;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;

namespace Net8.CoreService.Controllers
{

    [Area(CoreServiceRemoteServiceConsts.ModuleName)]
    [RemoteService(Name = CoreServiceRemoteServiceConsts.RemoteServiceName)]
    [Route("api/AccountService")]
    public class AccountServiceController : CoreServiceController, IAccountAppService
    {
        private readonly IAccountAppService _iAccountAppService;

        public AccountServiceController(IAccountAppService iAccountAppService)
        {
            _iAccountAppService = iAccountAppService;
        }

        [HttpGet]
        [Route("GetAccountUserProfiles")]
        [Authorize]
        public async Task<DtoAccountUserProfileModel> GetAccountUserProfiles(string userId)
        {
            return await _iAccountAppService.GetAccountUserProfiles(userId);
        }

    }
}
