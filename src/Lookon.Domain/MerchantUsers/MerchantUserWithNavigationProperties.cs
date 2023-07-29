using LookOn.Users;
using LookOn.Merchants;

namespace LookOn.MerchantUsers
{
    public class MerchantUserWithNavigationProperties
    {
        public MerchantUser MerchantUser { get; set; }

        public AppUser AppUser { get; set; }
        public Merchant Merchant { get; set; }
        
    }
}