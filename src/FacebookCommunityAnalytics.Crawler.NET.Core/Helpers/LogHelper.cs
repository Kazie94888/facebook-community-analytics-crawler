using System.Diagnostics;
using System.IO;
using System.Reflection;
using log4net;
using log4net.Config;

namespace FacebookCommunityAnalytics.Crawler.NET.Core.Helpers
{
    public class LogHelper
    {
        public static readonly ILog Log = LogManager.GetLogger(typeof(LogHelper));
        public LogHelper(string username)
        {
            var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
            GlobalContext.Properties["fname"] = username;
            XmlConfigurator.Configure(logRepository, new FileInfo("Configurations/log4net.config"));
        }

        public static void Info(string message)
        {
            Log.Info(message);
            Debug.WriteLine(message);
        }
        public static void Error(string message)
        {
            Log.Error(message);
            Debug.WriteLine(message);
        }
    }
}