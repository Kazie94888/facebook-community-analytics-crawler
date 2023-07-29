using LookOn.MerchantSyncInfos;
using LookOn.MerchantUsers;
using LookOn.MerchantSubscriptions;
using LookOn.MerchantStores;
using LookOn.Platforms;
using LookOn.UserInfos;
using LookOn.Categories;
using System;
using LookOn.Shared;
using LookOn.Users;
using Volo.Abp.AutoMapper;
using LookOn.Merchants;
using AutoMapper;
using LookOn.Dashboards.DashboardBase;
using LookOn.Dashboards.DashboardBases;
using LookOn.Dashboards.Page1;
using LookOn.Dashboards.Page2;
using LookOn.Dashboards.Page3;
using LookOn.Feedbacks;
using LookOn.Insights;
using Category = LookOn.Categories.Category;

namespace LookOn;

public class LookOnApplicationAutoMapperProfile : Profile
{
    public LookOnApplicationAutoMapperProfile()
    {
        /* You can configure your AutoMapper mapping configuration here.
         * Alternatively, you can split your mapping configurations
         * into multiple profile classes for a better organization. */
        CreateMap<AppUser, AppUserDto>().ReverseMap();
        CreateMap<MerchantCreateDto, Merchant>().IgnoreFullAuditedObjectProperties().Ignore(x => x.Id);
        CreateMap<MerchantUpdateDto, Merchant>().IgnoreFullAuditedObjectProperties().Ignore(x => x.Id);

        CreateMap<Merchant, MerchantDto>();
        CreateMap<MerchantWithNavigationProperties, MerchantWithNavigationPropertiesDto>();
        CreateMap<MetricConfigs, MetricConfigsDto>();
        CreateMap<MetricConfigsDto, MetricConfigs>();
        CreateMap<Merchants.MerchantSocialCommunity, MerchantSocialCommunityDto>().ReverseMap();
        CreateMap<MerchantUserScan, MerchantSocialUserSyncInfo>().ReverseMap();
        CreateMap<MerchantEcomScan, MerchantEcomScanDto>().ReverseMap();

        // CreateMap<MerchantSocialSyncInfo_User, MerchantSocialUserSyncInfo>().ReverseMap();
        CreateMap<AppUser, LookupDto<Guid>>().ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.Email));

        CreateMap<CategoryCreateDto, Category>().IgnoreFullAuditedObjectProperties().Ignore(x => x.Id);
        CreateMap<CategoryUpdateDto, Category>().IgnoreFullAuditedObjectProperties().Ignore(x => x.Id);
        CreateMap<Category, CategoryDto>();

        CreateMap<Category, LookupDto<Guid?>>().ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.Name));

        CreateMap<UserInfoCreateDto, UserInfo>().IgnoreFullAuditedObjectProperties().Ignore(x => x.Id);
        CreateMap<UserInfoUpdateDto, UserInfo>().IgnoreFullAuditedObjectProperties().Ignore(x => x.Id);
        CreateMap<UserInfo, UserInfoDto>();
        CreateMap<UserInfoWithNavigationProperties, UserInfoWithNavigationPropertiesDto>();
        CreateMap<UserDto, AppUser>().IgnoreFullAuditedObjectProperties().Ignore(x => x.Id).ReverseMap();
        CreateMap<AppUser, LookupDto<Guid?>>().ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.UserName));

        CreateMap<UserInfoCreateDto, UserInfo>().IgnoreFullAuditedObjectProperties().Ignore(x => x.Id);
        CreateMap<UserInfoUpdateDto, UserInfo>().IgnoreFullAuditedObjectProperties().Ignore(x => x.Id);

        CreateMap<PlatformCreateDto, Platform>().IgnoreFullAuditedObjectProperties().Ignore(x => x.Id);
        CreateMap<PlatformUpdateDto, Platform>().IgnoreFullAuditedObjectProperties().Ignore(x => x.Id);
        CreateMap<Platform, PlatformDto>();

        CreateMap<PlatformCreateDto, Platform>().IgnoreFullAuditedObjectProperties().Ignore(x => x.Id);
        CreateMap<PlatformUpdateDto, Platform>().IgnoreFullAuditedObjectProperties().Ignore(x => x.Id);

        CreateMap<MerchantStoreCreateDto, MerchantStore>().IgnoreFullAuditedObjectProperties().Ignore(x => x.Id);
        CreateMap<MerchantStoreUpdateDto, MerchantStore>().IgnoreFullAuditedObjectProperties().Ignore(x => x.Id);
        CreateMap<MerchantStore, MerchantStoreDto>();
        CreateMap<MerchantStoreWithNavigationProperties, MerchantStoreWithNavigationPropertiesDto>();
        CreateMap<Merchant, LookupDto<Guid?>>().ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.Name));
        CreateMap<Platform, LookupDto<Guid?>>().ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.Name));

        CreateMap<MerchantSubscriptionCreateDto, MerchantSubscription>().IgnoreFullAuditedObjectProperties().Ignore(x => x.Id);
        CreateMap<MerchantSubscriptionUpdateDto, MerchantSubscription>().IgnoreFullAuditedObjectProperties().Ignore(x => x.Id);
        CreateMap<MerchantSubscription, MerchantSubscriptionDto>();
        CreateMap<MerchantSubscriptionWithNavigationProperties, MerchantSubscriptionWithNavigationPropertiesDto>();

        CreateMap<EcomMetric_Summary, EcomMetric_SummaryDto>();
        CreateMap<EcomMetric_RevenueSummary, EcomMetric_RevenueSummaryDto>();
        CreateMap<EcomMetric_RevenueByDate, EcomMetric_RevenueByDateDto>();
        CreateMap<EcomMetric_RevenueByProduct, EcomMetric_RevenueByProductDto>();
        CreateMap<EcomMetric_RevenueByLocation, EcomMetric_RevenueByLocationDto>();

        CreateMap<SocialMetric_Demographic, SocialMetric_DemographicDto>();
        CreateMap<SocialMetric_Gender, SocialMetric_GenderDto>();
        CreateMap<SocialMetric_AgeRange, SocialMetric_AgeRangeDto>();
        CreateMap<SocialMetric_Relationship, SocialMetric_RelationshipDto>();
        CreateMap<SocialMetric_LocationByProvince, SocialMetric_LocationByProvinceDto>();
        CreateMap<SocialMetric_CommunityInteraction, SocialMetric_CommunityInteractionDto>();
        CreateMap<SocialMetric_TopFollower, SocialMetric_TopFollowerDto>();
        CreateMap<SocialMetric_TopLikedPage, SocialMetric_TopLikedPageDto>();
        CreateMap<SocialMetric_TopCheckinLocation, SocialMetric_TopCheckinLocationDto>();
        CreateMap<SocialMetric_TopGroup, SocialMetric_TopGroupDto>();

       
        CreateMap<GenderComparision, GenderComparisionDto>();
        CreateMap<AgeComparision, AgeComparisionDto>();
        CreateMap<CarOwnerComparision, CarOwnerComparisionDto>();
        CreateMap<HouseOwnerComparision, HouseOwnerComparisionDto>();
        CreateMap<RelationshipComparision, RelationshipComparisionDto>();
        
        CreateMap<Metric, MetricDto>();
        CreateMap<MetricItem, MetricItemDto>();
        CreateMap<InsightUser, InsightUserDto>();
        CreateMap<EcomMetric_Advanced, EcomMetric_AdvancedDto>();
        CreateMap<SocialMetric_Insight, SocialMetric_InsightDto>();
        CreateMap<SocialMetric_Comparision, SocialMetric_ComparisionDto>();

        CreateMap<MerchantUserCreateDto, MerchantUser>().IgnoreAuditedObjectProperties().Ignore(x => x.Id);
        CreateMap<MerchantUserUpdateDto, MerchantUser>().IgnoreAuditedObjectProperties().Ignore(x => x.Id);
        CreateMap<MerchantUser, MerchantUserDto>();
        CreateMap<MerchantUserWithNavigationProperties, MerchantUserWithNavigationPropertiesDto>();
        CreateMap<Merchant, LookupDto<Guid>>().ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.Name));

        CreateMap<MerchantSyncInfoCreateDto, MerchantSyncInfo>().IgnoreFullAuditedObjectProperties().Ignore(x => x.Id);
        CreateMap<MerchantSyncInfoUpdateDto, MerchantSyncInfo>().IgnoreFullAuditedObjectProperties().Ignore(x => x.Id);
        CreateMap<MerchantSyncInfo, MerchantSyncInfoDto>();
        CreateMap<MerchantSyncInfoWithNavigationProperties, MerchantSyncInfoWithNavigationPropertiesDto>();
        CreateMap<Merchant, LookupDto<Guid?>>().ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.Email));

        CreateMap<MerchantSyncInfoCreateDto, MerchantSyncInfo>().IgnoreFullAuditedObjectProperties().Ignore(x => x.Id);
        CreateMap<MerchantSyncInfoUpdateDto, MerchantSyncInfo>().IgnoreFullAuditedObjectProperties().Ignore(x => x.Id);

        CreateMap<UserFeedbackDto, UserFeedback>();
    }
}