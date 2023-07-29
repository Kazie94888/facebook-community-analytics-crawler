using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LookOn.Core.Shared.Enums;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc;

namespace LookOn.Web.Pages.TransationInsights.Components;

[ViewComponent]
public class SocialBaseComponent : AbpViewComponent
{
    public virtual async Task<IViewComponentResult> InvokeAsync(Guid merchantId, TimeFrameType timeFrameType, IList<string> communityPageIds)
    {
        return await Task.FromResult<IViewComponentResult>(View());
    }
}