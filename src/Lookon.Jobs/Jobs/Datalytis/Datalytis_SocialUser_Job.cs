using System;
using System.Linq;
using LookOn.Integrations.Datalytis;
using System.Threading.Tasks;
using Hangfire;
using LookOn.Core.Extensions;
using LookOn.Integrations.Datalytis.Configs;
using LookOn.Merchants;
using LookOn.MerchantSubscriptions;
using LookOn.MerchantSyncInfos;
using Volo.Abp.Domain.Repositories;

namespace LookOn.Jobs.Jobs.Datalytis
{
    public class Social_UserRequest_Job : BackgroundJobBase
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
        }

        protected override async Task DoExecute()
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

    public class Social_UserStatus_Job : BackgroundJobBase
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
        }

        protected override async Task DoExecute()
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

    public class Social_UserSync_Job : BackgroundJobBase
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
            _syncInfoRepo  = syncInfoRepo;
        }

        protected override async Task DoExecute()
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