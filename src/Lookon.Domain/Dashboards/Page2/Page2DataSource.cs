using System;
using System.Collections.Generic;
using LookOn.Integrations.Datalytis.Models.Entities;

namespace LookOn.Dashboards.Page2;

public class Page2DataSource
{
    public Guid                  MerchantId { get; set; }
    public Page2DataSourceSocial SocialData { get; set; }
}

public class Page2DataSourceSocial
{
    public List<DatalytisUser>              SocialUsersNoPurchase  { get; set; }
    public List<DatalytisUser>              SocialUsersHasPurchase { get; set; }
    public List<DatalytisUserSocialInsight> SocialInsights         { get; set; }
}