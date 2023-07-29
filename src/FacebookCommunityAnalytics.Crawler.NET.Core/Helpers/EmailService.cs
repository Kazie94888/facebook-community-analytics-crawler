using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using SendGrid;
using SendGrid.Helpers.Mail;
using Attachment = System.Net.Mail.Attachment;

namespace FacebookCommunityAnalytics.Crawler.NET.Core.Helpers
{
    public static class EmailService
    {
        public static async Task SendMail(string subject, string content, string attachmentFilePath, IList<string> emailAddresses)
        {
            var client = new SendGridClient("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");
            
            var from = new EmailAddress("gdl.noreply@gdl.vn", "gdl.noreply@gdl.vn");
            var to = new EmailAddress("dev@veek.vn", "dev@veek.vn");
            var plainTextContent = content;
            var htmlContent = $"<strong>{content}</strong>";
            SendGridMessage msg;
            if (emailAddresses.Any())
            {
                var tos = new List<EmailAddress>();
                foreach (var emailAddress in emailAddresses)
                {
                    tos.Add(new EmailAddress(emailAddress,emailAddress));
                }
                tos.Add(to);
                
                msg  = MailHelper.CreateSingleEmailToMultipleRecipients(from, tos, subject, plainTextContent,
                    htmlContent);
            }
            else
            {
                msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            }
            
            if (!string.IsNullOrWhiteSpace(attachmentFilePath))
            {
                var bytes = File.ReadAllBytes(attachmentFilePath);
                var fileBase64String = Convert.ToBase64String(bytes);
                
                string fileName = new FileInfo(attachmentFilePath).Name;

                msg.AddAttachment(fileName,fileBase64String);
            }
            
            await client.SendEmailAsync(msg);
        }
        
        public static async Task SendUsingGmail(string subject, string content, string attachmentFilePath, IList<string> emailAddresses)
        {
            try
            {
                var from             = "system@gdl.vn";
                var password         = "yozebskdnusiwllc";
                
                var to               = "dev@veek.vn";
                var plainTextContent = content;
                var htmlContent      = $"<strong>{content}</strong>";

                var mailMessage = new MailMessage(from, to, subject, plainTextContent) {IsBodyHtml = true};
                
                if (!string.IsNullOrWhiteSpace(attachmentFilePath))
                {
                    var       fileName   = new FileInfo(attachmentFilePath).Name;
                    // var       bytes      = File.ReadAllBytes(attachmentFilePath);
                    // using var stream     = new MemoryStream(bytes);
                    var       attachment = new Attachment(attachmentFilePath);
                    
                    mailMessage.Attachments.Add(attachment);
                }

                if (emailAddresses.Any())
                {
                    foreach (var emailAddress in emailAddresses)
                    {
                        mailMessage.To.Add(emailAddress);
                    }
                }


                var client = new SmtpClient();
                client.Port                  = 587;  
                client.Host                  = "smtp.gmail.com";
                client.EnableSsl             = true;  
                client.UseDefaultCredentials = false;  
                client.Credentials           = new NetworkCredential(from, password);
                client.DeliveryMethod        = SmtpDeliveryMethod.Network;  
                await client.SendMailAsync(mailMessage);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}