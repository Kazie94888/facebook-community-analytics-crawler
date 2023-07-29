namespace LookOn.Integrations.Datalytis.Configs;

public class DatalytisGlobalConfig
{
    public const int    RateLimitInMs     = 1000;
    public const string ApiBaseUrl             = "https://api.datalytis.com";
    public const string InsightBaseUrl         = "https://insight.datalytis.com";
    public const string DefaultParam           = "email=LookOn@gmail.com&token=$2y$10$MGx2hiARZBxlNw6b9zLNt.4BirOH3TWRRfJb8Ksixgt3QKf72xUta";
    public const int    DefaultPageSize        = 300;
    public const int    DefaultPageSize_Insert = 1000;

    // Social configs
    public const string VaiThuHayPageId = "505030503209494";

    public const int SocialUserSyncAfterDays = 30;
}