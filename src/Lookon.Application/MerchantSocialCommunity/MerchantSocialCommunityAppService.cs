using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using LookOn.Core.Extensions;
using LookOn.Emails;
using LookOn.Enums;
using LookOn.Merchants;
using LookOn.MerchantSyncInfos;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp;
using Volo.Abp.Application.Services;
using LookOn.Permissions;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Repositories;

namespace LookOn.MerchantSocialCommunity
{
    [RemoteService(IsEnabled = false)]
    [Authorize(LookOnPermissions.MerchantSocialCommunities.Default)]
    public class MerchantSocialCommunityAppService : ApplicationService, IMerchantSocialCommunityAppService
    {
        private readonly IRepository<Merchant, Guid> _merchantRepository;
        private readonly IMerchantSyncInfoRepository _merchantSyncInfoRepository;
        private readonly EmailManager                _emailManager;

        public MerchantSocialCommunityAppService(IRepository<Merchant, Guid> merchantRepository, IMerchantSyncInfoRepository merchantSyncInfoRepository, EmailManager emailManager)
        {
            _merchantRepository         = merchantRepository;
            _merchantSyncInfoRepository = merchantSyncInfoRepository;
            _emailManager               = emailManager;
        }

        public async Task<PagedResultDto<MerchantSocialCommunityDto>> GetMerchantSocialCommunities(MerchantSocialCommunityFilterDto filterDto)
        {
            var merchants = await _merchantRepository.GetListAsync();
            var query = merchants.AsQueryable()
                                 .WhereIf(filterDto.MerchantId is not null, m => m.Id == filterDto.MerchantId.Value)
                                 .SelectMany(m => m.Communities)
                                 .WhereIf(filterDto.FilterText.IsNotNullOrSpace(), m => m.SocialCommunityName.Contains(filterDto.FilterText))
                                 .WhereIf(filterDto.HasCommunityId is true,        m => m.SocialCommunityId != null)
                                 .WhereIf(filterDto.HasCommunityId is false,       m => m.SocialCommunityId == null);

            var items = await query.PageBy(filterDto.SkipCount, filterDto.MaxResultCount).ToDynamicListAsync<Merchants.MerchantSocialCommunity>();

            var itemsDto = ObjectMapper.Map<List<Merchants.MerchantSocialCommunity>, List<MerchantSocialCommunityDto>>(items);
            foreach (var item in itemsDto)
            {
                var merchant = merchants.FirstOrDefault(m => m.Communities.Any(_ => _.SocialCommunityId == item.SocialCommunityId));
                if (merchant == null) continue;

                item.MerchantId   = merchant.Id;
                item.MerchantName = merchant.Name;
            }

            return new PagedResultDto<MerchantSocialCommunityDto> { TotalCount = query.ToList().Count, Items = itemsDto };
        }

        public async Task<MerchantSocialCommunityDto> GetMerchantSocialCommunity(MerchantSocialCommunityRequest request)
        {
            // get merchant
            var merchant = await _merchantRepository.FindAsync(request.MerchantId);

            // get community
            var community = merchant.Communities.FirstOrDefault(_ => _.SocialCommunityName == request.CommunityName);

            // map to dto
            var commDto = ObjectMapper.Map<Merchants.MerchantSocialCommunity, MerchantSocialCommunityDto>(community);
            commDto.MerchantId   = merchant.Id;
            commDto.MerchantName = merchant.Name;

            return commDto;
        }

        public async Task UpdateMerchantSocialCommunity(MerchantSocialCommunityDto merchantSocialCommunity)
        {
            // get merchant
            var merchant = await _merchantRepository.FindAsync(merchantSocialCommunity.MerchantId);

            // get community
            var community = merchant.Communities.FirstOrDefault(_ => _.SocialCommunityName == merchantSocialCommunity.SocialCommunityName);

            var isSendApprovedNotification = community?.VerificationStatus is not SocialCommunityVerificationStatus.Approved
                                  && merchantSocialCommunity.VerificationStatus is SocialCommunityVerificationStatus.Approved;
            var isSendRejectedNotification = community?.VerificationStatus is not SocialCommunityVerificationStatus.Rejected
                                          && merchantSocialCommunity.VerificationStatus is SocialCommunityVerificationStatus.Rejected;

            // check status and update on scan user
            if (community?.VerificationStatus != merchantSocialCommunity.VerificationStatus)
            {
                var merchantSyncInfo = await _merchantSyncInfoRepository.FirstOrDefaultAsync(_ => _.MerchantId == merchantSocialCommunity.MerchantId);
                var userScans        = merchantSyncInfo.SocialScan.UserScans;
                if (merchantSocialCommunity.VerificationStatus == SocialCommunityVerificationStatus.Approved)
                {
                    userScans.Add(new MerchantUserScan() { SocialCommunityId = merchantSocialCommunity.SocialCommunityId });
                }

                if (merchantSocialCommunity.VerificationStatus.IsIn(SocialCommunityVerificationStatus.Pending, SocialCommunityVerificationStatus.Rejected))
                {
                    merchantSyncInfo.SocialScan.UserScans = userScans.Where(_ => _.SocialCommunityId != merchantSocialCommunity.SocialCommunityId).ToList();
                }

                // save merchant sync info
                await _merchantSyncInfoRepository.UpdateAsync(merchantSyncInfo, true);
            }

            // map to dto
            ObjectMapper.Map(merchantSocialCommunity, community);

            // save merchant
            await _merchantRepository.UpdateAsync(merchant, true);

            // send email to merchant
            if (isSendApprovedNotification) await _emailManager.Send_VerifyCommunityNotification(merchant, community?.SocialCommunityName, true);
            if (isSendRejectedNotification) await _emailManager.Send_VerifyCommunityNotification(merchant, community?.SocialCommunityName, false);
        }
    }
}