using System;
using System.Collections.Generic;
using System.Linq;
using LookOn.Enums;

namespace LookOn.Dashboards.Page3;

public class Page3DataRequest
{
    public Guid         MerchantId         { get; set; }
    public List<string> SocialCommunityIds { get; set; }
    public Page3Filter  Filter             { get; set; }
}

public class Page3Filter
{
    public IList<GenderType>         GenderTypes        { get; set; }
    public IList<RelationshipStatus> RelationshipStatus { get; set; }
    public IList<AgeSegmentEnum>     AgeSegmentEnums    { get; set; }
    public IList<string>             Cities             { get; set; }
    public bool?                     CarOwner           { get; set; }

    public bool IsMatching(Page3Filter page3Filter)
    {
        return IsMatchingGenderTypes(page3Filter.GenderTypes)
            && IsMatchingRelationshipStatus(page3Filter.RelationshipStatus)
            && IsMatchingAgeSegmentEnums(page3Filter.AgeSegmentEnums)
            && IsMatchingCities(page3Filter.Cities)
            && IsMatchingCarOwner(page3Filter.CarOwner);
    }

    private bool IsMatchingGenderTypes(IList<GenderType> genderTypes)
    {
        return (GenderTypes == null && genderTypes == null)
            || (GenderTypes       != null
             && genderTypes       != null
             && genderTypes.Count == GenderTypes.Count
             && genderTypes.All(genderType => GenderTypes.Contains(genderType)));
    }

    private bool IsMatchingRelationshipStatus(IList<RelationshipStatus> relationshipStatus)
    {
        return (RelationshipStatus == null && relationshipStatus == null)
            || (RelationshipStatus       != null
             && relationshipStatus       != null
             && relationshipStatus.Count == RelationshipStatus.Count
             && relationshipStatus.All(x => RelationshipStatus.Contains(x)));
    }

    private bool IsMatchingAgeSegmentEnums(IList<AgeSegmentEnum> ageSegmentEnums)
    {
        return (AgeSegmentEnums == null && ageSegmentEnums == null)
            || (AgeSegmentEnums       != null
             && ageSegmentEnums       != null
             && ageSegmentEnums.Count == AgeSegmentEnums.Count
             && ageSegmentEnums.All(x => AgeSegmentEnums.Contains(x)));
    }

    private bool IsMatchingCities(IList<string> cities)
    {
        return (Cities == null && cities == null)
            || (Cities != null && cities != null && cities.Count == Cities.Count && cities.All(x => Cities.Contains(x)));
    }

    private bool IsMatchingCarOwner(bool? carOwner)
    {
        return carOwner == CarOwner;
    }
}