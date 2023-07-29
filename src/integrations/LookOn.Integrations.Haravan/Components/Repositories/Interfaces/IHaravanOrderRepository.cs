using LookOn.Integrations.Haravan.Models.Entities;
using LookOn.Integrations.Haravan.Models.Enums;
using Volo.Abp.Domain.Repositories;

namespace LookOn.Integrations.Haravan.Components.Repositories.Interfaces;

public interface IHaravanOrderRepository : IRepository<HaravanOrder, Guid>
{
    Task<List<HaravanOrder>> GetListAsync(Guid               merchantId,
                                    HaravanFulfillmentStatus? fulfillmentStatus,
                                    DateTime           fromTime,
                                    DateTime           endTime,
                                    CancellationToken  cancellationToken = default);

    Task<long> CountAsync(Guid               merchantId,
                          HaravanFulfillmentStatus? fulfillmentStatus,
                          DateTime           fromTime,
                          DateTime           endTime,
                          CancellationToken  cancellationToken = default);

    Task<List<long>> GetHaravanCustomerId(Guid               merchantId,
                                           HaravanFulfillmentStatus? fulfillmentStatus,
                                           DateTime           fromTime,
                                           DateTime           endTime,
                                           CancellationToken  cancellationToken = default);

    Task<decimal> GetTotalRevenueByHaravanCustomerIds(Guid              merchantId,
                                                      List<long>        haravanCustomerIds,
                                                      DateTime          fromTime,
                                                      DateTime          endTime,
                                                      CancellationToken cancellationToken = default);
}