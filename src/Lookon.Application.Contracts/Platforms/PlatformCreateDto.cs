using System;
using System.ComponentModel.DataAnnotations;

namespace LookOn.Platforms
{
    public class PlatformCreateDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Url { get; set; }
        public string LogoUrl { get; set; }
    }
}