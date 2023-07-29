using System;
using System.Collections.Generic;

namespace LookOn.Dashboards.Page2;

public class Page2DataRequest
{
    public Guid MerchantId { get; set; }
    public List<string>  SocialCommunityIds { get; set; }
}