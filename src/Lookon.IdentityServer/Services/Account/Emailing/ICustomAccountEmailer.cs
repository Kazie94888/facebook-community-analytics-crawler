using System.Threading.Tasks;
using Volo.Abp.Identity;

namespace LookOn.Services.Account.Emailing;

public interface ICustomAccountEmailer
{
    Task SendPasswordResetLinkAsync(
        IdentityUser user,
        string       resetToken,
        string       appName,
        string       returnUrl     = null,
        string       returnUrlHash = null
    );
}