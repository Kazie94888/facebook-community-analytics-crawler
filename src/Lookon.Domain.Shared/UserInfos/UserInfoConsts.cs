namespace LookOn.UserInfos
{
    public static class UserInfoConsts
    {
        private const string DefaultSorting = "{0}IdentificationNumber asc";

        public static string GetDefaultSorting(bool withEntityName)
        {
            return string.Format(DefaultSorting, withEntityName ? "UserInfo." : string.Empty);
        }

    }
}