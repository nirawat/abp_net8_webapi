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
    [Route("api/CoreService")]
    public class CoreServicesController : CoreServiceController, ICoreAppService
    {
        private readonly ICoreAppService _iCoreAppService;

        public CoreServicesController(ICoreAppService iCoreAppService)
        {
            _iCoreAppService = iCoreAppService;
        }

        [HttpGet]
        [Route("SignalRSendMessage")]
        public async Task SignalRSendMessage(string code, string message)
        {
            await _iCoreAppService.SignalRSendMessage(code, message);
        }

    }
}
