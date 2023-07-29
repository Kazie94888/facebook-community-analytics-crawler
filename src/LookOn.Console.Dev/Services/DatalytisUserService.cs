using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LookOn.Core.Extensions;
using LookOn.Integrations.Datalytis;
using LookOn.Integrations.Datalytis.Components.Repositories;
using LookOn.Integrations.Datalytis.Components.Repositories.Interfaces;
using LookOn.Integrations.Datalytis.Helpers;
using LookOn.Merchants;
using Volo.Abp.Domain.Repositories;

namespace LookOn.Console.Dev.Services;

public class DatalytisUserService : DevService
{
    public IDatalytisUserRepository DatalytisUserRepo  { get; set; }
    public IMerchantRepository      MerchantRepository { get; set; }
    public DatalytisManager         DatalytisManager   { get; set; } 
    
    public override Task Test()
    {
        Log(GetType().FullName);
        return Task.CompletedTask;
    }

    public async Task Migrate_Relationship_Gender()
    {
        var users = await DatalytisUserRepo.GetListAsync();
        foreach (var batch in users.Partition(1000))
        {
            var list = batch.ToList();
            foreach (var user in list)
            {
                user.Gender = DatalytisMapper.ToGenderType(user.Sex);
                user.RelationshipStatus = DatalytisMapper.ToRelationshipStatus(user.Relationship);
            }

            await DatalytisUserRepo.UpdateManyAsync(list);
            Log($"Migrate_Relationship_Gender {list.Count} at {DateTime.UtcNow}");
        }
        
        Log("COMPLETED - Migrate_Relationship_Gender");
    }

    public async Task PrintRelationships()
    {
        var users = await DatalytisUserRepo.GetListAsync();
        var list    = users.Select(_ => _.Relationship).Distinct().ToList();

        foreach (var item in list)
        {
            Log(item);
        }
        // LogDebug(relationships.JoinAsString(", "));
    }
    
    public async Task PrintSexes()
    {
        var users = await DatalytisUserRepo.GetListAsync();
        var list    = users.Select(_ => _.Sex).Distinct().ToList();

        foreach (var item in list)
        {
            Log(item);
        }
        // LogDebug(list.JoinAsString(", "));
    }
}