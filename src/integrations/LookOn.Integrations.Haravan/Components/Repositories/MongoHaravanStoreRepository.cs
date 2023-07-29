using System.Linq.Dynamic.Core;
using LookOn.Integrations.Haravan.Components.Repositories.Interfaces;
using LookOn.Integrations.Haravan.Models.Entities;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Volo.Abp.Domain.Repositories.MongoDB;
using Volo.Abp.MongoDB;

namespace LookOn.Integrations.Haravan.Components.Repositories;

public class MongoHaravanStoreRepository : MongoDbRepository<HaravanDbContext, HaravanStore, Guid>, IHaravanStoreRepository
{
    public MongoHaravanStoreRepository(IMongoDbContextProvider<HaravanDbContext> dbContextProvider) : base(dbContextProvider)
    {
    }

    public async Task<List<HaravanStore>> GetListAsync(string            filter            = null,
                                                       Guid?             appUserId         = null,
                                                       string            storeId           = null,
                                                       string            storeName         = null,
                                                       string            email             = null,
                                                       string            phone             = null,
                                                       string            sorting           = null,
                                                       int               maxResultCount    = int.MaxValue,
                                                       int               skipCount         = 0,
                                                       CancellationToken cancellationToken = default)
    {
        var query = ApplyFilter((await GetMongoQueryableAsync(cancellationToken)),
                                filter,
                                appUserId,
                                storeId,
                                storeName,
                                email,
                                phone);
        query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? HaravanStoreConsts.GetDefaultSorting(false) : sorting);

        return await query.As<IMongoQueryable<HaravanStore>>()
                          .PageBy<HaravanStore, IMongoQueryable<HaravanStore>>(skipCount, maxResultCount)
                          .ToListAsync(GetCancellationToken(cancellationToken));
    }

    public async Task<long> GetCountAsync(string            filter            = null,
                                          Guid?             appUserId         = null,
                                          string            storeId           = null,
                                          string            storeName         = null,
                                          string            email             = null,
                                          string            phone             = null,
                                          CancellationToken cancellationToken = default)
    {
        var query = ApplyFilter((await GetMongoQueryableAsync(cancellationToken)),
                                filter,
                                appUserId,
                                storeId,
                                storeName,
                                email,
                                phone);

        return await query.As<IMongoQueryable<HaravanStore>>().LongCountAsync(GetCancellationToken(cancellationToken));
    }

    protected virtual IQueryable<HaravanStore> ApplyFilter(IQueryable<HaravanStore> query,
                                                           string                   filter    = null,
                                                           Guid?                    appUserId = null,
                                                           string                   storeId   = null,
                                                           string                   storeName = null,
                                                           string                   email     = null,
                                                           string                   phone     = null)
    {
        return query
              .WhereIf(!filter.IsNullOrWhiteSpace(),
                       x => x.StoreId.Contains(filter) || x.StoreName.Contains(filter) || x.Email.Contains(filter) || x.Phone.Contains(filter))
              .WhereIf(appUserId.HasValue,              x => x.AppUserId.Equals(appUserId))
              .WhereIf(!storeId.IsNullOrWhiteSpace(),   x => x.StoreId.Equals(storeId))
              .WhereIf(!storeName.IsNullOrWhiteSpace(), x => x.Phone.Contains(storeName))
              .WhereIf(!phone.IsNullOrWhiteSpace(),     x => x.Phone.Contains(phone))
              .WhereIf(!email.IsNullOrWhiteSpace(),     x => x.Phone.Equals(email));
    }
}