using System.Threading.Tasks;
using LookOn.Emails;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Application.Services;

namespace LookOn.Dev;

[AllowAnonymous]
public class DevAppService : ApplicationService, IDevAppService
{
    private readonly EmailManager _emailManager;

    public DevAppService(EmailManager emailManager)
    {
        _emailManager = emailManager;
    }

    public async Task TestAsync()
    {
       await  _emailManager.SendEmailAsync(new EmailSendingArgs() { ToEmailAddress = "romeohuy@gmail.com", Body = "test", Subject = "test", IsBodyHtml = true });
    }
}