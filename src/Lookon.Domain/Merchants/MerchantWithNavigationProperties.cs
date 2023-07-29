using LookOn.Users;
using LookOn.Categories;

namespace LookOn.Merchants
{
    public class MerchantWithNavigationProperties
    {
        public Merchant Merchant { get; set; }

        public AppUser AppUser { get; set; }
        public Category Category { get; set; }
        
    }
}