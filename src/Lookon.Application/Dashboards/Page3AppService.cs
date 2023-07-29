using System.Threading.Tasks;
using LookOn.Dashboards.Page3;
using LookOn.Permissions;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp;

namespace LookOn.Dashboards;

[RemoteService(IsEnabled = false)]
[Authorize(LookOnPermissions.Page3.Default)]
public class Page3AppService: LookOnAppService, IPage3AppService
{
    private readonly Page3Manager _page3Manager;

    public Page3AppService(Page3Manager page3Manager)
    {
        _page3Manager = page3Manager;
    }

    public async Task<Page3MetricDto> GetMetrics(GetMetricsInput input)
    {
        var page3DataRequest = new Page3DataRequest { MerchantId = input.MerchantId, SocialCommunityIds = input.SocialCommunityIds };
        if (input.Filter is not null)
        {
            page3DataRequest.Filter = new Page3Filter
            {
                Cities             = input.Filter.Cities,
                CarOwner           = input.Filter.CarOwner,
                GenderTypes        = input.Filter.GenderTypes,
                RelationshipStatus = input.Filter.RelationshipStatus,
                AgeSegmentEnums    = input.Filter.AgeSegmentEnums
            };
        }
        var metric = await _page3Manager.GetMetrics(page3DataRequest);
        
        return ObjectMapper.Map<Page3Metric, Page3MetricDto>(metric);
    }
}