using LookOn.Shared;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Application.Dtos;
using LookOn.MerchantUsers;

namespace LookOn.Controllers.MerchantUsers
{
    [RemoteService]
    [Area("app")]
    [ControllerName("MerchantUser")]
    [Route("api/app/merchant-users")]
    public class MerchantUserController : AbpController, IMerchantUsersAppService
    {
        private readonly IMerchantUsersAppService _merchantUsersAppService;

        public MerchantUserController(IMerchantUsersAppService merchantUsersAppService)
        {
            _merchantUsersAppService = merchantUsersAppService;
        }

        [HttpGet] public Task<PagedResultDto<MerchantUserWithNavigationPropertiesDto>> GetListAsync(GetMerchantUsersInput input)
        {
            return _merchantUsersAppService.GetListAsync(input);
        }

        [HttpGet] [Route("with-navigation-properties/{id}")]
        public Task<MerchantUserWithNavigationPropertiesDto> GetWithNavigationPropertiesAsync(Guid id)
        {
            return _merchantUsersAppService.GetWithNavigationPropertiesAsync(id);
        }

        [HttpGet] [Route("{id}")] public virtual Task<MerchantUserDto> GetAsync(Guid id)
        {
            return _merchantUsersAppService.GetAsync(id);
        }

        [HttpGet] [Route("app-user-lookup")] public Task<PagedResultDto<LookupDto<Guid>>> GetAppUserLookupAsync(LookupRequestDto input)
        {
            return _merchantUsersAppService.GetAppUserLookupAsync(input);
        }

        [HttpGet] [Route("merchant-lookup")] public Task<PagedResultDto<LookupDto<Guid>>> GetMerchantLookupAsync(LookupRequestDto input)
        {
            return _merchantUsersAppService.GetMerchantLookupAsync(input);
        }

        [HttpPost] public virtual Task<MerchantUserDto> CreateAsync(MerchantUserCreateDto input)
        {
            return _merchantUsersAppService.CreateAsync(input);
        }

        [HttpPut] [Route("{id}")] public virtual Task<MerchantUserDto> UpdateAsync(Guid id, MerchantUserUpdateDto input)
        {
            return _merchantUsersAppService.UpdateAsync(id, input);
        }

        [HttpGet("gets-by-merchant")]
        public Task<PagedResultDto<MerchantUserWithNavigationPropertiesDto>> GetsByMerchantAsync(GetMerchantUsersInput input)
        {
            return _merchantUsersAppService.GetsByMerchantAsync(input);
        }
        [HttpPost("add-merchant-user")]
        public Task AddUserAsync(Guid merchantId, string userNameOrEmail)
        {
            return _merchantUsersAppService.AddUserAsync(merchantId, userNameOrEmail);
        }

        [HttpDelete] [Route("{id}")] public virtual Task DeleteAsync(Guid id)
        {
            return _merchantUsersAppService.DeleteAsync(id);
        }
    }
}