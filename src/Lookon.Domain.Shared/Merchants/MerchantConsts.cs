namespace LookOn.Merchants
{
    public static class MerchantConsts
    {
        private const string DefaultSorting = "{0}Name asc";

        public static string GetDefaultSorting(bool withEntityName)
        {
            return string.Format(DefaultSorting, withEntityName ? "Merchant." : string.Empty);
        }

        public const int NameMaxLength = 250;
        public const int PhoneMaxLength = 14;
    }
}