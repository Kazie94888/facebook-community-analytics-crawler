using System;
using System.Threading.Tasks;
using LookOn.Core.Shared.Enums;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc;

namespace LookOn.Web.Pages.TransationInsights.Components;

public abstract class EcommerceBaseComponent : AbpViewComponent
{
    
    public virtual Task<IViewComponentResult> InvokeAsync(Guid merchantId, TimeFrameType timeFrameType)
    {
        return Task.FromResult<IViewComponentResult>(View());
    }
}