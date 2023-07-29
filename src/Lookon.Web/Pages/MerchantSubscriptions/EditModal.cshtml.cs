using LookOn.Shared;
using LookOn.Merchants;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Volo.Abp.Application.Dtos;
using LookOn.MerchantSubscriptions;

namespace LookOn.Web.Pages.MerchantSubscriptions
{
    public class EditModalModel : LookOnPageModel
    {
        [HiddenInput]
        [BindProperty(SupportsGet = true)]
        public Guid Id { get; set; }

        [BindProperty]
        public MerchantSubscriptionUpdateDto MerchantSubscription { get; set; }

        public MerchantDto Merchant { get; set; }

        private readonly IMerchantSubscriptionsAppService _merchantSubscriptionsAppService;

        public EditModalModel(IMerchantSubscriptionsAppService merchantSubscriptionsAppService)
        {
            _merchantSubscriptionsAppService = merchantSubscriptionsAppService;
        }

        public async Task OnGetAsync()
        {
            var merchantSubscriptionWithNavigationPropertiesDto = await _merchantSubscriptionsAppService.GetWithNavigationPropertiesAsync(Id);
            MerchantSubscription = ObjectMapper.Map<MerchantSubscriptionDto, MerchantSubscriptionUpdateDto>(merchantSubscriptionWithNavigationPropertiesDto.MerchantSubscription);

            Merchant = merchantSubscriptionWithNavigationPropertiesDto.Merchant;

        }

        public async Task<IActionResult> OnPostAsync()
        {
            await _merchantSubscriptionsAppService.UpdateAsync(Id, MerchantSubscription);
            return NoContent();
        }
    }
}