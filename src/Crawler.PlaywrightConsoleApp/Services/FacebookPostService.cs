using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Crawler.PlaywrightConsoleApp.Models;
using Microsoft.Playwright;
using Humanizer;

namespace Crawler.PlaywrightConsoleApp.Services
{
    public class FacebookPostService
    {
        private const int TimeSleep = 2000;
        //private static string[] _fbPosting = new string[]{"/html[1]/body[1]/div[1]/div[1]/div[1]/div[1]/div[3]/div[1]/div[1]/div[1]/div[1]/div[2]/div[1]/div[1]/div[4]/div[2]/div[1]/div[2]/div[1]/div[1]/div[1]/div[1]/div[2]/div[1]",
        //    "/html[1]/body[1]/div[1]/div[1]/div[1]/div[1]/div[4]/div[1]/div[1]/div[1]/div[1]/div[2]/div[1]/div[1]/div[1]/form[1]/div[1]/div[1]/div[1]/div[2]/div[2]/div[1]/div[1]/div[1]/div[1]/div[1]/div[1]/div[1]/div[1]/div[2]/div[1]/div[1]/div[1]/div[1]",
        //    "/html[1]/body[1]/div[1]/div[1]/div[1]/div[1]/div[4]/div[1]/div[1]/div[1]/div[1]/div[2]/div[1]/div[1]/div[1]/form[1]/div[1]/div[1]/div[1]/div[2]/div[3]/div[4]/div[1]"};

        private static readonly string JoinGroupButtonXPath = "/html/body/div[1]/div/div[4]/div/div[1]/div/div[2]/div/div[1]/div/button";

        public async Task JoinGroupAsync(IPage page)
        {
            var joinGroupButton = await page.QuerySelectorAsync($"xpath={JoinGroupButtonXPath}");
            if (joinGroupButton != null)
            {
                await joinGroupButton.ClickAsync();
            }
        }

        private static readonly string[] FbPosting =
        {
            "/html/body/div[1]/div/div[4]/div/div[1]/div/div[3]/div/div[1]/div[2]",
            "/html/body/div[2]/div[1]/div/div[2]/div/div/div[5]/div[3]/form/div[3]/div[3]/textarea",
            "/html/body/div[2]/div[1]/div/div[2]/div/div/div[5]/div[3]/div/div/button" //button submit
        };

        public async Task PostToFacebook(IPage page, PostModel model)
        {
            try
            {
                await page.ClickAsync($"xpath={FbPosting[0]}");

                await page.WaitForTimeoutAsync(TimeSleep);

                var textAreaInput = await page.QuerySelectorAsync($"xpath={FbPosting[1]}");
                if (textAreaInput != null)
                {
                    await textAreaInput.TypeAsync(model.Content);
                }

                await page.SetInputFilesAsync("input#photo_input", model.Images);

                //Do post
                var buttonSubmit = await page.QuerySelectorAsync($"xpath={FbPosting[2]}");

                if (buttonSubmit != null) await buttonSubmit.ClickAsync();
            }
            catch (Exception e)
            {
                Debug.WriteLine(" ***************** Error: ********************");
                Debug.Write(e.Message);
                Debug.WriteLine(" *********************************************");
            }
        }

        public async Task<List<string>> GetAllPostsFanpageVerMobile(IPage page, string pageUrl)
        {
            await page.GotoAsync(pageUrl);

            while (true)
            {
                var element = await page.QuerySelectorAsync("#see_more_cards_id");
                if (element != null)
                {
                    await element.ScrollIntoViewIfNeededAsync();
                }
                else
                {
                    element = await page.QuerySelectorAsync("#m_more_item");
                    if (element != null) 
                        await element.ScrollIntoViewIfNeededAsync();
                }
                
                await page.WaitForLoadStateAsync(LoadState.NetworkIdle, new PageWaitForLoadStateOptions
                {
                    Timeout = 120000
                });
                
                var queryHeaders = await page.QuerySelectorAllAsync("[data-sigil='m-feed-voice-subtitle']");

                var listPosts = new List<string>();
                var checkConditionReturn = 0;
                foreach (var queryHeader in queryHeaders)
                {
                    var titleBlock = await queryHeader.QuerySelectorAsync("a");
                    if (titleBlock != null)
                    {
                        var checkTime = await titleBlock.QuerySelectorAsync("abbr");
                        if (checkTime == null || StopCondition(await checkTime.InnerTextAsync()))
                        {
                            checkConditionReturn++;
                            if (checkConditionReturn == 10)
                            {
                                return listPosts;
                            }
                            else
                            {
                                continue;
                            }
                        }

                        var url = await titleBlock.GetAttributeAsync("href");
                        if (!string.IsNullOrEmpty(url) && url.Contains("?"))
                        {
                            url = url.Split('?').FirstOrDefault();
                        }

                        if (url != null && url.Contains("//m."))
                        {
                            url = url.Replace("//m.", "//www.");
                        }

                        listPosts.Add(url);
                    }
                    else
                    {
                        return listPosts;
                    }

                    checkConditionReturn = 0;
                }

                await page.WaitForTimeoutAsync(2000);
            }
        }

        private bool StopCondition(string postCreatedTime)
        {
            postCreatedTime = postCreatedTime.ToLowerInvariant();
            // return postCreatedTime.Contains("day")
            //        || postCreatedTime.Contains("ngày")
            //        || postCreatedTime.Contains("yesterday")
            //        || postCreatedTime.Contains("hôm qua");
            return !postCreatedTime.Contains("phút")
                   && !postCreatedTime.Contains("mins")
                   &&  !postCreatedTime.Contains("giờ")
                   &&  !postCreatedTime.Contains("hrs");
        }

        public async Task<SourcePostModel> ExtractData(IPage page, string postUrl)
        {
            await page.GotoAsync(postUrl);
            
            await page.WaitForLoadStateAsync(LoadState.NetworkIdle);
            var reactionButton = await page.QuerySelectorAsync("xpath=/html/body/div[1]/div/div[1]/div/div[3]/div/div/div[1]/div[1]/div[4]/div/div/div/div/div/div/div[1]/div/div/div/div/div/div/div/div/div/div/div[2]/div/div[4]/div/div/div[1]/div/div[1]/div/div[1]/div/span/div");
            if (reactionButton == null)
                return null;
            await reactionButton.ClickAsync();
            await page.WaitForTimeoutAsync(3000);
            var postModel = new SourcePostModel() { Link = postUrl};
            var regex = new Regex("[0-9]+K");
            var dialogPostElementHandle = await page.QuerySelectorAsync("xpath=/html/body/div[1]/div/div[1]/div/div[4]/div/div/div[1]/div/div[2]/div/div/div/div[1]/div/div/div/div/div[2]");
            if (dialogPostElementHandle == null) return null;
            
            var allReactionSpan = await dialogPostElementHandle.QuerySelectorAllAsync("span[dir='auto']");
            var totalReaction = 0;
            foreach (var reactionSpan in allReactionSpan)
            {
                var innerString = await reactionSpan.InnerTextAsync();
                if (!string.IsNullOrEmpty(innerString))
                {
                    var reactionString = Regex.Replace(innerString, "[^0-9]+", string.Empty);
                    int.TryParse(reactionString, out var reactionCount);
                    if (regex.IsMatch(reactionString)) reactionCount *= 1000;
                    totalReaction += reactionCount;
                }
            }

            postModel.ReactionCount = totalReaction.ToString();

            var shareElement = await page.QuerySelectorAsync("xpath=/html/body/div[1]/div/div[1]/div/div[3]/div/div/div[1]/div[1]/div[4]/div/div/div/div/div/div/div[1]/div/div/div/div/div/div/div/div/div/div/div[2]/div/div[4]/div/div/div[1]/div/div[1]/div/div[2]/div[2]/span/div/span");
            if (shareElement != null)
            {
                var shareText = await shareElement.InnerTextAsync();
                var shareCountString = Regex.Replace(shareText, "[^0-9]+", string.Empty);
                int.TryParse(shareCountString, out int shareCount);
                if (regex.IsMatch(shareText)) shareCount *= 1000;
                postModel.ShareCount = shareCount.ToString();
            }

            var commentElement = await page.QuerySelectorAsync("xpath=/html/body/div[1]/div/div[1]/div/div[3]/div/div/div[1]/div[1]/div[4]/div/div/div/div/div/div/div[1]/div/div/div/div/div/div/div/div/div/div/div[2]/div/div[4]/div/div/div[1]/div/div[1]/div/div[2]/div[1]/div/span");
            if (commentElement != null)
            {
                var commentText = await commentElement.InnerTextAsync();
                var countString = Regex.Replace(commentText, "[^0-9]+", string.Empty);
                int.TryParse(countString, out int commentCount);
                if (regex.IsMatch(commentText)) commentCount *= 1000;
                postModel.CommentCount = commentCount.ToString();
            }

            return postModel;
        }
    }
}