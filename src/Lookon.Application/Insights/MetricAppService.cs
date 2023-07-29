using System.Linq;
using System.Threading.Tasks;
using LookOn.Core.Extensions;
using LookOn.Core.Shared.Enums;
using LookOn.Insights.Metrics;
using LookOn.Permissions;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp;

namespace LookOn.Insights;

[RemoteService(IsEnabled = false)]
[Authorize(LookOnPermissions.Categories.Default)]
public class MetricAppService : LookOnAppService, IMetricAppService
{
    private MetricManager MetricManager { get; set; }
    public MetricAppService(MetricManager metricManager)
    {
        MetricManager = metricManager;
    }
    
    public async Task<MetricDto> GetInsight(GetMetricInput input)
    {
        if (input.TimeFrame != TimeFrameType.AllTime)
        {
            var timeRange = input.TimeFrame.GetDateByTimeFrameType(input.EcomFromDateTime, true);
            if (!input.EcomFromDateTime.HasValue && !input.EcomToDateTime.HasValue)
            {
                input.EcomFromDateTime = timeRange.Item1;
                input.EcomToDateTime   = timeRange.Item2;
            }
        }

        var getMetricRequest = new GetMetricRequest()
        {
            MerchantId = input.MerchantId,
            MetricDataSourceType = input.MetricDataSourceType,
            TimeFrame = input.TimeFrame,
            From = input.EcomFromDateTime,
            To   = input.EcomToDateTime
        };
        
        var metric = await MetricManager.GetMetricsCache(input.MerchantId);
        if (metric == null || MetricManager.IsExpired(metric))
        {
            metric = new Metric()
            {
                MerchantId = input.MerchantId
            };
        }
        if (metric.Items.FirstOrDefault(_ => _.DataSourceType == input.MetricDataSourceType && _.TimeFrameType == input.TimeFrame) is null)
        {
            metric = await MetricManager.GetMetrics(metric, getMetricRequest);
            await MetricManager.DoSaveMetrics(getMetricRequest, metric);
        }
        return ObjectMapper.Map<Metric, MetricDto>(metric);
    }

    public async Task<InsightUserDto> GetInsightUserCount(GetInsightUserInput input)
    {
        return ObjectMapper.Map<InsightUser, InsightUserDto>(await MetricManager.GetInsightUserCount(input.MerchantId));
    }
}