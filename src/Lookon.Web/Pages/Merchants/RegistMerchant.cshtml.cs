using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using LookOn.Merchants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;

namespace LookOn.Web.Pages.Merchants;

[Authorize]
public class RegistMerchantModel : AbpPageModel
{
    [BindProperty]
    public RegistMerchantInput RegistMerchant { get; set; }

    private readonly IMerchantExtendAppService _merchantExtendAppService;
    private readonly IMerchantsAppService _merchantsAppService;

    public RegistMerchantModel(IMerchantExtendAppService merchantExtendAppService, IMerchantsAppService merchantsAppService)
    {
        _merchantExtendAppService = merchantExtendAppService;
        _merchantsAppService = merchantsAppService;
    }

    public async Task<IActionResult> OnGetAsync()
    {
        var merchant = await _merchantExtendAppService.GetCurrentMerchantAsync();
        if (merchant != null)
        {
           return RedirectToPage("../Index");
        }

        var currentUser = CurrentUser.GetAllClaims();

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (ModelState.IsValid)
        {
            var merchantCreate = ObjectMapper.Map<RegistMerchantInput, MerchantCreateDto>(RegistMerchant);
            if (CurrentUser.Id != null)
            {
                merchantCreate.OwnerAppUserId = CurrentUser.Id.Value;
                await _merchantsAppService.CreateAsync(merchantCreate);

               return RedirectToPage("../Index");
            }
        }

        return Page();
    }
    
    public class RegistMerchantInput
    {
        [Required]
        [StringLength(MerchantConsts.NameMaxLength)]
        public string Name { get; set; }
        [Required]
        [StringLength(MerchantConsts.PhoneMaxLength)]
        public string Phone { get; set; }
        public string Address { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        public string Fax { get; set; }
    }
}