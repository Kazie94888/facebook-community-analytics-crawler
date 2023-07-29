using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace LookOn.Categories
{
    public interface ICategoryRepository : IRepository<Category, Guid>
    {
        Task<List<Category>> GetListAsync(
            string filterText = null,
            string name = null,
            string code = null,
            string description = null,
            int? orderMin = null,
            int? orderMax = null,
            string sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default
        );

        Task<long> GetCountAsync(
            string filterText = null,
            string name = null,
            string code = null,
            string description = null,
            int? orderMin = null,
            int? orderMax = null,
            CancellationToken cancellationToken = default);
    }
}