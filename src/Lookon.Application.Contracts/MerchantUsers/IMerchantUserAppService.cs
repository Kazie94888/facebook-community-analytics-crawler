using LookOn.Shared;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LookOn.Merchants;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace LookOn.MerchantUsers
{
    public interface IMerchantUsersAppService : IApplicationService
    {
        Task<PagedResultDto<MerchantUserWithNavigationPropertiesDto>> GetListAsync(GetMerchantUsersInput input);

        Task<MerchantUserWithNavigationPropertiesDto> GetWithNavigationPropertiesAsync(Guid id);

        Task<MerchantUserDto> GetAsync(Guid id);

        Task<PagedResultDto<LookupDto<Guid>>> GetAppUserLookupAsync(LookupRequestDto input);

        Task<PagedResultDto<LookupDto<Guid>>> GetMerchantLookupAsync(LookupRequestDto input);

        Task DeleteAsync(Guid id);

        Task<MerchantUserDto> CreateAsync(MerchantUserCreateDto input);

        Task<MerchantUserDto> UpdateAsync(Guid id, MerchantUserUpdateDto input);
        
        //Extend Methods
        Task<PagedResultDto<MerchantUserWithNavigationPropertiesDto>> GetsByMerchantAsync(GetMerchantUsersInput input);
        Task AddUserAsync(Guid merchantId, string userNameOrEmail);
    }
}