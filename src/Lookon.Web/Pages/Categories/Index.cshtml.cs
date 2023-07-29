using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;
using Volo.Abp.AspNetCore.Mvc.UI.Bootstrap.TagHelpers.Form;
using LookOn.Categories;
using LookOn.Shared;

namespace LookOn.Web.Pages.Categories
{
    public class IndexModel : AbpPageModel
    {
        public string NameFilter { get; set; }
        public string CodeFilter { get; set; }
        public string DescriptionFilter { get; set; }
        public int? OrderFilterMin { get; set; }

        public int? OrderFilterMax { get; set; }

        private readonly ICategoriesAppService _categoriesAppService;

        public IndexModel(ICategoriesAppService categoriesAppService)
        {
            _categoriesAppService = categoriesAppService;
        }

        public async Task OnGetAsync()
        {

            await Task.CompletedTask;
        }
    }
}