using LookOn.MerchantSyncInfos;
using LookOn.MerchantUsers;
using LookOn.MerchantSubscriptions;
using LookOn.MerchantStores;
using LookOn.Platforms;
using LookOn.UserInfos;
using LookOn.Categories;
using LookOn.Merchants;
using AutoMapper;
using LookOn.Core.Extensions;
using LookOn.Enums;
using LookOn.HaravanWebhooks;
using LookOn.Web.Models;
using LookOn.Web.Pages.Merchants;

namespace LookOn.Web;

public class LookOnWebAutoMapperProfile : Profile
{
    public LookOnWebAutoMapperProfile()
    {
        //Define your AutoMapper configuration here for the Web project.

        CreateMap<MerchantDto, MerchantUpdateDto>();

        CreateMap<RegistMerchantModel.RegistMerchantInput, MerchantCreateDto>();

        CreateMap<CategoryDto, CategoryUpdateDto>();

        CreateMap<UserInfoDto, UserInfoUpdateDto>();

        CreateMap<PlatformDto, PlatformUpdateDto>();

        CreateMap<MerchantStoreDto, MerchantStoreUpdateDto>();

        CreateMap<MerchantSubscriptionDto, MerchantSubscriptionUpdateDto>();

        CreateMap<MerchantUserDto, MerchantUserUpdateDto>();

        CreateMap<MerchantSyncInfoDto, MerchantSyncInfoUpdateDto>();

        CreateMap<AppSubscriptionResponse, AppSubscriptionInput>()
           .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToEnumOrDefaultIgnoreCase<HaravanSubscriptionStatus>()));
    }
}