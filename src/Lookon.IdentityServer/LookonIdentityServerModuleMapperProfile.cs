using AutoMapper;
using Volo.Abp.Identity;

namespace LookOn;

public class LookOnIdentityServerModuleMapperProfile : Profile
{
    public LookOnIdentityServerModuleMapperProfile()
    {
        CreateMap<IdentityUser, IdentityUserDto>();
        CreateMap<IdentitySecurityLog, IdentitySecurityLogDto>();
    }
}