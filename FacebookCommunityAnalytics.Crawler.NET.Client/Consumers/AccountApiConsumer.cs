using System;
using FacebookCommunityAnalytics.Crawler.NET.Client.Clients;
using FacebookCommunityAnalytics.Crawler.NET.Client.Core;
using FacebookCommunityAnalytics.Crawler.NET.Client.Entities;
using FacebookCommunityAnalytics.Crawler.NET.Client.Enums;
using RestSharp;

namespace FacebookCommunityAnalytics.Crawler.NET.Client.Consumers
{
    public class AccountApiConsumer : BaseApiConsumer
    {
        public AccountApiConsumer(RestClient client) : base(client)
        {
        }

        public ApiResponse<Account> Get(Guid id)
        {
            var restRequest = new RestRequest($"/app/accounts/{id}", Method.GET);
            return HandleResponse<Account>(Execute(restRequest));
        }

        public ApiResponse<Account> Update(Guid id, AccountUpdateDto updateDto)
        {
            var restRequest = new RestRequest($"/app/accounts/{id}", Method.PUT);
            restRequest.AddJsonBody(updateDto);

            return HandleResponse<Account>(Execute(restRequest));
        }

        public ApiResponse<Account> UpdateAccountInfo(Guid id, AccountType? accountType, AccountStatus? accountStatus)
        {
            var accResponse = Get(id);
            if (!accResponse.IsSuccess || accResponse.Resource == null)
            {
                return new ApiResponse<Account> {IsSuccess = false};
            }

            var acc = accResponse.Resource;
            return Update
            (
                id,
                new AccountUpdateDto
                {
                    AccountType = accountType?? acc.accountType,
                    AccountStatus = accountStatus?? acc.accountStatus,
                    Password = acc.password,
                    Username = acc.username,
                    IsActive = acc.isActive,
                    TwoFactorCode = acc.twoFactorCode,
                    Email = acc.email,
                    EmailPassword = acc.emailPassword
                }
            );

        }
    }
}