using LookOn.Shared;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Volo.Abp.Application.Dtos;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using LookOn.MerchantSubscriptions;

namespace LookOn.Web.Pages.MerchantSubscriptions
{
    public class CreateModalModel : LookOnPageModel
    {
        [BindProperty]
        public MerchantSubscriptionCreateDto MerchantSubscription { get; set; }

        private readonly IMerchantSubscriptionsAppService _merchantSubscriptionsAppService;

        public CreateModalModel(IMerchantSubscriptionsAppService merchantSubscriptionsAppService)
        {
            _merchantSubscriptionsAppService = merchantSubscriptionsAppService;
        }

        public async Task OnGetAsync()
        {
            MerchantSubscription = new MerchantSubscriptionCreateDto();

            await Task.CompletedTask;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            await _merchantSubscriptionsAppService.CreateAsync(MerchantSubscription);
            return NoContent();
        }
    }
}