using System.Threading.Tasks;
using Hangfire;
using LookOn.Integrations.Datalytis;
using LookOn.Merchants;
using LookOn.MerchantSubscriptions;
using LookOn.MerchantSyncInfos;

namespace LookOn.Jobs.Jobs.Datalytis;

public class Page1_Social_InsightRequest_Job : BackgroundJobBase
{
    private readonly DatalytisManager            _datalytisManager;
    private readonly MerchantSubscriptionManager _merchantSubscriptionManager;
    private readonly IMerchantRepository         _merchantRepository;
    private readonly IMerchantSyncInfoRepository _merchantSyncInfoRepository;
    public Page1_Social_InsightRequest_Job(DatalytisManager datalytisManager, MerchantSubscriptionManager merchantSubscriptionManager, IMerchantRepository merchantRepository, IMerchantSyncInfoRepository merchantSyncInfoRepository)
    {
        _datalytisManager                = datalytisManager;
        _merchantSubscriptionManager     = merchantSubscriptionManager;
        _merchantRepository              = merchantRepository;
        _merchantSyncInfoRepository = merchantSyncInfoRepository;
    }

    protected override async Task DoExecute()
    {
        var merchants = await _merchantSubscriptionManager.GetActiveMerchants();
        foreach (var merchant in merchants)
        {
            BackgroundJob.Enqueue(() => _datalytisManager.MetricSocialInsights_Request(merchant.Id));
        }
    }
}

public class Page2_Social_InsightRequest_Job : BackgroundJobBase
{
    private readonly DatalytisManager            _datalytisManager;
    private readonly MerchantSubscriptionManager _merchantSubscriptionManager;

    public Page2_Social_InsightRequest_Job(DatalytisManager datalytisManager, MerchantSubscriptionManager merchantSubscriptionManager)
    {
        _datalytisManager                 = datalytisManager;
        _merchantSubscriptionManager = merchantSubscriptionManager;
    }

    protected override async Task DoExecute()
    {
        var merchants = await _merchantSubscriptionManager.GetActiveMerchants();
        foreach (var merchant in merchants)
        {
            //BackgroundJob.Enqueue(() => _datalytisManager.Page2SocialInsights_Request(merchant.Id));
        }
    }
}
    
public class Page1_Social_InsightStatus_Job : BackgroundJobBase
{
    private readonly DatalytisManager            _datalytisManager;
    private readonly MerchantSubscriptionManager _merchantSubscriptionManager;
    private readonly IMerchantRepository         _merchantRepository;

    public Page1_Social_InsightStatus_Job(DatalytisManager datalytisManager, MerchantSubscriptionManager merchantSubscriptionManager, IMerchantRepository merchantRepository, IMerchantSyncInfoRepository merchantSyncInfoRepository)
    {
        _datalytisManager                = datalytisManager;
        _merchantSubscriptionManager     = merchantSubscriptionManager;
        _merchantRepository              = merchantRepository;
    }

    protected override async Task DoExecute()
    {
        var merchants = await _merchantSubscriptionManager.GetActiveMerchants();
        foreach (var merchant in merchants)
        {
            BackgroundJob.Enqueue(() => _datalytisManager.MetricSocialInsights_Status(merchant.Id));
        }
    }
}

public class Page2_Social_InsightStatus_Job : BackgroundJobBase
{
    private readonly DatalytisManager            _datalytisManager;
    private readonly MerchantSubscriptionManager _merchantSubscriptionManager;

    public Page2_Social_InsightStatus_Job(DatalytisManager datalytisManager, MerchantSubscriptionManager merchantSubscriptionManager)
    {
        _datalytisManager                 = datalytisManager;
        _merchantSubscriptionManager = merchantSubscriptionManager;
    }

    protected override async Task DoExecute()
    {
        var merchants = await _merchantSubscriptionManager.GetActiveMerchants();
        foreach (var merchant in merchants)
        {
            //BackgroundJob.Enqueue(() => _datalytisManager.Page2SocialInsights_Status(merchant.Id));
        }
    }
}

public class Page1_Social_InsightSync_Job : BackgroundJobBase
{
    private readonly DatalytisManager            _datalytisManager;
    private readonly MerchantSubscriptionManager _merchantSubscriptionManager;
    private readonly IMerchantRepository         _merchantRepository;

    public Page1_Social_InsightSync_Job(DatalytisManager datalytisManager, MerchantSubscriptionManager merchantSubscriptionManager, IMerchantRepository merchantRepository, IMerchantSyncInfoRepository merchantSyncInfoRepository)
    {
        _datalytisManager            = datalytisManager;
        _merchantSubscriptionManager = merchantSubscriptionManager;
        _merchantRepository          = merchantRepository;
    }

    protected override async Task DoExecute()
    {
        var merchants = await _merchantSubscriptionManager.GetActiveMerchants();
        foreach (var merchant in merchants)
        {
            BackgroundJob.Enqueue(() => _datalytisManager.MetricSocialInsights_Sync(merchant.Id));
        }
    }
}

public class Page2_Social_InsightSync_Job : BackgroundJobBase
{
    private readonly DatalytisManager            _datalytisManager;
    private readonly MerchantSubscriptionManager _merchantSubscriptionManager;

    public Page2_Social_InsightSync_Job(DatalytisManager datalytisManager, MerchantSubscriptionManager merchantSubscriptionManager)
    {
        _datalytisManager                 = datalytisManager;
        _merchantSubscriptionManager = merchantSubscriptionManager;
    }

    protected override async Task DoExecute()
    {
        var merchants = await _merchantSubscriptionManager.GetActiveMerchants();
        foreach (var merchant in merchants)
        {
            //BackgroundJob.Enqueue(() => _datalytisManager.Page2SocialInsights_Sync(merchant.Id));
        }
    }
}