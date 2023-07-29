using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace LookOn.MerchantSyncInfos
{
    public interface IMerchantSyncInfoRepository : IRepository<MerchantSyncInfo, Guid>
    {
        Task<MerchantSyncInfoWithNavigationProperties> GetWithNavigationPropertiesAsync(
    Guid id,
    CancellationToken cancellationToken = default
);

        Task<List<MerchantSyncInfoWithNavigationProperties>> GetListWithNavigationPropertiesAsync(
            string filterText = null,
            string merchantEmail = null,
            Guid? merchantId = null,
            string sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default
        );

        Task<List<MerchantSyncInfo>> GetListAsync(
                    string filterText = null,
                    string merchantEmail = null,
                    string sorting = null,
                    int maxResultCount = int.MaxValue,
                    int skipCount = 0,
                    CancellationToken cancellationToken = default
                );

        Task<long> GetCountAsync(
            string filterText = null,
            string merchantEmail = null,
            Guid? merchantId = null,
            CancellationToken cancellationToken = default);
    }
}