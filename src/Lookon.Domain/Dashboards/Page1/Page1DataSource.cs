using System;
using System.Collections.Generic;
using LookOn.Integrations.Datalytis.Models.Entities;
using LookOn.Integrations.Haravan.Models.Entities;

namespace LookOn.Dashboards.Page1;

public class Page1DataSource
{
    public Guid                  MerchantId { get; set; }
    public Page1DataSourceEcom   EcomData   { get; set; }
    public Page1DataSourceSocial SocialData   { get; set; }
}
public class Page1DataSourceEcom
{
    public Page1DataSourceEcom()
    {
        EcomOrders    = new List<HaravanOrder>();
        AllEcomOrders = new List<HaravanOrder>();
        EcomCustomers = new List<HaravanCustomer>();
    }
    public List<HaravanOrder>    EcomOrders    { get; set; }
    public List<HaravanOrder>    AllEcomOrders { get; set; }
    public List<HaravanCustomer> EcomCustomers { get; set; }
}

public class Page1DataSourceSocial
{
    public Page1DataSourceSocial()
    {
        SocialUsers    = new List<DatalytisUser>();
        SocialInsights = new List<DatalytisUserSocialInsight>();
    }
    public List<DatalytisUser>              SocialUsers    { get; set; }
    public List<DatalytisUserSocialInsight> SocialInsights { get; set; }
}