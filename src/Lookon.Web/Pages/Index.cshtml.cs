using System.Threading.Tasks;
using JetBrains.Annotations;
using LookOn.Consts;
using LookOn.Core.Extensions;
using LookOn.Core.Shared.Enums;
using LookOn.Merchants;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LookOn.Web.Pages;

public class IndexModel : LookOnPageModel
{
    private readonly IMerchantsAppService _merchantsAppService;
    private readonly IMerchantExtendAppService _merchantExtendAppService;
    public IndexModel(IMerchantsAppService merchantsAppService, IMerchantExtendAppService merchantExtendAppService)
    {
        _merchantsAppService = merchantsAppService;
        _merchantExtendAppService = merchantExtendAppService;
    }

    public Task<IActionResult> OnGetAsync()
    {
        return CurrentUser.IsAuthenticated switch
        {
            false => Task.FromResult<IActionResult>(LocalRedirect($"~/Account/Login")),
            true when CurrentUser.IsInRole(RolesConsts.Merchant) =>
                Task.FromResult<IActionResult>(LocalRedirect($"~/insights")),
            _ => Task.FromResult<IActionResult>(Page())
        };
    }
    public async Task OnPostLoginAsync()
    {
        await HttpContext.ChallengeAsync("oidc");
    }
}
