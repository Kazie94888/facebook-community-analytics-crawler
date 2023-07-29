using LookOn.Shared;
using LookOn.Categories;
using LookOn.Users;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
using LookOn.Emails;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using LookOn.Permissions;
using LookOn.Merchants;
using LookOn.MerchantSocialCommunity;

namespace LookOn.Merchants
{
    [RemoteService(IsEnabled = false)]
    [Authorize(LookOnPermissions.Merchants.Default)]
    public class MerchantsAppService : LookOnAppService, IMerchantsAppService
    {
        private readonly IMerchantRepository         _merchantRepository;
        private readonly IRepository<AppUser, Guid>  _appUserRepository;
        private readonly IRepository<Category, Guid> _categoryRepository;
        
        private readonly EmailManager _emailManager;

        public MerchantsAppService(IMerchantRepository merchantRepository, IRepository<AppUser, Guid> appUserRepository, IRepository<Category, Guid> categoryRepository, EmailManager emailManager)
        {
            _merchantRepository = merchantRepository;
            _appUserRepository  = appUserRepository;
            _categoryRepository = categoryRepository;
            _emailManager  = emailManager;
        }

        public virtual async Task<PagedResultDto<MerchantWithNavigationPropertiesDto>> GetListAsync(GetMerchantsInput input)
        {
            var totalCount = await _merchantRepository.GetCountAsync(input.FilterText,
                                                                     input.Name,
                                                                     input.Phone,
                                                                     input.Address,
                                                                     input.Email,
                                                                     input.Fax,
                                                                     input.OwnerAppUserId,
                                                                     input.CategoryId);
            var items = await _merchantRepository.GetListWithNavigationPropertiesAsync(input.FilterText,
                                                                                       input.Name,
                                                                                       input.Phone,
                                                                                       input.Address,
                                                                                       input.Email,
                                                                                       input.Fax,
                                                                                       input.OwnerAppUserId,
                                                                                       input.CategoryId,
                                                                                       input.Sorting,
                                                                                       input.MaxResultCount,
                                                                                       input.SkipCount);

            return new PagedResultDto<MerchantWithNavigationPropertiesDto>
            {
                TotalCount = totalCount, Items = ObjectMapper.Map<List<MerchantWithNavigationProperties>, List<MerchantWithNavigationPropertiesDto>>(items)
            };
        }

        public virtual async Task<MerchantWithNavigationPropertiesDto> GetWithNavigationPropertiesAsync(Guid id)
        {
            return ObjectMapper.Map<MerchantWithNavigationProperties, MerchantWithNavigationPropertiesDto>(await _merchantRepository.GetWithNavigationPropertiesAsync(id));
        }

        public virtual async Task<MerchantDto> GetAsync(Guid id)
        {
            return ObjectMapper.Map<Merchant, MerchantDto>(await _merchantRepository.GetAsync(id));
        }

        public virtual async Task<PagedResultDto<LookupDto<Guid>>> GetAppUserLookupAsync(LookupRequestDto input)
        {
            var query = (await _appUserRepository.GetQueryableAsync())
               .WhereIf(!string.IsNullOrWhiteSpace(input.Filter),
                        x => x.Email != null && x.Email.Contains(input.Filter));

            var lookupData = await query.PageBy(input.SkipCount, input.MaxResultCount).ToDynamicListAsync<AppUser>();
            var totalCount = query.Count();
            return new PagedResultDto<LookupDto<Guid>> { TotalCount = totalCount, Items = ObjectMapper.Map<List<AppUser>, List<LookupDto<Guid>>>(lookupData) };
        }

        public virtual async Task<PagedResultDto<LookupDto<Guid?>>> GetCategoryLookupAsync(LookupRequestDto input)
        {
            var query = (await _categoryRepository.GetQueryableAsync())
               .WhereIf(!string.IsNullOrWhiteSpace(input.Filter),
                        x => x.Name != null && x.Name.Contains(input.Filter));

            var lookupData = await query.PageBy(input.SkipCount, input.MaxResultCount).ToDynamicListAsync<Category>();
            var totalCount = query.Count();
            return new PagedResultDto<LookupDto<Guid?>> { TotalCount = totalCount, Items = ObjectMapper.Map<List<Category>, List<LookupDto<Guid?>>>(lookupData) };
        }

        [Authorize(LookOnPermissions.Merchants.Delete)]
        public virtual async Task DeleteAsync(Guid id)
        {
            await _merchantRepository.DeleteAsync(id);
        }

        [Authorize(LookOnPermissions.Merchants.Create)]
        public virtual async Task<MerchantDto> CreateAsync(MerchantCreateDto input)
        {
            if (input.OwnerAppUserId == default)
            {
                throw new UserFriendlyException(Err["MerchantUser.RequiredField", L["AppUser"]]);
            }

            var merchant = ObjectMapper.Map<MerchantCreateDto, Merchant>(input);
            merchant.TenantId = CurrentTenant.Id;
            merchant          = await _merchantRepository.InsertAsync(merchant, autoSave: true);
            return ObjectMapper.Map<Merchant, MerchantDto>(merchant);
        }

        [Authorize(LookOnPermissions.Merchants.Edit)]
        public virtual async Task<MerchantDto> UpdateAsync(Guid id, MerchantUpdateDto input)
        {
            if (input.OwnerAppUserId == default)
            {
                throw new UserFriendlyException(Err["MerchantUser.RequiredField", L["AppUser"]]);
            }

            var merchant = await _merchantRepository.GetAsync(id);
            ObjectMapper.Map(input, merchant);
            merchant = await _merchantRepository.UpdateAsync(merchant, autoSave: true);
            return ObjectMapper.Map<Merchant, MerchantDto>(merchant);
        }

        [Authorize(LookOnPermissions.Merchants.Edit)]
        public async Task<MetricConfigsDto> UpdateMetricConfigures(Guid id, MetricConfigsDto input)
        {
            var merchant        = await _merchantRepository.GetAsync(id);
            ObjectMapper.Map(input, merchant.MetricConfigs);
            merchant = await _merchantRepository.UpdateAsync(merchant, autoSave: true);
            return ObjectMapper.Map<Merchant, MerchantDto>(merchant).MetricConfigs;
        }

        public async Task SendNewCommunityNotification(Guid merchantId, string url, bool invalidCommunity, string communityName)
        {
            var merchant = await _merchantRepository.GetAsync(merchantId);
            await _emailManager.Send_NewCommunityNotification(merchant, url, invalidCommunity, communityName);
        }
        
        public async Task DeleteCommunity(MerchantSocialCommunityRequest request)
        {
            var merchant = await _merchantRepository.FirstOrDefaultAsync(_ => _.Id == request.MerchantId);
            if(merchant is null) throw new UserFriendlyException("Merchant not found");
            var deletedCommunity = merchant.Communities.FirstOrDefault(_ => _.SocialCommunityName == request.CommunityName);
            if(deletedCommunity is null) throw new UserFriendlyException("Community not found");
            merchant.Communities = merchant.Communities.Where(_ => _.SocialCommunityName != request.CommunityName).ToList();
            await _merchantRepository.UpdateAsync(merchant);
        }
    }
}