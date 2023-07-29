using Volo.Abp.Application.Dtos;
using System;

namespace LookOn.MerchantUsers
{
    public class GetMerchantUsersInput : PagedAndSortedResultRequestDto
    {
        public string FilterText { get; set; }

        public bool? IsActive { get; set; }
        public Guid? AppUserId { get; set; }
        public Guid? MerchantId { get; set; }

        public GetMerchantUsersInput()
        {

        }
    }
}