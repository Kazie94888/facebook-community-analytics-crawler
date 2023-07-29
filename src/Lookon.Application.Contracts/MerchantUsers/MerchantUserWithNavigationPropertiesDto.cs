using LookOn.Users;
using LookOn.Merchants;

using System;
using Volo.Abp.Application.Dtos;

namespace LookOn.MerchantUsers
{
    public class MerchantUserWithNavigationPropertiesDto
    {
        public MerchantUserDto MerchantUser { get; set; }

        public AppUserDto AppUser { get; set; }
        public MerchantDto Merchant { get; set; }

    }
}