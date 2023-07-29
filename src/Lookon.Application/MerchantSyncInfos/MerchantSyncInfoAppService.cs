using LookOn.Shared;
using LookOn.Merchants;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
using FluentDateTime;
using LookOn.Enums;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using LookOn.Permissions;
using LookOn.MerchantSyncInfos;
using LookOn.MerchantUsers;

namespace LookOn.MerchantSyncInfos
{
    [RemoteService(IsEnabled = false)]
    [Authorize(LookOnPermissions.MerchantSyncInfos.Default)]
    public class MerchantSyncInfosAppService : ApplicationService, IMerchantSyncInfosAppService
    {
        private readonly IMerchantSyncInfoRepository _merchantSyncInfoRepository;
        private readonly IRepository<Merchant, Guid> _merchantRepository;
        private readonly IMerchantUserRepository     _merchantUserRepository;

        public MerchantSyncInfosAppService(IMerchantSyncInfoRepository merchantSyncInfoRepository,
                                           IRepository<Merchant, Guid> merchantRepository,
                                           IMerchantUserRepository     merchantUserRepository)
        {
            _merchantSyncInfoRepository = merchantSyncInfoRepository;
            _merchantRepository         = merchantRepository;
            _merchantUserRepository     = merchantUserRepository;
        }

        public virtual async Task<PagedResultDto<MerchantSyncInfoWithNavigationPropertiesDto>> GetListAsync(GetMerchantSyncInfosInput input)
        {
            var totalCount = await _merchantSyncInfoRepository.GetCountAsync(input.FilterText, input.MerchantEmail, input.MerchantId);
            var items = await _merchantSyncInfoRepository.GetListWithNavigationPropertiesAsync(input.FilterText,
                                                                                               input.MerchantEmail,
                                                                                               input.MerchantId,
                                                                                               input.Sorting,
                                                                                               input.MaxResultCount,
                                                                                               input.SkipCount);

            return new PagedResultDto<MerchantSyncInfoWithNavigationPropertiesDto>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<MerchantSyncInfoWithNavigationProperties>, List<MerchantSyncInfoWithNavigationPropertiesDto>>(items)
            };
        }

        public virtual async Task<MerchantSyncInfoWithNavigationPropertiesDto> GetWithNavigationPropertiesAsync(Guid id)
        {
            return ObjectMapper
               .Map<MerchantSyncInfoWithNavigationProperties, MerchantSyncInfoWithNavigationPropertiesDto>(await _merchantSyncInfoRepository
                   .GetWithNavigationPropertiesAsync(id));
        }

        public virtual async Task<MerchantSyncInfoDto> GetAsync(Guid id)
        {
            return ObjectMapper.Map<MerchantSyncInfo, MerchantSyncInfoDto>(await _merchantSyncInfoRepository.GetAsync(id));
        }

        public async Task<MerchantSyncInfoDto> GetByMerchantIdAsync(Guid merchantId)
        {
            return ObjectMapper.Map<MerchantSyncInfo, MerchantSyncInfoDto>(await _merchantSyncInfoRepository.FirstOrDefaultAsync(info => info.MerchantId == merchantId));
        }

        public async Task<MerchantSyncStatus> GetMerchantSyncStatus(Guid merchantId)
        {
            var merchantSyncInfo = await _merchantSyncInfoRepository.FirstOrDefaultAsync(_ => _.MerchantId == merchantId);
            return merchantSyncInfo?.MerchantSyncStatus ?? 0;
        }
        
        public virtual async Task<PagedResultDto<LookupDto<Guid?>>> GetMerchantLookupAsync(LookupRequestDto input)
        {
            var query = (await _merchantRepository.GetQueryableAsync()).WhereIf(!string.IsNullOrWhiteSpace(input.Filter),
                                                                                x => x.Email != null && x.Email.Contains(input.Filter));

            var lookupData = await query.PageBy(input.SkipCount, input.MaxResultCount).ToDynamicListAsync<Merchant>();
            var totalCount = query.Count();
            return new PagedResultDto<LookupDto<Guid?>>
            {
                TotalCount = totalCount, Items = ObjectMapper.Map<List<Merchant>, List<LookupDto<Guid?>>>(lookupData)
            };
        }

        [Authorize(LookOnPermissions.MerchantSyncInfos.Delete)]
        public virtual async Task DeleteAsync(Guid id)
        {
            await _merchantSyncInfoRepository.DeleteAsync(id);
        }

        [Authorize(LookOnPermissions.MerchantSyncInfos.Create)]
        public virtual async Task<MerchantSyncInfoDto> CreateAsync(MerchantSyncInfoCreateDto input)
        {
            var merchantSyncInfo = ObjectMapper.Map<MerchantSyncInfoCreateDto, MerchantSyncInfo>(input);

            merchantSyncInfo = await _merchantSyncInfoRepository.InsertAsync(merchantSyncInfo, autoSave: true);
            return ObjectMapper.Map<MerchantSyncInfo, MerchantSyncInfoDto>(merchantSyncInfo);
        }

        [Authorize(LookOnPermissions.MerchantSyncInfos.Edit)]
        public virtual async Task<MerchantSyncInfoDto> UpdateAsync(Guid id, MerchantSyncInfoUpdateDto input)
        {
            var merchantSyncInfo = await _merchantSyncInfoRepository.GetAsync(id);
            ObjectMapper.Map(input, merchantSyncInfo);
            merchantSyncInfo = await _merchantSyncInfoRepository.UpdateAsync(merchantSyncInfo, autoSave: true);
            return ObjectMapper.Map<MerchantSyncInfo, MerchantSyncInfoDto>(merchantSyncInfo);
        }

        public async Task<PagedResultDto<MerchantSocialUserSyncInfo>> GetMerchantSocialSyncInfos()
        {
            var merchantSocialUserInfos = new List<MerchantSocialUserSyncInfo>();
            var merchants               = await _merchantRepository.GetListAsync();
            foreach (var merchant in merchants)
            {
                var merchantSyncInfo = await _merchantSyncInfoRepository.FirstOrDefaultAsync(_ => _.MerchantId == merchant.Id);
                var result = merchantSyncInfo is null
                                 ? new List<MerchantSocialUserSyncInfo>()
                                 : ObjectMapper.Map<List<MerchantUserScan>, List<MerchantSocialUserSyncInfo>>(merchantSyncInfo.SocialScan.UserScans);
                result = result.Select(_ =>
                                {
                                    _.MerchantName       = merchant.Name;
                                    _.MerchantSyncInfoId = merchantSyncInfo?.Id;
                                    return _;
                                })
                               .ToList();
                merchantSocialUserInfos.AddRange(result);
            }

            return new PagedResultDto<MerchantSocialUserSyncInfo>() {Items = merchantSocialUserInfos, TotalCount = merchantSocialUserInfos.Count};
        }

        public async Task<MerchantSocialUserSyncInfo> GetMerchantSocialSyncInfo(Guid merchantSyncInfoId, string socialCommunityId)
        {
            var merchantSyncInfo = await _merchantSyncInfoRepository.FirstOrDefaultAsync(_ => _.Id == merchantSyncInfoId);
            var merchantUserScan = merchantSyncInfo?.SocialScan.UserScans.FirstOrDefault(_ => _.SocialCommunityId == socialCommunityId);
            var merchantSocialUserSyncInfo =
                merchantUserScan is null ? null : ObjectMapper.Map<MerchantUserScan, MerchantSocialUserSyncInfo>(merchantUserScan);
            if (merchantSocialUserSyncInfo is not null) merchantSocialUserSyncInfo.MerchantSyncInfoId = merchantSyncInfoId;
            return merchantSocialUserSyncInfo;
        }

        public async Task UpdateMerchantUserScanStatus(MerchantSocialUserSyncInfo merchantSocialUserSyncInfo)
        {
            var merchantSyncInfo = await _merchantSyncInfoRepository.FirstOrDefaultAsync(_ => _.Id == merchantSocialUserSyncInfo.MerchantSyncInfoId);
            var merchantUserScan =
                merchantSyncInfo?.SocialScan.UserScans.FirstOrDefault(_ => _.SocialCommunityId == merchantSocialUserSyncInfo.SocialCommunityId);
            if (merchantUserScan is null) return;

            await _merchantSyncInfoRepository.UpdateAsync(merchantSyncInfo);
        }

        public async Task ForceSyncOrders(Guid merchantSyncInfoId)
        {
            var merchantSyncInfo = await _merchantSyncInfoRepository.FirstOrDefaultAsync(_ => _.Id == merchantSyncInfoId);
            if (merchantSyncInfo is null) return;

            if (!merchantSyncInfo.EcomScan.IsFirstSyncCompleted) return;

            merchantSyncInfo.EcomScan.LastCleanOrderSyncedAt = DateTime.UtcNow.PreviousMonth();
            merchantSyncInfo.EcomScan.LastRawOrderSyncedAt   = DateTime.UtcNow.PreviousMonth();
                
            await _merchantSyncInfoRepository.UpdateAsync(merchantSyncInfo);
        }
    }
}