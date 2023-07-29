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
    public class UpdateStatusModalModel : LookOnPageModel
    {
        [HiddenInput]
        [BindProperty(SupportsGet = true)]
        public Guid Id { get; set; }

        [BindProperty]
        public MerchantSubscriptionUpdateDto MerchantSubscription { get; set; }


        private readonly IMerchantSubscriptionsAppService _merchantSubscriptionsAppService;

        public UpdateStatusModalModel(IMerchantSubscriptionsAppService merchantSubscriptionsAppService)
        {
            _merchantSubscriptionsAppService = merchantSubscriptionsAppService;
        }

        public async Task OnGetAsync()
        {
            var merchantSubscriptionWithNavigationPropertiesDto = await _merchantSubscriptionsAppService.GetWithNavigationPropertiesAsync(Id);
            MerchantSubscription = ObjectMapper.Map<MerchantSubscriptionDto, MerchantSubscriptionUpdateDto>(merchantSubscriptionWithNavigationPropertiesDto.MerchantSubscription);
        }

        public async Task<IActionResult> OnPostAsync()
        {
            await _merchantSubscriptionsAppService.UpdateSubscriptionStatus(new UpdateSubscriptionStatusInput()
            {
                MerchantSubscriptionId       = Id,
                SubscriptionStatus = MerchantSubscription.SubscriptionStatus
            });
            return NoContent();
        }
    }
}