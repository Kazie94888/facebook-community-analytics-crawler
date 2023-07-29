using System.Threading.Tasks;
using Hangfire;
using LookOn.Core.Helpers;
using LookOn.Insights.Metrics;
using LookOn.MerchantSubscriptions;
using Volo.Abp.BackgroundWorkers.Hangfire;

namespace LookOn.BackgroundWorkers.LookOn;

public class Data_MetricsPrepareData_Job : HangfireBackgroundWorkerBase
{
    private readonly MetricDataManager           _metricDataManager;
    private readonly MerchantSubscriptionManager _merchantSubscriptionManager;

    public Data_MetricsPrepareData_Job(MetricDataManager metricDataManager, MerchantSubscriptionManager merchantSubscriptionManager)
    {
        _metricDataManager           = metricDataManager;
        _merchantSubscriptionManager = merchantSubscriptionManager;
    
        RecurringJobId = nameof(Data_MetricsPrepareData_Job);
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
            BackgroundJob.Enqueue(() => _metricDataManager.PrepareMetricDataSource(merchant.Id));
        }
    }
}