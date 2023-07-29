using System.Threading.Tasks;
using LookOn.Merchants;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;

namespace LookOn.Web.Pages.Terms;

public class TermModalModel : AbpPageModel
{
    [BindProperty] public bool AgreeTerm { get; set; } = true;
    private readonly      IMerchantExtendAppService _merchantExtendAppService;

    public TermModalModel(IMerchantExtendAppService merchantExtendAppService)
    {
        _merchantExtendAppService = merchantExtendAppService;
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (AgreeTerm)
        {
            var currentMerchant = await _merchantExtendAppService.GetCurrentMerchantAsync();
            await _merchantExtendAppService.UpdateTermAsync(currentMerchant.Id, AgreeTerm);
        }

        return NoContent();
    }
}