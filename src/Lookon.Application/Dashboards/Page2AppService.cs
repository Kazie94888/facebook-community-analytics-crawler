using System.Threading.Tasks;
using LookOn.Dashboards.Page2;
using LookOn.Permissions;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp;

namespace LookOn.Dashboards;

[RemoteService(IsEnabled = false)]
[Authorize(LookOnPermissions.Page2.Default)]
public class Page2AppService : LookOnAppService, IPage2AppService
{
    private readonly Page2Manager _page2Manager;
    public Page2AppService(Page2Manager page2Manager)
    {
        _page2Manager = page2Manager;
    }

    public async Task<Page2MetricDto> GetMetrics(GetMetricsInput input)
    {
        var metric = await _page2Manager.GetMetrics(new Page2DataRequest
        {
            MerchantId = input.MerchantId, SocialCommunityIds = input.SocialCommunityIds
        });
        
        return ObjectMapper.Map<Page2Metric, Page2MetricDto>(metric);
    }
}