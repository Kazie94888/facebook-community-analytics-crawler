using LookOn.Users;
using LookOn.Categories;

using System;
using Volo.Abp.Application.Dtos;

namespace LookOn.Merchants
{
    public class MerchantWithNavigationPropertiesDto
    {
        public MerchantDto Merchant { get; set; }

        public AppUserDto AppUser { get; set; }
        public CategoryDto Category { get; set; }

    }
}