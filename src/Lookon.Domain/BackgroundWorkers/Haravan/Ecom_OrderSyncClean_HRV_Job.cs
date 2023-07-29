using System.Threading.Tasks;
using Hangfire;
using LookOn.Core.Helpers;
using LookOn.Integrations.Haravan;
using LookOn.MerchantSubscriptions;
using LookOn.MerchantSyncInfos;
using Volo.Abp.BackgroundWorkers.Hangfire;

namespace LookOn.BackgroundWorkers.Haravan;

public class Ecom_OrderSyncClean_HRV_Job : HangfireBackgroundWorkerBase
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
        
        RecurringJobId = nameof(Ecom_OrderSyncClean_HRV_Job);
        CronExpression = Cron.Never();
        if (EnvironmentHelper.IsProduction())
        {
            CronExpression = "*/30 * * * *"; // every 30 minutes
        }
    }

    public override async Task DoWorkAsync()
    {
        var merchants = await _merchantSubscriptionManager.GetActiveMerchants();
        foreach (var merchant in merchants)
        {
            BackgroundJob.Enqueue(() => _haravanSyncManager.SyncOrders(merchant.Id));
        }
    }
}