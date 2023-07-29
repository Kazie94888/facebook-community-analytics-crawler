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

namespace LookOn.UserInfos
{
    public class MongoUserInfoRepository : MongoDbRepository<LookOnMongoDbContext, UserInfo, Guid>, IUserInfoRepository
    {
        public MongoUserInfoRepository(IMongoDbContextProvider<LookOnMongoDbContext> dbContextProvider)
            : base(dbContextProvider)
        {
        }

        public async Task<UserInfoWithNavigationProperties> GetWithNavigationPropertiesAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var userInfo = await (await GetMongoQueryableAsync(cancellationToken))
                .FirstOrDefaultAsync(e => e.Id == id, GetCancellationToken(cancellationToken));

            var appUser = await (await GetDbContextAsync(cancellationToken)).Users.AsQueryable().FirstOrDefaultAsync(e => e.Id == userInfo.AppUserId, cancellationToken: cancellationToken);

            return new UserInfoWithNavigationProperties
            {
                UserInfo = userInfo,
                AppUser = appUser,

            };
        }

        public async Task<List<UserInfoWithNavigationProperties>> GetListWithNavigationPropertiesAsync(
            string filterText = null,
            string identificationNumber = null,
            Guid? appUserId = null,
            string sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter((await GetMongoQueryableAsync(cancellationToken)), filterText, identificationNumber, appUserId);
            var userInfos = await query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? UserInfoConsts.GetDefaultSorting(false) : sorting.Split('.').Last())
                .As<IMongoQueryable<UserInfo>>()
                .PageBy<UserInfo, IMongoQueryable<UserInfo>>(skipCount, maxResultCount)
                .ToListAsync(GetCancellationToken(cancellationToken));

            var dbContext = await GetDbContextAsync(cancellationToken);
            return userInfos.Select(s => new UserInfoWithNavigationProperties
            {
                UserInfo = s,
                AppUser = dbContext.Users.AsQueryable().FirstOrDefault(e => e.Id == s.AppUserId),

            }).ToList();
        }

        public async Task<List<UserInfo>> GetListAsync(
            string filterText = null,
            string identificationNumber = null,
            string sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter((await GetMongoQueryableAsync(cancellationToken)), filterText, identificationNumber);
            query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? UserInfoConsts.GetDefaultSorting(false) : sorting);
            return await query.As<IMongoQueryable<UserInfo>>()
                .PageBy<UserInfo, IMongoQueryable<UserInfo>>(skipCount, maxResultCount)
                .ToListAsync(GetCancellationToken(cancellationToken));
        }

        public async Task<long> GetCountAsync(
           string filterText = null,
           string identificationNumber = null,
           Guid? appUserId = null,
           CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter((await GetMongoQueryableAsync(cancellationToken)), filterText, identificationNumber, appUserId);
            return await query.As<IMongoQueryable<UserInfo>>().LongCountAsync(GetCancellationToken(cancellationToken));
        }

        protected virtual IQueryable<UserInfo> ApplyFilter(
            IQueryable<UserInfo> query,
            string filterText,
            string identificationNumber = null,
            Guid? appUserId = null)
        {
            return query
                .WhereIf(!string.IsNullOrWhiteSpace(filterText), e => e.IdentificationNumber.Contains(filterText))
                    .WhereIf(!string.IsNullOrWhiteSpace(identificationNumber), e => e.IdentificationNumber.Contains(identificationNumber))
                    .WhereIf(appUserId != null && appUserId != Guid.Empty, e => e.AppUserId == appUserId);
        }
    }
}