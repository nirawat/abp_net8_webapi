using Net8.CoreService.Models.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Net8.CoreService.AppServices
{
    public interface ICoreAppService
    {
        Task SignalRSendMessage(string code, string message);
    }
}
