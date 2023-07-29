using AutoMapper;
using LookOn.Core.Extensions;
using LookOn.Integrations.Haravan.Helpers;
using LookOn.Integrations.Haravan.Models.Entities;
using LookOn.Integrations.Haravan.Models.Enums;
using LookOn.Integrations.Haravan.Models.RawModels;
using Volo.Abp.AutoMapper;

namespace LookOn.Integrations.Haravan;

public class HaravanAutoMapperProfile : Profile
{
    private long? MapHaravanCustomerId(HRVOrderRaw raw)
    {
        return raw.Customer == null ? -1 : raw.Customer.Id;
    }
    
    public HaravanAutoMapperProfile()
    {
        CreateMap<HRVOrderRaw, HaravanOrder>()
           .IgnoreAuditedObjectProperties()
           .Ignore(des => des.Id)
           .Ignore(des => des.StoreId)
           .Ignore(des => des.MerchantId)
           .Ignore(des => des.MerchantStoreId)
           .Ignore(des => des.FinancialStatus)
           .Ignore(des => des.FulfillmentStatus)
           .ForMember(des => des.HaravanCustomerId, act => { act.MapFrom(src => MapHaravanCustomerId(src)); })
           .ForMember(des => des.FinancialStatus, act => { act.MapFrom(src => src.FinancialStatus.ToEnumOrDefault<HaravanFinancialStatus>()); })
           .ForMember(des => des.ClosedStatus, act => { act.MapFrom(src => HaravanMapper.ToClosedStatus(src.ClosedStatus)); })
           .ForMember(des => des.CancelledStatus, act => { act.MapFrom(src => HaravanMapper.ToCancelledStatus(src.CancelledStatus)); })
           .ForMember(des => des.ConfirmedStatus, act => { act.MapFrom(src => HaravanMapper.ToConfirmedStatus(src.ConfirmedStatus)); })
           .ForMember(des => des.OrderProcessingStatus, act => { act.MapFrom(src => HaravanMapper.ToOrderProcessingStatus(src.OrderProcessingStatus)); })
           .ForMember(des => des.FulfillmentStatus, act => { act.MapFrom(src => src.FulfillmentStatus.ToEnumOrDefault<HaravanFulfillmentStatus>()); });

        CreateMap<HRVBillingAddressRaw, HaravanBillingAddress>();
        CreateMap<HRVClientDetailsRaw, HaravanClientDetails>();
        CreateMap<HRVDiscountCodeRaw, HaravanDiscountCode>();
        CreateMap<HRVFulfillmentRaw, HaravanFulfillment>();
        
        CreateMap<HRVLineItemRaw, HaravanLineItem>()
           .ForMember(des => des.FulfillmentStatus, act => { act.MapFrom(src => src.FulfillmentStatus.ToEnumOrDefault<HaravanFulfillmentStatus>()); });
        
        CreateMap<HRVRefundRaw, HaravanRefund>();
        CreateMap<HRVShippingAddressRaw, HaravanShippingAddress>();
        CreateMap<HRVShippingLineRaw, HaravanShippingLine>();
        CreateMap<HRVTransactionRaw, HaravanTransaction>() //.Ignore(x => x.Status)
           .ForMember(des => des.Status, act => { act.MapFrom(src => src.Status.ToEnumOrDefault<HaravanTransactionStatus>()); });

        CreateMap<HRVNoteAttributeRaw, HaravanNoteAttribute>();
        CreateMap<HRVRefundLineItemRaw, HaravanRefundLineItem>();
        CreateMap<HRVAppliedDiscountRaw, HaravanAppliedDiscount>();
        CreateMap<HRVImageRaw, HaravanImage>();
        CreateMap<HRVPropertyRaw, HaravanProperty>();
        CreateMap<HRVRedeemModelRaw, HaravanRedeemModel>();

        CreateMap<HRVCustomerRaw, HaravanCustomer>()
           .IgnoreAuditedObjectProperties()
           .Ignore(x => x.Id)
           .Ignore(x => x.StoreId)
           .Ignore(x => x.MerchantId)
           .Ignore(x => x.MerchantStoreId)
           .ForMember(d => d.CustomerId, opt => opt.MapFrom(s => s.Id))
           .ForMember(d => d.Phone,      opt => opt.MapFrom(s => s.Phone.ToInternationalPhoneNumberFromVN()));
        CreateMap<HRVAddressRaw, HaravanAddress>();
        CreateMap<HRVDefaultAddressRaw, HaravanDefaultAddress>();
    }
}