using Volo.Abp.Application.Dtos;
using System;

namespace LookOn.Platforms
{
    public class GetPlatformsInput : PagedAndSortedResultRequestDto
    {
        public string FilterText { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }
        public string Url { get; set; }
        public string LogoUrl { get; set; }

        public GetPlatformsInput()
        {

        }
    }
}