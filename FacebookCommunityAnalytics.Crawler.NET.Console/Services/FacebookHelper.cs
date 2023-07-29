using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using FacebookCommunityAnalytics.Crawler.NET.Console.Playwrights;
using FacebookCommunityAnalytics.Crawler.NET.Core.Extensions;
using Flurl;
using Microsoft.Playwright;

namespace FacebookCommunityAnalytics.Crawler.NET.Console.Services
{
    public static class FacebookHelper
    {
        // public static readonly string Selector_BtnJoinGroup = "xpath=/html/body/div[1]/div/div[4]/div/div[1]/div/div[2]/div/div[1]/div/button";
        public static readonly string Selector_BtnJoinGroup = "xpath=/html/body/div[1]/div/div[1]/div/div[3]/div/div/div[1]/div[1]/div[1]/div[2]/div/div[2]/div/div/div[2]/div/div[1]/div/div/div";
        public static readonly string Selector_ImgNoPermissionViewPost = "xpath=/html/body/div[1]/div/div[1]/div/div[3]/div/div/div[1]/div[1]/div/div/div[1]/div[1]/img";
        public static readonly string Selector_GroupUserPost = "div[role='article']";
        public static readonly string Selector_ButtonJoinGroup_En = "div[aria-label='Join Group']";
        public static readonly string Selector_ButtonJoinGroup_Vi = "div[aria-label='Tham gia nhóm']";

        public const string Image_PageNotFound = "//img[@src='https://static.xx.fbcdn.net/rsrc.php/v3/yp/r/U4B06nLMGQt.png']";
        public const string Image_GroupPermission = "//img[@src='/images/comet/empty_states_icons/permissions/permissions_gray_wash.svg']";
        public static async Task<bool> IsJoinedGroup(this IPage page)
        {
            var selector_Joined_En = "div[aria-label='Joined']";
            var selector_Joined_Vi = "div[aria-label='Đã tham gia']";
            var label_Joined_En = await page.QuerySelectorAsync(selector_Joined_En);
            var label_Joined_Vi = await page.QuerySelectorAsync(selector_Joined_Vi);
            return label_Joined_En != null || label_Joined_Vi != null;
        }
        
        public static async Task JoinGroup(this IPage page)
        {
            try
            {
                var btnJoinGroup = await page.QuerySelectorAsync(
                    "//div[(@aria-label='Join group' or @aria-label = 'Tham gia nhóm') and @role='button']//ancestor::span[text()='Join group' or text()='Tham gia nhóm']");
                if (btnJoinGroup != null)
                {
                    await btnJoinGroup.ClickAsync();
                    var selector_JoinedGroup =
                        "//div[(@aria-label='Joined' or @aria-label = 'Đã tham gia') and @role='button']//ancestor::span[text()='Joined' or text()='Đã tham gia']";
                    await page.WaitForSelectorAsync(selector_JoinedGroup);
                }
            }
            catch (Exception e)
            {
                System.Console.WriteLine(e.Message);
            }
            
        }

        public static async Task CheckDoneButton(this IPage page)
        {
            var selector_Done_En = "//div[@aria-label='Done' and @tabindex='0']";
            var selector_Done_Vi = "//div[@aria-label='Xong' and @tabindex='0']";
            var btn_Done_En = await page.QuerySelectorAsync(selector_Done_En);
            var btn_Done_Vi = await page.QuerySelectorAsync(selector_Done_Vi);
            if (btn_Done_En != null)
            {
                await btn_Done_En.Click();
            }
            else if(btn_Done_Vi != null)
            {
                await btn_Done_Vi.Click();
            }

            var selector_Next_En = "//div[@aria-label='Next' and @tabindex='0']";
            var selector_Next_Vi = "//div[@aria-label='Tiếp Tục' and @tabindex='0']";
            var btn_Next_En = await page.QuerySelectorAsync(selector_Next_En);
            var btn_Next_Vi = await page.QuerySelectorAsync(selector_Next_Vi);
            if (btn_Next_En != null)
            {
                await btn_Next_En.Click();
            }
            else if(btn_Next_Vi != null)
            {
                await btn_Next_Vi.Click();
            }
            
            var selector_Start_En = "//div[@aria-label='Get Started' and @tabindex='0']";
            var selector_Start_Vi = "//div[@aria-label='Bắt Đầu' and @tabindex='0']";
            var btn_Start_En = await page.QuerySelectorAsync(selector_Start_En);
            var btn_Start_Vi = await page.QuerySelectorAsync(selector_Start_Vi);
            if (btn_Start_En != null)
            {
                await btn_Start_En.Click();
            }
            else if(btn_Start_Vi != null)
            {
                await btn_Start_Vi.Click();
            }
            
            var selector_Dialog = "//div[@role='dialog']";
            var btn_Dialog = await page.QuerySelectorAsync(selector_Dialog);
            if (btn_Dialog != null)
            {
                await btn_Dialog.Click();
            }

            var selector_Introduction = "//div[@aria-label='Close Introduction']";
            var btn_CloseIntroduction = await page.QuerySelectorAsync(selector_Introduction);
            if (btn_CloseIntroduction != null)
            {
                await btn_CloseIntroduction.Click();
            }
        }

        /// <summary>
        /// Return false if already joined, return try if join button is clicked
        /// </summary>
        /// <param name="page"></param>
        /// <param name="postUrl"></param>
        /// <returns>true if can click joined, false if already join group or it is a page, can't join</returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public static async Task<bool> GotoAndJoinGroup(this IPage page, string postUrl)
        {
            if (StringExtensions.IsNullOrEmpty(postUrl))
            {
                throw new ArgumentNullException("GotoAndJoinGroup > PostUrl");
            }

            if (postUrl.Contains("groups"))
            {
                string groupUrl = "";
                if (postUrl.Contains("user"))
                {
                    groupUrl = postUrl.Substring(0, postUrl.IndexOf("user"));
                } 
                else if (postUrl.Contains("posts"))
                {
                    groupUrl = postUrl.Substring(0, postUrl.IndexOf("posts"));
                } 
                else if (postUrl.Contains("permalink"))
                {
                    groupUrl = postUrl.Substring(0, postUrl.IndexOf("permalink"));
                }
                
                await page.GotoAsync(groupUrl);
                await page.WaitForLoadStateAsync(LoadState.NetworkIdle);

                var isJoined = await page.IsJoinedGroup();
                if (isJoined)
                {
                    return false;
                }

                await JoinGroup(page);
                return true;
            }

            return false;
        }

        public static async Task<bool> IsPageNotFound(this IPage page)
        {
            var imgPageNotFound = await page.QuerySelectorAsync(Image_PageNotFound);
            return imgPageNotFound != null;
        }

        public static async Task<bool> CanNotAccessPost(this IPage page, string postUrl)
        {
            var img_NoPermission = await page.QuerySelectorAsync(Selector_ImgNoPermissionViewPost);
            if (img_NoPermission == null)
            {
                return false;
            }

            var src = await img_NoPermission.GetAttributeAsync("src");

            return (src.IsNotNullOrEmpty() && src.Contains("permission"));
        }
        
        
        public static async Task<bool> IsBlockedTemporary(this IPage page, string postUrl)
        {
            var img_NoPermission = await page.QuerySelectorAsync(Selector_ImgNoPermissionViewPost);
            if (img_NoPermission == null)
            {
                return false;
            }

            var src = await img_NoPermission.GetAttributeAsync("src");

            var isBlockedTemporary = (src.IsNotNullOrEmpty() && src.Contains("rsrc.php/y5/r/Mszq4yIBziR.svg"));
            var ele_BlockedTemporary = await page.QuerySelectorAsync(FacebookConsts.Selector_BlockedTemporary);
            
            return isBlockedTemporary || ele_BlockedTemporary != null;
        }



        public static async Task<bool> CanAccessPost(this IPage page, string postUrl)
        {
            return !await CanNotAccessPost(page, postUrl);
        }

        public static bool IsToday(string postCreatedAt)
        {
            postCreatedAt = postCreatedAt.ToLower();

            var today = postCreatedAt.Contains("now")
                        || postCreatedAt.Contains("vừa xong")
                        || postCreatedAt.Contains("phút") || postCreatedAt.Contains("mins")
                        || postCreatedAt.Contains("giờ") || postCreatedAt.Contains("hrs");

            return today;
        }

        public static bool IsNotToday(string postCreatedAt)
        {
            return StringExtensions.IsNullOrEmpty(postCreatedAt) || !IsToday(postCreatedAt);
        }

        public static List<string> GetHashtags(string value)
        {
            var regex = new Regex(@"#\w+");
            var matches = regex.Matches(value);

            return matches.Select(_ => _.ToString()).ToList();
        }

        public static List<string> GetLinks(string message)
        {
            List<string> links = new List<string>();
            var urlRx = new Regex(@"((https?|ftp|file)\://|www.)[A-Za-z0-9\.\-]+(/[A-Za-z0-9\?\&\=;\+!'\(\)\*\-\._~%]*)*", RegexOptions.IgnoreCase);

            MatchCollection matches = urlRx.Matches(message);
            foreach (Match match in matches)
            {
                links.Add(match.Value);
            }

            return links;
        }
        
        public static string GetProfileFacebookId(string input)
        {
            if (StringExtensions.IsNullOrEmpty(input)) return string.Empty;

            input = input.ToLower().Trim().Trim('/');
            if (input.Contains("facebook.com"))
            {
                var id = string.Empty;
        
                var fullUrlRegex1 = new Regex("(.*)facebook.com/profile.php[?]id=([a-zA-Z0-9\\.]+)");
                var fullUrlRegex2 = new Regex("(.*)facebook.com/groups/[a-zA-Z0-9\\.]+/user(.*)/[a-zA-Z0-9\\.]+");
                var fullUrlRegex3 = new Regex("(.*)facebook.com/[a-zA-Z0-9\\.]+");

                if (fullUrlRegex1.IsMatch(input))
                {
                    id = new Url(input).QueryParams.FirstOrDefault().Value.ToString();
                }
                else if (fullUrlRegex2.IsMatch(input))
                {
                    id = Regex.Split(input, "/user/").LastOrDefault();
                }
                else if (fullUrlRegex3.IsMatch(input))
                {
                    id = fullUrlRegex3.Match(input).Value.Split('/').LastOrDefault();
                }

                return id;
            }
            else
            {
                return input;
            }
        }
    }
}