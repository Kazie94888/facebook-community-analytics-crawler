using System.Threading.Tasks;
using Hangfire;
using LookOn.Core.Helpers;
using LookOn.Integrations.Datalytis;
using LookOn.Merchants;
using LookOn.MerchantSubscriptions;
using LookOn.MerchantSyncInfos;
using Volo.Abp.BackgroundWorkers.Hangfire;

namespace LookOn.BackgroundWorkers.Datalytis
{
    public class Social_UserRequest_Job : HangfireBackgroundWorkerBase
    {
        private readonly DatalytisManager            _datalytisManager;
        private readonly MerchantSubscriptionManager _merchantSubscriptionManager;
        private readonly IMerchantRepository         _merchantRepository;
        private readonly IMerchantSyncInfoRepository _merchantSyncInfoRepository;

        public Social_UserRequest_Job(DatalytisManager            datalytisManager,
                                      MerchantSubscriptionManager merchantSubscriptionManager,
                                      IMerchantRepository         merchantRepository,
                                      IMerchantSyncInfoRepository merchantSyncInfoRepository)
        {
            _datalytisManager            = datalytisManager;
            _merchantSubscriptionManager = merchantSubscriptionManager;
            _merchantRepository          = merchantRepository;
            _merchantSyncInfoRepository  = merchantSyncInfoRepository;

            RecurringJobId = nameof(Social_UserRequest_Job);
            CronExpression = Cron.Never();
            if (EnvironmentHelper.IsProduction())
            {
                CronExpression = "*/30 * * * *"; // every 30 minutes
            }
        }

        public override async Task DoWorkAsync()
        {
            var merchants = await _merchantSubscriptionManager.GetActiveMerchants();
            foreach (var merchant in merchants)
            {
                var merchantSyncInfo = await _merchantSyncInfoRepository.GetAsync(_ => _.MerchantId == merchant.Id);
                foreach (var community in merchantSyncInfo.SocialScan.UserScans)
                {
                    BackgroundJob.Enqueue(() => _datalytisManager.SocialUsers_Request(merchant.Id, community.SocialCommunityId));
                }
            }
        }
    }

    public class Social_UserStatus_Job : HangfireBackgroundWorkerBase
    {
        private readonly DatalytisManager            _datalytisManager;
        private readonly MerchantSubscriptionManager _merchantSubscriptionManager;
        private readonly IMerchantRepository         _merchantRepository;
        private readonly IMerchantSyncInfoRepository _merchantSyncInfoRepository;

        public Social_UserStatus_Job(DatalytisManager            datalytisManager,
                                     MerchantSubscriptionManager merchantSubscriptionManager,
                                     IMerchantRepository         merchantRepository,
                                     IMerchantSyncInfoRepository merchantSyncInfoRepository)
        {
            _datalytisManager            = datalytisManager;
            _merchantSubscriptionManager = merchantSubscriptionManager;
            _merchantRepository          = merchantRepository;
            _merchantSyncInfoRepository  = merchantSyncInfoRepository;

            RecurringJobId = nameof(Social_UserStatus_Job);

            CronExpression = Cron.Never();
            if (EnvironmentHelper.IsProduction())
            {
                CronExpression = "*/30 * * * *"; // every 30 minutes
            }
        }

        public override async Task DoWorkAsync()
        {
            var merchants = await _merchantSubscriptionManager.GetActiveMerchants();
            foreach (var merchant in merchants)
            {
                var merchantSyncInfo = await _merchantSyncInfoRepository.GetAsync(_ => _.MerchantId == merchant.Id);
                foreach (var community in merchantSyncInfo.SocialScan.UserScans)
                {
                    BackgroundJob.Enqueue(() => _datalytisManager.SocialUsers_Status(merchant.Id, community.SocialCommunityId));
                }
            }
        }
    }

    public class Social_UserSync_Job : HangfireBackgroundWorkerBase
    {
        private readonly DatalytisManager            _datalytisManager;
        private readonly MerchantSubscriptionManager _merchantSubscriptionManager;
        private readonly IMerchantSyncInfoRepository _syncInfoRepo;

        public Social_UserSync_Job(DatalytisManager            datalytisManager,
                                   MerchantSubscriptionManager merchantSubscriptionManager,
                                   IMerchantRepository         merchantRepo,
                                   IMerchantSyncInfoRepository syncInfoRepo)
        {
            _datalytisManager            = datalytisManager;
            _merchantSubscriptionManager = merchantSubscriptionManager;
            _syncInfoRepo                = syncInfoRepo;

            RecurringJobId = nameof(Social_UserSync_Job);
            CronExpression = Cron.Never();
            if (EnvironmentHelper.IsProduction())
            {
                CronExpression = "*/30 * * * *"; // every 30 minutes
            }
        }

        public override async Task DoWorkAsync()
        {
            var merchants = await _merchantSubscriptionManager.GetActiveMerchants();
            foreach (var merchant in merchants)
            {
                var syncInfo = await _syncInfoRepo.GetAsync(_ => _.MerchantId == merchant.Id);

                foreach (var userScan in syncInfo.SocialScan.UserScans)
                {
                    if (userScan.ShouldSync)
                    {
                        BackgroundJob.Enqueue(() => _datalytisManager.SocialUsers_Sync(merchant.Id, userScan.SocialCommunityId));
                    }
                }
            }
        }
    }
}