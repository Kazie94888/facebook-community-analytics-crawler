using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LookOn.Core.Extensions;
using LookOn.Merchants;
using LookOn.MerchantSyncInfos;
using LookOn.MerchantUsers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;

namespace LookOn.Web.Pages.Merchants;

[Authorize]
public class ManageModel : AbpPageModel
{
    private readonly IMerchantsAppService _merchantsAppService;

    private readonly IMerchantExtendAppService _merchantExtendAppService;

    private readonly IMerchantUsersAppService     _merchantUsersAppService;
    private readonly IMerchantSyncInfosAppService _merchantSyncInfosAppService;
    
    public ManageModel(IMerchantsAppService merchantsAppService, IMerchantExtendAppService merchantExtendAppService, IMerchantUsersAppService merchantUsersAppService, IMerchantSyncInfosAppService merchantSyncInfosAppService)
    {
        _merchantsAppService              = merchantsAppService;
        _merchantExtendAppService         = merchantExtendAppService;
        _merchantUsersAppService          = merchantUsersAppService;
        _merchantSyncInfosAppService = merchantSyncInfosAppService;
    }

    [BindProperty] public MerchantDto               Merchant { get; set; }
    // [BindProperty] public MerchantSocialSyncInfoDto MerchantSocialSyncInfo { get; set; }
    //
    [BindProperty] public MetricConfigsDto MetricConfigs        { get; set; }
    [BindProperty] public string           PhoneNumber          { get; set; }
    [BindProperty] public string           Fax { get; set; }

    //public List<MerchantUserWithNavigationPropertiesDto> MerchantStaffs { get; set; }
    public async Task OnGetAsync()
    {
        await GetInitData();
    }

    private async Task GetInitData()
    {
        Merchant    = await _merchantExtendAppService.GetCurrentMerchantAsync();
        PhoneNumber = Merchant.Phone;
        Fax         = Merchant.Fax;
        //MerchantStaffs   = await _merchantUsersAppService.GetsByMerchantAsync(Merchant.Id);
        // var merchantSyncInfoDto = await _merchantSyncInfosAppService.GetByMerchantIdAsync(Merchant.Id);
        // MerchantSocialSyncInfo = merchantSyncInfoDto.SocialSyncInfo;
        // if (MerchantSocialSyncInfo != null && MerchantSocialSyncInfo.Communities.IsNullOrEmpty())
        // {
        //     MerchantSocialSyncInfo.Communities.Add(new MerchantSocialUserSyncInfo());
        // }
        MetricConfigs = Merchant.MetricConfigs;
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (ModelState.IsValid)
        {
            var phoneNumber = PhoneNumber.RemoveNonNumeric().ToInternationalPhoneNumber();
            if (PhoneNumber.IsNotNullOrEmpty() && !phoneNumber.ValidatePhoneNumber())
            {
                throw new UserFriendlyException(L["PhoneNotValid"]);
            }

            if (Fax.IsNotNullOrEmpty() && !Fax.ValidateFaxNumber())
            {
                throw new UserFriendlyException(L["FaxNotValid"]);
            }
            
            Merchant.Phone = phoneNumber;
            Merchant.Fax   = Fax;
            var merchantUpdate = ObjectMapper.Map<MerchantDto, MerchantUpdateDto>(Merchant);
            await _merchantsAppService.UpdateAsync(Merchant.Id, merchantUpdate);
            Alerts.Success(L["Successfully"]);
        }
        await GetInitData();
        return Content("");
    }

    // public async Task<IActionResult> OnPostAddUserAsync()
    // {
    //     if (AddUserNameOrEmailInput.IsNotNullOrEmpty())
    //     {
    //         try
    //         {
    //             await _merchantUsersAppService.AddUserAsync(Merchant.Id, AddUserNameOrEmailInput);
    //         }
    //         catch (Exception e)
    //         {
    //             Alerts.Danger(e.Message);
    //         }
    //     }
    //     await GetInitData();
    //
    //     return RedirectToPage("~/Pages/Merchants/Manage");
    // }
}