using System.Threading.Tasks;
using LookOn.Insights;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;

namespace LookOn.Controllers.Insights;

[RemoteService]
[Area("app")]
[ControllerName("Insight")]
[Route("api/app/insights")]
public class MetricController : AbpController, IMetricAppService
{
    private IMetricAppService MetricAppService { get; set; }
    public MetricController(IMetricAppService metricAppService)
    {
        MetricAppService = metricAppService;
    }
    
    [HttpGet]
    [Route("get-insights")]
    public Task<MetricDto> GetInsight(GetMetricInput input)
    {
        return MetricAppService.GetInsight(input);
    }

    [HttpGet]
    [Route("get-insight-user-count")]
    public Task<InsightUserDto> GetInsightUserCount(GetInsightUserInput input)
    {
        return MetricAppService.GetInsightUserCount(input);
    }
}