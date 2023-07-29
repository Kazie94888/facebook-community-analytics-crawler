using LookOn.Core.Extensions;
using LookOn.Integrations.Haravan.Models.Enums;

namespace LookOn.Integrations.Haravan.Helpers;

public static class HaravanMapper
{
    public static HaravanClosedStatus ToClosedStatus(string value)
    {
        if (value.IsNullOrWhiteSpace()) return HaravanClosedStatus.Unknown;
    
        var unclosed = new[] { "unclosed", "un_closed", "un closed"};
        var closed   = new[] { "closed" };
    
        if (value.IsInIgnoreCase(unclosed)) return HaravanClosedStatus.Unclosed;
        if (value.IsInIgnoreCase(closed)) return HaravanClosedStatus.Closed;
    
        return HaravanClosedStatus.Unknown;
    }
    public static HaravanCancelledStatus ToCancelledStatus(string value)
    {
        if (value.IsNullOrWhiteSpace()) return HaravanCancelledStatus.Unknown;
    
        var uncancelled = new[] { "uncancelled", "un_cancelled", "un cancelled"};
        var cancelled   = new[] { "cancelled","cancel" };
    
        if (value.IsInIgnoreCase(uncancelled)) return HaravanCancelledStatus.Uncancelled;
        if (value.IsInIgnoreCase(cancelled)) return HaravanCancelledStatus.Cancelled;
    
        return HaravanCancelledStatus.Unknown;
    }
    public static HaravanConfirmedStatus ToConfirmedStatus(string value)
    {
        if (value.IsNullOrWhiteSpace()) return HaravanConfirmedStatus.Unknown;
    
        var unconfirmed = new[] { "unconfirmed", "un_confirmed", "un confirmed"};
        var confirmed   = new[] { "confirmed","confirm" };
    
        if (value.IsInIgnoreCase(unconfirmed)) return HaravanConfirmedStatus.Unconfirmed;
        if (value.IsInIgnoreCase(confirmed)) return HaravanConfirmedStatus.Confirmed;
    
        return HaravanConfirmedStatus.Unknown;
    }
    public static HaravanOrderProcessingStatus ToOrderProcessingStatus(string value)
    {
        if (value.IsNullOrWhiteSpace()) return HaravanOrderProcessingStatus.Unknown;
    
        var cancel           = new[] { "cancel", "cancelled"};
        var self_delivery    = new[] { "self_delivery","self delivery", "selfdelivery" };
        var pending          = new[] { "pending" };
        var carrier_delivery = new[] { "carrier_delivery", "carrierdelivery", "carrier delivery", };
        var confirmed        = new[] { "confirmed", "confirm" };
        var complete         = new[] { "complete", "completed" };
    
        if (value.IsInIgnoreCase(cancel)) return HaravanOrderProcessingStatus.Cancel;
        if (value.IsInIgnoreCase(self_delivery)) return HaravanOrderProcessingStatus.SelfDelivery;
    
        if (value.IsInIgnoreCase(pending)) return HaravanOrderProcessingStatus.Pending;
        if (value.IsInIgnoreCase(carrier_delivery)) return HaravanOrderProcessingStatus.CarrierDelivery;
        if (value.IsInIgnoreCase(confirmed)) return HaravanOrderProcessingStatus.Confirmed;
        if (value.IsInIgnoreCase(complete)) return HaravanOrderProcessingStatus.Complete;
    
        return HaravanOrderProcessingStatus.Unknown;
    }
    
}