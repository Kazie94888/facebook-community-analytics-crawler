using System.Collections.Generic;
using System.Threading.Tasks;
using LookOn.Merchants;
using Volo.Abp.Application.Services;

namespace LookOn.Dashboards.Page1;

public interface IPage1AppService : IApplicationService
{
    Task<Page1MetricDto> GetMetrics(GetMetricsInput input);
    Task<List<MerchantDto>> GetMerchants();
}