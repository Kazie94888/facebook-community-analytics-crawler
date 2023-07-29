using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace LookOn.Integrations.Haravan.Models.RawModels;

public class HRVShopResponseRaw
{
    [JsonProperty("shop")] public HRVShopRaw HrvShopRaw { get; set; }
}

public class HRVShopRaw
{
    [JsonProperty("address1")] public string Address1 { get; set; }
    [JsonProperty("email")]    public string Email    { get; set; }
    [JsonProperty("id")]       public long   Id       { get; set; }
    [JsonProperty("name")]     public string Name     { get; set; }
    [JsonProperty("phone")]    public string Phone    { get; set; }

    // [JsonProperty("city")]                       public string   City                     { get; set; }
    // [JsonProperty("country")]                    public string   Country                  { get; set; }
    // [JsonProperty("country_code")]               public string   CountryCode              { get; set; }
    // [JsonProperty("country_name")]               public string   CountryName              { get; set; }
    // [JsonProperty("created_at")]                 public DateTime CreatedAt                { get; set; }
    // [JsonProperty("customer_email")]             public string   CustomerEmail            { get; set; }
    // [JsonProperty("currency")]                   public string   Currency                 { get; set; }
    // [JsonProperty("domain")]                     public string   Domain                   { get; set; }

    // [JsonProperty("google_apps_domain")]         public string GoogleAppsDomain        { get; set; }
    // [JsonProperty("google_apps_login_enabled")]  public bool?  GoogleAppsLoginEnabled  { get; set; }

    // [JsonProperty("latitude")]                   public double Latitude                { get; set; }
    // [JsonProperty("longitude")]                  public double Longitude               { get; set; }
    // [JsonProperty("money_format")]               public string MoneyFormat             { get; set; }
    // [JsonProperty("money_with_currency_format")] public string MoneyWithCurrencyFormat { get; set; }
    // [JsonProperty("myharavan_domain")]           public string MyharavanDomain         { get; set; }

    // [JsonProperty("plan_name")]                  public string PlanName                { get; set; }
    // [JsonProperty("display_plan_name")]          public string DisplayPlanName         { get; set; }
    // [JsonProperty("password_enabled")]           public bool   PasswordEnabled         { get; set; }

    // [JsonProperty("province")]                   public string Province                { get; set; }
    // [JsonProperty("province_code")]              public string ProvinceCode            { get; set; }
    // [JsonProperty("public")]                     public object Public                  { get; set; }
    // [JsonProperty("shop_owner")]                 public string ShopOwner               { get; set; }
    // [JsonProperty("source")]                     public object Source                  { get; set; }
    // [JsonProperty("tax_shipping")]               public bool   TaxShipping             { get; set; }
    // [JsonProperty("taxes_included")]             public bool?   TaxesIncluded            { get; set; }
    // [JsonProperty("county_taxes")]               public object   CountyTaxes              { get; set; }
    // [JsonProperty("timezone")]                   public string   Timezone                 { get; set; }
    // [JsonProperty("zip")]                        public string   Zip                      { get; set; }
    // [JsonProperty("has_storefront")]             public bool     HasStorefront            { get; set; }
    // [JsonProperty("shop_plan_id")]               public int      ShopPlanId               { get; set; }
    // [JsonProperty("inventory_method")]           public string   InventoryMethod          { get; set; }
    // [JsonProperty("fullname_checkout_behavior")] public string   FullnameCheckoutBehavior { get; set; }
    // [JsonProperty("email_checkout_behavior")]    public string   EmailCheckoutBehavior    { get; set; }
    // [JsonProperty("phone_checkout_behavior")]    public string   PhoneCheckoutBehavior    { get; set; }
    // [JsonProperty("address_checkout_behavior")]  public string   AddressCheckoutBehavior  { get; set; }
    // [JsonProperty("district_checkout_behavior")] public string   DistrictCheckoutBehavior { get; set; }
    // [JsonProperty("ward_checkout_behavior")]     public string   WardCheckoutBehavior     { get; set; }
    // [JsonProperty("primary_locale")]             public string   PrimaryLocale            { get; set; }
}