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

namespace LookOn.Merchants
{
    public class MongoMerchantRepository : MongoDbRepository<LookOnMongoDbContext, Merchant, Guid>, IMerchantRepository
    {
        public MongoMerchantRepository(IMongoDbContextProvider<LookOnMongoDbContext> dbContextProvider)
            : base(dbContextProvider)
        {
        }

        public async Task<MerchantWithNavigationProperties> GetWithNavigationPropertiesAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var merchant = await (await GetMongoQueryableAsync(cancellationToken))
                .FirstOrDefaultAsync(e => e.Id == id, GetCancellationToken(cancellationToken));

            var appUser = await (await GetDbContextAsync(cancellationToken)).Users.AsQueryable().FirstOrDefaultAsync(e => e.Id == merchant.OwnerAppUserId, cancellationToken: cancellationToken);
            var category = await (await GetDbContextAsync(cancellationToken)).Categories.AsQueryable().FirstOrDefaultAsync(e => e.Id == merchant.CategoryId, cancellationToken: cancellationToken);

            return new MerchantWithNavigationProperties
            {
                Merchant = merchant,
                AppUser = appUser,
                Category = category,

            };
        }

        public async Task<List<MerchantWithNavigationProperties>> GetListWithNavigationPropertiesAsync(
            string filterText = null,
            string name = null,
            string phone = null,
            string address = null,
            string email = null,
            string fax = null,
            Guid? ownerAppUserId = null,
            Guid? categoryId = null,
            string sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter((await GetMongoQueryableAsync(cancellationToken)), filterText, name, phone, address, email, fax, ownerAppUserId, categoryId);
            var merchants = await query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? MerchantConsts.GetDefaultSorting(false) : sorting.Split('.').Last())
                .As<IMongoQueryable<Merchant>>()
                .PageBy<Merchant, IMongoQueryable<Merchant>>(skipCount, maxResultCount)
                .ToListAsync(GetCancellationToken(cancellationToken));

            var dbContext = await GetDbContextAsync(cancellationToken);
            return merchants.Select(s => new MerchantWithNavigationProperties
            {
                Merchant = s,
                AppUser = dbContext.Users.AsQueryable().FirstOrDefault(e => e.Id == s.OwnerAppUserId),
                Category = dbContext.Categories.AsQueryable().FirstOrDefault(e => e.Id == s.CategoryId),

            }).ToList();
        }

        public async Task<List<Merchant>> GetListAsync(
            string filterText = null,
            string name = null,
            string phone = null,
            string address = null,
            string email = null,
            string fax = null,
            string sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter((await GetMongoQueryableAsync(cancellationToken)), filterText, name, phone, address, email, fax);
            query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? MerchantConsts.GetDefaultSorting(false) : sorting);
            return await query.As<IMongoQueryable<Merchant>>()
                .PageBy<Merchant, IMongoQueryable<Merchant>>(skipCount, maxResultCount)
                .ToListAsync(GetCancellationToken(cancellationToken));
        }

        public async Task<long> GetCountAsync(
           string filterText = null,
           string name = null,
           string phone = null,
           string address = null,
           string email = null,
           string fax = null,
           Guid? ownerAppUserId = null,
           Guid? categoryId = null,
           CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter((await GetMongoQueryableAsync(cancellationToken)), filterText, name, phone, address, email, fax, ownerAppUserId, categoryId);
            return await query.As<IMongoQueryable<Merchant>>().LongCountAsync(GetCancellationToken(cancellationToken));
        }

        protected virtual IQueryable<Merchant> ApplyFilter(
            IQueryable<Merchant> query,
            string filterText,
            string name = null,
            string phone = null,
            string address = null,
            string email = null,
            string fax = null,
            Guid? ownerAppUserId = null,
            Guid? categoryId = null)
        {
            return query
                .WhereIf(!string.IsNullOrWhiteSpace(filterText), e => e.Name.Contains(filterText) || e.Phone.Contains(filterText) || e.Address.Contains(filterText) || e.Email.Contains(filterText) || e.Fax.Contains(filterText))
                    .WhereIf(!string.IsNullOrWhiteSpace(name), e => e.Name.Contains(name))
                    .WhereIf(!string.IsNullOrWhiteSpace(phone), e => e.Phone.Contains(phone))
                    .WhereIf(!string.IsNullOrWhiteSpace(address), e => e.Address.Contains(address))
                    .WhereIf(!string.IsNullOrWhiteSpace(email), e => e.Email.Contains(email))
                    .WhereIf(!string.IsNullOrWhiteSpace(fax), e => e.Fax.Contains(fax))
                    .WhereIf(ownerAppUserId != null && ownerAppUserId != Guid.Empty, e => e.OwnerAppUserId == ownerAppUserId)
                    .WhereIf(categoryId != null && categoryId != Guid.Empty, e => e.CategoryId == categoryId);
        }
    }
}