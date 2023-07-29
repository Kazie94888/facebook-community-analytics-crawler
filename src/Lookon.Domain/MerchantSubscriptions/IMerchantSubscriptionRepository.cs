using LookOn.Enums;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace LookOn.MerchantSubscriptions
{
    public interface IMerchantSubscriptionRepository : IRepository<MerchantSubscription, Guid>
    {
        Task<MerchantSubscriptionWithNavigationProperties> GetWithNavigationPropertiesAsync(
    Guid id,
    CancellationToken cancellationToken = default
);

        Task<List<MerchantSubscriptionWithNavigationProperties>> GetListWithNavigationPropertiesAsync(
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
            CancellationToken cancellationToken = default
        );

        Task<List<MerchantSubscription>> GetListAsync(
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
                    CancellationToken cancellationToken = default
                );

        Task<long> GetCountAsync(
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
            CancellationToken cancellationToken = default);
    }
}