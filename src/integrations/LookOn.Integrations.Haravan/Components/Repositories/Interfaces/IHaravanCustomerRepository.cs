using LookOn.Integrations.Haravan.Models.Entities;
using LookOn.Integrations.Haravan.Models.ValueObjects;
using Volo.Abp.Domain.Repositories;

namespace LookOn.Integrations.Haravan.Components.Repositories.Interfaces;

public interface IHaravanCustomerRepository : IRepository<HaravanCustomer, Guid>
{
    Task<List<HaravanCustomerPhoneNoAndEmail>> GetCusPhoneNosAndEmails(IList<long> haravanCustomerIds, CancellationToken cancellationToken                                 = default);
    Task<List<long>> GetMerchantCusIdsByPhoneNos(Guid merchantId, IList<string> phoneNumbers, CancellationToken cancellationToken = default);
    Task<long>                                 CountTotal(Guid                     merchantId,         CancellationToken cancellationToken = default);

}