using System;
using Volo.Abp.Application.Dtos;

namespace LookOn.MerchantSocialCommunity;

public class MerchantSocialCommunityFilterDto : PagedAndSortedResultRequestDto
{
    public string FilterText { get; set; }
    public Guid? MerchantId { get; set; }
    public bool? HasCommunityId { get; set; }
}