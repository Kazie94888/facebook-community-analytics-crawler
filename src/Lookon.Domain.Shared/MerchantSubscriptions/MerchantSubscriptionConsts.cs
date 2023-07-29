namespace LookOn.MerchantSubscriptions
{
    public static class MerchantSubscriptionConsts
    {
        private const string DefaultSorting = "{0}StartDateTime asc";

        public static string GetDefaultSorting(bool withEntityName)
        {
            return string.Format(DefaultSorting, withEntityName ? "MerchantSubscription." : string.Empty);
        }

    }
}