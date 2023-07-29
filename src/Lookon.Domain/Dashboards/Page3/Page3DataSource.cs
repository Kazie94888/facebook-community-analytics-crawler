using System;
using System.Collections.Generic;
using LookOn.Integrations.Datalytis.Models.Entities;

namespace LookOn.Dashboards.Page3;

public class Page3DataSource
{
    public Guid                  MerchantId { get; set; }
    public Page3DataSourceSocial SocialData { get; set; }
}

public class Page3DataSourceSocial
{
    public List<DatalytisUser>              SocialUsers           { get; set; }
    public List<DatalytisUser>              SocialUsersNoPurchase { get; set; }
    public List<DatalytisUserSocialInsight> SocialInsights        { get; set; }
}