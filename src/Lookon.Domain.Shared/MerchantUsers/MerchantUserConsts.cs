namespace LookOn.MerchantUsers
{
    public static class MerchantUserConsts
    {
        private const string DefaultSorting = "{0}IsActive asc";

        public static string GetDefaultSorting(bool withEntityName)
        {
            return string.Format(DefaultSorting, withEntityName ? "MerchantUser." : string.Empty);
        }

    }
}