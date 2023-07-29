using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;
using LookOn.Core.Extensions;
using LookOn.MongoDB;
using LookOn.Users;
using Volo.Abp.Domain.Repositories.MongoDB;
using Volo.Abp.MongoDB;
using MongoDB.Driver.Linq;
using MongoDB.Driver;

namespace LookOn.MerchantUsers
{
    public class MongoMerchantUserRepository : MongoDbRepository<LookOnMongoDbContext, MerchantUser, Guid>, IMerchantUserRepository
    {
        public MongoMerchantUserRepository(IMongoDbContextProvider<LookOnMongoDbContext> dbContextProvider) : base(dbContextProvider)
        {
        }

        public async Task<MerchantUserWithNavigationProperties> GetWithNavigationPropertiesAsync(
            Guid              id,
            CancellationToken cancellationToken = default)
        {
            var merchantUser =
                await (await GetMongoQueryableAsync(cancellationToken)).FirstOrDefaultAsync(e => e.Id == id, GetCancellationToken(cancellationToken));

            var appUser = await (await GetDbContextAsync(cancellationToken)).Users.AsQueryable()
                                                                            .FirstOrDefaultAsync(e => e.Id == merchantUser.AppUserId,
                                                                                                 cancellationToken: cancellationToken);
            var merchant = await (await GetDbContextAsync(cancellationToken)).Merchants.AsQueryable()
                                                                             .FirstOrDefaultAsync(e => e.Id == merchantUser.MerchantId,
                                                                                                  cancellationToken: cancellationToken);

            return new MerchantUserWithNavigationProperties {MerchantUser = merchantUser, AppUser = appUser, Merchant = merchant,};
        }

        public async Task<List<MerchantUserWithNavigationProperties>> GetListWithNavigationPropertiesAsync(
            string            filterText        = null,
            bool?             isActive          = null,
            Guid?             appUserId         = null,
            Guid?             merchantId        = null,
            string            sorting           = null,
            int               maxResultCount    = int.MaxValue,
            int               skipCount         = 0,
            CancellationToken cancellationToken = default)
        {
            var dbContext = await GetDbContextAsync(cancellationToken);
            var query = ApplyFilter((await GetMongoQueryableAsync(cancellationToken)),
                                    dbContext.Users.AsQueryable(),
                                    filterText,
                                    isActive,
                                    appUserId,
                                    merchantId);
            var merchantUsers =
                await query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? MerchantUserConsts.GetDefaultSorting(false) : sorting.Split('.').Last())
                           .As<IMongoQueryable<MerchantUser>>()
                           .PageBy<MerchantUser, IMongoQueryable<MerchantUser>>(skipCount, maxResultCount)
                           .ToListAsync(GetCancellationToken(cancellationToken));

            return merchantUsers.Select(s => new MerchantUserWithNavigationProperties
                                 {
                                     MerchantUser = s,
                                     AppUser      = dbContext.Users.AsQueryable().FirstOrDefault(e => e.Id     == s.AppUserId),
                                     Merchant     = dbContext.Merchants.AsQueryable().FirstOrDefault(e => e.Id == s.MerchantId),
                                 })
                                .ToList();
        }

        public async Task<List<MerchantUser>> GetListAsync(string            filterText        = null,
                                                           bool?             isActive          = null,
                                                           string            sorting           = null,
                                                           int               maxResultCount    = int.MaxValue,
                                                           int               skipCount         = 0,
                                                           CancellationToken cancellationToken = default)
        {
            var dbContext = await GetDbContextAsync(cancellationToken);
            var query = ApplyFilter((await GetMongoQueryableAsync(cancellationToken)),
                                    dbContext.Users.AsQueryable(),
                                    filterText,
                                    isActive);
            query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? MerchantUserConsts.GetDefaultSorting(false) : sorting);
            return await query.As<IMongoQueryable<MerchantUser>>()
                              .PageBy<MerchantUser, IMongoQueryable<MerchantUser>>(skipCount, maxResultCount)
                              .ToListAsync(GetCancellationToken(cancellationToken));
        }

        public async Task<long> GetCountAsync(string            filterText        = null,
                                              bool?             isActive          = null,
                                              Guid?             appUserId         = null,
                                              Guid?             merchantId        = null,
                                              CancellationToken cancellationToken = default)
        {
            var dbContext = await GetDbContextAsync(cancellationToken);
            var query = ApplyFilter((await GetMongoQueryableAsync(cancellationToken)),
                                    dbContext.Users.AsQueryable(),
                                    filterText,
                                    isActive,
                                    appUserId,
                                    merchantId);
            return await query.As<IMongoQueryable<MerchantUser>>().LongCountAsync(GetCancellationToken(cancellationToken));
        }

        protected virtual IQueryable<MerchantUser> ApplyFilter(IQueryable<MerchantUser> query,
                                                               IQueryable<AppUser>      appUsers,
                                                               string                   filterText,
                                                               bool?                    isActive   = null,
                                                               Guid?                    appUserId  = null,
                                                               Guid?                    merchantId = null)
        {
            query = query.WhereIf(isActive.HasValue, e => e.IsActive == isActive)
                         .WhereIf(appUserId                          != null && appUserId  != Guid.Empty, e => e.AppUserId  == appUserId)
                         .WhereIf(merchantId                         != null && merchantId != Guid.Empty, e => e.MerchantId == merchantId);

            if (filterText.IsNotNullOrEmpty())
            {
                var matchUserIds = appUsers
                                  .WhereIf(!string.IsNullOrWhiteSpace(filterText),
                                           e => e.Email.Contains(filterText) || e.UserName.Contains(filterText))
                                  .Select(x => x.Id)
                                  .ToList();
                query = query.WhereIf(matchUserIds.IsNotNullOrEmpty(), e => matchUserIds.Contains(e.AppUserId));
            }
            else
            {
                var matchUserIds = appUsers.Select(x => x.Id).ToList();
                query = query.WhereIf(matchUserIds.IsNotNullOrEmpty(), e => matchUserIds.Contains(e.AppUserId));
            }

            return query;
        }
    }
}