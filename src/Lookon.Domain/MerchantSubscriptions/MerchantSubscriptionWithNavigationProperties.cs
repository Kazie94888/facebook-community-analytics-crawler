using LookOn.Merchants;

namespace LookOn.MerchantSubscriptions
{
    public class MerchantSubscriptionWithNavigationProperties
    {
        public MerchantSubscription MerchantSubscription { get; set; }

        public Merchant Merchant { get; set; }
        
    }
}