using LookOn.Shared;
using LookOn.Platforms;
using LookOn.Merchants;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using LookOn.Permissions;
using LookOn.MerchantStores;

namespace LookOn.MerchantStores
{
    [RemoteService(IsEnabled = false)]
    [Authorize(LookOnPermissions.MerchantStores.Default)]
    public class MerchantStoresAppService : LookOnAppService, IMerchantStoresAppService
    {
        private readonly IMerchantStoreRepository    _merchantStoreRepository;
        private readonly IRepository<Merchant, Guid> _merchantRepository;
        private readonly IRepository<Platform, Guid> _platformRepository;

        public MerchantStoresAppService(IMerchantStoreRepository    merchantStoreRepository,
                                        IRepository<Merchant, Guid> merchantRepository,
                                        IRepository<Platform, Guid> platformRepository)
        {
            _merchantStoreRepository = merchantStoreRepository;
            _merchantRepository      = merchantRepository;
            _platformRepository      = platformRepository;
        }

        public virtual async Task<PagedResultDto<MerchantStoreWithNavigationPropertiesDto>> GetListAsync(GetMerchantStoresInput input)
        {
            var totalCount = await _merchantStoreRepository.GetCountAsync(input.FilterText,
                                                                          input.Name,
                                                                          input.Code,
                                                                          input.Active,
                                                                          input.MerchantId,
                                                                          input.PlatformId);
            var items = await _merchantStoreRepository.GetListWithNavigationPropertiesAsync(input.FilterText,
                                                                                            input.Name,
                                                                                            input.Code,
                                                                                            input.Active,
                                                                                            input.MerchantId,
                                                                                            input.PlatformId,
                                                                                            input.Sorting,
                                                                                            input.MaxResultCount,
                                                                                            input.SkipCount);
            return new PagedResultDto<MerchantStoreWithNavigationPropertiesDto>
            {
                TotalCount = totalCount, Items = ObjectMapper.Map<List<MerchantStoreWithNavigationProperties>, List<MerchantStoreWithNavigationPropertiesDto>>(items)
            };
        }

        public virtual async Task<MerchantStoreWithNavigationPropertiesDto> GetWithNavigationPropertiesAsync(Guid id)
        {
            return ObjectMapper
               .Map<MerchantStoreWithNavigationProperties, MerchantStoreWithNavigationPropertiesDto>(await _merchantStoreRepository.GetWithNavigationPropertiesAsync(id));
        }

        public virtual async Task<MerchantStoreDto> GetAsync(Guid id)
        {
            return ObjectMapper.Map<MerchantStore, MerchantStoreDto>(await _merchantStoreRepository.GetAsync(id));
        }

        public virtual async Task<PagedResultDto<LookupDto<Guid?>>> GetMerchantLookupAsync(LookupRequestDto input)
        {
            var query = (await _merchantRepository.GetQueryableAsync()).WhereIf(!string.IsNullOrWhiteSpace(input.Filter),
                                                                                x => x.Name != null && x.Name.Contains(input.Filter));
            var lookupData = await query.PageBy(input.SkipCount,
                                                input.MaxResultCount)
                                        .ToDynamicListAsync<Merchant>();
            var totalCount = query.Count();
            return new PagedResultDto<LookupDto<Guid?>> { TotalCount = totalCount, Items = ObjectMapper.Map<List<Merchant>, List<LookupDto<Guid?>>>(lookupData) };
        }

        public virtual async Task<PagedResultDto<LookupDto<Guid?>>> GetPlatformLookupAsync(LookupRequestDto input)
        {
            var query = (await _platformRepository.GetQueryableAsync()).WhereIf(!string.IsNullOrWhiteSpace(input.Filter),
                                                                                x => x.Name != null && x.Name.Contains(input.Filter));
            var lookupData = await query.PageBy(input.SkipCount,
                                                input.MaxResultCount)
                                        .ToDynamicListAsync<Platform>();
            var totalCount = query.Count();
            return new PagedResultDto<LookupDto<Guid?>> { TotalCount = totalCount, Items = ObjectMapper.Map<List<Platform>, List<LookupDto<Guid?>>>(lookupData) };
        }

        [Authorize(LookOnPermissions.MerchantStores.Delete)]
        public virtual async Task DeleteAsync(Guid id)
        {
            await _merchantStoreRepository.DeleteAsync(id);
        }

        [Authorize(LookOnPermissions.MerchantStores.Create)]
        public virtual async Task<MerchantStoreDto> CreateAsync(MerchantStoreCreateDto input)
        {
            var merchantStore = ObjectMapper.Map<MerchantStoreCreateDto, MerchantStore>(input);
            merchantStore.TenantId = CurrentTenant.Id;
            merchantStore = await _merchantStoreRepository.InsertAsync(merchantStore,
                                                                       autoSave: true);
            return ObjectMapper.Map<MerchantStore, MerchantStoreDto>(merchantStore);
        }

        [Authorize(LookOnPermissions.MerchantStores.Edit)]
        public virtual async Task<MerchantStoreDto> UpdateAsync(Guid id, MerchantStoreUpdateDto input)
        {
            var merchantStore = await _merchantStoreRepository.GetAsync(id);
            ObjectMapper.Map(input,
                             merchantStore);
            merchantStore = await _merchantStoreRepository.UpdateAsync(merchantStore,
                                                                       autoSave: true);
            return ObjectMapper.Map<MerchantStore, MerchantStoreDto>(merchantStore);
        }
    }
}