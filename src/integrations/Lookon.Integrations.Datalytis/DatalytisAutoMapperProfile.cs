using System;
using AutoMapper;
using LookOn.Core.Extensions;
using LookOn.Core.Helpers;
using LookOn.Enums;
using LookOn.Integrations.Datalytis.Helpers;
using LookOn.Integrations.Datalytis.Models.Entities;
using LookOn.Integrations.Datalytis.Models.RawModels;
using Volo.Abp.AutoMapper;

namespace LookOn.Integrations.Datalytis;

public class DatalytisAutoMapperProfile : Profile
{
    public DatalytisAutoMapperProfile()
    {
        CreateMap<DatalytisUserRaw, DatalytisUser>()
           .Ignore(x => x.MerchantFbPageIds)
           .ForMember(x => x.Phone,
                      act =>
                      {
                          act.NullSubstitute(string.Empty);
                          act.AddTransform(_ => _.Trim());
                      })
           .ForMember(x => x.Phone,              act => { act.MapFrom(x => x.Phone.ToInternationalPhoneNumberFromVN()); })
           .ForMember(x => x.Birthday,           act => { act.MapFrom(x => DatalytisMapper.ToDatalytisBirthday(x.Birthday)); })
           .ForMember(x => x.Gender,             act => { act.MapFrom(x => DatalytisMapper.ToGenderType(x.Sex)); })
           .ForMember(x => x.RelationshipStatus, act => { act.MapFrom(x => DatalytisMapper.ToRelationshipStatus(x.Relationship)); });

        CreateMap<UserSocialInsightRaw, DatalytisUserSocialInsight>()
           .Ignore(x => x.Id)
           .IgnoreAuditedObjectProperties()
           .ForMember(x => x.InsightId,   atc => { atc.MapFrom(x => x.Id); })
           .ForMember(x => x.CreatedTime, atc => { atc.MapFrom(x => x.CreatedTime.ToNullableDateTime()); });
        CreateMap<UserSocialInsightCategoryItemRaw, UserSocialInsightCategoryItem>();
        CreateMap<UserSocialInsightCategoryRaw, UserSocialInsightCategory>();
    }
}