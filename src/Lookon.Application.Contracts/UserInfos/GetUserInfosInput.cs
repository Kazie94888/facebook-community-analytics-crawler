using Volo.Abp.Application.Dtos;
using System;

namespace LookOn.UserInfos
{
    public class GetUserInfosInput : PagedAndSortedResultRequestDto
    {
        public string FilterText { get; set; }

        public string IdentificationNumber { get; set; }
        public Guid? AppUserId { get; set; }

        public GetUserInfosInput()
        {

        }
    }
}