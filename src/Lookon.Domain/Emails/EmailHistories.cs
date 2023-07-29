using System;
using LookOn.Enums;
using Volo.Abp.Domain.Entities.Auditing;

namespace LookOn.Emails;

public class EmailHistories : AuditedEntity<Guid>
{
    public Guid?     MerchantId   { get; set; }
    public EmailType EmailType    { get; set; }
    public DateTime? SendAt    { get; set; }
    public bool      IsSuccess    { get; set; }
    public string    Notification { get; set; }
}