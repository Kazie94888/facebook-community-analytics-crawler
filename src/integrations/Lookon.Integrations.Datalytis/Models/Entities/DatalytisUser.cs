using System;
using System.Collections.Generic;
using LookOn.Enums;
using LookOn.Integrations.Datalytis.Models.Enums;
using Volo.Abp.Domain.Entities.Auditing;

namespace LookOn.Integrations.Datalytis.Models.Entities;

public class DatalytisUser : AuditedEntity<Guid>
{
    public DatalytisUser()
    {
        MerchantFbPageIds = new List<string>();
    }

    public List<string> MerchantFbPageIds { get; set; }
    public string       Uid               { get; set; }

    // 84383960591
    public string Phone    { get; set; }
    public string Email    { get; set; }
    public string Name     { get; set; }
    public string FullName { get; set; }

    //"1989-07-31"
    public DateTime?          Birthday           { get; set; }
    public string             Sex                { get; set; }
    public GenderType         Gender             { get; set; }
    public string             Relationship       { get; set; }
    public RelationshipStatus RelationshipStatus { get; set; }
    public string             Address            { get; set; }
    public string             City               { get; set; }
    public int                Friends            { get; set; }
    public int                Follow             { get; set; }
    public string             Cmnd               { get; set; }

    // Notes
    public string    Note1              { get; set; }
    public string    Note2              { get; set; }
    public string    Note3              { get; set; }
    public string    Note4              { get; set; }
    public string    Note5              { get; set; }
    public string    Note6              { get; set; }
    public string    Note7              { get; set; }
    public DateTime? InsightRequestedAt { get; set; }
    public bool      IsQualified        => Gender != GenderType.Unknown && Birthday != null && RelationshipStatus != RelationshipStatus.Unknown;
}