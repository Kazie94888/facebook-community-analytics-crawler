using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Volo.Abp.AspNetCore.Mvc.UI.Bootstrap.TagHelpers.Form;

namespace LookOn.Web.Pages.Shared;

public class FeedbackModel : LookOnPageModel
{
    [TextArea(Rows = 10)]
    public string Description { get; set; }

    [BindProperty(SupportsGet = true)]
    public decimal Score { get; set; }

    public async Task OnGetAsync(int score = 0)
    {
        this.Score = score;
    }
}