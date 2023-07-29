using Volo.Abp.Domain.Entities.Auditing;

namespace LookOn.Integrations.Haravan.Models.Entities;

public class HaravanCustomer : AuditedEntity<Guid>
{
    public Guid? StoreId         { get; set; }
    public Guid? MerchantId      { get; set; }
    public Guid? MerchantStoreId { get; set; }

    // from json
    public bool?                 AcceptsMarketing    { get; set; }
    public List<HaravanAddress>  Addresses           { get; set; }
    public DateTime?             CreatedAt           { get; set; }
    public HaravanDefaultAddress DefaultAddress      { get; set; }
    public string                Email               { get; set; }
    public string                Phone               { get; set; }
    public string                FirstName           { get; set; }
    public long?                 CustomerId          { get; set; }
    public string                LastName            { get; set; }
    public long?                 LastOrderId         { get; set; }
    public string                LastOrderName       { get; set; }
    public string                Note                { get; set; }
    public long?                 OrdersCount         { get; set; }
    public string                State               { get; set; }
    public string                Tags                { get; set; }
    public decimal?              TotalSpent          { get; set; }
    public DateTime?             UpdatedAt           { get; set; }
    public bool?                 VerifiedEmail       { get; set; }
    public DateTime?             Birthday            { get; set; }
    public int?                  Gender              { get; set; }
    public DateTime?             LastOrderDate       { get; set; }
    public string                MultipassIdentifier { get; set; }
}

public class HaravanAddress
{
    public string Address1     { get; set; }
    public string Address2     { get; set; }
    public string City         { get; set; }
    public string Company      { get; set; }
    public string Country      { get; set; }
    public string FirstName    { get; set; }
    public long?  Id           { get; set; }
    public string LastName     { get; set; }
    public string Phone        { get; set; }
    public string Province     { get; set; }
    public string Zip          { get; set; }
    public string Name         { get; set; }
    public string ProvinceCode { get; set; }
    public string CountryCode  { get; set; }
    public bool?  Default      { get; set; }
    public string District     { get; set; }
    public string DistrictCode { get; set; }
    public string Ward         { get; set; }
    public string WardCode     { get; set; }
}

public class HaravanDefaultAddress
{
    public string Address1     { get; set; }
    public string Address2     { get; set; }
    public string City         { get; set; }
    public string Company      { get; set; }
    public string Country      { get; set; }
    public string FirstName    { get; set; }
    public long?  Id           { get; set; }
    public string LastName     { get; set; }
    public string Phone        { get; set; }
    public string Province     { get; set; }
    public string Zip          { get; set; }
    public string Name         { get; set; }
    public string ProvinceCode { get; set; }
    public string CountryCode  { get; set; }
    public bool?  Default      { get; set; }
    public string District     { get; set; }
    public string DistrictCode { get; set; }
    public string Ward         { get; set; }
    public string WardCode     { get; set; }
}