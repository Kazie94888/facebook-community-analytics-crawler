using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentDateTime;
using LookOn.Core.Extensions;
using LookOn.Emails;
using LookOn.Enums;
using LookOn.Merchants;
using LookOn.MerchantSyncInfos;
using Volo.Abp.Domain.Repositories;

namespace LookOn.MerchantSubscriptions;

public class MerchantSubscriptionManager : LookOnManager
{
    // repositories
    private readonly IMerchantRepository             _merchantRepo;
    private readonly IMerchantSubscriptionRepository _merchantSubscriptionRepo;
    private readonly IMerchantSyncInfoRepository     _merchantSyncInfoRepository;
    // managers
    private readonly EmailManager _emailManager;

    public MerchantSubscriptionManager(IMerchantRepository             merchantRepo,
                                       IMerchantSubscriptionRepository merchantSubscriptionRepo,
                                       EmailManager                    emailManager,
                                       IMerchantSyncInfoRepository     merchantSyncInfoRepository)
    {
        _merchantRepo                    = merchantRepo;
        _merchantSubscriptionRepo        = merchantSubscriptionRepo;
        _emailManager                    = emailManager;
        _merchantSyncInfoRepository = merchantSyncInfoRepository;
    }

    public async Task<List<Merchant>> GetActiveMerchants()
    {
        var merchants       = await _merchantRepo.GetListAsync();
        var activeMerchants = new List<Merchant>();
        foreach (var merchant in merchants)
        {
            var activeMerchantSubscription = await GetActiveSubscription(merchant.Id);
            if (activeMerchantSubscription is null) continue;

            activeMerchants.Add(merchant);
        }

        return activeMerchants;
    }
    // public async Task<List<Merchant>> GetMerchantsSyncSocial()
    // {
    //     var merchants       = await _merchantRepo.GetListAsync();
    //     var activeMerchants = new List<Merchant>();
    //     foreach (var merchant in merchants)
    //     {
    //         var activeMerchantSubscription = await GetActiveSubscription(merchant.Id);
    //         if (activeMerchantSubscription is null) continue;
    //         var merchantSyncInfo = await _merchantSyncInfoRepository.FirstOrDefaultAsync(x => x.MerchantId == merchant.Id);
    //         if (merchantSyncInfo != null)
    //         {
    //             var merchantSocialScan = merchantSyncInfo.SocialScan.UserScans.fio;
    //             if ()
    //             {
    //                 
    //             }
    //         }
    //         activeMerchants.Add(merchant);
    //     }
    //
    //     return activeMerchants;
    // }

    public SubscriptionType GetSubscriptionType(string haravanPlanId)
    {
        //Starter
        if (haravanPlanId.Equals("plan_app_6775_1041"))
        {
            return SubscriptionType.Starter;
        }
        //Growth
        if (haravanPlanId.Equals("plan_app_6775_1043"))
        {
            return SubscriptionType.Growth;
        }
        //Enterprise
        if (haravanPlanId.Equals("plan_app_6775_1045"))
        {
            return SubscriptionType.Enterprise;
        }

        return SubscriptionType.Trial;
    }
    public async Task<MerchantSubscription> GetActiveSubscription(Guid merchantId)
    {
       return await _merchantSubscriptionRepo.FirstOrDefaultAsync(m => m.MerchantId         == merchantId
                                                                     && m.SubscriptionStatus == SubscriptionStatus.Active);
        
    }

    public SubscriptionConfig InitSubscriptionConfig(SubscriptionType subscriptionType)
    {
        var subscription = new SubscriptionConfig {SubscriptionType = subscriptionType};
        switch (subscriptionType)
        {
            case SubscriptionType.Trial:
            case SubscriptionType.Starter:
            {
                subscription.MaxSocialUser                    = 5000;
                subscription.MaxSocialUserInsight             = 5000;
                subscription.MaxSocialUserInsightAddInPercent = 0;
                subscription.MaxSocialPage                    = 1;
                subscription.HasDailyReport                   = false;
                subscription.HasWeeklyReport                  = false;
                subscription.HasMonthlyReport                 = true;
                break;
            }

            case SubscriptionType.Growth:
            {
                subscription.MaxSocialUser                    = 20000;
                subscription.MaxSocialUserInsight             = 5000;
                subscription.MaxSocialUserInsightAddInPercent = 0.2m;
                subscription.MaxSocialPage                    = 2;
                subscription.HasDailyReport                   = false;
                subscription.HasWeeklyReport                  = true;
                subscription.HasMonthlyReport                 = true;
                break;
            }

            case SubscriptionType.Plus:
            {
                subscription.MaxSocialUser                    = 50000;
                subscription.MaxSocialUserInsight             = 10000;
                subscription.MaxSocialUserInsightAddInPercent = 0.4m;
                subscription.MaxSocialPage                    = 5;
                subscription.HasDailyReport                   = true;
                subscription.HasWeeklyReport                  = true;
                subscription.HasMonthlyReport                 = true;
                break;
            }
                
            case SubscriptionType.Enterprise:
            {
                subscription.MaxSocialUser                    = 200000;
                subscription.MaxSocialUserInsight             = 10000;
                subscription.MaxSocialUserInsightAddInPercent = 0.6m;
                subscription.MaxSocialPage                    = 100;
                subscription.HasDailyReport                   = true;
                subscription.HasWeeklyReport                  = true;
                subscription.HasMonthlyReport                 = true;
                break;
            }

            default: throw new ArgumentOutOfRangeException();
        }

        return subscription;
    }

    public async Task UpdateSubscriptionStatus(Guid merchantSubscriptionId, SubscriptionStatus subscriptionStatus)
    {
        var merchantSubscription = await _merchantSubscriptionRepo.GetAsync(merchantSubscriptionId);
        if (merchantSubscription is null) return;
        
        merchantSubscription.SubscriptionStatus = subscriptionStatus;
        await _merchantSubscriptionRepo.UpdateAsync(merchantSubscription);
    }

    public async Task SetSubscription(Guid merchantId, SubscriptionType subscriptionType, DateTime from)
    {
        var merchant = await _merchantRepo.GetAsync(merchantId);
        if (merchant is null) return;

        var                     currentActive = await GetActiveSubscription(merchantId);
        bool                    shouldActiveNewSubscription;
        DateTime                newSubscriptionDateTime;
        SubscriptionEmailStatus subEmailType;

        if (currentActive is not null)
        {
            var updateSubscription = currentActive.SubscriptionType.ToInt() < subscriptionType.ToInt();
            shouldActiveNewSubscription = currentActive.EndDateTime is not null && currentActive.EndDateTime.Value.IsAfter(from) || updateSubscription;

            // expiring old subscription if shouldActiveNewSubscription = true
            currentActive.SubscriptionStatus = shouldActiveNewSubscription ? SubscriptionStatus.Expired : SubscriptionStatus.Active;
            newSubscriptionDateTime          = updateSubscription ? DateTime.UtcNow : currentActive.EndDateTime ?? from;
            await _merchantSubscriptionRepo.UpdateAsync(currentActive);

            subEmailType = currentActive.SubscriptionType == subscriptionType ? SubscriptionEmailStatus.Extend : SubscriptionEmailStatus.Upgrade;
        }
        else
        {
            shouldActiveNewSubscription = from.IsBefore(DateTime.UtcNow.AddTicks(-1));
            newSubscriptionDateTime     = from;

            subEmailType = SubscriptionEmailStatus.Added;
        }

        var status = shouldActiveNewSubscription ? SubscriptionStatus.Active : SubscriptionStatus.Pending;
        var merchantSubscription = new MerchantSubscription
        {
            MerchantId         = merchant.Id,
            MerchantEmail      = merchant.Email,
            StartDateTime      = newSubscriptionDateTime,
            EndDateTime        = subscriptionType == SubscriptionType.Trial ? newSubscriptionDateTime.AddDays(7) : newSubscriptionDateTime.NextYear(),
            SubscriptionType   = subscriptionType,
            SubscriptionStatus = status,
            SubscriptionConfig = InitSubscriptionConfig(subscriptionType)
        };

        // save new subscription
        await _merchantSubscriptionRepo.InsertAsync(merchantSubscription);

        // send email to merchant
        await _emailManager.Send_SubNotification(merchant, subEmailType);
    }

    public async Task ScanSubscriptions()
    {
        // 1. expire the old ones
        var expiredSubs = await _merchantSubscriptionRepo.GetListAsync(_ => _.SubscriptionStatus == SubscriptionStatus.Active
                                                                         && _.EndDateTime        != null
                                                                         && _.EndDateTime        < DateTime.UtcNow);

        foreach (var sub in expiredSubs)
        {
            sub.SubscriptionStatus = SubscriptionStatus.Expired;
        }

        if (expiredSubs.IsNotNullOrEmpty()) await _merchantSubscriptionRepo.UpdateManyAsync(expiredSubs);

        // 2. active the new ones
        var subsToActive = await _merchantSubscriptionRepo.GetListAsync(_ => _.SubscriptionStatus == SubscriptionStatus.Pending
                                                                          && _.StartDateTime      != null
                                                                          && _.StartDateTime      >= DateTime.UtcNow);
        foreach (var sub in subsToActive)
        {
            sub.SubscriptionStatus = SubscriptionStatus.Active;
        }

        if (subsToActive.IsNotNullOrEmpty()) await _merchantSubscriptionRepo.UpdateManyAsync(subsToActive);

        // 3. check and send email if the subscription is expired, 
        await SendNotificationSubscriptionEmail();
    }

    /// <summary>
    /// the subscription is expired for next 30 days (cc support@lookon.vn)
    /// the subscription is expired for next 1 week (cc support@lookon.vn)
    /// the subscription is expired for next 1 day (cc support@lookon.vn)
    /// </summary>
    private async Task SendNotificationSubscriptionEmail()
    {
        // only check if the end date is the next 30 days
        var activeSubs = await _merchantSubscriptionRepo.GetListAsync(_ => _.SubscriptionStatus == SubscriptionStatus.Active
                                                                        && _.EndDateTime        != null
                                                                        && _.EndDateTime        <= DateTime.UtcNow.NextMonth());

        foreach (var sub in activeSubs)
        {
            var isNextPendingSubscription = await _merchantSubscriptionRepo.FindAsync(_ => _.MerchantId         == sub.MerchantId
                                                                                       && _.SubscriptionStatus == SubscriptionStatus.Pending
                                                                                       && _.StartDateTime      != null
                                                                                       && _.StartDateTime      >= sub.EndDateTime);

            // if the next subscription is pending, then don't send email
            if (isNextPendingSubscription is not null) continue;

            SubscriptionEmailStatus subEmailType = 0;
            var                     endDateTime  = sub.EndDateTime?.Date;

            // the subscription is expired for next 30 days
            if (endDateTime == DateTime.UtcNow.NextMonth().Date)
            {
                subEmailType = SubscriptionEmailStatus.ExpirationSoon1Month;
            }

            // the subscription is expired for next 1 week
            else if (endDateTime == DateTime.UtcNow.AddDays(7).Date)
            {
                subEmailType = SubscriptionEmailStatus.ExpirationSoon1Week;
            }

            // the subscription is expired for next 1 day
            else if (endDateTime == DateTime.UtcNow.NextDay().Date)
            {
                subEmailType = SubscriptionEmailStatus.ExpirationSoon1Day;
            }

            if (subEmailType > 0)
            {
                var merchant = await _merchantRepo.GetAsync(sub.MerchantId);
                // send email to merchant
                await _emailManager.Send_SubNotification(merchant, subEmailType);
            }
        }
    }
}