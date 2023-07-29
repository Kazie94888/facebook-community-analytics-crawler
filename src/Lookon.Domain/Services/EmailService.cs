using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Emailing;
using Volo.Abp.Security.Encryption;

namespace LookOn.Services;
public class EmailService : ITransientDependency
{
    private readonly IEmailSender             _emailSender;
    public           IStringEncryptionService EncryptionService { get; set; }

    public EmailService(IEmailSender emailSender)
    {
        _emailSender = emailSender;
    }

    public async Task SendEmailAsync()
    {
        var encryptedGmailPassword = EncryptionService.Encrypt("P@$$w0rdAdmin");
        await _emailSender.SendAsync("romeohuy@gmail.com", "EcomEmail subject", "This is the email body...");
    }
}