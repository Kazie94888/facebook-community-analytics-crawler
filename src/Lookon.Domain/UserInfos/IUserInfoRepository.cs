using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace LookOn.UserInfos
{
    public interface IUserInfoRepository : IRepository<UserInfo, Guid>
    {
        Task<UserInfoWithNavigationProperties> GetWithNavigationPropertiesAsync(
    Guid id,
    CancellationToken cancellationToken = default
);

        Task<List<UserInfoWithNavigationProperties>> GetListWithNavigationPropertiesAsync(
            string filterText = null,
            string identificationNumber = null,
            Guid? appUserId = null,
            string sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default
        );

        Task<List<UserInfo>> GetListAsync(
                    string filterText = null,
                    string identificationNumber = null,
                    string sorting = null,
                    int maxResultCount = int.MaxValue,
                    int skipCount = 0,
                    CancellationToken cancellationToken = default
                );

        Task<long> GetCountAsync(
            string filterText = null,
            string identificationNumber = null,
            Guid? appUserId = null,
            CancellationToken cancellationToken = default);
    }
}