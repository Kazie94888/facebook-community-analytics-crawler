namespace LookOn.MerchantSyncInfos
{
    public static class MerchantSyncInfoConsts
    {
        private const string DefaultSorting = "{0}MerchantEmail asc";

        public static string GetDefaultSorting(bool withEntityName)
        {
            return string.Format(DefaultSorting, withEntityName ? "MerchantSyncInfo." : string.Empty);
        }

    }
}