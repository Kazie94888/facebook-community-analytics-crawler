using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace LookOn.Users;

public interface IUserAppService : IApplicationService
{
    Task<AppUserDto> GetByEmailAsync(string email);
}