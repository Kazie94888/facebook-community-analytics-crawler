using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace LookOn.MerchantStores
{
    public interface IMerchantStoreRepository : IRepository<MerchantStore, Guid>
    {
        Task<MerchantStoreWithNavigationProperties> GetWithNavigationPropertiesAsync(
    Guid id,
    CancellationToken cancellationToken = default
);

        Task<List<MerchantStoreWithNavigationProperties>> GetListWithNavigationPropertiesAsync(
            string filterText = null,
            string name = null,
            string code = null,
            bool? active = null,
            Guid? merchantId = null,
            Guid? platformId = null,
            string sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default
        );

        Task<List<MerchantStore>> GetListAsync(
                    string filterText = null,
                    string name = null,
                    string code = null,
                    bool? active = null,
                    string sorting = null,
                    int maxResultCount = int.MaxValue,
                    int skipCount = 0,
                    CancellationToken cancellationToken = default
                );

        Task<long> GetCountAsync(
            string filterText = null,
            string name = null,
            string code = null,
            bool? active = null,
            Guid? merchantId = null,
            Guid? platformId = null,
            CancellationToken cancellationToken = default);
    }
}