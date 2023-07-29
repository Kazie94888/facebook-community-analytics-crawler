using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace LookOn.Platforms
{
    public interface IPlatformRepository : IRepository<Platform, Guid>
    {
        Task<List<Platform>> GetListAsync(
            string filterText = null,
            string name = null,
            string description = null,
            string url = null,
            string logoUrl = null,
            string sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default
        );

        Task<long> GetCountAsync(
            string filterText = null,
            string name = null,
            string description = null,
            string url = null,
            string logoUrl = null,
            CancellationToken cancellationToken = default);
    }
}