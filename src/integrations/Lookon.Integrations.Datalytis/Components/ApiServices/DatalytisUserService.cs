using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using LookOn.Core.Extensions;
using LookOn.Integrations.Datalytis.Components.ApiConsumers;
using LookOn.Integrations.Datalytis.Configs;
using LookOn.Integrations.Datalytis.Models.Entities;
using LookOn.Integrations.Datalytis.Models.Enums;
using LookOn.Integrations.Datalytis.Models.RawModels;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using IObjectMapper = Volo.Abp.ObjectMapping.IObjectMapper;

namespace LookOn.Integrations.Datalytis.Components.ApiServices;

public class DatalytisUserService : ITransientDependency
{
    public IObjectMapper ObjectMapper { get; set; }

    // private stuff
    private readonly IRepository<DatalytisUser> _datalytisUserRepo;
    private readonly DatalytisApiConsumer       _datalytisApiConsumer;

    public DatalytisUserService(IRepository<DatalytisUser> datalytisUserRepo, DatalytisApiConsumer datalytisApiConsumer)
    {
        _datalytisUserRepo    = datalytisUserRepo;
        _datalytisApiConsumer = datalytisApiConsumer;
    }

    public async Task<DatalytisSyncStatus> SocialUsers_Sync(string pageId, int pageNumber)
    {
        var response = await _datalytisApiConsumer.SocialUsers_Get(pageId, pageNumber, true);
        if (!response.Success)
        {
            return response.StatusCode == (int) HttpStatusCode.NotFound ? DatalytisSyncStatus.NotFound : DatalytisSyncStatus.Failed;
        }

        if (response.Data.IsNullOrEmpty()) return DatalytisSyncStatus.Completed;

        var payload       = ObjectMapper.Map<List<DatalytisUserRaw>, List<DatalytisUser>>(response.Data);
        var phoneNumbers  = payload.Select(x => x.Phone).ToList();
        var existingUsers = await _datalytisUserRepo.GetListAsync(x => phoneNumbers.Contains(x.Phone));

        Debug.WriteLine(nameof(SocialUsers_Sync),
                        $"SocialUsers_Sync {pageId} - Page {pageNumber} - Page Size {DatalytisGlobalConfig.DefaultPageSize}");

        var newUsers    = new ConcurrentBag<DatalytisUser>();
        var updateUsers = new ConcurrentBag<DatalytisUser>();

        Parallel.ForEach(payload,
                         user =>
                         {
                             var existing = existingUsers.FirstOrDefault(c => c.Phone == user.Phone);
                             if (existing is not null)
                             {
                                 if (existing.MerchantFbPageIds.Contains(pageId)) return;
                                 existing.MerchantFbPageIds.Add(pageId);
                                 updateUsers.Add(existing);
                             }
                             else
                             {
                                 user.MerchantFbPageIds.Add(pageId);
                                 newUsers.Add(user);
                             }
                         });

        if (newUsers.IsNotNullOrEmpty())
        {
            foreach (var users in newUsers.Partition(100))
            {
                await _datalytisUserRepo.InsertManyAsync(users);
            }
        }

        if (updateUsers.IsNotNullOrEmpty())
        {
            foreach (var users in updateUsers.Partition(100))
            {
                await _datalytisUserRepo.UpdateManyAsync(users);
            }
        }

        return DatalytisSyncStatus.InProgress;
    }

    public async Task<SocialUsers_Response> SocialUsers_Request(SocialUsers_Request request)
    {
        return await _datalytisApiConsumer.SocialUsers_Request(request);
    }

    public async Task<DatalytisUsersResponse> DatalytisUsers_Request(List<string> phones, bool rateLimit = false)
    {
        return await _datalytisApiConsumer.GetUsersByPhone(phones, rateLimit);
    }

    public async Task<SocialUsers_Status> SocialPageUsers_Status(string socialCommunityId)
    {
        return await _datalytisApiConsumer.SocialUsers_Status(socialCommunityId);
    }

    /// <summary>
    /// Sync user list of merchant community, by community id 
    /// </summary>
    /// <param name="pageId"></param>
    public async Task<bool> SocialUsers_Sync(string pageId)
    {
        var pageNumber = 1;
        while (true)
        {
            var response = await SocialUsers_Sync(pageId, pageNumber);
            switch (response)
            {
                case DatalytisSyncStatus.InProgress:
                    pageNumber++;
                    break;

                case DatalytisSyncStatus.Completed:
                    Debug.WriteLine($"SocialUsers_Sync COMPLETED");
                    return true;

                default: return false;
            }
        }
    }

    public async Task FakeUserData(int count = 100)
    {
        var rand = new Random();
        for (var i = 0; i < count; i++)
        {
            var gender    = RandomExtensions.RandomGender();
            var surName   = RandomExtensions.RandomSurName();
            var firstName = RandomExtensions.RandomFirstName(gender);
            var customer = new DatalytisUser
            {
                Uid          = RandomExtensions.RandomNumber(15),
                Phone        = "0" + RandomExtensions.RandomNumber(),
                Friends      = rand.Next(50, 1000),
                Follow       = rand.Next(50, 1000),
                Email        = $"{surName.ToLower()}_{firstName.ToLower()}@gmail.com",
                Name         = $"{surName} {firstName}",
                Sex          = gender,
                City         = RandomExtensions.RandomProvince(),
                Note5        = null,
                Birthday     = RandomExtensions.RandomBirthDay(),
                Note7        = null,
                Relationship = null
            };
            await _datalytisUserRepo.InsertAsync(customer);
        }
    }
}