using System;
using System.Diagnostics;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using System.Web;
using LookOn.Core.Extensions;
using LookOn.Localization;
using Microsoft.Extensions.Localization;
using Volo.Abp.Account.Emailing;
using Volo.Abp.Account.Emailing.Templates;
using Volo.Abp.Account.Localization;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Emailing;
using Volo.Abp.Identity;
using Volo.Abp.MultiTenancy;
using Volo.Abp.TextTemplating;
using Volo.Abp.UI.Navigation.Urls;

namespace LookOn.Services.Account.Emailing;

public class CustomAccountEmailer : ICustomAccountEmailer, ITransientDependency
{
    protected        ITemplateRenderer                 TemplateRenderer { get; }
    protected        IEmailSender                      EmailSender      { get; }
    protected        IStringLocalizer<AccountResource> StringLocalizer  { get; }
    private readonly IStringLocalizer<LookOnResource>  _localizer;
    protected        IAppUrlProvider                   AppUrlProvider { get; }
    protected        ICurrentTenant                    CurrentTenant  { get; }

    public CustomAccountEmailer(
        IEmailSender                      emailSender,
        ITemplateRenderer                 templateRenderer,
        IStringLocalizer<AccountResource> stringLocalizer,
        IAppUrlProvider                   appUrlProvider,
        ICurrentTenant                    currentTenant,
        IStringLocalizer<LookOnResource>  localizer)
    {
        EmailSender      = emailSender;
        StringLocalizer  = stringLocalizer;
        AppUrlProvider   = appUrlProvider;
        CurrentTenant    = currentTenant;
        _localizer  = localizer;
        TemplateRenderer = templateRenderer;
    }

    public async Task SendPasswordResetLinkAsync(
        IdentityUser user,
        string       resetToken,
        string       appName,
        string       returnUrl     = null,
        string       returnUrlHash = null)
    {
        Debug.Assert(CurrentTenant.Id == user.TenantId, "This method can only work for current tenant!");

        var url = await AppUrlProvider.GetResetPasswordUrlAsync(appName);

        //TODO: Use AbpAspNetCoreMultiTenancyOptions to get the key
        var link = $"{url}?userId={user.Id}&{TenantResolverConsts.DefaultTenantKey}={user.TenantId}&resetToken={UrlEncoder.Default.Encode(resetToken)}";

        if (!returnUrl.IsNullOrEmpty())
        {
            link += "&returnUrl=" + NormalizeReturnUrl(returnUrl);
        }

        if (!returnUrlHash.IsNullOrEmpty())
        {
            link += "&returnUrlHash=" + returnUrlHash;
        }

        var emailLayout = "<div><div class=\"container\" style=\"display: flex\">"
                        + "<div class=\"image\"><img src=\"https://insights.lookon.vn/images/subscription-img.png\" width=\"auto\" height=\"150px\"></div>"
                        + "<div class=\"content\" style=\"padding: 0 20px 20px\">"
                        + "<p style=\"padding-top: 0px\">Hi {0},</p>{1}<div><a href = {2} >{3}</a ></div ></div></div>"
                        + "<div class=\"thanks\" >"
                        + "<p style=\"margin-bottom: 0px;\">{4},</p >"
                        + "<p style=\"margin-top: 0px;\">LookOn Team</p></div></div>";
        
        var emailContent = emailLayout.FormatWith(user.UserName,
                                                  _localizer["ResetPasswordEmailBody"].Value,
                                                  link,
                                                  _localizer["ResetPasswordEmailLink"].Value,
                                                  _localizer["Thanks"].Value);
        
        // var emailContent = await TemplateRenderer.RenderAsync(
        //     AccountEmailTemplates.PasswordResetLink,
        //     new { link = link }
        // );

        await EmailSender.SendAsync(
            user.Email,
            _localizer["ResetPasswordEmailTitle"].Value,
            emailContent
        );
    }
    

    private string NormalizeReturnUrl(string returnUrl)
    {
        if (returnUrl.IsNullOrEmpty())
        {
            return returnUrl;
        }

        //Handling openid connect login
        if (returnUrl.StartsWith("/connect/authorize/callback", StringComparison.OrdinalIgnoreCase))
        {
            if (returnUrl.Contains("?"))
            {
                var queryPart = returnUrl.Split('?')[1];
                var queryParameters = queryPart.Split('&');
                foreach (var queryParameter in queryParameters)
                {
                    if (queryParameter.Contains("="))
                    {
                        var queryParam = queryParameter.Split('=');
                        if (queryParam[0] == "redirect_uri")
                        {
                            return HttpUtility.UrlDecode(queryParam[1]);
                        }
                    }
                }
            }
        }

        return returnUrl;
    }
}