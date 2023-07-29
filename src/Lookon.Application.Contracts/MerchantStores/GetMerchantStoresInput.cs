using Volo.Abp.Application.Dtos;
using System;

namespace LookOn.MerchantStores
{
    public class GetMerchantStoresInput : PagedAndSortedResultRequestDto
    {
        public string FilterText { get; set; }

        public string Name { get; set; }
        public string Code { get; set; }
        public bool? Active { get; set; }
        public Guid? MerchantId { get; set; }
        public Guid? PlatformId { get; set; }

        public GetMerchantStoresInput()
        {

        }
    }
}