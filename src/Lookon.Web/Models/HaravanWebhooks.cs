using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace LookOn.Web.Models;

public class AppSubscriptionResponse
{
    [JsonProperty("id")]
    public string Id { get; set; }

    [JsonProperty("client_id")]
    public string ClientId { get; set; }

    [JsonProperty("created_at")]
    public DateTime? CreatedAt { get; set; }

    [JsonProperty("expired_at")]
    public DateTime? ExpiredAt { get; set; }

    [JsonProperty("updated_at")]
    public DateTime? UpdatedAt { get; set; }

    [JsonProperty("cancelled_at")]
    public DateTime? CancelledAt { get; set; }

    [JsonProperty("total")]
    public double Total { get; set; }

    [JsonProperty("amount_discount")]
    public double AmountDiscount { get; set; }

    [JsonProperty("amount_paid")]
    public double AmountPaid { get; set; }

    [JsonProperty("quantity")]
    public double Quantity { get; set; }

    [JsonProperty("status")]
    public string Status { get; set; }

    [JsonProperty("plan_id")]
    public string PlanId { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("price")]
    public double Price { get; set; }
    
    [JsonIgnore]
    public string StoreCode { get; set; }
}

public class HaravanAppSubscriptionsResponse
{
    [JsonProperty("app_subscriptions")]
    public List<AppSubscriptionResponse> AppSubscriptions { get; set; }
}