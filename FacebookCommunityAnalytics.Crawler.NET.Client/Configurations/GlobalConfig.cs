using System.Collections.Generic;
using FacebookCommunityAnalytics.Crawler.NET.Client.Enums;

namespace FacebookCommunityAnalytics.Crawler.NET.Client.Configurations
{
    public class LogLevel
    {
        public string Default { get; set; }
        public string System { get; set; }
        public string Microsoft { get; set; }
    }

    public class Logging
    {
        public LogLevel LogLevel { get; set; }
    }

    public class CrawlConfig
    {
        public string ApiGet2FACodeUrl { get; set; }
        public string IpLookUpUrl { get; set; }
        public string RootUrl { get; set; }
        public string TempFilesPath { get; set; }
        public string UserDataDirRoot { get; set; }
        public string BannedAccountsLogPath { get; set; }
        public int ActionDelay { get; set; }
        public int TypingDelay { get; set; }
        public int BreakDelay { get; set; }
        public int ScrollTimeout { get; set; }

        public int BatchSize_Max_Default { get; set; }
        public int BatchSize_Max_SelectiveGroupPost { get; set; }
        public int BatchSize_Max_AutoLike { get; set; }
        public int BatchSize_Max_AutoComment { get; set; }
        
        public int BatchSize_Max_TiktokSelectiveVideo { get; set; }
        public int BatchSize_Max_GroupPost { get; set; }
        public int BatchSize_Max_GroupUserPost { get; set; }
        public int BatchSize_Max_PagePost { get; set; }
        
        public int Crawl_MaxThread_SelectiveGroupPost { get; set; }
        public int Crawl_MaxThread_AutoLike { get; set; }
        public int Crawl_MaxThread_AutoComment { get; set; }
        public int Crawl_MaxThread_TiktokSelectiveVideo { get; set; }
        
        public int Crawl_MaxThread_TiktokMCNVideo { get; set; }
        public int Crawl_MaxThread_GroupPost { get; set; }
        public int Crawl_MaxThread_GroupUserPost { get; set; }
        public int Crawl_MaxThread_PagePost { get; set; }
        public int Crawl_MaxThread_JoinGroup { get; set; }
        public int Crawl_MaxThread_TiktokVideo { get; set; }
        
    }

    public class ApiConfig
    {
        public string ApiUrl { get; set; }
        
        public string ApiEmailUrl { get; set; }
        public string AuthUrl { get; set; }
        public string ViotpUrl { get; set; }
    }

    public class TiktokConfig
    {
        public List<string> DailyReportEmails { get; set; }
    }

    public class GlobalConfig
    {
        public Logging Logging { get; set; }
        public CrawlConfig CrawlConfig { get; set; }
        public ApiConfig ApiConfig { get; set; }
        public TiktokConfig TiktokConfig { get; set; }
    }
}