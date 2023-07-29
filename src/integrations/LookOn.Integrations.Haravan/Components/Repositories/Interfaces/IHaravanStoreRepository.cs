using LookOn.Integrations.Haravan.Models.Entities;
using Volo.Abp.Domain.Repositories;

namespace LookOn.Integrations.Haravan.Components.Repositories.Interfaces;

public interface IHaravanStoreRepository : IRepository<HaravanStore, Guid>
{
    Task<List<HaravanStore>> GetListAsync(
        string filter = null,
        Guid? appUserId = null,
        string storeId = null,
        string storeName = null,
        string email = null,
        string phone = null,
        string sorting = null,
        int maxResultCount = int.MaxValue,
        int skipCount = 0,
        CancellationToken cancellationToken = default);
    
    Task<long> GetCountAsync(
        string filter = null,
        Guid? appUserId = null,
        string storeId = null,
        string storeName = null,
        string email = null,
        string phone = null,
        CancellationToken cancellationToken = default);

}