using System.Threading.Tasks;
using LookOn.Dashboards.Page2;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;

namespace LookOn.Controllers.Dashboards.Page2;

[RemoteService]
[Area("app")]
[ControllerName("Page2")]
[Route("api/app/dashboards/page2")]
public class Page2Controller : AbpController, IPage2AppService
{
    private readonly IPage2AppService _page2AppService;

    public Page2Controller(IPage2AppService page2AppService)
    {
        _page2AppService = page2AppService;
    }

    [HttpGet]
    [Route("get-metrics")]
    public async Task<Page2MetricDto> GetMetrics(GetMetricsInput input)
    {
        return await _page2AppService.GetMetrics(input);
    }
}