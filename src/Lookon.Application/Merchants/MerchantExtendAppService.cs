using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LookOn.MerchantSyncInfos;
using LookOn.MerchantUsers;
using LookOn.Permissions;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;

namespace LookOn.Merchants;

[Authorize(LookOnPermissions.Merchants.Default)]
public class MerchantExtendAppService : LookOnAppService, IMerchantExtendAppService
{
    private readonly IRepository<Merchant>       _merchantRepository;
    private readonly IMerchantUserRepository     _merchantUserRepository;
    private readonly IMerchantSyncInfoRepository _merchantSyncInfoRepository;

    public MerchantExtendAppService(IRepository<Merchant>       merchantRepository,
                                    IMerchantUserRepository     merchantUserRepository,
                                    IMerchantSyncInfoRepository merchantSyncInfoRepository)
    {
        _merchantRepository         = merchantRepository;
        _merchantUserRepository     = merchantUserRepository;
        _merchantSyncInfoRepository = merchantSyncInfoRepository;
    }

    public async Task<MerchantDto> GetCurrentMerchantAsync()
    {
        var merchantUser = await _merchantUserRepository.FirstOrDefaultAsync(x => x.AppUserId == CurrentUser.Id);
        if (merchantUser is null) throw new UserFriendlyException("User is not assign to any merchant");

        var merchant = await _merchantRepository.GetAsync(x => x.Id == merchantUser.MerchantId);
        return merchant is not null ? ObjectMapper.Map<Merchant, MerchantDto>(merchant) : null;
    }

    public async Task UpdateTermAsync(Guid merchantId, bool agree)
    {
        if (agree)
        {
            var merchant = await _merchantRepository.GetAsync(x => x.Id == merchantId);
            merchant.IsTermAccepted = true;
            merchant.TermAcceptedAt = DateTime.UtcNow;
            await _merchantRepository.UpdateAsync(merchant);
        }
    }
}