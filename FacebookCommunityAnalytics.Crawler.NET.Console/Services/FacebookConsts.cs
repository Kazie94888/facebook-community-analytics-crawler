using System.Text.RegularExpressions;

namespace FacebookCommunityAnalytics.Crawler.NET.Console.Services
{
    public static class FacebookConsts
    {
        public const string Selector_GroupPost = "xpath=//div[@role='article' and boolean(@aria-posinset)]";
        
        public const string Selector_PostParentCreatedAt = "xpath=/div/div/div/div/div/div[2]/div/div[2]/div/div[2]/div/div[2]";
        public const string Selector_PostCreatedAt = "//* >> a";

        public const string Selector_PostParentCreatedBy = "xpath=/div/div/div/div/div/div[2]/div/div[2]/div/div[2]/div/div[1]";
        public const string Selector_PostCreatedBy = "//* >> a";

        public const string Selector_Reaction = "//ancestor::div[@aria-label='Like' or @aria-label='Thích']/../../../../div[1]/div[1]/div[1]/div/span/div/span[2]";
        public const string Selector_Comment = "//ancestor::div[@aria-label='Like' or @aria-label='Thích']/../../../../div[1]//ancestor::span[contains(text(),'Comment') or contains(text(),'comment') or contains(text(),'bình luận')]";
        public const string Selector_Share = "//ancestor::div[@aria-label='Like' or @aria-label='Thích']/../../../../div[1]//ancestor::span[contains(text(),'Share') or contains(text(),'share') or contains(text(),'chia sẻ')]";

        public const string Selector_ToolTip = "span[role='tooltip']";
        public const string Selector_ToolTipItem = "span[role='tooltip'] >> ul > li";
        
        public const string Selector_PageName = "xpath=//div[@role='main']/div[1]/div[2]/div/div//span[1]";
        public const string Selector_PagePosts = "//span[text()='{0}']/ancestor::div[@role='article' and boolean(@tabindex) = false]";
        public const string Selector_PagePostsMultipleFeeds = "//ancestor::div[@role='article' and boolean(@tabindex) = false]";
        public const string Selector_PageFeeds = "//span[text()='{0}']/ancestor::div[@role='feed' and boolean(@tabindex) = false]";
        
        /// <summary>
        /// Share same selector (group post and page post)
        /// </summary>
        // public const string Selector_PagePostReaction = "xpath=//span[@role='toolbar']/../div/span/div/span[2]/span/span";
        public const string Selector_PagePostReaction = "xpath=//span[@role='toolbar']/../div";

        public const string Selector_GroupPosts = "xpath=//div[@role='article' and boolean(@tabindex) = false and boolean(@style) = false and div[boolean(@role) = false and ancestor::div[@role='feed']]]";

        public const string Selector_BlockedTemporary
            = "//div[@class = 'w0hvl6rk qjjbsfad']/span[@class = 'd2edcug0 hpfvmrgz qv66sw1b c1et5uql lr9zc1uh a8c37x1j keod5gw0 nxhoafnm aigsh9s9 ns63r2gh fe6kdd0r mau55g9w c8b282yb iv3no6db o3w64lxj b2s5l15y hnhda86s m9osqain oqcyycmt']";

        public const string Selector_CheckpointForm = "form[action*='checkpoint']";
        public const string Selector_RequestAReviewButtonEn = "button[type='submit'][value='Request a Review']";
        public const string Selector_RequestAReviewButtonVi = "button[type='submit'][value='Yêu cầu xem xét lại']";
        
        public static Regex GetKRegex()
        {
            return new Regex("[0-9]+K");
        }

        public static Regex GetHashtagRegex()
        {
            return new Regex(@"#\w+");
        }
    }
}