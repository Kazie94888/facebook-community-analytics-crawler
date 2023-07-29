using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace LookOn.MerchantUsers
{
    public interface IMerchantUserRepository : IRepository<MerchantUser, Guid>
    {
        Task<MerchantUserWithNavigationProperties> GetWithNavigationPropertiesAsync(
    Guid id,
    CancellationToken cancellationToken = default
);

        Task<List<MerchantUserWithNavigationProperties>> GetListWithNavigationPropertiesAsync(
            string filterText = null,
            bool? isActive = null,
            Guid? appUserId = null,
            Guid? merchantId = null,
            string sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default
        );

        Task<List<MerchantUser>> GetListAsync(
                    string filterText = null,
                    bool? isActive = null,
                    string sorting = null,
                    int maxResultCount = int.MaxValue,
                    int skipCount = 0,
                    CancellationToken cancellationToken = default
                );

        Task<long> GetCountAsync(
            string filterText = null,
            bool? isActive = null,
            Guid? appUserId = null,
            Guid? merchantId = null,
            CancellationToken cancellationToken = default);
    }
}