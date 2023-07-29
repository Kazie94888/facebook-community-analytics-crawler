using System.Threading.Tasks;
using LookOn.Consts;
using LookOn.Core.Extensions;
using LookOn.Enums;
using LookOn.Localization;
using LookOn.Merchants;
using LookOn.MerchantSyncInfos;
using Microsoft.Extensions.Localization;

namespace LookOn.Emails;

public class EmailTemplateManager : LookOnManager
{
    private readonly IStringLocalizer<LookOnResource> _localizer;

    public EmailTemplateManager(IStringLocalizer<LookOnResource> localizer)
    {
        _localizer = localizer;
    }

    public Task<EmailSendingArgs> GetMerchantSyncStatusEmailTemplate(string pageName, Merchant merchant, MerchantSyncStatus status)
    {
        var emailLayout = "<div> <div class=\"title\">"
                        + "<h2>{0}</h2></div>"
                        + "<div class=\"container\" style=\"display: flex\">"
                        + "<div class=\"image\"><img src=\"https://insights.lookon.vn/images/subscription-img.png\" width=\"auto\" height=\"150px\"></div>"
                        + "<div class=\"content\" style=\"padding: 0 20px 20px\">"
                        + "<p style=\"padding-top: 0px\">Hi {1},</p>{2} </div></div>"
                        + "<div class=\"thanks\" >"
                        + "<p style=\"margin-bottom: 0px;\">{3},</p >"
                        + "<p style=\"margin-top: 0px;\">LookOn Team</p></div></div>";

        var emailBody    = (_localizer[$"{pageName}.Enum:MerchantSyncStatus:{status.ToInt()}:EmailBody"]).Value;
        var emailTitle   = (_localizer[$"{pageName}.Enum:MerchantSyncStatus:{status.ToInt()}:EmailTitle"]).Value;
        var emailSubject = (_localizer[$"{pageName}.Enum:MerchantSyncStatus:{status.ToInt()}:EmailSubject", merchant.Name]).Value;
        var emailContent = emailLayout.FormatWith(emailTitle,
                                                  merchant.Name,
                                                  emailBody,
                                                  _localizer["Thanks"].Value);

        return Task.FromResult(new EmailSendingArgs { Subject = emailSubject, Body = emailContent });
    }

    public Task<EmailSendingArgs> GetSubscriptionEmailStatusEmailTemplate(Merchant merchant, SubscriptionEmailStatus status)
    {
        var emailLayout = "<div> <div class=\"title\">"
                        + "<h2>{0}</h2></div>"
                        + "<div class=\"container\" style=\"display: flex\">"
                        + "<div class=\"image\"><img src=\"https://insights.lookon.vn/images/subscription-img.png\" width=\"auto\" height=\"150px\"></div>"
                        + "<div class=\"content\" style=\"padding: 0 20px 20px\">"
                        + "<p style=\"padding-top: 0px\">Hi {1},</p>{2} </div></div>"
                        + "<div class=\"thanks\" >"
                        + "<p style=\"margin-bottom: 0px;\">{3},</p >"
                        + "<p style=\"margin-top: 0px;\">LookOn Team</p></div></div>";

        var emailBody    = (_localizer[$"Enum:SubscriptionEmailStatus:{status.ToInt()}:EmailBody"]).Value;
        var emailSubject = (_localizer[$"Enum:SubscriptionEmailStatus:{status.ToInt()}:EmailSubject", merchant.Name]).Value;
        var emailTitle   = (_localizer[$"Enum:SubscriptionEmailStatus:{status.ToInt()}:EmailTitle"]).Value;
        var emailContent = emailLayout.FormatWith(emailTitle,
                                                  merchant.Name?.Trim(),
                                                  emailBody,
                                                  _localizer["Thanks"].Value);

        return Task.FromResult(new EmailSendingArgs { Subject = emailSubject, Body = emailContent });
    }

    public Task<EmailSendingArgs> GetAssignUserEmailTemplate(Merchant merchant, string email, string password, bool isNewUser)
    {
        var emailLayout = "<div> <div class=\"title\">"
                        + "<div class=\"container\" style=\"display: flex\">"
                        + "<div class=\"image\"><img src=\"https://insights.lookon.vn/images/subscription-img.png\" width=\"auto\" height=\"150px\"></div>"
                        + "<div class=\"content\" style=\"padding: 0 20px 20px\">"
                        + "<p style=\"padding-top: 0px\">Hi,</p> {0} </div></div>"
                        + "<div class=\"thanks\" >"
                        + "<p style=\"margin-bottom: 0px;\">{1},</p >"
                        + "<p style=\"margin-top: 0px;\">LookOn Team</p></div></div>";

        var emailSubject = isNewUser ? (L["EmailSubject.NewUserAssigned", merchant.Name]).Value : (L["EmailSubject.UserAssigned", merchant.Name]).Value;
        var emailBody    = isNewUser ? (L["EmailBody.NewUserAssigned"]).Value : (L["EmailBody.UserAssigned"]).Value;
        
        emailBody = isNewUser
                        ? emailBody.FormatWith(merchant.Name,
                                               email,
                                               password,
                                               Configuration["App:Domain"])
                        : emailBody.FormatWith(merchant.Name, Configuration["App:Domain"]);

        var emailContent = emailLayout.FormatWith(emailBody, _localizer["Thanks"].Value);

        return Task.FromResult(new EmailSendingArgs { Subject = emailSubject, Body = emailContent });
    }

    public async Task<EmailSendingArgs> GetNewCommunityEmailTemplate(Merchant merchant, string url, bool invalidPage, string communityName)
    {
        var emailLayout = "<div> <div class=\"title\">"
                        + "<div class=\"container\" style=\"display: flex\">"
                        + "<div class=\"image\"><img src=\"https://insights.lookon.vn/images/subscription-img.png\" width=\"auto\" height=\"150px\"></div>"
                        + "<div class=\"content\" style=\"padding: 0 20px 20px\">"
                        + "<p style=\"padding-top: 0px\">Hi,</p> {0} </div></div>"
                        + "<div class=\"thanks\" >"
                        + "<p style=\"margin-bottom: 0px;\">{1},</p >"
                        + "<p style=\"margin-top: 0px;\">LookOn Team</p></div></div>";

        var emailSubject = invalidPage ? (L["EmailSubject.AddingNewInvalidCommunityPage", merchant.Name, communityName]).Value : (L["EmailSubject.AddingNewCommunityPage", merchant.Name, communityName]).Value;
        var emailBody    = (L["EmailBody.AddingNewCommunityPage"]).Value;

        emailBody = emailBody.FormatWith(merchant.Name, url);

        var emailContent = emailLayout.FormatWith(emailBody, _localizer["Thanks"].Value);

        return await Task.FromResult(new EmailSendingArgs { Subject = emailSubject, Body = emailContent });
    }

    public async Task<EmailSendingArgs> GetVerifyCommunityEmailTemplate(Merchant merchant, string communityName, bool isApproved)
    {
        var emailLayout = "<div> <div class=\"title\">"
                        + "<div class=\"container\" style=\"display: flex\">"
                        + "<div class=\"image\"><img src=\"https://insights.lookon.vn/images/subscription-img.png\" width=\"auto\" height=\"150px\"></div>"
                        + "<div class=\"content\" style=\"padding: 0 20px 20px\">"
                        + "<p style=\"padding-top: 0px\">Hi {0},</p> {1} </div></div>"
                        + "<div class=\"thanks\" >"
                        + "<p style=\"margin-bottom: 0px;\">{2},</p >"
                        + "<p style=\"margin-top: 0px;\">LookOn Team</p></div></div>";

        var emailSubject = isApproved ? (L["EmailSubject.ApprovedCommunityPage", merchant.Name, communityName]).Value : (L["EmailSubject.RejectCommunityPage", merchant.Name, communityName]).Value;
        var emailBody    = isApproved ? (L["EmailBody.ApprovedCommunityPage"]).Value : (L["EmailBody.RejectCommunityPage"]).Value;

        emailBody = emailBody.FormatWith(communityName);

        var emailContent = emailLayout.FormatWith(merchant.Name, emailBody, _localizer["Thanks"].Value);

        return await Task.FromResult(new EmailSendingArgs { Subject = emailSubject, Body = emailContent });
    }
}