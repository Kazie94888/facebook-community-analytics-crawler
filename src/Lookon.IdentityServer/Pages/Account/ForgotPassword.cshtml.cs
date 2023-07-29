using System.Threading.Tasks;
using LookOn.Services;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.Account;
using Volo.Abp.Account.Public.Web.Pages.Account;

namespace LookOn.Pages.Account;

public class CustomForgotPasswordModel : ForgotPasswordModel
{
    private readonly ICustomAccountAppService _customAccountAppService;

    public CustomForgotPasswordModel(ICustomAccountAppService customAccountAppService)
    {
        _customAccountAppService = customAccountAppService;
    }

    public override async Task<IActionResult> OnPostAsync()
    {
        try
        {
            await _customAccountAppService.SendPasswordResetCodeAsync(
                                                                      new SendPasswordResetCodeDto
                                                                      {
                                                                          Email         = Email,
                                                                          AppName       = "MVC", //TODO: Const!
                                                                          ReturnUrl     = ReturnUrl,
                                                                          ReturnUrlHash = ReturnUrlHash
                                                                      }
                                                                     );
        }
        catch (UserFriendlyException e)
        {
            Alerts.Danger(GetLocalizeExceptionMessage(e));
            return Page();
        }


        return RedirectToPage(
                              "./PasswordResetLinkSent",
                              new {
                                  returnUrl     = ReturnUrl,
                                  returnUrlHash = ReturnUrlHash
                              });
    }
}