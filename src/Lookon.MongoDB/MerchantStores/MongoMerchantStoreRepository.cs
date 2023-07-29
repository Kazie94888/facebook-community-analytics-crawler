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

namespace LookOn.MerchantStores
{
    public class MongoMerchantStoreRepository : MongoDbRepository<LookOnMongoDbContext, MerchantStore, Guid>, IMerchantStoreRepository
    {
        public MongoMerchantStoreRepository(IMongoDbContextProvider<LookOnMongoDbContext> dbContextProvider)
            : base(dbContextProvider)
        {
        }

        public async Task<MerchantStoreWithNavigationProperties> GetWithNavigationPropertiesAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var merchantStore = await (await GetMongoQueryableAsync(cancellationToken))
                .FirstOrDefaultAsync(e => e.Id == id, GetCancellationToken(cancellationToken));

            var merchant = await (await GetDbContextAsync(cancellationToken)).Merchants.AsQueryable().FirstOrDefaultAsync(e => e.Id == merchantStore.MerchantId, cancellationToken: cancellationToken);
            var platform = await (await GetDbContextAsync(cancellationToken)).Platforms.AsQueryable().FirstOrDefaultAsync(e => e.Id == merchantStore.PlatformId, cancellationToken: cancellationToken);

            return new MerchantStoreWithNavigationProperties
            {
                MerchantStore = merchantStore,
                Merchant = merchant,
                Platform = platform,

            };
        }

        public async Task<List<MerchantStoreWithNavigationProperties>> GetListWithNavigationPropertiesAsync(
            string filterText = null,
            string name = null,
            string code = null,
            bool? active = null,
            Guid? merchantId = null,
            Guid? platformId = null,
            string sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter((await GetMongoQueryableAsync(cancellationToken)), filterText, name, code, active, merchantId, platformId);
            var merchantStores = await query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? MerchantStoreConsts.GetDefaultSorting(false) : sorting.Split('.').Last())
                .As<IMongoQueryable<MerchantStore>>()
                .PageBy<MerchantStore, IMongoQueryable<MerchantStore>>(skipCount, maxResultCount)
                .ToListAsync(GetCancellationToken(cancellationToken));

            var dbContext = await GetDbContextAsync(cancellationToken);
            return merchantStores.Select(s => new MerchantStoreWithNavigationProperties
            {
                MerchantStore = s,
                Merchant = dbContext.Merchants.AsQueryable().FirstOrDefault(e => e.Id == s.MerchantId),
                Platform = dbContext.Platforms.AsQueryable().FirstOrDefault(e => e.Id == s.PlatformId),

            }).ToList();
        }

        public async Task<List<MerchantStore>> GetListAsync(
            string filterText = null,
            string name = null,
            string code = null,
            bool? active = null,
            string sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter((await GetMongoQueryableAsync(cancellationToken)), filterText, name, code, active);
            query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? MerchantStoreConsts.GetDefaultSorting(false) : sorting);
            return await query.As<IMongoQueryable<MerchantStore>>()
                .PageBy<MerchantStore, IMongoQueryable<MerchantStore>>(skipCount, maxResultCount)
                .ToListAsync(GetCancellationToken(cancellationToken));
        }

        public async Task<long> GetCountAsync(
           string filterText = null,
           string name = null,
           string code = null,
           bool? active = null,
           Guid? merchantId = null,
           Guid? platformId = null,
           CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter((await GetMongoQueryableAsync(cancellationToken)), filterText, name, code, active, merchantId, platformId);
            return await query.As<IMongoQueryable<MerchantStore>>().LongCountAsync(GetCancellationToken(cancellationToken));
        }

        protected virtual IQueryable<MerchantStore> ApplyFilter(
            IQueryable<MerchantStore> query,
            string filterText,
            string name = null,
            string code = null,
            bool? active = null,
            Guid? merchantId = null,
            Guid? platformId = null)
        {
            return query
                .WhereIf(!string.IsNullOrWhiteSpace(filterText), e => e.Name.Contains(filterText) || e.Code.Contains(filterText))
                    .WhereIf(!string.IsNullOrWhiteSpace(name), e => e.Name.Contains(name))
                    .WhereIf(!string.IsNullOrWhiteSpace(code), e => e.Code.Contains(code))
                    .WhereIf(active.HasValue, e => e.Active == active)
                    .WhereIf(merchantId != null && merchantId != Guid.Empty, e => e.MerchantId == merchantId)
                    .WhereIf(platformId != null && platformId != Guid.Empty, e => e.PlatformId == platformId);
        }
    }
}