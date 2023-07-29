using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Org.BouncyCastle.Math.EC;
using Volo.Abp.Account.Public.Web.Pages.Account;
using Volo.Abp.AspNetCore.Mvc.ApplicationConfigurations;

namespace LookOn.Pages.Account;

public class CustomResetPasswordConfirmationModel : ResetPasswordConfirmationModel
{
    private IConfiguration Configuration { get; }
    public  string         InsightUrl    { get; set; }

    public CustomResetPasswordConfirmationModel(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public override Task<IActionResult> OnGetAsync()
    {
        InsightUrl = Configuration["App:InsightsUrl"];
        return Task.FromResult<IActionResult>(Page());
    }
}