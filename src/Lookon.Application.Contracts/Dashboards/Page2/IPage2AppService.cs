using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace LookOn.Dashboards.Page2;

public interface IPage2AppService : IApplicationService
{
    Task<Page2MetricDto> GetMetrics(GetMetricsInput input);
}