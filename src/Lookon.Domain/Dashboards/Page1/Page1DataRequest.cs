using System;
using System.Collections.Generic;
using LookOn.Core.Shared.Enums;
using LookOn.Merchants;

namespace LookOn.Dashboards.Page1;

public class Page1DataRequest
{
    public Guid          MerchantId                     { get; set; }
    public TimeFrameType TimeFrame                      { get; set; }
    public DateTime      From                           { get; set; }
    public DateTime      To                             { get; set; }
    public List<string>  SocialCommunityIds             { get; set; }
}