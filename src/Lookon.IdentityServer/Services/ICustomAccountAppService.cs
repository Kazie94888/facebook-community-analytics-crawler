using System.Threading.Tasks;
using Volo.Abp.Account;
using Volo.Abp.Application.Services;
using Volo.Abp.Identity;

namespace LookOn.Services;

public interface ICustomAccountAppService : IApplicationService
{
    Task<IdentityUserDto> RegisterAsync(RegisterDto input);

    Task SendPasswordResetCodeAsync(SendPasswordResetCodeDto input);

    Task ResetPasswordAsync(ResetPasswordDto input);
}