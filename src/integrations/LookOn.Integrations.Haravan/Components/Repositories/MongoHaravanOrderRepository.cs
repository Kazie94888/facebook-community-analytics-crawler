using LookOn.Integrations.Haravan.Components.Repositories.Interfaces;
using LookOn.Integrations.Haravan.Models.Entities;
using LookOn.Integrations.Haravan.Models.Enums;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Volo.Abp.Domain.Repositories.MongoDB;
using Volo.Abp.MongoDB;

namespace LookOn.Integrations.Haravan.Components.Repositories;

public class MongoHaravanOrderRepository : MongoDbRepository<HaravanDbContext, HaravanOrder, Guid>, IHaravanOrderRepository
{
    public MongoHaravanOrderRepository(IMongoDbContextProvider<HaravanDbContext> dbContextProvider) : base(dbContextProvider)
    {
    }

    public async Task<List<HaravanOrder>> GetListAsync(Guid                      merchantId,
                                                       HaravanFulfillmentStatus? fulfillmentStatus,
                                                       DateTime                  fromTime,
                                                       DateTime                  endTime,
                                                       CancellationToken         cancellationToken = default)
    {
        var query = ApplyFilter(await GetMongoQueryableAsync(cancellationToken),
                                merchantId,
                                fulfillmentStatus,
                                fromTime,
                                endTime);
        return await query.As<IMongoQueryable<HaravanOrder>>().ToListAsync(GetCancellationToken(cancellationToken));
    }

    public async Task<long> CountAsync(Guid                      merchantId,
                                       HaravanFulfillmentStatus? fulfillmentStatus,
                                       DateTime                  fromTime,
                                       DateTime                  endTime,
                                       CancellationToken         cancellationToken = default)
    {
        // return (await GetListAsync(merchantId, fulfillmentStatus, fromTime, endTime)).Count;

        var queryable = ApplyFilter(await GetMongoQueryableAsync(cancellationToken),
                                    merchantId,
                                    fulfillmentStatus,
                                    fromTime,
                                    endTime)
           .Select(order => order.Id);
        return await queryable.As<IMongoQueryable<Guid>>().LongCountAsync(GetCancellationToken(cancellationToken));
    }

    public async Task<List<long>> GetHaravanCustomerId(Guid                      merchantId,
                                                       HaravanFulfillmentStatus? HRVFulfillmentStatus,
                                                       DateTime                  fromTime,
                                                       DateTime                  endTime,
                                                       CancellationToken         cancellationToken = default)
    {
        var queryable = ApplyFilter(await GetMongoQueryableAsync(cancellationToken),
                                    merchantId,
                                    HRVFulfillmentStatus,
                                    fromTime,
                                    endTime)
                       .Where(order => order.HaravanCustomerId.HasValue)
                       .Select(order => order.HaravanCustomerId.Value)
                       .Distinct();

        return await queryable.As<IMongoQueryable<long>>().ToListAsync(GetCancellationToken(cancellationToken));
    }

    public async Task<decimal> GetTotalRevenueByHaravanCustomerIds(Guid              merchantId,
                                                                   List<long>        haravanCustomerIds,
                                                                   DateTime          fromTime,
                                                                   DateTime          endTime,
                                                                   CancellationToken cancellationToken = default)
    {
        var queryable = ApplyFilter(await GetMongoQueryableAsync(cancellationToken),
                                    merchantId,
                                    HaravanFulfillmentStatus.Fulfilled,
                                    fromTime,
                                    endTime)
                       .Where(order => order.HaravanCustomerId.HasValue && haravanCustomerIds.Contains(order.HaravanCustomerId.Value))
                       .Sum(order => order.TotalPrice ?? 0);
        return queryable;

        // .Select(order => order.TotalPrice);

        // return (await queryable.As<IMongoQueryable<decimal>>().ToListAsync(GetCancellationToken(cancellationToken))).Sum(arg => arg);
    }

    protected virtual IQueryable<HaravanOrder> ApplyFilter(IQueryable<HaravanOrder>  query,
                                                           Guid                      merchantId,
                                                           HaravanFulfillmentStatus? fulfillmentStatus,
                                                           DateTime                  fromTime,
                                                           DateTime                  endTime)
    {
        return query.Where(o => o.MerchantId == merchantId
                             && (!fulfillmentStatus.HasValue || o.FulfillmentStatus == fulfillmentStatus)
                             && o.ConfirmedAt.HasValue
                             && o.CreatedAt.HasValue
                             && o.CreatedAt.Value >= fromTime
                             && o.CreatedAt.Value <= endTime);
    }
}