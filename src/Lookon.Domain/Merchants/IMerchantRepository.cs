using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace LookOn.Merchants
{
    public interface IMerchantRepository : IRepository<Merchant, Guid>
    {
        Task<MerchantWithNavigationProperties> GetWithNavigationPropertiesAsync(
    Guid id,
    CancellationToken cancellationToken = default
);

        Task<List<MerchantWithNavigationProperties>> GetListWithNavigationPropertiesAsync(
            string filterText = null,
            string name = null,
            string phone = null,
            string address = null,
            string email = null,
            string fax = null,
            Guid? ownerAppUserId = null,
            Guid? categoryId = null,
            string sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default
        );

        Task<List<Merchant>> GetListAsync(
                    string filterText = null,
                    string name = null,
                    string phone = null,
                    string address = null,
                    string email = null,
                    string fax = null,
                    string sorting = null,
                    int maxResultCount = int.MaxValue,
                    int skipCount = 0,
                    CancellationToken cancellationToken = default
                );

        Task<long> GetCountAsync(
            string filterText = null,
            string name = null,
            string phone = null,
            string address = null,
            string email = null,
            string fax = null,
            Guid? ownerAppUserId = null,
            Guid? categoryId = null,
            CancellationToken cancellationToken = default);
    }
}