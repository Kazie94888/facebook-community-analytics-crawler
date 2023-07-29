using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using LookOn.Consts;
using LookOn.Core.Extensions;
using Microsoft.Extensions.Configuration;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Emailing;
using Volo.Abp.Emailing.Smtp;
using Volo.Abp.SettingManagement;
using Volo.Abp.TextTemplating;

namespace LookOn.Emails;

public class EmailSendingJob: AsyncBackgroundJob<EmailSendingArgs>, ITransientDependency
{
    public override async Task ExecuteAsync(EmailSendingArgs args)
    {
        var mailMessage = new MailMessage()
        {
            To         = { args.ToEmailAddress },
            Subject    = args.Subject,
            Body       = args.Body,
            IsBodyHtml = args.IsBodyHtml
        };
        foreach (var ccEmail in args.CCEmailAddress)
        {
            mailMessage.CC.Add(new MailAddress(ccEmail));
        }
        await SendEmailAsync(mailMessage);
    }
    
    public ISmtpEmailSender  SmtpEmailSender  { get; set; }
        public ITemplateRenderer TemplateRenderer { get; set; }
        public ISettingManager   SettingManager   { get; set; }
        public IConfiguration    Configuration    { get; set; }

        public async Task SendEmailAsync(MailMessage message)
        {
            try
            {
                var from                  = await SettingManager.GetOrNullGlobalAsync(EmailSettingNames.Smtp.UserName);
                var host                  = await SettingManager.GetOrNullGlobalAsync(EmailSettingNames.Smtp.Host);
                var port                  = await SettingManager.GetOrNullGlobalAsync(EmailSettingNames.Smtp.Port);
                var enableSsl             = await SettingManager.GetOrNullGlobalAsync(EmailSettingNames.Smtp.EnableSsl);
                var useDefaultCredentials = await SettingManager.GetOrNullGlobalAsync(EmailSettingNames.Smtp.UseDefaultCredentials);
                var password              = Configuration["Settings:Abp.Mailing.Smtp.PasswordDefault"];

                var client = new SmtpClient();
                client.Port                  = port.ToIntODefault();
                client.Host                  = host;
                client.EnableSsl             = enableSsl.ToBool();
                client.UseDefaultCredentials = useDefaultCredentials.ToBool();
                client.Credentials           = new NetworkCredential(from, password);
                client.DeliveryMethod        = SmtpDeliveryMethod.Network;

                message.IsBodyHtml =   message.IsBodyHtml;
                message.From       ??= new MailAddress(from);
                
                var enableSendClientEmail = Configuration["Abp.Mailing.EnableSendClientEmail"].ToBool();
                if (!enableSendClientEmail)
                {
                    message.To.Clear();
                    message.To.Add(new MailAddress(EmailAddressConsts.DevEmailVeek));
                }
                else
                {
                    message.CC.Add(new MailAddress(EmailAddressConsts.TestEmail));
                }
                await client.SendMailAsync(message);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public Stream GetStream(byte[] excelBytes)
        {
            using var steam = new MemoryStream(excelBytes, true);
            steam.Write(excelBytes, 0, excelBytes.Length);
            steam.Position = 0;
            return steam;
        }
}