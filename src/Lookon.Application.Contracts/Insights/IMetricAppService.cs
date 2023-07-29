using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace LookOn.Insights;

public interface IMetricAppService : IApplicationService
{
    public Task<MetricDto> GetInsight(GetMetricInput               input);
    Task<InsightUserDto>   GetInsightUserCount(GetInsightUserInput input);
}