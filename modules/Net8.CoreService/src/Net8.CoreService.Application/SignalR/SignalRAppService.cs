using Microsoft.AspNetCore.SignalR;
using Net8.CoreService.Models.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Net8.CoreService.SignalR
{
    public class SignalRAppService
    {
        private readonly IHubContext<SignalRHub> _signalRHub;

        private readonly string _signalRChanel = "signalr-chanel";

        public SignalRAppService(IHubContext<SignalRHub> signalRHub)
        {
            _signalRHub = signalRHub;
        }

        public void SignalRSendMessageSync(string code, string message)
        {
            SignalRSendMessage(code, message).GetAwaiter().GetResult();
        }

        public async Task SignalRSendMessage(string code, string message) => await _signalRHub.Clients.All.SendAsync(_signalRChanel, code, new SignalRResponseModel() { id = code, name = message });
    }
}
