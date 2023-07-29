using System;
using System.Collections.Generic;
using LookOn.Enums;

namespace LookOn.Dashboards.Page3;

public class GetMetricsInput
{
    public Guid          MerchantId         { get; set; }
    public List<string>  SocialCommunityIds { get; set; }
    public DynamicFilter Filter             { get; set; }
}

public class DynamicFilter
{
    public IList<GenderType>         GenderTypes        { get; set; }
    public IList<RelationshipStatus> RelationshipStatus { get; set; }
    public IList<AgeSegmentEnum>     AgeSegmentEnums    { get; set; }
    public IList<string>             Cities             { get; set; }
    public bool?                     CarOwner           { get; set; }
    public bool?                     HouseOwner         { get; set; }
}