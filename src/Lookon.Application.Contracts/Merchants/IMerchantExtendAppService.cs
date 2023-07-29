using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LookOn.MerchantSyncInfos;
using Volo.Abp.Application.Services;

namespace LookOn.Merchants;

public interface IMerchantExtendAppService : IApplicationService
{
    Task<MerchantDto>               GetCurrentMerchantAsync();

    Task UpdateTermAsync(Guid merchantId, bool agree);

    // Task<MerchantSyncInfoDto>       UpdateMerchantSyncInfo(MerchantSocialSyncInfoDto merchantSocialSyncInfo);
}