namespace FacebookCommunityAnalytics.Crawler.NET.Service.Models
{
    public class LoginInput
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string TwoFACode { get; set; }
    }
}