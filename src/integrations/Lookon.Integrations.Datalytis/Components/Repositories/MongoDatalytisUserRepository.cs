using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LookOn.Core.Extensions;
using LookOn.Integrations.Datalytis.Components.Repositories.Interfaces;
using LookOn.Integrations.Datalytis.Models.Entities;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Volo.Abp.Domain.Repositories.MongoDB;
using Volo.Abp.MongoDB;

namespace LookOn.Integrations.Datalytis.Components.Repositories;

public class MongoDatalytisUserRepository : MongoDbRepository<DatalytisDbContext, DatalytisUser, Guid>, IDatalytisUserRepository
{
    public MongoDatalytisUserRepository(IMongoDbContextProvider<DatalytisDbContext> dbContextProvider) : base(dbContextProvider)
    {
    }

    public async Task<List<DatalytisUser>> GetPageUsers(List<string>                        communityPageIds,
                                                        IList<KeyValuePair<string, string>> phoneNosEmails,
                                                        CancellationToken                   cancellationToken = default)
    {
        var phoneNos = phoneNosEmails.Where(pair => pair.Key.TrimSafe().IsNotNullOrWhiteSpace()).Select(pair => pair.Key).Distinct().ToList();
        var emails   = phoneNosEmails.Where(pair => pair.Value.TrimSafe().IsNotNullOrWhiteSpace()).Select(pair => pair.Value).Distinct().ToList();

        var queryable = (await GetMongoQueryableAsync(cancellationToken)).Where(user => communityPageIds.Any(_ => user.MerchantFbPageIds.Contains(_))
                                                                                     && (phoneNos.Contains(user.Phone)
                                                                                      || emails.Contains(user.Email)));

        var datalytisUsers = (await queryable.As<IMongoQueryable<DatalytisUser>>().ToListAsync(GetCancellationToken(cancellationToken)))
                            .DistinctBy(datalytisUser => datalytisUser.Uid)
                            .ToList();

        return datalytisUsers;
    }

    public async Task<List<DatalytisUser>> GetUsersNotMetPhoneNosAndEmails(List<string>                        communityPageIds,
                                                                            IList<KeyValuePair<string, string>> phoneNosEmails,
                                                                            CancellationToken                   cancellationToken = default)
    {
        var phoneNos = phoneNosEmails.Where(pair => pair.Key.TrimSafe().IsNotNullOrWhiteSpace()).Select(pair => pair.Key).Distinct().ToList();
        var emails   = phoneNosEmails.Where(pair => pair.Value.TrimSafe().IsNotNullOrWhiteSpace()).Select(pair => pair.Value).Distinct().ToList();

        var queryable = (await GetMongoQueryableAsync(cancellationToken)).Where(user => communityPageIds.Any(s => user.MerchantFbPageIds.Contains(s))
                                                                                     && !phoneNos.Contains(user.Phone)
                                                                                     && !emails.Contains(user.Email));
        
        var datalytisUsers = (await queryable.As<IMongoQueryable<DatalytisUser>>().ToListAsync(GetCancellationToken(cancellationToken)))
                            .DistinctBy(datalytisUser => datalytisUser.Uid)
                            .ToList();

        return datalytisUsers;
    }

    public async Task<long> CountUsers(List<string> communityPageIds, CancellationToken cancellationToken = default)
    {
        var queryable = (await GetMongoQueryableAsync(cancellationToken)).Where(user => communityPageIds.Any(s => user.MerchantFbPageIds.Contains(s)));

        return await queryable.As<IMongoQueryable<DatalytisUser>>().LongCountAsync(GetCancellationToken(cancellationToken));
    }
}