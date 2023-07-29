using System;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using LookOn.Consts;
using LookOn.Core.Extensions;
using LookOn.Enums;
using LookOn.Merchants;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Emailing;
using Volo.Abp.Emailing.Templates;
using Volo.Abp.TextTemplating;
using Volo.Abp.SettingManagement;
using MerchantSyncStatus = LookOn.Enums.MerchantSyncStatus;

namespace LookOn.Emails;

// TODOO Tuan: doc here: https://docs.abp.io/en/commercial/latest/modules/text-template-management
public class EmailManager : EmailDomainServiceBase
{
    private readonly ITemplateRenderer                 _templateRenderer;
    private readonly IRepository<EmailHistories, Guid> _emailHistoryRepository;

    // managers
    private readonly EmailTemplateManager _emailTemplateManager;

    public EmailManager(ITemplateRenderer                 templateRenderer,
                        EmailTemplateManager              emailTemplateManager,
                        IRepository<EmailHistories, Guid> emailHistoryRepository,
                        IBackgroundJobManager             backgroundJobManager) : base(backgroundJobManager)
    {
        _templateRenderer       = templateRenderer;
        _emailTemplateManager   = emailTemplateManager;
        _emailHistoryRepository = emailHistoryRepository;
    }

    public async Task SendEmailAsync(string to, string subject, string bodyHtml)
    {
        var from = await SettingManager.GetOrNullGlobalAsync(EmailSettingNames.Smtp.UserName);
        await base.SendEmailAsync(new EmailSendingArgs(to, subject, bodyHtml));
    }

    private async Task<EmailHistories> GetLastEmail(Guid merchantId, EmailType emailType)
    {
        return (await _emailHistoryRepository.GetQueryableAsync()).Where(x => x.IsSuccess && x.MerchantId == merchantId && x.EmailType == emailType)
                                                                  .OrderByDescending(_ => _.CreationTime)
                                                                  .FirstOrDefault();
    }

    /// <summary>
    /// Send sync noti email to merchant.
    /// </summary>
    /// <param name="merchant"></param>
    /// <param name="pageName"></param>
    /// <param name="status"></param>
    public async Task Send_SyncNotification(string pageName, Merchant merchant, MerchantSyncStatus status)
    {
        var emailType = status switch
        {
            MerchantSyncStatus.Pending                           => EmailType.MerchantSyncPending,
            MerchantSyncStatus.SyncEcomOrderCompleted            => EmailType.MerchantSyncEcomOrderCompleted,
            MerchantSyncStatus.SyncSocialUserRequestCompleted    => EmailType.MerchantSyncSocialUserRequestCompleted,
            MerchantSyncStatus.SyncSocialUserScanCompleted       => EmailType.MerchantSyncSocialUserScanCompleted,
            MerchantSyncStatus.SyncSocialUserSyncCompleted       => EmailType.MerchantSyncSocialUserSyncCompleted,
            MerchantSyncStatus.SyncSocialInsightRequestInitiated => EmailType.PageMetric_MerchantSyncSocialInsightRequestInitiated,
            MerchantSyncStatus.SyncSocialInsightRequestCompleted => EmailType.PageMetric_MerchantSyncSocialInsightRequestCompleted,
            MerchantSyncStatus.SyncSocialInsightScanCompleted    => EmailType.PageMetric_MerchantSyncSocialInsightScanCompleted,
            MerchantSyncStatus.DashboardDataReady                => EmailType.PageMetric_MerchantDashboardDataReady,
            _                                                    => EmailType.Unknown
        };

        var lastEmail = await GetLastEmail(merchant.Id, emailType);
        if (lastEmail?.CreationTime.Date == DateTime.UtcNow.Date) return;

        // get email body and subject
        var emailMessage = await _emailTemplateManager.GetMerchantSyncStatusEmailTemplate(pageName, merchant, status);
        if (EnumerableExtensions.IsNullOrEmpty(emailMessage.Body) || EnumerableExtensions.IsNullOrEmpty(emailMessage.Subject)) return;

        // render email body and subject
        var bodyTemplate = await _templateRenderer.RenderAsync(StandardEmailTemplates.Message, new { message = emailMessage.Body });

        // send email
        var message = new EmailSendingArgs
        {
            Subject = emailMessage.Subject, Body = bodyTemplate, IsBodyHtml = true, ToEmailAddress = merchant.Email,
        };
        var sendingEmail = new EmailHistories
        {
            MerchantId   = merchant.Id,
            EmailType    = emailType,
            SendAt       = DateTime.UtcNow,
            IsSuccess    = true,
            Notification = null
        };
        try
        {
            await base.SendEmailAsync(message);
        }
        catch (Exception e)
        {
            sendingEmail.IsSuccess    = false;
            sendingEmail.Notification = e.Message;
        }

        await _emailHistoryRepository.InsertAsync(sendingEmail);
    }

    /// <summary>
    /// Send sub noti email to merchant.
    /// </summary>
    /// <param name="merchant"></param>
    /// <param name="status"></param>
    public async Task Send_SubNotification(Merchant merchant, SubscriptionEmailStatus status)
    {
        var emailType = status switch
        {
            SubscriptionEmailStatus.Added                => EmailType.SubscriptionAdded,
            SubscriptionEmailStatus.Extend               => EmailType.SubscriptionExtend,
            SubscriptionEmailStatus.Upgrade              => EmailType.SubscriptionUpgrade,
            SubscriptionEmailStatus.ExpirationSoon1Month => EmailType.SubscriptionExpirationSoon1Month,
            SubscriptionEmailStatus.ExpirationSoon1Week  => EmailType.SubscriptionExpirationSoon1Week,
            SubscriptionEmailStatus.ExpirationSoon1Day   => EmailType.SubscriptionExpirationSoon1Day,
            _                                            => EmailType.Unknown
        };

        var lastEmail = await GetLastEmail(merchant.Id, emailType);
        if (lastEmail?.CreationTime.Date == DateTime.UtcNow.Date) return;

        // get email body and subject
        var emailMessage = await _emailTemplateManager.GetSubscriptionEmailStatusEmailTemplate(merchant, status);
        if (EnumerableExtensions.IsNullOrEmpty(emailMessage.Body) || EnumerableExtensions.IsNullOrEmpty(emailMessage.Subject)) return;

        // render email body and subject
        var bodyTemplate = await _templateRenderer.RenderAsync(StandardEmailTemplates.Message, new { message = emailMessage.Body });

        // cc to support email
        var supportEmail = await SettingManager.GetOrNullGlobalAsync(EmailSettingNames.Smtp.UserName);

        // send email
        var message = new EmailSendingArgs
        {
            Subject        = emailMessage.Subject,
            Body           = bodyTemplate,
            IsBodyHtml     = true,
            ToEmailAddress = merchant.Email,
            CCEmailAddress = { supportEmail }
        };

        var sendingEmail = new EmailHistories
        {
            MerchantId   = merchant.Id,
            EmailType    = emailType,
            SendAt       = DateTime.UtcNow,
            IsSuccess    = true,
            Notification = null
        };
        try
        {
            await base.SendEmailAsync(message);
        }
        catch (Exception e)
        {
            sendingEmail.IsSuccess    = false;
            sendingEmail.Notification = e.Message;
        }

        await _emailHistoryRepository.InsertAsync(sendingEmail);
    }

    public async Task SendAssignUserEmail(Merchant merchant, string email, string password, bool isNewUser)
    {
        // get email body and subject
        var emailMessage = await _emailTemplateManager.GetAssignUserEmailTemplate(merchant,
                                                                                  email,
                                                                                  password,
                                                                                  isNewUser);
        if (EnumerableExtensions.IsNullOrEmpty(emailMessage.Body) || EnumerableExtensions.IsNullOrEmpty(emailMessage.Subject)) return;

        // render email body and subject
        var bodyTemplate = await _templateRenderer.RenderAsync(StandardEmailTemplates.Message, new { message = emailMessage.Body });

        // cc to support email
        var supportEmail = await SettingManager.GetOrNullGlobalAsync(EmailSettingNames.Smtp.UserName);

        // send email
        var message = new EmailSendingArgs()
        {
            Subject        = emailMessage.Subject,
            Body           = bodyTemplate,
            IsBodyHtml     = true,
            ToEmailAddress = email,
            CCEmailAddress = { supportEmail }
        };

        var sendingEmail = new EmailHistories
        {
            MerchantId   = merchant.Id,
            EmailType    = EmailType.NewAccount,
            SendAt       = DateTime.UtcNow,
            IsSuccess    = true,
            Notification = null
        };
        try
        {
            await base.SendEmailAsync(message);
        }
        catch (Exception e)
        {
            sendingEmail.IsSuccess    = false;
            sendingEmail.Notification = e.Message;
        }

        await _emailHistoryRepository.InsertAsync(sendingEmail);
    }

    public async Task Send_NewCommunityNotification(Merchant merchant, string url, bool invalidCommunity, string communityName)
    {
        // get email body and subject
        var emailMessage = await _emailTemplateManager.GetNewCommunityEmailTemplate(merchant, url, invalidCommunity, communityName);
        if (EnumerableExtensions.IsNullOrEmpty(emailMessage.Body) || EnumerableExtensions.IsNullOrEmpty(emailMessage.Subject)) return;

        // render email body and subject
        var bodyTemplate = await _templateRenderer.RenderAsync(StandardEmailTemplates.Message, new { message = emailMessage.Body });

        // send and cc to admin, support email 
        var supportEmail = await SettingManager.GetOrNullGlobalAsync(EmailSettingNames.Smtp.UserName);
        var adminEmail   = EmailAddressConsts.AdminEmail;

        // send email
        var message = new EmailSendingArgs()
        {
            Subject        = emailMessage.Subject,
            Body           = bodyTemplate,
            IsBodyHtml     = true,
            ToEmailAddress = adminEmail,
            CCEmailAddress = { supportEmail }
        };

        var sendingEmail = new EmailHistories
        {
            MerchantId   = merchant.Id,
            EmailType    = EmailType.NewCommunity,
            SendAt       = DateTime.UtcNow,
            IsSuccess    = true,
            Notification = null
        };

        try
        {
            await base.SendEmailAsync(message);
        }
        catch (Exception e)
        {
            sendingEmail.IsSuccess    = false;
            sendingEmail.Notification = e.Message;
        }

        await _emailHistoryRepository.InsertAsync(sendingEmail);
    }

    public async Task Send_VerifyCommunityNotification(Merchant merchant, string communityName, bool isApproved)
    {
        // get email body and subject
        var emailMessage = await _emailTemplateManager.GetVerifyCommunityEmailTemplate(merchant, communityName, isApproved);
        if (EnumerableExtensions.IsNullOrEmpty(emailMessage.Body) || EnumerableExtensions.IsNullOrEmpty(emailMessage.Subject)) return;

        // render email body and subject
        var bodyTemplate = await _templateRenderer.RenderAsync(StandardEmailTemplates.Message, new { message = emailMessage.Body });

        // send and cc to admin, support email 
        var supportEmail = await SettingManager.GetOrNullGlobalAsync(EmailSettingNames.Smtp.UserName);

        // send email
        var message = new EmailSendingArgs()
        {
            Subject        = emailMessage.Subject,
            Body           = bodyTemplate,
            IsBodyHtml     = true,
            ToEmailAddress = merchant.Email,
            CCEmailAddress = { supportEmail }
        };

        var sendingEmail = new EmailHistories
        {
            MerchantId   = merchant.Id,
            EmailType    = EmailType.NewCommunity,
            SendAt       = DateTime.UtcNow,
            IsSuccess    = true,
            Notification = null
        };

        try
        {
            await base.SendEmailAsync(message);
        }
        catch (Exception e)
        {
            sendingEmail.IsSuccess    = false;
            sendingEmail.Notification = e.Message;
        }

        await _emailHistoryRepository.InsertAsync(sendingEmail);
    }
}