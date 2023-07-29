using LookOn.Integrations.Haravan.Models.Enums;
using Volo.Abp.Domain.Entities.Auditing;

namespace LookOn.Integrations.Haravan.Models.Entities;

public class HaravanOrder : AuditedEntity<Guid>
{
    public HaravanBillingAddress        BillingAddress        { get; set; }
    public string                       BrowserIp             { get; set; }
    public bool?                        BuyerAcceptsMarketing { get; set; }
    public string                       CancelReason          { get; set; }
    public DateTime?                    CancelledAt           { get; set; }
    public string                       CartToken             { get; set; }
    public string                       CheckoutToken         { get; set; }
    public HaravanClientDetails         HaravanClientDetails  { get; set; }
    public DateTime?                    ClosedAt              { get; set; }
    public DateTime?                    CreatedAt             { get; set; }
    public string                       Currency              { get; set; }
    public long?                        HaravanCustomerId     { get; set; }
    public List<HaravanDiscountCode>    DiscountCodes         { get; set; }
    public string                       Email                 { get; set; }
    public HaravanFinancialStatus       FinancialStatus       { get; set; }
    public List<HaravanFulfillment>     Fulfillments          { get; set; }
    public HaravanFulfillmentStatus     FulfillmentStatus     { get; set; }
    public string                       Tags                  { get; set; }
    public string                       Gateway               { get; set; }
    public string                       GatewayCode           { get; set; }
    public long?                        OrderId               { get; set; }
    public string                       LandingSite           { get; set; }
    public string                       LandingSiteRef        { get; set; }
    public string                       Source                { get; set; }
    public List<HaravanLineItem>        LineItems             { get; set; }
    public string                       Name                  { get; set; }
    public string                       Note                  { get; set; }
    public int                          Number                { get; set; }
    public string                       OrderNumber           { get; set; }
    public object                       ProcessingMethod      { get; set; }
    public string                       ReferringSite         { get; set; }
    public List<HaravanRefund>          Refunds               { get; set; }
    public HaravanShippingAddress       ShippingAddress       { get; set; }
    public List<HaravanShippingLine>    ShippingLines         { get; set; }
    public string                       SourceName            { get; set; }
    public decimal?                     SubtotalPrice         { get; set; }
    public string                       TaxLines              { get; set; }
    public bool?                        TaxesIncluded         { get; set; }
    public string                       Token                 { get; set; }
    public decimal?                     TotalDiscounts        { get; set; }
    public decimal?                     TotalLineItemsPrice   { get; set; }
    public decimal?                     TotalPrice            { get; set; }
    public decimal?                     TotalTax              { get; set; }
    public decimal?                     TotalWeight           { get; set; }
    public DateTime?                    UpdatedAt             { get; set; }
    public List<HaravanTransaction>     Transactions          { get; set; }
    public List<HaravanNoteAttribute>   NoteAttributes        { get; set; }
    public DateTime?                    ConfirmedAt           { get; set; }
    public HaravanClosedStatus          ClosedStatus          { get; set; }
    public HaravanCancelledStatus       CancelledStatus       { get; set; }
    public HaravanConfirmedStatus       ConfirmedStatus       { get; set; }
    public long?                        AssignedLocationId    { get; set; }
    public string                       AssignedLocationName  { get; set; }
    public DateTime?                    AssignedLocationAt    { get; set; }
    public DateTime?                    ExportedConfirmAt     { get; set; }
    public long?                        UserId                { get; set; }
    public long?                        DeviceId              { get; set; }
    public long?                        LocationId            { get; set; }
    public string                       LocationName          { get; set; }
    public long?                        RefOrderId            { get; set; }
    public DateTime?                    RefOrderDate          { get; set; }
    public string                       RefOrderNumber        { get; set; }
    public string                       UtmSource             { get; set; }
    public string                       UtmMedium             { get; set; }
    public string                       UtmCampaign           { get; set; }
    public string                       UtmTerm               { get; set; }
    public string                       UtmContent            { get; set; }
    public string                       PaymentUrl            { get; set; }
    public string                       ContactEmail          { get; set; }
    public HaravanOrderProcessingStatus OrderProcessingStatus { get; set; }
    public long?                        PrevOrderId           { get; set; }
    public string                       PrevOrderNumber       { get; set; }
    public DateTime?                    PrevOrderDate         { get; set; }
    public HaravanRedeemModel           HaravanRedeemModel    { get; set; }
    public Guid?                        StoreId               { get; set; }
    public Guid?                        MerchantId            { get; set; }
    public Guid?                        MerchantStoreId       { get; set; }

    public HaravanOrder()
    {
    }
}

public class HaravanRedeemModel
{
    public long?    PayloadId     { get; set; }
    public decimal? UsedAmount    { get; set; }
    public long?    TransactionId { get; set; }
    public string   DiscountType  { get; set; }
    public decimal? Discount      { get; set; }
    public decimal? MaxPerOrder   { get; set; }
    public decimal? Amount        { get; set; }
    public string   Name          { get; set; }
}

public class HaravanNoteAttribute
{
    public string Name  { get; set; }
    public string Value { get; set; }
}

public class HaravanShippingLine
{
    public string   Code   { get; set; }
    public decimal? Price  { get; set; }
    public string   Source { get; set; }
    public string   Title  { get; set; }
}

public class HaravanBillingAddress
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

public class HaravanShippingAddress
{
    public string   Address1     { get; set; }
    public string   Address2     { get; set; }
    public string   City         { get; set; }
    public string   Company      { get; set; }
    public string   Country      { get; set; }
    public string   FirstName    { get; set; }
    public string   LastName     { get; set; }
    public decimal? Latitude     { get; set; }
    public decimal? Longitude    { get; set; }
    public string   Phone        { get; set; }
    public string   Province     { get; set; }
    public string   Zip          { get; set; }
    public string   Name         { get; set; }
    public string   ProvinceCode { get; set; }
    public string   CountryCode  { get; set; }
    public string   District     { get; set; }
    public string   DistrictCode { get; set; }
    public string   Ward         { get; set; }
    public string   WardCode     { get; set; }
}

public class HaravanClientDetails
{
    public string   AcceptLanguage { get; set; }
    public string   BrowserIp      { get; set; }
    public string   SessionHash    { get; set; }
    public string   UserAgent      { get; set; }
    public decimal? BrowserHeight  { get; set; }
    public decimal? BrowserWidth   { get; set; }
}

public class HaravanDiscountCode
{
    public decimal? Amount       { get; set; }
    public string   Code         { get; set; }
    public string   Type         { get; set; }
    public bool?    IsCouponCode { get; set; }
}

public class HaravanProperty
{
    public string Name  { get; set; }
    public string Value { get; set; }
}

public class HaravanAppliedDiscount
{
    public string   Description { get; set; }
    public decimal? Amount      { get; set; }
}

public class HaravanImage
{
    public string Src { get; set; }
}

public class HaravanFulfillment
{
    public DateTime?             CreatedAt                 { get; set; }
    public long?                 Id                        { get; set; }
    public long?                 OrderId                   { get; set; }
    public string                Receipt                   { get; set; }
    public string                Status                    { get; set; }
    public string                TrackingCompany           { get; set; }
    public string                TrackingCompanyCode       { get; set; }
    public List<string>          TrackingNumbers           { get; set; }
    public string                TrackingNumber            { get; set; }
    public string                TrackingUrl               { get; set; }
    public List<string>          TrackingUrls              { get; set; }
    public DateTime?             UpdatedAt                 { get; set; }
    public List<HaravanLineItem> LineItems                 { get; set; }
    public string                Province                  { get; set; }
    public string                ProvinceCode              { get; set; }
    public string                District                  { get; set; }
    public string                DistrictCode              { get; set; }
    public string                Ward                      { get; set; }
    public string                WardCode                  { get; set; }
    public decimal?              CodAmount                 { get; set; }
    public string                CarrierStatusName         { get; set; }
    public string                CarrierCodStatusName      { get; set; }
    public string                CarrierStatusCode         { get; set; }
    public string                CarrierCodStatusCode      { get; set; }
    public long?                 LocationId                { get; set; }
    public string                LocationName              { get; set; }
    public string                Note                      { get; set; }
    public string                CarrierServicePackageName { get; set; }
    public string                CouponCode                { get; set; }
    public DateTime?             ReadyToPickDate           { get; set; }
    public DateTime?             PickingDate               { get; set; }
    public DateTime?             DeliveringDate            { get; set; }
    public DateTime?             DeliveredDate             { get; set; }
    public DateTime?             ReturnDate                { get; set; }
    public DateTime?             NotMeetCustomerDate       { get; set; }
    public DateTime?             WaitingForReturnDate      { get; set; }
    public DateTime?             CodPaidDate               { get; set; }
    public DateTime?             CodReceiptDate            { get; set; }
    public DateTime?             CodPendingDate            { get; set; }
    public DateTime?             CodNotReceiptDate         { get; set; }
    public DateTime?             CancelDate                { get; set; }
    public bool?                 IsViewBefore              { get; set; }
    public string                Country                   { get; set; }
    public string                CountryCode               { get; set; }
    public string                ZipCode                   { get; set; }
    public string                City                      { get; set; }
    public decimal?              RealShippingFee           { get; set; }
    public string                ShippingNotes             { get; set; }
    public decimal?              TotalWeight               { get; set; }
    public decimal?              PackageLength             { get; set; }
    public decimal?              PackageWidth              { get; set; }
    public decimal?              PackageHeight             { get; set; }
    public string                BoxmeServicecode          { get; set; }
    public int?                  TransportType             { get; set; }
    public string                Address                   { get; set; }
    public string                SenderPhone               { get; set; }
    public string                SenderName                { get; set; }
    public string                CarrierServiceCode        { get; set; }
    public decimal?              FromLongtitude            { get; set; }
    public decimal?              FromLatitude              { get; set; }
    public decimal?              ToLongtitude              { get; set; }
    public decimal?              ToLatitude                { get; set; }
    public string                SortCode                  { get; set; }
    public bool?                 IsDropOff                 { get; set; }
    public bool?                 IsInsurance               { get; set; }
    public decimal?              InsurancePrice            { get; set; }
    public bool?                 IsOpenBox                 { get; set; }
    public string                RequestId                 { get; set; }
    public string                CarrierOptions            { get; set; }
    public string                NoteAttributes            { get; set; }
    public string                FirstName                 { get; set; }
    public string                LastName                  { get; set; }
    public string                ShippingAddress           { get; set; }
    public string                ShippingPhone             { get; set; }
}

public class HaravanRefund
{
    public DateTime?                   CreatedAt       { get; set; }
    public long?                       Id              { get; set; }
    public string                      Note            { get; set; }
    public List<HaravanRefundLineItem> RefundLineItems { get; set; }
    public string                      Restock         { get; set; }
    public string                      UserId          { get; set; }
    public long?                       OrderId         { get; set; }
    public long?                       LocationId      { get; set; }
    public List<HaravanTransaction>    Transactions    { get; set; }
}

public class HaravanTransaction
{
    public decimal?                 Amount                { get; set; }
    public string                   Authorization         { get; set; }
    public DateTime?                CreatedAt             { get; set; }
    public string                   DeviceId              { get; set; }
    public string                   Gateway               { get; set; }
    public long?                    Id                    { get; set; }
    public string                   Kind                  { get; set; }
    public long?                    OrderId               { get; set; }
    public string                   Receipt               { get; set; }
    public HaravanTransactionStatus Status                { get; set; }
    public string                   UserId                { get; set; }
    public long?                    LocationId            { get; set; }
    public string                   PaymentDetails        { get; set; }
    public long?                    ParentId              { get; set; }
    public string                   Currency              { get; set; }
    public string                   HaravanTransactionId  { get; set; }
    public string                   ExternalTransactionId { get; set; }
}

public class HaravanRefundLineItem
{
    public long?           Id              { get; set; }
    public HaravanLineItem HaravanLineItem { get; set; }
    public long?           LineItemId      { get; set; }
    public long?           Quantity        { get; set; }
}

public class HaravanLineItem
{
    public int?                         FulfillableQuantity        { get; set; }
    public string                       FulfillmentService         { get; set; }
    public HaravanFulfillmentStatus     FulfillmentStatus          { get; set; }
    public decimal?                     Grams                      { get; set; }
    public long?                        Id                         { get; set; }
    public decimal?                     Price                      { get; set; }
    public int?                         ProductId                  { get; set; }
    public int?                         Quantity                   { get; set; }
    public bool?                        RequiresShipping           { get; set; }
    public string                       Sku                        { get; set; }
    public string                       Title                      { get; set; }
    public int?                         VariantId                  { get; set; }
    public string                       VariantTitle               { get; set; }
    public string                       Vendor                     { get; set; }
    public string                       Name                       { get; set; }
    public string                       VariantInventoryManagement { get; set; }
    public List<HaravanProperty>        Properties                 { get; set; }
    public bool?                        ProductExists              { get; set; }
    public decimal?                     PriceOriginal              { get; set; }
    public decimal?                     PricePromotion             { get; set; }
    public string                       Type                       { get; set; }
    public bool?                        GiftCard                   { get; set; }
    public bool?                        Taxable                    { get; set; }
    public string                       TaxLines                   { get; set; }
    public string                       Barcode                    { get; set; }
    public List<HaravanAppliedDiscount> AppliedDiscounts           { get; set; }
    public decimal?                     TotalDiscount              { get; set; }
    public HaravanImage                 HaravanImage               { get; set; }
    public bool?                        NotAllowPromotion          { get; set; }
    public decimal?                     MaCostAmount               { get; set; }
    public decimal?                     ActualPrice                { get; set; }
}