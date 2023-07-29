using System;
using LookOn.Enums;

namespace LookOn.RequestResponses;

// all merchant subscription requests here
public class SetSubscriptionInput
{
    public Guid             MerchantId       { get; set; }
    public SubscriptionType SubscriptionType { get; set; }
    public DateTime         From             { get; set; }
}
public class UpdateSubscriptionStatusInput
{
    public Guid               MerchantSubscriptionId         { get; set; }
    public SubscriptionStatus SubscriptionStatus { get; set; }
}
