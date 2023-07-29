using System;
using System.Collections.Generic;
using Volo.Abp.Domain.Entities.Auditing;

namespace LookOn.Integrations.Datalytis.Models.Entities;

public class DatalytisUserSocialInsight : AuditedEntity<Guid>
{
    public string    Uid          { get; set; }
    public DateTime? LastSyncedAt { get; set; }

    // public List<UserSocialInsight> Insights     { get; set; }

    // UserSocialInsight
    public string                              InsightId   { get; set; }
    public string                              Name        { get; set; }

    public string                    Url      { get; set; }
    public UserSocialInsightCategory Category { get; set; }


    //  "2022-02-07 11:30:38"
    public DateTime? CreatedTime { get; set; }
    public string    Location    { get; set; }
    public int       Type        { get; set; }
}

public class UserSocialInsightCategoryItem
{
    public string Id   { get; set; }
    public string Name { get; set; }
}

public class UserSocialInsightCategory
{
    public string                              Category     { get; set; }
    public List<UserSocialInsightCategoryItem> CategoryList { get; set; }
}

// public class UserSocialInsight
// {
//     public string           Id          { get; set; }
//     public string           Name        { get; set; }
//     public string           Category    { get; set; }
//     public List<SocialPage> CategoryList { get; set; }
//     public string           Url         { get; set; }
//
//     //  "2022-02-07 11:30:38"
//     public DateTime? CreatedTime { get; set; }
//     public string    Location    { get; set; }
//     public int       Type        { get; set; }
// }