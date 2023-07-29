using LookOn.Core.Extensions;
using LookOn.Integrations.Haravan.Components.Repositories.Interfaces;
using LookOn.Integrations.Haravan.Models.Entities;
using LookOn.Integrations.Haravan.Models.ValueObjects;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Volo.Abp.Domain.Repositories.MongoDB;
using Volo.Abp.MongoDB;

namespace LookOn.Integrations.Haravan.Components.Repositories;

public class MongoHaravanCustomerRepository : MongoDbRepository<HaravanDbContext, HaravanCustomer, Guid>, IHaravanCustomerRepository
{
    public MongoHaravanCustomerRepository(IMongoDbContextProvider<HaravanDbContext> dbContextProvider) : base(dbContextProvider)
    {
    }

    public async Task<List<HaravanCustomerPhoneNoAndEmail>> GetCusPhoneNosAndEmails(IList<long> haravanCustomerIds, CancellationToken cancellationToken = default)
    {
        var queryable = (await GetMongoQueryableAsync(cancellationToken))
                       .Where(customer => customer.CustomerId.HasValue
                                       && haravanCustomerIds.Contains(customer.CustomerId.Value))
                       .GroupBy(customer => customer.CustomerId.Value)
                       .Select(grouping => new HaravanCustomerPhoneNoAndEmail{ Email = grouping.First().Email, HaravanCustomerId = grouping.Key, PhoneNo = grouping.First().Phone});

        return await queryable.As<IMongoQueryable<HaravanCustomerPhoneNoAndEmail>>().ToListAsync(GetCancellationToken(cancellationToken));
    }

    public async Task<long> CountTotal(Guid merchantId, CancellationToken cancellationToken = default)
    {
        var queryable = (await GetMongoQueryableAsync(cancellationToken)).Where(customer => customer.MerchantId == merchantId)
                                                                         .Select(customer => customer.Id)
                                                                         .Distinct();

        return await queryable.As<IMongoQueryable<Guid>>().LongCountAsync(GetCancellationToken(cancellationToken));
    }

    public async Task<List<long>> GetMerchantCusIdsByPhoneNos(Guid              merchantId,
                                                              IList<string>     phoneNumbers,
                                                              CancellationToken cancellationToken = default)
    {
        var queryable = (await GetMongoQueryableAsync(cancellationToken))
                       .Where(customer => customer.MerchantId == merchantId && phoneNumbers.Contains(customer.Phone) && customer.CustomerId.HasValue)
                       .Select(customer => customer.CustomerId.Value).Distinct();
        
        return await queryable.As<IMongoQueryable<long>>().ToListAsync(GetCancellationToken(cancellationToken));
    }
}