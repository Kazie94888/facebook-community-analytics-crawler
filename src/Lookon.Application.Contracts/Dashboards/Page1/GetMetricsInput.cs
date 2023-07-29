using System;
using System.Collections.Generic;
using LookOn.Core.Shared.Enums;
using LookOn.Merchants;

namespace LookOn.Dashboards.Page1;

public class GetMetricsInput
{
    public Guid          MerchantId                     { get; set; }
    public TimeFrameType TimeFrameType                  { get; set; }
    public DateTime      From                           { get; set; }
    public DateTime      To                             { get; set; }
    public List<string>  SocialCommunityIds             { get; set; }
}