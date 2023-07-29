using System;
using System.Threading.Tasks;
using Hangfire;
using LookOn.Integrations.Haravan;
using LookOn.Merchants;
using LookOn.MerchantSubscriptions;
using LookOn.MerchantSyncInfos;
using Volo.Abp.Domain.Repositories;

namespace LookOn.Jobs.Jobs.Haravan;

public class Ecom_OrderSyncClean_HRV_Job : BackgroundJobBase
{
    private readonly HaravanSyncManager          _haravanSyncManager;
    private readonly MerchantSubscriptionManager _merchantSubscriptionManager;
    private readonly IMerchantSyncInfoRepository _syncInfoRepo;

    public Ecom_OrderSyncClean_HRV_Job(HaravanSyncManager     haravanSyncManager,
                                  MerchantSubscriptionManager merchantSubscriptionManager,
                                  IMerchantSyncInfoRepository syncInfoRepo)
    {
        _haravanSyncManager          = haravanSyncManager;
        _merchantSubscriptionManager = merchantSubscriptionManager;
        _syncInfoRepo           = syncInfoRepo;
    }

    protected override async Task DoExecute()
    {
        var merchants = await _merchantSubscriptionManager.GetActiveMerchants();
        foreach (var merchant in merchants)
        {
            BackgroundJob.Enqueue(() => _haravanSyncManager.SyncOrders(merchant.Id));
        }
    }
}