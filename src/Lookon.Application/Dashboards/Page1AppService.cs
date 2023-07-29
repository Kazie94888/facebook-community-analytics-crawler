using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LookOn.Core.Shared.Enums;
using LookOn.Dashboards.Page1;
using LookOn.Merchants;
using LookOn.Permissions;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp;

namespace LookOn.Dashboards;

[RemoteService(IsEnabled = false)]
[Authorize(LookOnPermissions.Page1.Default)]
public class Page1AppService: LookOnAppService, IPage1AppService
{
    private readonly Page1Manager _page1Manager;

    public Page1AppService(Page1Manager page1Manager)
    {
        _page1Manager = page1Manager;
    }

    public async Task<Page1MetricDto> GetMetrics(GetMetricsInput input)
    {
        var metric = await _page1Manager.GetMetrics(new Page1DataRequest
        {
           MerchantId         = input.MerchantId,
           TimeFrame          = input.TimeFrameType,
           From               = input.From,
           To                 = input.To  ,
           SocialCommunityIds = input.SocialCommunityIds,
        });

        return ObjectMapper.Map<Page1Metric, Page1MetricDto>(metric);
    }

    public async Task<List<MerchantDto>> GetMerchants()
    {
        return ObjectMapper.Map<List<Merchant>, List<MerchantDto>>(await _page1Manager.GetMerchants());
    }
}