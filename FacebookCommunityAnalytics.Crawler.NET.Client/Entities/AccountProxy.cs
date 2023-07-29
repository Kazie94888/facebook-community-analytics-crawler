using System;
using System.Diagnostics;
using FacebookCommunityAnalytics.Crawler.NET.Client.Enums;

namespace FacebookCommunityAnalytics.Crawler.NET.Client.Entities
{
     [DebuggerDisplay("{DebuggerDisplay}")]
    public class AccountProxyItem
    {
        public AccountProxy accountProxy { get; set; }
        public Account account { get; set; }
        public ApiProxy proxy { get; set; }
        
        private string DebuggerDisplay
        {
            get
            {
                return account?.username;
            }
        }
    }

    public class AccountProxy
    {
        public string id { get; set; }
        public DateTime? creationTime { get; set; }
        public string creatorId { get; set; }
        public DateTime? lastModificationTime { get; set; }
        public string lastModifierId { get; set; }
        public bool isDeleted { get; set; }
        public string deleterId { get; set; }
        public DateTime? deletionTime { get; set; }
        public string description { get; set; }
        public string accountId { get; set; }
        public string proxyId { get; set; }
        public bool IsCrawling { get; set; }
        public DateTime CrawledAt { get; set; }
    }

    [DebuggerDisplay("{username}")]
    public class Account
    {
        public string id { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public string twoFactorCode { get; set; }
        public bool isActive { get; set; }
        public AccountType accountType { get; set; }
        public AccountStatus accountStatus { get; set; }
        
        public string email { get; set; }
        
        public string emailPassword { get; set; }
    }

    public class ApiProxy
    {
        public string id { get; set; }
        public DateTime? creationTime { get; set; }
        public string creatorId { get; set; }
        public DateTime? lastModificationTime { get; set; }
        public string lastModifierId { get; set; }
        public bool isDeleted { get; set; }
        public string deleterId { get; set; }
        public DateTime? deletionTime { get; set; }
        public string ip { get; set; }
        public int port { get; set; }
        public string protocol { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public DateTime? lastPingDateTime { get; set; }
        public bool isActive { get; set; }
    }
    
    public class AccountUpdateDto
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string TwoFactorCode { get; set; }
        public AccountType AccountType { get; set; }
        public AccountStatus AccountStatus { get; set; }
        public bool IsActive { get; set; }
        public string Description { get; set; }
        public string Email { get; set; }
        public string EmailPassword { get; set; }
    }
}