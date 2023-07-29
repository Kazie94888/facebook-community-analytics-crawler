using System;
using System.Collections.Generic;
using System.Diagnostics;
using FacebookCommunityAnalytics.Crawler.NET.Client.Enums;

namespace FacebookCommunityAnalytics.Crawler.NET.Client.Entities
{
    #region CrawlResult/UncrawledPosts

    public class GetUncrawledGroupUserApiRequest
    {
    }

    public class GetUncrawledGroupUserApiResponse
    {
        public int Count { get; set; }
        public List<UncrawledGroupUserItem> Items { get; set; }

        public GetUncrawledGroupUserApiResponse()
        {
            Items = new List<UncrawledGroupUserItem>();
        }
    }

    public class UncrawledGroupUserItem
    {
        public string GroupName { get; set; }
        public string GroupFid { get; set; }

        public List<UncrawledGroupUserCrawlUrlItem> UrlItems { get; set; }

        public UncrawledGroupUserItem()
        {
            UrlItems = new List<UncrawledGroupUserCrawlUrlItem>();
        }

        public List<CrawlModelBase> ToCrawlModelBase()
        {
            var crawlModelBases = new List<CrawlModelBase>();
            foreach (var groupUserCrawlUrlItem in UrlItems)
            {
                crawlModelBases.Add(new CrawlModelBase
                {
                    Fuid = groupUserCrawlUrlItem.Fuid,
                    GroupFid = groupUserCrawlUrlItem.GroupFid,
                    Url = groupUserCrawlUrlItem.Url
                });
            }

            return crawlModelBases;
        }
    }

    public class UncrawledGroupUserCrawlUrlItem : CrawlModelBase
    {
        public string GroupName { get; set; }
        public Guid UserId { get; set; }
        public string UserCode { get; set; }
    }
    
    #endregion

    #region ACCOUNT

    public class GetAccountsRequest
    {
        public AccountStatus AccountStatus { get; set; }
    }
    
    public class AccountDto
    {
        public string Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string TwoFactorCode { get; set; }
        public AccountType AccountType { get; set; }
        public AccountStatus AccountStatus { get; set; }
        public string Description { get; set; }
        public string Email { get; set; }
        public string EmailPassword { get; set; }
        public bool IsActive { get; set; }

        public AccountUpdateDto ToAccountUpdateDto()
        {
            return new AccountUpdateDto
            {
                Description = Description,
                Email = Email,
                Password = Password,
                Username = Username,
                AccountStatus = AccountStatus,
                AccountType = AccountType,
                EmailPassword = EmailPassword,
                IsActive = IsActive,
                TwoFactorCode = TwoFactorCode
            };
        }
    }

    #endregion

    #region ACCOUNT PROXIES

    public class GetAccountProxiesRequest
    {
        public AccountType AccountType { get; set; }
    }

    public class ResetAccountsCrawlStatusRequest
    {
        public List<Guid> AccountProxyIds { get; set; }
        public AccountType AccountType { get; set; }
    }

    #endregion
}