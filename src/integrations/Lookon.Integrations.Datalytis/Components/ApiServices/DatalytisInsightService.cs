using System;
using System.Collections.Generic;
using System.Linq;
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

public class DatalytisInsightService : ITransientDependency
{
    public           IObjectMapper                           ObjectMapper { get; set; }
    private readonly IRepository<DatalytisUserSocialInsight> _insightRepository;
    private readonly DatalytisApiConsumer                    _datalytisApiConsumer;

    public DatalytisInsightService(IRepository<DatalytisUserSocialInsight> insightRepository, DatalytisApiConsumer datalytisApiConsumer)
    {
        _insightRepository    = insightRepository;
        _datalytisApiConsumer = datalytisApiConsumer;
    }

    // public async Task FakeInsightData(int count = 100)
    // {
    //     var rand = new Random();
    //     for (var i = 0; i < count; i++)
    //     {
    //         var insights     = new List<UserSocialInsight>();
    //         var insightCount = rand.Next(1, 20);
    //         for (var j = 0; j < insightCount; j++)
    //         {
    //             var categoryName = RandomExtensions.RandomString(20);
    //             insights.Add(new UserSocialInsight
    //             {
    //                 Id       = RandomExtensions.RandomNumber(15),
    //                 Name     = RandomExtensions.RandomString(20),
    //                 Category = categoryName,
    //                 CategoryList = new List<SocialPage>()
    //                 {
    //                     new SocialPage() { Id = RandomExtensions.RandomNumber(15), Name = categoryName }
    //                 },
    //                 Url         = null,
    //                 CreatedTime = DateTime.UtcNow,
    //                 Location    = null,
    //                 Type        = rand.Next(0, 2)
    //             });
    //         }
    //
    //         var dataInsight = new DatalytisUserSocialInsight { Uid = RandomExtensions.RandomNumber(15), Insights = insights };
    //         await _insightRepository.InsertAsync(dataInsight);
    //     }
    // }

    public async Task<DatalytisSyncStatus> UserSocialInsights_Sync(string insightRequestId)
    {
        var response = await _datalytisApiConsumer.Insights_User_Get(insightRequestId);
        if (!response.Success)
        {
            if (response.StatusCode.IsIn(500, 404))
            {
                return DatalytisSyncStatus.NotFound;
            }
            else
            {
                return DatalytisSyncStatus.Failed;
            }
        }

        if (response.Data.Insights.IsNullOrEmpty()) return DatalytisSyncStatus.Completed;
        
        var datalytisUserSocialInsights = ObjectMapper.Map<List<UserSocialInsightRaw>, List<DatalytisUserSocialInsight>>(response.Data.Insights);
        datalytisUserSocialInsights = datalytisUserSocialInsights.DistinctBy(_ => _.InsightId).ToList();
        foreach (var batch in datalytisUserSocialInsights.Partition(DatalytisGlobalConfig.DefaultPageSize_Insert))
        {
            // TODOO Improvements: get existing uids from batch => insert many NON existing
            await _insightRepository.InsertManyAsync(batch, autoSave: true);
        }
        return DatalytisSyncStatus.Completed;
    }

    // public async Task<DatalytisSyncStatus> UserSocialInsights_GetMany(List<string> uids)
    // {
    //     var response = await _datalytisApiConsumer.Insights_User_Get(insightRequestId);
    //     if (!response.Success)
    //     {
    //         if (response.StatusCode == 500)
    //         {
    //             return DatalytisSyncStatus.NotFound;
    //         }
    //         else
    //         {
    //             return DatalytisSyncStatus.Failed;
    //         }
    //     }
    //     
    //     var payload = ObjectMapper.Map<DatalytisUserSocialInsightRaw, DatalytisUserSocialInsight>(response);
    //     
    //     await _insightRepository.InsertAsync(payload);
    //
    //     return DatalytisSyncStatus.Completed;
    // }

    public async Task<DatalytisSocialInsight_Response> UserSocialInsights_Request(DatalytisSocialInsight_Request request)
    {
        return await _datalytisApiConsumer.Insights_User_Request(request);
    }

    public async Task<bool> UserSocialInsights_Status(string requestId)
    {
        return await _datalytisApiConsumer.Insights_User_GetStatus(requestId);
    }
}