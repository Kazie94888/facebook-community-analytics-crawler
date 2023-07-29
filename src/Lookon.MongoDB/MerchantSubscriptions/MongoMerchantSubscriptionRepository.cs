using LookOn.Enums;
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

namespace LookOn.MerchantSubscriptions
{
    public class MongoMerchantSubscriptionRepository : MongoDbRepository<LookOnMongoDbContext, MerchantSubscription, Guid>, IMerchantSubscriptionRepository
    {
        public MongoMerchantSubscriptionRepository(IMongoDbContextProvider<LookOnMongoDbContext> dbContextProvider)
            : base(dbContextProvider)
        {
        }

        public async Task<MerchantSubscriptionWithNavigationProperties> GetWithNavigationPropertiesAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var merchantSubscription = await (await GetMongoQueryableAsync(cancellationToken))
                .FirstOrDefaultAsync(e => e.Id == id, GetCancellationToken(cancellationToken));

            var merchant = await (await GetDbContextAsync(cancellationToken)).Merchants.AsQueryable().FirstOrDefaultAsync(e => e.Id == merchantSubscription.MerchantId, cancellationToken: cancellationToken);

            return new MerchantSubscriptionWithNavigationProperties
            {
                MerchantSubscription = merchantSubscription,
                Merchant = merchant,

            };
        }

        public async Task<List<MerchantSubscriptionWithNavigationProperties>> GetListWithNavigationPropertiesAsync(
            string filterText = null,
            DateTime? startDateTimeMin = null,
            DateTime? startDateTimeMax = null,
            DateTime? endDateTimeMin = null,
            DateTime? endDateTimeMax = null,
            SubscriptionType? subscriptionType = null,
            SubscriptionStatus? subscriptionStatus = null,
            DateTime? notificationDateMin = null,
            DateTime? notificationDateMax = null,
            bool? notificationSent = null,
            DateTime? notificationSentAtMin = null,
            DateTime? notificationSentAtMax = null,
            Guid? merchantId = null,
            string sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter((await GetMongoQueryableAsync(cancellationToken)), filterText, startDateTimeMin, startDateTimeMax, endDateTimeMin, endDateTimeMax, subscriptionType, subscriptionStatus, notificationDateMin, notificationDateMax, notificationSent, notificationSentAtMin, notificationSentAtMax, merchantId);
            var merchantSubscriptions = await query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? MerchantSubscriptionConsts.GetDefaultSorting(false) : sorting.Split('.').Last())
                .As<IMongoQueryable<MerchantSubscription>>()
                .PageBy<MerchantSubscription, IMongoQueryable<MerchantSubscription>>(skipCount, maxResultCount)
                .ToListAsync(GetCancellationToken(cancellationToken));

            var dbContext = await GetDbContextAsync(cancellationToken);
            return merchantSubscriptions.Select(s => new MerchantSubscriptionWithNavigationProperties
            {
                MerchantSubscription = s,
                Merchant = dbContext.Merchants.AsQueryable().FirstOrDefault(e => e.Id == s.MerchantId),

            }).ToList();
        }

        public async Task<List<MerchantSubscription>> GetListAsync(
            string filterText = null,
            DateTime? startDateTimeMin = null,
            DateTime? startDateTimeMax = null,
            DateTime? endDateTimeMin = null,
            DateTime? endDateTimeMax = null,
            SubscriptionType? subscriptionType = null,
            SubscriptionStatus? subscriptionStatus = null,
            DateTime? notificationDateMin = null,
            DateTime? notificationDateMax = null,
            bool? notificationSent = null,
            DateTime? notificationSentAtMin = null,
            DateTime? notificationSentAtMax = null,
            string sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter((await GetMongoQueryableAsync(cancellationToken)), filterText, startDateTimeMin, startDateTimeMax, endDateTimeMin, endDateTimeMax, subscriptionType, subscriptionStatus, notificationDateMin, notificationDateMax, notificationSent, notificationSentAtMin, notificationSentAtMax);
            query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? MerchantSubscriptionConsts.GetDefaultSorting(false) : sorting);
            return await query.As<IMongoQueryable<MerchantSubscription>>()
                .PageBy<MerchantSubscription, IMongoQueryable<MerchantSubscription>>(skipCount, maxResultCount)
                .ToListAsync(GetCancellationToken(cancellationToken));
        }

        public async Task<long> GetCountAsync(
           string filterText = null,
           DateTime? startDateTimeMin = null,
           DateTime? startDateTimeMax = null,
           DateTime? endDateTimeMin = null,
           DateTime? endDateTimeMax = null,
           SubscriptionType? subscriptionType = null,
           SubscriptionStatus? subscriptionStatus = null,
           DateTime? notificationDateMin = null,
           DateTime? notificationDateMax = null,
           bool? notificationSent = null,
           DateTime? notificationSentAtMin = null,
           DateTime? notificationSentAtMax = null,
           Guid? merchantId = null,
           CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter((await GetMongoQueryableAsync(cancellationToken)), filterText, startDateTimeMin, startDateTimeMax, endDateTimeMin, endDateTimeMax, subscriptionType, subscriptionStatus, notificationDateMin, notificationDateMax, notificationSent, notificationSentAtMin, notificationSentAtMax, merchantId);
            return await query.As<IMongoQueryable<MerchantSubscription>>().LongCountAsync(GetCancellationToken(cancellationToken));
        }

        protected virtual IQueryable<MerchantSubscription> ApplyFilter(
            IQueryable<MerchantSubscription> query,
            string filterText,
            DateTime? startDateTimeMin = null,
            DateTime? startDateTimeMax = null,
            DateTime? endDateTimeMin = null,
            DateTime? endDateTimeMax = null,
            SubscriptionType? subscriptionType = null,
            SubscriptionStatus? subscriptionStatus = null,
            DateTime? notificationDateMin = null,
            DateTime? notificationDateMax = null,
            bool? notificationSent = null,
            DateTime? notificationSentAtMin = null,
            DateTime? notificationSentAtMax = null,
            Guid? merchantId = null)
        {
            return query
                .WhereIf(!string.IsNullOrWhiteSpace(filterText), e => true)
                    .WhereIf(startDateTimeMin.HasValue, e => e.StartDateTime >= startDateTimeMin.Value)
                    .WhereIf(startDateTimeMax.HasValue, e => e.StartDateTime <= startDateTimeMax.Value)
                    .WhereIf(endDateTimeMin.HasValue, e => e.EndDateTime >= endDateTimeMin.Value)
                    .WhereIf(endDateTimeMax.HasValue, e => e.EndDateTime <= endDateTimeMax.Value)
                    .WhereIf(subscriptionType.HasValue, e => e.SubscriptionType == subscriptionType)
                    .WhereIf(subscriptionStatus.HasValue, e => e.SubscriptionStatus == subscriptionStatus)
                    .WhereIf(notificationDateMin.HasValue, e => e.NotificationDate >= notificationDateMin.Value)
                    .WhereIf(notificationDateMax.HasValue, e => e.NotificationDate <= notificationDateMax.Value)
                    .WhereIf(notificationSent.HasValue, e => e.NotificationSent == notificationSent)
                    .WhereIf(notificationSentAtMin.HasValue, e => e.NotificationSentAt >= notificationSentAtMin.Value)
                    .WhereIf(notificationSentAtMax.HasValue, e => e.NotificationSentAt <= notificationSentAtMax.Value)
                    .WhereIf(merchantId != null && merchantId != Guid.Empty, e => e.MerchantId == merchantId);
        }
    }
}