using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Volo.Abp.Application.Dtos;
using LookOn.Categories;

namespace LookOn.Web.Pages.Categories
{
    public class EditModalModel : LookOnPageModel
    {
        [HiddenInput]
        [BindProperty(SupportsGet = true)]
        public Guid Id { get; set; }

        [BindProperty]
        public CategoryUpdateDto Category { get; set; }

        private readonly ICategoriesAppService _categoriesAppService;

        public EditModalModel(ICategoriesAppService categoriesAppService)
        {
            _categoriesAppService = categoriesAppService;
        }

        public async Task OnGetAsync()
        {
            var category = await _categoriesAppService.GetAsync(Id);
            Category = ObjectMapper.Map<CategoryDto, CategoryUpdateDto>(category);

        }

        public async Task<IActionResult> OnPostAsync()
        {
            await _categoriesAppService.UpdateAsync(Id, Category);
            return NoContent();
        }
    }
}