using LookOn.Merchants;
using LookOn.Platforms;

using System;
using Volo.Abp.Application.Dtos;

namespace LookOn.MerchantStores
{
    public class MerchantStoreWithNavigationPropertiesDto
    {
        public MerchantStoreDto MerchantStore { get; set; }

        public MerchantDto Merchant { get; set; }
        public PlatformDto Platform { get; set; }

    }
}