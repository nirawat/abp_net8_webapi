using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Net8.CoreService.Samples;

public interface ISampleAppService : IApplicationService
{
    Task<SampleDto> GetAsync();

    Task<SampleDto> GetAuthorizedAsync();
}
