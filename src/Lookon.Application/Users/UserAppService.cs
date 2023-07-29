using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;

namespace LookOn.Users;

[Authorize]
public class UserAppService : ApplicationService, IUserAppService
{
    private readonly IRepository<AppUser> _appUserRepository;

    public UserAppService(IRepository<AppUser> appUserRepository)
    {
        _appUserRepository = appUserRepository;
    }

    public async Task<AppUserDto> GetByEmailAsync(string email)
    {
        try
        {
            email = email.Trim();
            var appUser = await _appUserRepository.GetAsync(x => x.Email == email);
            return ObjectMapper.Map<AppUser, AppUserDto>(appUser);
        }
        catch (EntityNotFoundException)
        {
            throw new UserFriendlyException(L["StaffEmail.NotFound", email]);
        }
    }
}