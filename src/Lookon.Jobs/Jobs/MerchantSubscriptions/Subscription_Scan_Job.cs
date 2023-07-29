using System.Threading.Tasks;
using LookOn.MerchantSubscriptions;

namespace LookOn.Jobs.Jobs.MerchantSubscriptions;

public class Subscription_Scan_Job : BackgroundJobBase
{
    private readonly MerchantSubscriptionManager _manager;

    public Subscription_Scan_Job(MerchantSubscriptionManager manager)
    {
        _manager = manager;
    }

    protected override async Task DoExecute()
    {
        await _manager.ScanSubscriptions();
    }
}