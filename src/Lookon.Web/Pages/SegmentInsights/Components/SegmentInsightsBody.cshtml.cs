using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using LookOn.Dashboards.Page3;
using LookOn.Enums;
using LookOn.Merchants;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace LookOn.Web.Pages.SegmentInsights.Components;

public class SegmentInsightsBody : LookOnPageModel
{
    public           MerchantDto        Merchant                { get; set; }
    public           Page3MetricDto     Page3Metric             { get; set; } = new ();
    public           MerchantSyncStatus Page3MerchantSyncStatus { get; set; }
    private readonly IPage3AppService   _page3AppService;

    public SegmentInsightsBody(IPage3AppService page3AppService)
    {
        _page3AppService = page3AppService;
    }
     
    public async Task OnGetAsync(DynamicFilter filter)
    {
        Merchant      = await CurrentMerchant();
        Page3Metric   = await _page3AppService.GetMetrics(new GetMetricsInput { MerchantId = Merchant.Id, SocialCommunityIds = Merchant.GetSocialCommunityIds(), Filter = filter});
    }
}