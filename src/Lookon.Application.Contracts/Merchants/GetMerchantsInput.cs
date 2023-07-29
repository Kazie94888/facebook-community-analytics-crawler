using Volo.Abp.Application.Dtos;
using System;

namespace LookOn.Merchants
{
    public class GetMerchantsInput : PagedAndSortedResultRequestDto
    {
        public string FilterText     { get; set; }
        public string Name           { get; set; }
        public string Phone          { get; set; }
        public string Address        { get; set; }
        public string Email          { get; set; }
        public string Fax            { get; set; }
        public Guid?  OwnerAppUserId { get; set; }
        public Guid?  CategoryId     { get; set; }

        public GetMerchantsInput()
        {
        }
    }
}