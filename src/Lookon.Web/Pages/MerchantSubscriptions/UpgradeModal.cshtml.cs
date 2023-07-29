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
using LookOn.RequestResponses;

namespace LookOn.Web.Pages.MerchantSubscriptions
{
    public class UpgradeModalModel : LookOnPageModel
    {
        [HiddenInput]
        [BindProperty(SupportsGet = true)]
        public Guid Id { get; set; }

        [BindProperty]
        public MerchantSubscriptionUpdateDto MerchantSubscription { get; set; }

        public MerchantDto Merchant { get; set; }

        private readonly IMerchantSubscriptionsAppService _merchantSubscriptionsAppService;

        public UpgradeModalModel(IMerchantSubscriptionsAppService merchantSubscriptionsAppService)
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
            await _merchantSubscriptionsAppService.SetSubscription(new SetSubscriptionInput
            {
                MerchantId       = MerchantSubscription.MerchantId,
                SubscriptionType = MerchantSubscription.SubscriptionType,
                From             = MerchantSubscription.StartDateTime ?? DateTime.Today,
            });
            return NoContent();
        }
    }
}