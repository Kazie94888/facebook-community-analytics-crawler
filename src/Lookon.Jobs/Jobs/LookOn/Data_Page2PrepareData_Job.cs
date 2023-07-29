using System.Threading.Tasks;
using Hangfire;
using LookOn.Dashboards;
using LookOn.MerchantSubscriptions;

namespace LookOn.Jobs.Jobs.LookOn;

public class Data_Page2PrepareData_Job : BackgroundJobBase
{
    private readonly Page2DataSourceManager _page2DataSourceManager;
    private readonly MerchantSubscriptionManager     _merchantSubscriptionManager;

    public Data_Page2PrepareData_Job(Page2DataSourceManager page2DataSourceManager, MerchantSubscriptionManager merchantSubscriptionManager)
    {
        _page2DataSourceManager           = page2DataSourceManager;
        _merchantSubscriptionManager = merchantSubscriptionManager;
    }

    protected override async Task DoExecute()
    {
        var merchants = await _merchantSubscriptionManager.GetActiveMerchants();
        foreach (var merchant in merchants)
        {
            BackgroundJob.Enqueue(() => _page2DataSourceManager.PreparePage2Data(merchant.Id));
        }
    }
}