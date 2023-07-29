using LookOn.Merchants;

using System;
using Volo.Abp.Application.Dtos;

namespace LookOn.MerchantSubscriptions
{
    public class MerchantSubscriptionWithNavigationPropertiesDto
    {
        public MerchantSubscriptionDto MerchantSubscription { get; set; }

        public MerchantDto Merchant { get; set; }

    }
}