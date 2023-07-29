using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;
using LookOn.MongoDB;
using Volo.Abp.Domain.Repositories.MongoDB;
using Volo.Abp.MongoDB;
using MongoDB.Driver.Linq;
using MongoDB.Driver;

namespace LookOn.Categories
{
    public class MongoCategoryRepository : MongoDbRepository<LookOnMongoDbContext, Category, Guid>, ICategoryRepository
    {
        public MongoCategoryRepository(IMongoDbContextProvider<LookOnMongoDbContext> dbContextProvider)
            : base(dbContextProvider)
        {
        }

        public async Task<List<Category>> GetListAsync(
            string filterText = null,
            string name = null,
            string code = null,
            string description = null,
            int? orderMin = null,
            int? orderMax = null,
            string sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter((await GetMongoQueryableAsync(cancellationToken)), filterText, name, code, description, orderMin, orderMax);
            query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? CategoryConsts.GetDefaultSorting(false) : sorting);
            return await query.As<IMongoQueryable<Category>>()
                .PageBy<Category, IMongoQueryable<Category>>(skipCount, maxResultCount)
                .ToListAsync(GetCancellationToken(cancellationToken));
        }

        public async Task<long> GetCountAsync(
           string filterText = null,
           string name = null,
           string code = null,
           string description = null,
           int? orderMin = null,
           int? orderMax = null,
           CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter((await GetMongoQueryableAsync(cancellationToken)), filterText, name, code, description, orderMin, orderMax);
            return await query.As<IMongoQueryable<Category>>().LongCountAsync(GetCancellationToken(cancellationToken));
        }

        protected virtual IQueryable<Category> ApplyFilter(
            IQueryable<Category> query,
            string filterText,
            string name = null,
            string code = null,
            string description = null,
            int? orderMin = null,
            int? orderMax = null)
        {
            return query
                .WhereIf(!string.IsNullOrWhiteSpace(filterText), e => e.Name.Contains(filterText) || e.Code.Contains(filterText) || e.Description.Contains(filterText))
                    .WhereIf(!string.IsNullOrWhiteSpace(name), e => e.Name.Contains(name))
                    .WhereIf(!string.IsNullOrWhiteSpace(code), e => e.Code.Contains(code))
                    .WhereIf(!string.IsNullOrWhiteSpace(description), e => e.Description.Contains(description))
                    .WhereIf(orderMin.HasValue, e => e.Order >= orderMin.Value)
                    .WhereIf(orderMax.HasValue, e => e.Order <= orderMax.Value);
        }
    }
}