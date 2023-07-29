using System.Threading.Tasks;
using Hangfire;
using LookOn.Core.Helpers;
using LookOn.Integrations.Datalytis;
using LookOn.Merchants;
using LookOn.MerchantSubscriptions;
using LookOn.MerchantSyncInfos;
using Volo.Abp.BackgroundWorkers.Hangfire;

namespace LookOn.BackgroundWorkers.Datalytis;

public class Metric_Social_InsightRequest_Job : HangfireBackgroundWorkerBase
{
    private readonly DatalytisManager            _datalytisManager;
    private readonly MerchantSubscriptionManager _merchantSubscriptionManager;

    public Metric_Social_InsightRequest_Job(DatalytisManager datalytisManager, MerchantSubscriptionManager merchantSubscriptionManager)
    {
        _datalytisManager                 = datalytisManager;
        _merchantSubscriptionManager = merchantSubscriptionManager;
          
        RecurringJobId = nameof(Metric_Social_InsightRequest_Job);
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
            BackgroundJob.Enqueue(() => _datalytisManager.MetricSocialInsights_Request(merchant.Id));
        }
    }
}
    
public class Metric_Social_InsightStatus_Job : HangfireBackgroundWorkerBase
{
    private readonly DatalytisManager            _datalytisManager;
    private readonly MerchantSubscriptionManager _merchantSubscriptionManager;

    public Metric_Social_InsightStatus_Job(DatalytisManager datalytisManager, MerchantSubscriptionManager merchantSubscriptionManager)
    {
        _datalytisManager                 = datalytisManager;
        _merchantSubscriptionManager = merchantSubscriptionManager;
        
        RecurringJobId = nameof(Metric_Social_InsightStatus_Job);
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
            BackgroundJob.Enqueue(() => _datalytisManager.MetricSocialInsights_Status(merchant.Id));
        }
    }
}


public class Metric_Social_InsightSync_Job : HangfireBackgroundWorkerBase
{
    private readonly DatalytisManager            _datalytisManager;
    private readonly MerchantSubscriptionManager _merchantSubscriptionManager;

    public Metric_Social_InsightSync_Job(DatalytisManager datalytisManager, MerchantSubscriptionManager merchantSubscriptionManager)
    {
        _datalytisManager            = datalytisManager;
        _merchantSubscriptionManager = merchantSubscriptionManager;
        
        RecurringJobId = nameof(Metric_Social_InsightSync_Job);
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
            BackgroundJob.Enqueue(() => _datalytisManager.MetricSocialInsights_Sync(merchant.Id));
        }
    }
}