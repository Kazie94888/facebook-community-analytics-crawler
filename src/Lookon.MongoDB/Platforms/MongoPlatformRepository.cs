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

namespace LookOn.Platforms
{
    public class MongoPlatformRepository : MongoDbRepository<LookOnMongoDbContext, Platform, Guid>, IPlatformRepository
    {
        public MongoPlatformRepository(IMongoDbContextProvider<LookOnMongoDbContext> dbContextProvider)
            : base(dbContextProvider)
        {
        }

        public async Task<List<Platform>> GetListAsync(
            string filterText = null,
            string name = null,
            string description = null,
            string url = null,
            string logoUrl = null,
            string sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter((await GetMongoQueryableAsync(cancellationToken)), filterText, name, description, url, logoUrl);
            query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? PlatformConsts.GetDefaultSorting(false) : sorting);
            return await query.As<IMongoQueryable<Platform>>()
                .PageBy<Platform, IMongoQueryable<Platform>>(skipCount, maxResultCount)
                .ToListAsync(GetCancellationToken(cancellationToken));
        }

        public async Task<long> GetCountAsync(
           string filterText = null,
           string name = null,
           string description = null,
           string url = null,
           string logoUrl = null,
           CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter((await GetMongoQueryableAsync(cancellationToken)), filterText, name, description, url, logoUrl);
            return await query.As<IMongoQueryable<Platform>>().LongCountAsync(GetCancellationToken(cancellationToken));
        }

        protected virtual IQueryable<Platform> ApplyFilter(
            IQueryable<Platform> query,
            string filterText,
            string name = null,
            string description = null,
            string url = null,
            string logoUrl = null)
        {
            return query
                .WhereIf(!string.IsNullOrWhiteSpace(filterText), e => e.Name.Contains(filterText) || e.Description.Contains(filterText) || e.Url.Contains(filterText) || e.LogoUrl.Contains(filterText))
                    .WhereIf(!string.IsNullOrWhiteSpace(name), e => e.Name.Contains(name))
                    .WhereIf(!string.IsNullOrWhiteSpace(description), e => e.Description.Contains(description))
                    .WhereIf(!string.IsNullOrWhiteSpace(url), e => e.Url.Contains(url))
                    .WhereIf(!string.IsNullOrWhiteSpace(logoUrl), e => e.LogoUrl.Contains(logoUrl));
        }
    }
}