using System.Threading.Tasks;
using LookOn.Dashboards.Page3;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;

namespace LookOn.Controllers.Dashboards.Page3;

[RemoteService]
[Area("app")]
[ControllerName("Page3")]
[Route("api/app/dashboards/page3")]
public class Page3Controller: AbpController, IPage3AppService
{
    private readonly IPage3AppService _page3AppService;

    public Page3Controller(IPage3AppService page3AppService)
    {
        _page3AppService = page3AppService;
    }

    [HttpGet]
    [Route("get-metrics")]
    public async Task<Page3MetricDto> GetMetrics(GetMetricsInput input)
    {
        return await _page3AppService.GetMetrics(input);
    }
}