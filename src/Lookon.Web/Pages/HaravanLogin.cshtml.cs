using System.Threading.Tasks;
using LookOn.Core.Extensions;
using LookOn.Core.Shared.Enums;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;

namespace LookOn.Web.Pages;

public class HaravanLoginModel : AbpPageModel
{
    public async Task OnGetAsync()
    {
        var properties = new AuthenticationProperties();
        properties.RedirectUri = $"{Request.Scheme}://{Request.Host}/transaction-insights/timeframe={TimeFrameType.Weekly.GetName()}";
        properties.SetParameter("Haravan", true); 
        await  HttpContext.ChallengeAsync("oidc" ,properties);
    }
}