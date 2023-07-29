using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.Account;
using Volo.Abp.Account.Public.Web.Pages.Account;
using Volo.Abp.Identity;
using Volo.Abp.Validation;

namespace LookOn.Pages.Account;

//TODO: Implement live password complexity check on the razor view!
public class CustomResetPasswordModel : ResetPasswordModel
{
    public override Task<IActionResult> OnGetAsync()
    {
        return Task.FromResult<IActionResult>(Page());
    }

    public override async Task<IActionResult> OnPostAsync()
    {
        try
        {
            ValidateModel();

            await AccountAppService.ResetPasswordAsync(
                                                       new ResetPasswordDto
                                                       {
                                                           UserId     = UserId,
                                                           ResetToken = ResetToken,
                                                           Password   = Password
                                                       }
                                                      );
        }
        catch (AbpIdentityResultException e)
        {
            if (!string.IsNullOrWhiteSpace(e.Message))
            {
                Alerts.Warning(GetLocalizeExceptionMessage(e));
                return Page();
            }

            throw;
        }
        catch (AbpValidationException e)
        {
            return Page();
        }

        //TODO: Try to automatically login!
        return RedirectToPage("./ResetPasswordConfirmation", new {
            returnUrl     = ReturnUrl,
            returnUrlHash = ReturnUrlHash
        });
    }

    protected override void ValidateModel()
    {
        if (!Equals(Password, ConfirmPassword))
        {
            ModelState.AddModelError("ConfirmPassword",
                                     L["'{0}' and '{1}' do not match.", "ConfirmPassword", "Password"]);
        }

        base.ValidateModel();
    }
}