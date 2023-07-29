using System;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using LookOn.Consts;
using LookOn.Core.Extensions;
using Microsoft.Extensions.Configuration;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.Domain.Services;
using Volo.Abp.Emailing;
using Volo.Abp.Emailing.Smtp;
using Volo.Abp.SettingManagement;
using Volo.Abp.TextTemplating;

namespace LookOn.Emails
{
    public interface IEmailDomainServiceBase : IDomainService
    {
        Task SendEmailAsync(EmailSendingArgs message);
    }

    public abstract class EmailDomainServiceBase : DomainService, IEmailDomainServiceBase
    {
        private readonly IBackgroundJobManager _backgroundJobManager;
        
        public ISettingManager SettingManager { get; set; }

        protected EmailDomainServiceBase(IBackgroundJobManager backgroundJobManager)
        {
            _backgroundJobManager = backgroundJobManager;
        }

        public async Task SendEmailAsync(EmailSendingArgs message)
        {
            await _backgroundJobManager.EnqueueAsync(message);
        }
    }
}