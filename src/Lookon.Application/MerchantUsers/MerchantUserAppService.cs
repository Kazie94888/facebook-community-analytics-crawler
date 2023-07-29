using LookOn.Shared;
using LookOn.Merchants;
using LookOn.Users;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
using LookOn.Consts;
using LookOn.Core.Extensions;
using LookOn.Core.Helpers;
using LookOn.Emails;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Repositories;
using LookOn.Permissions;
using LookOn.UserInfos;
using Volo.Abp.Identity;
using Humanizer;

namespace LookOn.MerchantUsers
{
    [RemoteService(IsEnabled = false)]
    [Authorize(LookOnPermissions.MerchantStaffs.Default)]
    public class MerchantUsersAppService : LookOnAppService, IMerchantUsersAppService
    {
        private readonly IMerchantUserRepository     _merchantUserRepository;
        private readonly IdentityUserManager         _identityUserManager;
        private readonly IRepository<AppUser, Guid>  _appUserRepository;
        private readonly IRepository<Merchant, Guid> _merchantRepository;
        private readonly IUserInfoRepository         _userInfoRepository;
        private readonly EmailManager                _emailManager;

        public MerchantUsersAppService(IMerchantUserRepository     merchantUserRepository,
                                       IRepository<AppUser, Guid>  appUserRepository,
                                       IRepository<Merchant, Guid> merchantRepository,
                                       EmailManager                emailManager,
                                       IdentityUserManager         identityUserManager,
                                       IUserInfoRepository         userInfoRepository)
        {
            _merchantUserRepository = merchantUserRepository;
            _appUserRepository      = appUserRepository;
            _merchantRepository     = merchantRepository;
            _emailManager           = emailManager;
            _identityUserManager    = identityUserManager;
            _userInfoRepository     = userInfoRepository;
        }

        public virtual async Task<PagedResultDto<MerchantUserWithNavigationPropertiesDto>> GetListAsync(GetMerchantUsersInput input)
        {
            var totalCount = await _merchantUserRepository.GetCountAsync(input.FilterText,
                                                                         input.IsActive,
                                                                         input.AppUserId,
                                                                         input.MerchantId);
            var items = await _merchantUserRepository.GetListWithNavigationPropertiesAsync(input.FilterText,
                                                                                           input.IsActive,
                                                                                           input.AppUserId,
                                                                                           input.MerchantId,
                                                                                           input.Sorting,
                                                                                           input.MaxResultCount,
                                                                                           input.SkipCount);

            return new PagedResultDto<MerchantUserWithNavigationPropertiesDto>
            {
                TotalCount = totalCount,
                Items      = ObjectMapper.Map<List<MerchantUserWithNavigationProperties>, List<MerchantUserWithNavigationPropertiesDto>>(items)
            };
        }

        public virtual async Task<MerchantUserWithNavigationPropertiesDto> GetWithNavigationPropertiesAsync(Guid id)
        {
            return ObjectMapper.Map<MerchantUserWithNavigationProperties, MerchantUserWithNavigationPropertiesDto>(await _merchantUserRepository
               .GetWithNavigationPropertiesAsync(id));
        }

        public virtual async Task<MerchantUserDto> GetAsync(Guid id)
        {
            return ObjectMapper.Map<MerchantUser, MerchantUserDto>(await _merchantUserRepository.GetAsync(id));
        }

        public virtual async Task<PagedResultDto<LookupDto<Guid>>> GetAppUserLookupAsync(LookupRequestDto input)
        {
            var query = (await _appUserRepository.GetQueryableAsync()).WhereIf(!string.IsNullOrWhiteSpace(input.Filter),
                                                                               x => x.Email != null && x.Email.Contains(input.Filter));

            var lookupData = await query.PageBy(input.SkipCount, input.MaxResultCount).ToDynamicListAsync<AppUser>();
            var totalCount = query.Count();
            return new PagedResultDto<LookupDto<Guid>>
            {
                TotalCount = totalCount, Items = ObjectMapper.Map<List<AppUser>, List<LookupDto<Guid>>>(lookupData)
            };
        }

        public virtual async Task<PagedResultDto<LookupDto<Guid>>> GetMerchantLookupAsync(LookupRequestDto input)
        {
            var query = (await _merchantRepository.GetQueryableAsync()).WhereIf(!string.IsNullOrWhiteSpace(input.Filter),
                                                                                x => x.Name != null && x.Name.Contains(input.Filter));

            var lookupData = await query.PageBy(input.SkipCount, input.MaxResultCount).ToDynamicListAsync<Merchant>();
            var totalCount = query.Count();
            return new PagedResultDto<LookupDto<Guid>>
            {
                TotalCount = totalCount, Items = ObjectMapper.Map<List<Merchant>, List<LookupDto<Guid>>>(lookupData)
            };
        }

        [Authorize(LookOnPermissions.MerchantStaffs.Delete)]
        public virtual async Task DeleteAsync(Guid id)
        {
            await _merchantUserRepository.DeleteAsync(id);
        }

        [Authorize(LookOnPermissions.MerchantStaffs.Create)]
        public virtual async Task<MerchantUserDto> CreateAsync(MerchantUserCreateDto input)
        {
            if (input.AppUserId == default)
            {
                throw new UserFriendlyException(Err["MerchantUser.RequiredField", L["AppUser"]]);
            }

            if (input.MerchantId == default)
            {
                throw new UserFriendlyException(Err["MerchantUser.RequiredField", L["Merchant"]]);
            }

            var appUser           = await _appUserRepository.FirstOrDefaultAsync(x => x.Id             == input.AppUserId);
            var merchantUserCheck = await _merchantUserRepository.FirstOrDefaultAsync(x => x.AppUserId == input.AppUserId);
            if (merchantUserCheck is not null)
            {
                if (merchantUserCheck.MerchantId == input.MerchantId)
                {
                    throw new UserFriendlyException(Err["MerchantUser.StaffAssigned", appUser.Email]);
                }
                else
                {
                    throw new UserFriendlyException(Err["MerchantUser.StaffAlreadyAssigned", appUser.Email]);
                }
            }

            var merchantUser = ObjectMapper.Map<MerchantUserCreateDto, MerchantUser>(input);

            var merchant = await _merchantRepository.GetAsync(input.MerchantId);
            merchantUser.MerchantEmail = merchant.Email;
            merchantUser               = await _merchantUserRepository.InsertAsync(merchantUser, autoSave: true);
            return ObjectMapper.Map<MerchantUser, MerchantUserDto>(merchantUser);
        }

        [Authorize(LookOnPermissions.MerchantStaffs.Edit)]
        public virtual async Task<MerchantUserDto> UpdateAsync(Guid id, MerchantUserUpdateDto input)
        {
            if (input.AppUserId == default)
            {
                throw new UserFriendlyException(Err["MerchantUser.RequiredField", L["AppUser"]]);
            }

            if (input.MerchantId == default)
            {
                throw new UserFriendlyException(Err["MerchantUser.RequiredField", L["Merchant"]]);
            }

            var merchantUser = await _merchantUserRepository.GetAsync(id);
            ObjectMapper.Map(input, merchantUser);
            merchantUser = await _merchantUserRepository.UpdateAsync(merchantUser, autoSave: true);
            return ObjectMapper.Map<MerchantUser, MerchantUserDto>(merchantUser);
        }

        public async Task<PagedResultDto<MerchantUserWithNavigationPropertiesDto>> GetsByMerchantAsync(GetMerchantUsersInput input)
        {
            var items = await _merchantUserRepository.GetListWithNavigationPropertiesAsync(merchantId: input.MerchantId);

            return new PagedResultDto<MerchantUserWithNavigationPropertiesDto>
            {
                TotalCount = items.Count,
                Items      = ObjectMapper.Map<List<MerchantUserWithNavigationProperties>, List<MerchantUserWithNavigationPropertiesDto>>(items)
            };
        }

        public async Task AddUserAsync(Guid merchantId, string email)
        {
            var isNewUser = false;
            var password  = string.Empty;

            var appUser = await _appUserRepository.FirstOrDefaultAsync(x => x.Email == email);
            if (appUser == null)
            {
                isNewUser = true;
                password  = $"LookOn${StringHelper.RandomStringAll(8).FirstLetterToUpper().Transform(To.LowerCase, To.TitleCase)}";
                appUser   = await CreateNewUserAsync(email, password);
            }

            // Check if user is already added to merchant
            var merchantUser = await _merchantUserRepository.FirstOrDefaultAsync(x => x.AppUserId == appUser.Id);
            if (merchantUser is not null)
            {
                if (merchantUser.MerchantId == merchantId)
                {
                    throw new UserFriendlyException(Err["MerchantUser.StaffAssigned", email]);
                }

                throw new UserFriendlyException(Err["MerchantUser.StaffAlreadyAssigned", email]);
            }

            await _merchantUserRepository.InsertAsync(new MerchantUser() {IsActive = true, MerchantId = merchantId, AppUserId = appUser.Id});

            // Send email to new user
            var merchant = await _merchantRepository.GetAsync(merchantId);
            await _emailManager.SendAssignUserEmail(merchant,
                                                    email,
                                                    password,
                                                    isNewUser);
        }

        private async Task<AppUser> CreateNewUserAsync(string email, string password)
        {
            var identityUser = new IdentityUser(Guid.Empty, email, email);

            await _identityUserManager.CreateAsync(identityUser, password);
            if (identityUser.Id != Guid.Empty)
            {
                await _identityUserManager.SetRolesAsync(identityUser, new List<string> {RolesConsts.Merchant});
                var newUserInfo = new UserInfo {AppUserId = identityUser.Id,};
                await _userInfoRepository.InsertAsync(newUserInfo);
            }

            var appUser = await _appUserRepository.FirstOrDefaultAsync(x => x.Email == email);
            if (appUser is null) throw new UserFriendlyException(L["CanNotCreateUser"]);
            return appUser;
        }
    }
}