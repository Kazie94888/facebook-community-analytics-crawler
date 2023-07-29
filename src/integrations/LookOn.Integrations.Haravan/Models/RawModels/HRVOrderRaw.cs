using Newtonsoft.Json;
using Volo.Abp.Domain.Entities.Auditing;

namespace LookOn.Integrations.Haravan.Models.RawModels
{
    public class HRVAddressRaw
    {
        [JsonProperty("address1")]      public string Address1     { get; set; }
        [JsonProperty("address2")]      public string Address2     { get; set; }
        [JsonProperty("city")]          public string City         { get; set; }
        [JsonProperty("company")]       public string Company      { get; set; }
        [JsonProperty("country")]       public string Country      { get; set; }
        [JsonProperty("first_name")]    public string FirstName    { get; set; }
        [JsonProperty("id")]            public long?  Id           { get; set; }
        [JsonProperty("last_name")]     public string LastName     { get; set; }
        [JsonProperty("phone")]         public string Phone        { get; set; }
        [JsonProperty("province")]      public string Province     { get; set; }
        [JsonProperty("zip")]           public string Zip          { get; set; }
        [JsonProperty("name")]          public string Name         { get; set; }
        [JsonProperty("province_code")] public string ProvinceCode { get; set; }
        [JsonProperty("country_code")]  public string CountryCode  { get; set; }
        [JsonProperty("default")]       public bool?  Default      { get; set; }
        [JsonProperty("district")]      public string District     { get; set; }
        [JsonProperty("district_code")] public string DistrictCode { get; set; }
        [JsonProperty("ward")]          public string Ward         { get; set; }
        [JsonProperty("ward_code")]     public string WardCode     { get; set; }
    }

    public class HRVAppliedDiscountRaw
    {
        [JsonProperty("description")] public string   Description { get; set; }
        [JsonProperty("amount")]      public decimal? Amount      { get; set; }
    }

    public class HRVBillingAddressRaw
    {
        [JsonProperty("address1")]      public string Address1     { get; set; }
        [JsonProperty("address2")]      public string Address2     { get; set; }
        [JsonProperty("city")]          public string City         { get; set; }
        [JsonProperty("company")]       public string Company      { get; set; }
        [JsonProperty("country")]       public string Country      { get; set; }
        [JsonProperty("first_name")]    public string FirstName    { get; set; }
        [JsonProperty("id")]            public long?  Id           { get; set; }
        [JsonProperty("last_name")]     public string LastName     { get; set; }
        [JsonProperty("phone")]         public string Phone        { get; set; }
        [JsonProperty("province")]      public string Province     { get; set; }
        [JsonProperty("zip")]           public string Zip          { get; set; }
        [JsonProperty("name")]          public string Name         { get; set; }
        [JsonProperty("province_code")] public string ProvinceCode { get; set; }
        [JsonProperty("country_code")]  public string CountryCode  { get; set; }
        [JsonProperty("default")]       public bool?  Default      { get; set; }
        [JsonProperty("district")]      public string District     { get; set; }
        [JsonProperty("district_code")] public string DistrictCode { get; set; }
        [JsonProperty("ward")]          public string Ward         { get; set; }
        [JsonProperty("ward_code")]     public string WardCode     { get; set; }
    }

    public class HRVClientDetailsRaw
    {
        [JsonProperty("accept_language")] public string   AcceptLanguage { get; set; }
        [JsonProperty("browser_ip")]      public string   BrowserIp      { get; set; }
        [JsonProperty("session_hash")]    public string   SessionHash    { get; set; }
        [JsonProperty("user_agent")]      public string   UserAgent      { get; set; }
        [JsonProperty("browser_height")]  public decimal? BrowserHeight  { get; set; }
        [JsonProperty("browser_width")]   public decimal? BrowserWidth   { get; set; }
    }

    public class HRVCustomerRaw
    {
        [JsonProperty("accepts_marketing")]    public bool?                AcceptsMarketing    { get; set; }
        [JsonProperty("addresses")]            public List<HRVAddressRaw>  Addresses           { get; set; }
        [JsonProperty("created_at")]           public DateTime?            CreatedAt           { get; set; }
        [JsonProperty("default_address")]      public HRVDefaultAddressRaw DefaultAddress      { get; set; }
        [JsonProperty("email")]                public string               Email               { get; set; }
        [JsonProperty("phone")]                public string               Phone               { get; set; }
        [JsonProperty("first_name")]           public string               FirstName           { get; set; }
        [JsonProperty("id")]                   public long?                Id                  { get; set; }
        [JsonProperty("last_name")]            public string               LastName            { get; set; }
        [JsonProperty("last_order_id")]        public long?                LastOrderId         { get; set; }
        [JsonProperty("last_order_name")]      public string               LastOrderName       { get; set; }
        [JsonProperty("note")]                 public string               Note                { get; set; }
        [JsonProperty("orders_count")]         public long?                OrdersCount         { get; set; }
        [JsonProperty("state")]                public string               State               { get; set; }
        [JsonProperty("tags")]                 public string               Tags                { get; set; }
        [JsonProperty("total_spent")]          public decimal?             TotalSpent          { get; set; }
        [JsonProperty("updated_at")]           public DateTime?            UpdatedAt           { get; set; }
        [JsonProperty("verified_email")]       public bool?                VerifiedEmail       { get; set; }
        [JsonProperty("birthday")]             public DateTime?            Birthday            { get; set; }
        [JsonProperty("gender")]               public int?                 Gender              { get; set; }
        [JsonProperty("last_order_date")]      public DateTime?            LastOrderDate       { get; set; }
        [JsonProperty("multipass_identifier")] public string               MultipassIdentifier { get; set; }
    }

    public class HRVDefaultAddressRaw
    {
        [JsonProperty("address1")]      public string Address1     { get; set; }
        [JsonProperty("address2")]      public string Address2     { get; set; }
        [JsonProperty("city")]          public string City         { get; set; }
        [JsonProperty("company")]       public string Company      { get; set; }
        [JsonProperty("country")]       public string Country      { get; set; }
        [JsonProperty("first_name")]    public string FirstName    { get; set; }
        [JsonProperty("id")]            public long?  Id           { get; set; }
        [JsonProperty("last_name")]     public string LastName     { get; set; }
        [JsonProperty("phone")]         public string Phone        { get; set; }
        [JsonProperty("province")]      public string Province     { get; set; }
        [JsonProperty("zip")]           public string Zip          { get; set; }
        [JsonProperty("name")]          public string Name         { get; set; }
        [JsonProperty("province_code")] public string ProvinceCode { get; set; }
        [JsonProperty("country_code")]  public string CountryCode  { get; set; }
        [JsonProperty("default")]       public bool?  Default      { get; set; }
        [JsonProperty("district")]      public string District     { get; set; }
        [JsonProperty("district_code")] public string DistrictCode { get; set; }
        [JsonProperty("ward")]          public string Ward         { get; set; }
        [JsonProperty("ward_code")]     public string WardCode     { get; set; }
    }

    public class HRVDiscountCodeRaw
    {
        [JsonProperty("amount")]         public decimal? Amount       { get; set; }
        [JsonProperty("code")]           public string   Code         { get; set; }
        [JsonProperty("type")]           public string   Type         { get; set; }
        [JsonProperty("is_coupon_code")] public bool?    IsCouponCode { get; set; }
    }

    public class HRVFulfillmentRaw
    {
        [JsonProperty("created_at")]                   public DateTime?            CreatedAt                 { get; set; }
        [JsonProperty("id")]                           public long?                Id                        { get; set; }
        [JsonProperty("order_id")]                     public long?                OrderId                   { get; set; }
        [JsonProperty("receipt")]                      public string               Receipt                   { get; set; }
        [JsonProperty("status")]                       public string               Status                    { get; set; }
        [JsonProperty("tracking_company")]             public string               TrackingCompany           { get; set; }
        [JsonProperty("tracking_company_code")]        public string               TrackingCompanyCode       { get; set; }
        [JsonProperty("tracking_numbers")]             public List<string>         TrackingNumbers           { get; set; }
        [JsonProperty("tracking_number")]              public string               TrackingNumber            { get; set; }
        [JsonProperty("tracking_url")]                 public string               TrackingUrl               { get; set; }
        [JsonProperty("tracking_urls")]                public List<string>         TrackingUrls              { get; set; }
        [JsonProperty("updated_at")]                   public DateTime?            UpdatedAt                 { get; set; }
        [JsonProperty("line_items")]                   public List<HRVLineItemRaw> LineItems                 { get; set; }
        [JsonProperty("province")]                     public string               Province                  { get; set; }
        [JsonProperty("province_code")]                public string               ProvinceCode              { get; set; }
        [JsonProperty("district")]                     public string               District                  { get; set; }
        [JsonProperty("district_code")]                public string               DistrictCode              { get; set; }
        [JsonProperty("ward")]                         public string               Ward                      { get; set; }
        [JsonProperty("ward_code")]                    public string               WardCode                  { get; set; }
        [JsonProperty("cod_amount")]                   public decimal?             CodAmount                 { get; set; }
        [JsonProperty("carrier_status_name")]          public string               CarrierStatusName         { get; set; }
        [JsonProperty("carrier_cod_status_name")]      public string               CarrierCodStatusName      { get; set; }
        [JsonProperty("carrier_status_code")]          public string               CarrierStatusCode         { get; set; }
        [JsonProperty("carrier_cod_status_code")]      public string               CarrierCodStatusCode      { get; set; }
        [JsonProperty("location_id")]                  public long?                LocationId                { get; set; }
        [JsonProperty("location_name")]                public string               LocationName              { get; set; }
        [JsonProperty("note")]                         public string               Note                      { get; set; }
        [JsonProperty("carrier_service_package_name")] public string               CarrierServicePackageName { get; set; }
        [JsonProperty("coupon_code")]                  public string               CouponCode                { get; set; }
        [JsonProperty("ready_to_pick_date")]           public DateTime?            ReadyToPickDate           { get; set; }
        [JsonProperty("picking_date")]                 public DateTime?            PickingDate               { get; set; }
        [JsonProperty("delivering_date")]              public DateTime?            DeliveringDate            { get; set; }
        [JsonProperty("delivered_date")]               public DateTime?            DeliveredDate             { get; set; }
        [JsonProperty("return_date")]                  public DateTime?            ReturnDate                { get; set; }
        [JsonProperty("not_meet_customer_date")]       public DateTime?            NotMeetCustomerDate       { get; set; }
        [JsonProperty("waiting_for_return_date")]      public DateTime?            WaitingForReturnDate      { get; set; }
        [JsonProperty("cod_paid_date")]                public DateTime?            CodPaidDate               { get; set; }
        [JsonProperty("cod_receipt_date")]             public DateTime?            CodReceiptDate            { get; set; }
        [JsonProperty("cod_pending_date")]             public DateTime?            CodPendingDate            { get; set; }
        [JsonProperty("cod_not_receipt_date")]         public DateTime?            CodNotReceiptDate         { get; set; }
        [JsonProperty("cancel_date")]                  public DateTime?            CancelDate                { get; set; }
        [JsonProperty("is_view_before")]               public bool?                IsViewBefore              { get; set; }
        [JsonProperty("country")]                      public string               Country                   { get; set; }
        [JsonProperty("country_code")]                 public string               CountryCode               { get; set; }
        [JsonProperty("zip_code")]                     public string               ZipCode                   { get; set; }
        [JsonProperty("city")]                         public string               City                      { get; set; }
        [JsonProperty("real_shipping_fee")]            public decimal?             RealShippingFee           { get; set; }
        [JsonProperty("shipping_notes")]               public string               ShippingNotes             { get; set; }
        [JsonProperty("total_weight")]                 public decimal?             TotalWeight               { get; set; }
        [JsonProperty("package_length")]               public decimal?             PackageLength             { get; set; }
        [JsonProperty("package_width")]                public decimal?             PackageWidth              { get; set; }
        [JsonProperty("package_height")]               public decimal?             PackageHeight             { get; set; }
        [JsonProperty("boxme_servicecode")]            public string               BoxmeServicecode          { get; set; }
        [JsonProperty("transport_type")]               public int?                 TransportType             { get; set; }
        [JsonProperty("address")]                      public string               Address                   { get; set; }
        [JsonProperty("sender_phone")]                 public string               SenderPhone               { get; set; }
        [JsonProperty("sender_name")]                  public string               SenderName                { get; set; }
        [JsonProperty("carrier_service_code")]         public string               CarrierServiceCode        { get; set; }
        [JsonProperty("from_longtitude")]              public decimal?             FromLongtitude            { get; set; }
        [JsonProperty("from_latitude")]                public decimal?             FromLatitude              { get; set; }
        [JsonProperty("to_longtitude")]                public decimal?             ToLongtitude              { get; set; }
        [JsonProperty("to_latitude")]                  public decimal?             ToLatitude                { get; set; }
        [JsonProperty("sort_code")]                    public string               SortCode                  { get; set; }
        [JsonProperty("is_drop_off")]                  public bool?                IsDropOff                 { get; set; }
        [JsonProperty("is_insurance")]                 public bool?                IsInsurance               { get; set; }
        [JsonProperty("insurance_price")]              public decimal?             InsurancePrice            { get; set; }
        [JsonProperty("is_open_box")]                  public bool?                IsOpenBox                 { get; set; }
        [JsonProperty("request_id")]                   public string               RequestId                 { get; set; }
        [JsonProperty("carrier_options")]              public string               CarrierOptions            { get; set; }
        [JsonProperty("note_attributes")]              public string               NoteAttributes            { get; set; }
        [JsonProperty("first_name")]                   public string               FirstName                 { get; set; }
        [JsonProperty("last_name")]                    public string               LastName                  { get; set; }
        [JsonProperty("shipping_address")]             public string               ShippingAddress           { get; set; }
        [JsonProperty("shipping_phone")]               public string               ShippingPhone             { get; set; }
    }

    public class HRVImageRaw
    {
        [JsonProperty("src")] public string Src { get; set; }
    }

    public class HRVLineItemRaw
    {
        [JsonProperty("fulfillable_quantity")]         public int?                        FulfillableQuantity        { get; set; }
        [JsonProperty("fulfillment_service")]          public string                      FulfillmentService         { get; set; }
        [JsonProperty("fulfillment_status")]           public string                      FulfillmentStatus          { get; set; }
        [JsonProperty("grams")]                        public decimal?                    Grams                      { get; set; }
        [JsonProperty("id")]                           public long?                       Id                         { get; set; }
        [JsonProperty("price")]                        public decimal?                    Price                      { get; set; }
        [JsonProperty("product_id")]                   public int?                        ProductId                  { get; set; }
        [JsonProperty("quantity")]                     public int?                        Quantity                   { get; set; }
        [JsonProperty("requires_shipping")]            public bool?                       RequiresShipping           { get; set; }
        [JsonProperty("sku")]                          public string                      Sku                        { get; set; }
        [JsonProperty("title")]                        public string                      Title                      { get; set; }
        [JsonProperty("variant_id")]                   public int?                        VariantId                  { get; set; }
        [JsonProperty("variant_title")]                public string                      VariantTitle               { get; set; }
        [JsonProperty("vendor")]                       public string                      Vendor                     { get; set; }
        [JsonProperty("name")]                         public string                      Name                       { get; set; }
        [JsonProperty("variant_inventory_management")] public string                      VariantInventoryManagement { get; set; }
        [JsonProperty("properties")]                   public List<HRVPropertyRaw>        Properties                 { get; set; }
        [JsonProperty("product_exists")]               public bool?                       ProductExists              { get; set; }
        [JsonProperty("price_original")]               public decimal?                    PriceOriginal              { get; set; }
        [JsonProperty("price_promotion")]              public decimal?                    PricePromotion             { get; set; }
        [JsonProperty("type")]                         public string                      Type                       { get; set; }
        [JsonProperty("gift_card")]                    public bool?                       GiftCard                   { get; set; }
        [JsonProperty("taxable")]                      public bool?                       Taxable                    { get; set; }
        [JsonProperty("tax_lines")]                    public string                      TaxLines                   { get; set; }
        [JsonProperty("barcode")]                      public string                      Barcode                    { get; set; }
        [JsonProperty("applied_discounts")]            public List<HRVAppliedDiscountRaw> AppliedDiscounts           { get; set; }
        [JsonProperty("total_discount")]               public decimal?                    TotalDiscount              { get; set; }
        [JsonProperty("image")]                        public HRVImageRaw                 ImageRaw                   { get; set; }
        [JsonProperty("not_allow_promotion")]          public bool?                       NotAllowPromotion          { get; set; }
        [JsonProperty("ma_cost_amount")]               public decimal?                    MaCostAmount               { get; set; }
        [JsonProperty("actual_price")]                 public decimal?                    ActualPrice                { get; set; }
    }

    public class HRVPropertyRaw
    {
        [JsonProperty("name")]  public string Name  { get; set; }
        [JsonProperty("value")] public string Value { get; set; }
    }

    public class HRVNoteAttributeRaw
    {
        [JsonProperty("name")]  public string Name  { get; set; }
        [JsonProperty("value")] public string Value { get; set; }
    }

    public class HRVOrderRaw : AuditedEntity<Guid>
    {
        public Guid? MerchantId      { get; set; }
        public Guid? StoreId         { get; set; }
        public Guid? MerchantStoreId { get; set; }

        // json props from documents
        [JsonProperty("billing_address")]         public HRVBillingAddressRaw      BillingAddress        { get; set; }
        [JsonProperty("browser_ip")]              public string                    BrowserIp             { get; set; }
        [JsonProperty("buyer_accepts_marketing")] public bool?                     BuyerAcceptsMarketing { get; set; }
        [JsonProperty("cancel_reason")]           public string                    CancelReason          { get; set; }
        [JsonProperty("cancelled_at")]            public DateTime?                 CancelledAt           { get; set; }
        [JsonProperty("cart_token")]              public string                    CartToken             { get; set; }
        [JsonProperty("checkout_token")]          public string                    CheckoutToken         { get; set; }
        [JsonProperty("client_details")]          public HRVClientDetailsRaw       ClientDetails         { get; set; }
        [JsonProperty("closed_at")]               public DateTime?                 ClosedAt              { get; set; }
        [JsonProperty("created_at")]              public DateTime?                 CreatedAt             { get; set; }
        [JsonProperty("currency")]                public string                    Currency              { get; set; }
        [JsonProperty("customer")]                public HRVCustomerRaw            Customer              { get; set; }
        [JsonProperty("discount_codes")]          public List<HRVDiscountCodeRaw>  DiscountCodes         { get; set; }
        [JsonProperty("email")]                   public string                    Email                 { get; set; }
        [JsonProperty("financial_status")]        public string                    FinancialStatus       { get; set; }
        [JsonProperty("fulfillments")]            public List<HRVFulfillmentRaw>   Fulfillments          { get; set; }
        [JsonProperty("fulfillment_status")]      public string                    FulfillmentStatus     { get; set; }
        [JsonProperty("tags")]                    public string                    Tags                  { get; set; }
        [JsonProperty("gateway")]                 public string                    Gateway               { get; set; }
        [JsonProperty("gateway_code")]            public string                    GatewayCode           { get; set; }
        [JsonProperty("id")]                      public long?                     OrderId               { get; set; }
        [JsonProperty("landing_site")]            public string                    LandingSite           { get; set; }
        [JsonProperty("landing_site_ref")]        public string                    LandingSiteRef        { get; set; }
        [JsonProperty("source")]                  public string                    Source                { get; set; }
        [JsonProperty("line_items")]              public List<HRVLineItemRaw>      LineItems             { get; set; }
        [JsonProperty("name")]                    public string                    Name                  { get; set; }
        [JsonProperty("note")]                    public string                    Note                  { get; set; }
        [JsonProperty("number")]                  public int?                      Number                { get; set; }
        [JsonProperty("order_number")]            public string                    OrderNumber           { get; set; }
        [JsonProperty("processing_method")]       public string                    ProcessingMethod      { get; set; }
        [JsonProperty("referring_site")]          public string                    ReferringSite         { get; set; }
        [JsonProperty("refunds")]                 public List<HRVRefundRaw>        Refunds               { get; set; }
        [JsonProperty("shipping_address")]        public HRVShippingAddressRaw     ShippingAddress       { get; set; }
        [JsonProperty("shipping_lines")]          public List<HRVShippingLineRaw>  ShippingLines         { get; set; }
        [JsonProperty("source_name")]             public string                    SourceName            { get; set; }
        [JsonProperty("subtotal_price")]          public decimal?                  SubtotalPrice         { get; set; }
        [JsonProperty("tax_lines")]               public string                    TaxLines              { get; set; }
        [JsonProperty("taxes_included")]          public bool?                     TaxesIncluded         { get; set; }
        [JsonProperty("token")]                   public string                    Token                 { get; set; }
        [JsonProperty("total_discounts")]         public decimal?                  TotalDiscounts        { get; set; }
        [JsonProperty("total_line_items_price")]  public decimal?                  TotalLineItemsPrice   { get; set; }
        [JsonProperty("total_price")]             public decimal?                  TotalPrice            { get; set; }
        [JsonProperty("total_tax")]               public decimal?                  TotalTax              { get; set; }
        [JsonProperty("total_weight")]            public decimal?                  TotalWeight           { get; set; }
        [JsonProperty("updated_at")]              public DateTime?                 UpdatedAt             { get; set; }
        [JsonProperty("transactions")]            public List<HRVTransactionRaw>   Transactions          { get; set; }
        [JsonProperty("note_attributes")]         public List<HRVNoteAttributeRaw> NoteAttributes        { get; set; }
        [JsonProperty("confirmed_at")]            public DateTime?                 ConfirmedAt           { get; set; }
        [JsonProperty("closed_status")]           public string                    ClosedStatus          { get; set; }
        [JsonProperty("cancelled_status")]        public string                    CancelledStatus       { get; set; }
        [JsonProperty("confirmed_status")]        public string                    ConfirmedStatus       { get; set; }
        [JsonProperty("assigned_location_id")]    public long?                     AssignedLocationId    { get; set; }
        [JsonProperty("assigned_location_name")]  public string                    AssignedLocationName  { get; set; }
        [JsonProperty("assigned_location_at")]    public DateTime?                 AssignedLocationAt    { get; set; }
        [JsonProperty("exported_confirm_at")]     public DateTime?                 ExportedConfirmAt     { get; set; }
        [JsonProperty("user_id")]                 public long?                     UserId                { get; set; }
        [JsonProperty("device_id")]               public long?                     DeviceId              { get; set; }
        [JsonProperty("location_id")]             public long?                     LocationId            { get; set; }
        [JsonProperty("location_name")]           public string                    LocationName          { get; set; }
        [JsonProperty("ref_order_id")]            public long?                     RefOrderId            { get; set; }
        [JsonProperty("ref_order_date")]          public DateTime?                 RefOrderDate          { get; set; }
        [JsonProperty("ref_order_number")]        public string                    RefOrderNumber        { get; set; }
        [JsonProperty("utm_source")]              public string                    UtmSource             { get; set; }
        [JsonProperty("utm_medium")]              public string                    UtmMedium             { get; set; }
        [JsonProperty("utm_campaign")]            public string                    UtmCampaign           { get; set; }
        [JsonProperty("utm_term")]                public string                    UtmTerm               { get; set; }
        [JsonProperty("utm_content")]             public string                    UtmContent            { get; set; }
        [JsonProperty("payment_url")]             public string                    PaymentUrl            { get; set; }
        [JsonProperty("contact_email")]           public string                    ContactEmail          { get; set; }
        [JsonProperty("order_processing_status")] public string                    OrderProcessingStatus { get; set; }
        [JsonProperty("prev_order_id")]           public long?                     PrevOrderId           { get; set; }
        [JsonProperty("prev_order_number")]       public string                    PrevOrderNumber       { get; set; }
        [JsonProperty("prev_order_date")]         public DateTime?                 PrevOrderDate         { get; set; }
        [JsonProperty("redeem_model")]            public HRVRedeemModelRaw         RedeemModel           { get; set; }
        public                                           bool                      IsProcessed           { get; set; }
        public                                           DateTime?                 ProcessedAt           { get; set; }
    }

    public class HRVRedeemModelRaw
    {
        [JsonProperty("payload_id")]     public long?    PayloadId     { get; set; }
        [JsonProperty("used_amount")]    public decimal? UsedAmount    { get; set; }
        [JsonProperty("transaction_id")] public long?    TransactionId { get; set; }
        [JsonProperty("discount_type")]  public string   DiscountType  { get; set; }
        [JsonProperty("discount")]       public decimal? Discount      { get; set; }
        [JsonProperty("max_per_order")]  public decimal? MaxPerOrder   { get; set; }
        [JsonProperty("amount")]         public decimal? Amount        { get; set; }
        [JsonProperty("name")]           public string   Name          { get; set; }
    }

    public class HRVShippingAddressRaw
    {
        [JsonProperty("address1")]      public string  Address1     { get; set; }
        [JsonProperty("address2")]      public string  Address2     { get; set; }
        [JsonProperty("city")]          public string  City         { get; set; }
        [JsonProperty("company")]       public string  Company      { get; set; }
        [JsonProperty("country")]       public string  Country      { get; set; }
        [JsonProperty("first_name")]    public string  FirstName    { get; set; }
        [JsonProperty("last_name")]     public string  LastName     { get; set; }
        [JsonProperty("latitude")]      public double? Latitude     { get; set; }
        [JsonProperty("longitude")]     public double? Longitude    { get; set; }
        [JsonProperty("phone")]         public string  Phone        { get; set; }
        [JsonProperty("province")]      public string  Province     { get; set; }
        [JsonProperty("zip")]           public string  Zip          { get; set; }
        [JsonProperty("name")]          public string  Name         { get; set; }
        [JsonProperty("province_code")] public string  ProvinceCode { get; set; }
        [JsonProperty("country_code")]  public string  CountryCode  { get; set; }
        [JsonProperty("district_code")] public string  DistrictCode { get; set; }
        [JsonProperty("district")]      public string  District     { get; set; }
        [JsonProperty("ward_code")]     public string  WardCode     { get; set; }
        [JsonProperty("ward")]          public string  Ward         { get; set; }
    }

    public class HRVShippingLineRaw
    {
        [JsonProperty("code")]   public string   Code   { get; set; }
        [JsonProperty("price")]  public decimal? Price  { get; set; }
        [JsonProperty("source")] public string   Source { get; set; }
        [JsonProperty("title")]  public string   Title  { get; set; }
    }

    public class HRVRefundRaw
    {
        [JsonProperty("created_at")]        public DateTime?                  CreatedAt       { get; set; }
        [JsonProperty("id")]                public long?                      Id              { get; set; }
        [JsonProperty("note")]              public string                     Note            { get; set; }
        [JsonProperty("refund_line_items")] public List<HRVRefundLineItemRaw> RefundLineItems { get; set; }
        [JsonProperty("restock")]           public string                     Restock         { get; set; }
        [JsonProperty("user_id")]           public string                     UserId          { get; set; }
        [JsonProperty("order_id")]          public long?                      OrderId         { get; set; }
        [JsonProperty("location_id")]       public long?                      LocationId      { get; set; }
        [JsonProperty("transactions")]      public List<HRVTransactionRaw>    Transactions    { get; set; }
    }

    public class HRVRefundLineItemRaw
    {
        [JsonProperty("id")]           public long?          Id          { get; set; }
        [JsonProperty("line_item")]    public HRVLineItemRaw LineItemRaw { get; set; }
        [JsonProperty("line_item_id")] public long?          LineItemId  { get; set; }
        [JsonProperty("quantity")]     public long?          Quantity    { get; set; }
    }

    public class HRVTransactionRaw
    {
        [JsonProperty("amount")]                  public decimal?  Amount                { get; set; }
        [JsonProperty("authorization")]           public string    Authorization         { get; set; }
        [JsonProperty("created_at")]              public DateTime? CreatedAt             { get; set; }
        [JsonProperty("device_id")]               public string    DeviceId              { get; set; }
        [JsonProperty("gateway")]                 public string    Gateway               { get; set; }
        [JsonProperty("id")]                      public long?     Id                    { get; set; }
        [JsonProperty("kind")]                    public string    Kind                  { get; set; }
        [JsonProperty("order_id")]                public long?     OrderId               { get; set; }
        [JsonProperty("receipt")]                 public string    Receipt               { get; set; }
        [JsonProperty("status")]                  public string    Status                { get; set; }
        [JsonProperty("user_id")]                 public string    UserId                { get; set; }
        [JsonProperty("location_id")]             public long?     LocationId            { get; set; }
        [JsonProperty("payment_details")]         public string    PaymentDetails        { get; set; }
        [JsonProperty("parent_id")]               public long?     ParentId              { get; set; }
        [JsonProperty("currency")]                public string    Currency              { get; set; }
        [JsonProperty("haravan_transaction_id")]  public string    HaravanTransactionId  { get; set; }
        [JsonProperty("external_transaction_id")] public string    ExternalTransactionId { get; set; }
    }

    public class HRVOrderResponseRaw
    {
        [JsonProperty("orders")] public List<HRVOrderRaw> Orders { get; set; }
    }
}