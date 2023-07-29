using System;

namespace Crawler.PlaywrightConsoleApp.Models.Apis
{
    public class FacebookAccount
    {
        public virtual string Username { get; set; }

        public virtual string Password { get; set; }

        public virtual string TwoFactorCode { get; set; }

        public virtual bool IsActive { get; set; }
    }

    public class ProxyAccount
    {
        public virtual string Ip { get; set; }

        public virtual int Port { get; set; }

        public virtual string Protocol { get; set; }

        public virtual string Username { get; set; }

        public virtual string Password { get; set; }

        public virtual DateTime LastPingDateTime { get; set; }

        public virtual bool IsActive { get; set; }
    }

    public class AccountFacebookProxy
    {
        public FacebookAccount Account { get; set; }
        public ProxyAccount Proxy { get; set; }
    }
}