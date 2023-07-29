using LookOn.Merchants;
using LookOn.Platforms;

namespace LookOn.MerchantStores
{
    public class MerchantStoreWithNavigationProperties
    {
        public MerchantStore MerchantStore { get; set; }

        public Merchant Merchant { get; set; }
        public Platform Platform { get; set; }
        
    }
}