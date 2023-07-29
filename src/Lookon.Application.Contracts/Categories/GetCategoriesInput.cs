using Volo.Abp.Application.Dtos;
using System;

namespace LookOn.Categories
{
    public class GetCategoriesInput : PagedAndSortedResultRequestDto
    {
        public string FilterText { get; set; }

        public string Name { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public int? OrderMin { get; set; }
        public int? OrderMax { get; set; }

        public GetCategoriesInput()
        {

        }
    }
}