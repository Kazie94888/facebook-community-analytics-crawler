using System;
using LookOn.Enums;

namespace LookOn.HaravanWebhooks;

public class AppSubscriptionInput
{
    public string Id { get; set; }

    public string ClientId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? ExpiredAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public DateTime? CancelledAt { get; set; }

    public decimal Total { get; set; }

    public decimal AmountDiscount { get; set; }

    public decimal AmountPaid { get; set; }

    public decimal Price    { get; set; }
    public int     Quantity { get; set; }

    public HaravanSubscriptionStatus Status { get; set; }

    public string PlanId { get; set; }

    public string Name { get; set; }

    
    public string StoreCode { get; set; }
}