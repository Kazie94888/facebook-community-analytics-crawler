using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using LookOn.Permissions;
using LookOn.Categories;

namespace LookOn.Categories
{
    [RemoteService(IsEnabled = false)]
    [Authorize(LookOnPermissions.Categories.Default)]
    public class CategoriesAppService : LookOnAppService, ICategoriesAppService
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoriesAppService(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public virtual async Task<PagedResultDto<CategoryDto>> GetListAsync(GetCategoriesInput input)
        {
            var totalCount = await _categoryRepository.GetCountAsync(input.FilterText, input.Name, input.Code, input.Description, input.OrderMin, input.OrderMax);
            var items = await _categoryRepository.GetListAsync(input.FilterText, input.Name, input.Code, input.Description, input.OrderMin, input.OrderMax, input.Sorting, input.MaxResultCount, input.SkipCount);

            return new PagedResultDto<CategoryDto>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<Category>, List<CategoryDto>>(items)
            };
        }

        public virtual async Task<CategoryDto> GetAsync(Guid id)
        {
            return ObjectMapper.Map<Category, CategoryDto>(await _categoryRepository.GetAsync(id));
        }

        [Authorize(LookOnPermissions.Categories.Delete)]
        public virtual async Task DeleteAsync(Guid id)
        {
            await _categoryRepository.DeleteAsync(id);
        }

        [Authorize(LookOnPermissions.Categories.Create)]
        public virtual async Task<CategoryDto> CreateAsync(CategoryCreateDto input)
        {

            var category = ObjectMapper.Map<CategoryCreateDto, Category>(input);

            category = await _categoryRepository.InsertAsync(category, autoSave: true);
            return ObjectMapper.Map<Category, CategoryDto>(category);
        }

        [Authorize(LookOnPermissions.Categories.Edit)]
        public virtual async Task<CategoryDto> UpdateAsync(Guid id, CategoryUpdateDto input)
        {

            var category = await _categoryRepository.GetAsync(id);
            ObjectMapper.Map(input, category);
            category = await _categoryRepository.UpdateAsync(category, autoSave: true);
            return ObjectMapper.Map<Category, CategoryDto>(category);
        }
    }
}