using Volo.Abp.Application.Dtos;
using System;

namespace LookOn.MerchantSyncInfos
{
    public class GetMerchantSyncInfosInput : PagedAndSortedResultRequestDto
    {
        public string FilterText { get; set; }

        public string MerchantEmail { get; set; }
        public Guid? MerchantId { get; set; }

        public GetMerchantSyncInfosInput()
        {

        }
    }
}