using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Castle.Core.Logging;
using LookOn.Core.Extensions;
using LookOn.Core.Helpers;
using LookOn.Enums;
using LookOn.Exceptions;
using LookOn.Merchants;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;

namespace LookOn.Web.Pages.MerchantConnects
{
    public class MaxSocialPageModalModel : LookOnPageModel
    {
        public MaxSocialPageModalModel()
        {
        }

        public async Task OnGetAsync()
        {
            await Task.CompletedTask;
        }
    }
}