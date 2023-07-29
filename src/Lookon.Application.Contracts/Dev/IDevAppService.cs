using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace LookOn.Dev;

public interface IDevAppService : IApplicationService
{
    Task TestAsync();
}