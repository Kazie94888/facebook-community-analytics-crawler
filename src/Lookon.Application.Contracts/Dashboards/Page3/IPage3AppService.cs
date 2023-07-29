using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace LookOn.Dashboards.Page3;

public interface IPage3AppService : IApplicationService
{
    Task<Page3MetricDto> GetMetrics(GetMetricsInput input);
}