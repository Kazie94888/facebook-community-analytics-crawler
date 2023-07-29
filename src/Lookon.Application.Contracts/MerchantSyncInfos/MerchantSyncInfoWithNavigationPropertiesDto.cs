using LookOn.Merchants;

using System;
using Volo.Abp.Application.Dtos;

namespace LookOn.MerchantSyncInfos
{
    public class MerchantSyncInfoWithNavigationPropertiesDto
    {
        public MerchantSyncInfoDto MerchantSyncInfo { get; set; }

        public MerchantDto Merchant { get; set; }

    }
}