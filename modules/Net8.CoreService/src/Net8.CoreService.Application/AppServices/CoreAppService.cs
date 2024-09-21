using Hangfire;
using Microsoft.AspNetCore.SignalR;
using Net8.CoreService.EntityFrameworkCore.Interface;
using Net8.CoreService.EntityFrameworkCore.Repositories;
using Net8.CoreService.Models.Dto;
using Net8.CoreService.Models.Response;
using Net8.CoreService.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Net8.CoreService.AppServices
{
    public class CoreAppService : CoreServiceAppService, ICoreAppService
    {
        //private readonly ICoreRepositories _iCoreRepositories;
        private readonly IHubContext<SignalRHub> _signalRHub;

        private readonly string _signalRChanel = "signalr-chanel";

        public CoreAppService(IHubContext<SignalRHub> signalRHub)
        {
            _signalRHub = signalRHub;
        }

        public async Task SignalRSendMessage(string code, string message) => await _signalRHub.Clients.All.SendAsync(_signalRChanel, code, new SignalRResponseModel() { id = code, name = message});

        public async Task HangfireSendMessageAuto()
        {
            Guid jobId = Guid.NewGuid();
            var manager = new RecurringJobManager();
            manager.AddOrUpdate(jobId.ToString(), () => SignalRSendMessage("Code A", string.Format("Hi my hangfire time {0}.", DateTime.Now.ToString("dd-mm-yyyy hh:mm:ss"))), "* * */1 * *", TimeZoneInfo.Local);
        }
            


    }
}
