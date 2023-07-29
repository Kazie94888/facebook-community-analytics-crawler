using System;
using System.Linq;
using System.Threading.Tasks;
using LookOn.Merchants;
using LookOn.MerchantStores;
using LookOn.MerchantSubscriptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LookOn.Web.Pages;

public class MerchantSubscriptionDetailModel : PageModel
{
    [BindProperty(SupportsGet = true)] public DateTime?                        StartDate            { get; set; }
    [BindProperty(SupportsGet = true)] public DateTime?                        EndDate              { get; set; }
    public                                    MerchantDto                      Merchant             { get; set; }
    public                                    MerchantSubscriptionDto          MerchantSubscription { get; set; }
    public                                    MerchantStoreDto                 MerchantStore        { get; set; }
    private readonly                          IMerchantExtendAppService        _merchantExtendAppService;
    private readonly                          IMerchantSubscriptionsAppService _merchantSubscriptionsAppService;
    private readonly                          IMerchantStoresExtendAppService  _merchantStoresExtendAppService;

    public MerchantSubscriptionDetailModel(IMerchantExtendAppService        merchantExtendAppService,
                                           IMerchantStoresExtendAppService  merchantStoresExtendAppService,
                                           IMerchantSubscriptionsAppService merchantSubscriptionsAppService)
    {
        _merchantExtendAppService        = merchantExtendAppService;
        _merchantStoresExtendAppService  = merchantStoresExtendAppService;
        _merchantSubscriptionsAppService = merchantSubscriptionsAppService;
    }

    public async Task OnGetAsync()
    {
        StartDate ??= DateTime.UtcNow.AddMonths(-1).ClearTime();

        EndDate ??= DateTime.UtcNow.ClearTime();

        Merchant = await _merchantExtendAppService.GetCurrentMerchantAsync();
        if (Merchant is not null)
        {
            MerchantSubscription = await _merchantSubscriptionsAppService.GetActiveSubscription(Merchant.Id);
        }

        if (Merchant != null) MerchantStore = await _merchantStoresExtendAppService.GetByMerchantAsync(Merchant.Id);
    }
}