namespace LookOn.MerchantStores
{
    public static class MerchantStoreConsts
    {
        private const string DefaultSorting = "{0}Name asc";

        public static string GetDefaultSorting(bool withEntityName)
        {
            return string.Format(DefaultSorting, withEntityName ? "MerchantStore." : string.Empty);
        }

    }
}