using System.Collections.Generic;
using System.Threading.Tasks;
using LookOn.Dashboards.Page1;
using LookOn.Merchants;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;

namespace LookOn.Controllers.Dashboards.Page1;

[RemoteService]
[Area("app")]
[ControllerName("Page1")]
[Route("api/app/dashboards/page1")]
public class Page1Controller :  AbpController, IPage1AppService
{
    private readonly IPage1AppService _page1AppService;

    public Page1Controller(IPage1AppService page1AppService)
    {
        _page1AppService = page1AppService;
    }

    [HttpGet]
    [Route("get-metrics")]
    public async Task<Page1MetricDto> GetMetrics(GetMetricsInput input)
    {
        return await _page1AppService.GetMetrics(input);
    }

    [HttpGet]
    [Route("get-merchants")]
    public async Task<List<MerchantDto>> GetMerchants()
    {
        return await _page1AppService.GetMerchants();
    }
}