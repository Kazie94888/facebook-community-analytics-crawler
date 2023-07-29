using System.Threading.Tasks;
using Hangfire;
using LookOn.Core.Helpers;
using LookOn.MerchantSubscriptions;
using Volo.Abp.BackgroundWorkers.Hangfire;

namespace LookOn.BackgroundWorkers.MerchantSubscriptions;

public class Subscription_Scan_Job : HangfireBackgroundWorkerBase
{
    private readonly MerchantSubscriptionManager _manager;

    public Subscription_Scan_Job(MerchantSubscriptionManager manager)
    {
        _manager       = manager;
        RecurringJobId = nameof(Subscription_Scan_Job);
        CronExpression = Cron.Never();
        if (EnvironmentHelper.IsProduction())
        {
            CronExpression = Cron.Daily();
        }
    }

    public override async Task DoWorkAsync()
    {
        await _manager.ScanSubscriptions();
    }
}