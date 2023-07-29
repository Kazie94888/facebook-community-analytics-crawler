using System.Threading.Tasks;
using Hangfire;
using LookOn.Dashboards;
using LookOn.MerchantSubscriptions;

namespace LookOn.Jobs.Jobs.LookOn;

public class Data_Page1PrepareData_Job : BackgroundJobBase
{
    private readonly Page1DataSourceManager  _page1DataSourceManager;
    private readonly MerchantSubscriptionManager _merchantSubscriptionManager;

    public Data_Page1PrepareData_Job(Page1DataSourceManager page1DataSourceManager, MerchantSubscriptionManager merchantSubscriptionManager)
    {
        _page1DataSourceManager           = page1DataSourceManager;
        _merchantSubscriptionManager = merchantSubscriptionManager;
    }

    protected override async Task DoExecute()
    {
        var merchants = await _merchantSubscriptionManager.GetActiveMerchants();
        foreach (var merchant in merchants)
        {
            BackgroundJob.Enqueue(() => _page1DataSourceManager.PreparePage1Data(merchant.Id));
        }
    }
}