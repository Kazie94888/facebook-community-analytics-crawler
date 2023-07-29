namespace LookOn.Platforms
{
    public static class PlatformConsts
    {
        private const string DefaultSorting = "{0}Name asc";

        public static string GetDefaultSorting(bool withEntityName)
        {
            return string.Format(DefaultSorting, withEntityName ? "Platform." : string.Empty);
        }

    }
}