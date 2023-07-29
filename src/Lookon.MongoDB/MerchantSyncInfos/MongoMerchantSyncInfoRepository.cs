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

namespace LookOn.MerchantSyncInfos
{
    public class MongoMerchantSyncInfoRepository : MongoDbRepository<LookOnMongoDbContext, MerchantSyncInfo, Guid>, IMerchantSyncInfoRepository
    {
        public MongoMerchantSyncInfoRepository(IMongoDbContextProvider<LookOnMongoDbContext> dbContextProvider)
            : base(dbContextProvider)
        {
        }

        public async Task<MerchantSyncInfoWithNavigationProperties> GetWithNavigationPropertiesAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var merchantSyncInfo = await (await GetMongoQueryableAsync(cancellationToken))
                .FirstOrDefaultAsync(e => e.Id == id, GetCancellationToken(cancellationToken));

            var merchant = await (await GetDbContextAsync(cancellationToken)).Merchants.AsQueryable().FirstOrDefaultAsync(e => e.Id == merchantSyncInfo.MerchantId, cancellationToken: cancellationToken);

            return new MerchantSyncInfoWithNavigationProperties
            {
                MerchantSyncInfo = merchantSyncInfo,
                Merchant = merchant,

            };
        }

        public async Task<List<MerchantSyncInfoWithNavigationProperties>> GetListWithNavigationPropertiesAsync(
            string filterText = null,
            string merchantEmail = null,
            Guid? merchantId = null,
            string sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter((await GetMongoQueryableAsync(cancellationToken)), filterText, merchantEmail, merchantId);
            var merchantSyncInfos = await query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? MerchantSyncInfoConsts.GetDefaultSorting(false) : sorting.Split('.').Last())
                .As<IMongoQueryable<MerchantSyncInfo>>()
                .PageBy<MerchantSyncInfo, IMongoQueryable<MerchantSyncInfo>>(skipCount, maxResultCount)
                .ToListAsync(GetCancellationToken(cancellationToken));

            var dbContext = await GetDbContextAsync(cancellationToken);
            return merchantSyncInfos.Select(s => new MerchantSyncInfoWithNavigationProperties
            {
                MerchantSyncInfo = s,
                Merchant = dbContext.Merchants.AsQueryable().FirstOrDefault(e => e.Id == s.MerchantId),

            }).ToList();
        }

        public async Task<List<MerchantSyncInfo>> GetListAsync(
            string filterText = null,
            string merchantEmail = null,
            string sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter((await GetMongoQueryableAsync(cancellationToken)), filterText, merchantEmail);
            query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? MerchantSyncInfoConsts.GetDefaultSorting(false) : sorting);
            return await query.As<IMongoQueryable<MerchantSyncInfo>>()
                .PageBy<MerchantSyncInfo, IMongoQueryable<MerchantSyncInfo>>(skipCount, maxResultCount)
                .ToListAsync(GetCancellationToken(cancellationToken));
        }

        public async Task<long> GetCountAsync(
           string filterText = null,
           string merchantEmail = null,
           Guid? merchantId = null,
           CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter((await GetMongoQueryableAsync(cancellationToken)), filterText, merchantEmail, merchantId);
            return await query.As<IMongoQueryable<MerchantSyncInfo>>().LongCountAsync(GetCancellationToken(cancellationToken));
        }

        protected virtual IQueryable<MerchantSyncInfo> ApplyFilter(
            IQueryable<MerchantSyncInfo> query,
            string filterText,
            string merchantEmail = null,
            Guid? merchantId = null)
        {
            return query
                .WhereIf(!string.IsNullOrWhiteSpace(filterText), e => e.MerchantEmail.Contains(filterText))
                    .WhereIf(!string.IsNullOrWhiteSpace(merchantEmail), e => e.MerchantEmail.Contains(merchantEmail))
                    .WhereIf(merchantId != null && merchantId != Guid.Empty, e => e.MerchantId == merchantId);
        }
    }
}