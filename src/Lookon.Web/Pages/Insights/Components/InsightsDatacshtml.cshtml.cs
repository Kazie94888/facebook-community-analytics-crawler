using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LookOn.Core.Extensions;
using LookOn.Core.Helpers;
using LookOn.Core.Shared.Enums;
using LookOn.Enums;
using LookOn.Insights;

namespace LookOn.Web.Pages.Insights.Components;

public class InsightsData : LookOnPageModel
{
    public  MetricDataSourceType DataType         { get; set; }
    public  TimeFrameType        TimeFrameType    { get; set; } = TimeFrameType.AllTime;
    public  MetricDto            Metric           { get; set; }
    public  MetricItemDto        MetricItem       { get; set; }
    public  string               EcomAdvanceSize  { get; set; }
    private IMetricAppService    MetricAppService { get; set; }

    public string AverageOrderTitle;

    public InsightsData(IMetricAppService metricAppService)
    {
        MetricAppService = metricAppService;
    }

    public async Task OnGetAsync(MetricDataSourceType dataType, string timeFrameTypeString = "7days")
    {
        var merchant = await CurrentMerchant();
        DataType = dataType;
        if (dataType.ToInt().IsInsightDataType(InsightDataHelper.EcomDataNums))
        {
            var fromDate = timeFrameTypeString switch
            {
                "1day"   => DateTime.UtcNow.AddDays(-1).Date,
                "7days"  => DateTime.UtcNow.AddDays(-7).Date,
                "30days" => DateTime.UtcNow.AddDays(-30).Date,
                _        => DateTime.UtcNow.AddDays(-7).Date
            };

            TimeFrameType = timeFrameTypeString switch
            {
                "1day"   => TimeFrameType.Daily,
                "7days"  => TimeFrameType.Weekly,
                "30days" => TimeFrameType.Monthly,
                _        => TimeFrameType.Weekly
            };
            Metric = await MetricAppService.GetInsight(new GetMetricInput()
            {
                MerchantId = merchant.Id, MetricDataSourceType = DataType, TimeFrame = TimeFrameType, EcomFromDateTime = fromDate, EcomToDateTime = DateTime.UtcNow.Date
            });
        }
        else
        {
            Metric = await MetricAppService.GetInsight(new GetMetricInput()
            {
                MerchantId = merchant.Id, MetricDataSourceType = DataType, TimeFrame = TimeFrameType.AllTime
            });
        }

        MetricItem = Metric.Items.FirstOrDefault(_ => _.DataSourceType == DataType && _.TimeFrameType == TimeFrameType);
        if (MetricItem is null) return;

        ViewData["TimeTitle"]                      = timeFrameTypeString;
        ViewData["Ecom_RetentionThresholdInMonth"] = merchant.MetricConfigs.Ecom_RetentionThresholdInMonth;
        ViewData["OrderTotalKPI"]                  = merchant.MetricConfigs.OrderTotalKPI;

        EcomAdvanceSize = DataType.ToInt() switch
        {
            1 => "_3", 2 => "_4", 3 => "_12", 5 => "_3", _ => EcomAdvanceSize
        };
        
        if (merchant.MetricConfigs.OrderTotalKPI == 0)
        {
            AverageOrderTitle = L["Page1.DashboardOther.ConfigurationOrderKPI"];
        }
        else
        {
            AverageOrderTitle = MetricItem.EcomAdvanced.AverageOrderValueComparedToTheWorldPercent >= 100
                                    ? L["Page1.DashboardOther.AverageOrderValueMoreThanWorldTitle"]
                                    : L["Page1.DashboardOther.AverageOrderValueLessThanWorldTitle"];
        }
    }
}