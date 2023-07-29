using System;
using System.ComponentModel.DataAnnotations;

namespace LookOn.Categories
{
    public class CategoryCreateDto
    {
        [Required]
        public string Name { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public int Order { get; set; }
    }
}