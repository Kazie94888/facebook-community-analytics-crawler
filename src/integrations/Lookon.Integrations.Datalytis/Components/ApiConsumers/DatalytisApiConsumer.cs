using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LookOn.Core.Extensions;
using LookOn.Integrations.Datalytis.Configs;
using LookOn.Integrations.Datalytis.Models.Enums;
using LookOn.Integrations.Datalytis.Models.RawModels;
using Newtonsoft.Json;
using RestSharp;
using Volo.Abp.DependencyInjection;

namespace LookOn.Integrations.Datalytis.Components.ApiConsumers;

public class DatalytisApiConsumer : ITransientDependency
{
    private readonly RestClient _restClient        = new(DatalytisGlobalConfig.ApiBaseUrl);
    private readonly RestClient _restClientInsight = new(DatalytisGlobalConfig.InsightBaseUrl);

    /// <summary>
    /// API #1 - Get User By Phone. Limit 100
    /// http://api.datalytis.com/phone?email=xxxx&token=xxxx&phone=84383960591,84937309292,84774696034
    /// </summary>
    /// <param name="phones">84383960591</param>
    /// <param name="rateLimit"></param>
    /// <returns></returns>
    public async Task<DatalytisUsersResponse> GetUsersByPhone(List<string> phones, bool rateLimit = false)
    {
        if (rateLimit) await Task.Delay(DatalytisGlobalConfig.RateLimitInMs);

        if (phones.IsNullOrEmpty()) return new DatalytisUsersResponse();

        var url = $"/phone?{DatalytisGlobalConfig.DefaultParam}" + $"&phone={phones.JoinAsString(",")}";

        var req = new RestRequest(url);
        // var res = await _restClient.GetAsync<DatalytisUsersResponse>(req);
        // return res is { Success: true } ? res : new DatalytisUsersResponse();
        
        var restResponse = await _restClient.GetAsync(req);
        if (restResponse.IsSuccessful && restResponse.Content.IsNotNullOrEmpty())
        {
            var res = JsonConvert.DeserializeObject<DatalytisUsersResponse>(restResponse.Content.CleanJsonFormat());
            return res;
        }

        return null;
    }

    /// <summary>
    /// API #2 - Get Community Users. Limit 100
    /// https://api.datalytis.com/audiences/customer?email=LookOn@gmail.com
    /// &token=$2y$10$MGx2hiARZBxlNw6b9zLNt.4BirOH3TWRRfJb8Ksixgt3QKf72xUta
    /// &social_id=1992288464359020
    /// </summary>
    /// <param name="pageId"></param>
    /// <param name="pageNumber"></param>
    /// <param name="rateLimit"></param>
    /// <returns>404 if no data -> CommunityUsers_Request</returns>
    /// <returns>200 maybe with data</returns>
    public async Task<DatalytisUsersResponse> SocialUsers_Get(string pageId, int pageNumber, bool rateLimit = false)
    {
        try
        {
            if (rateLimit) await Task.Delay(DatalytisGlobalConfig.RateLimitInMs);

            var url = $"/audiences/customer?{DatalytisGlobalConfig.DefaultParam}"
                    + $"&social_id={pageId}"
                    + $"&limit={DatalytisGlobalConfig.DefaultPageSize}"
                    + $"&page={pageNumber}";

            var req = new RestRequest(url);
            // var res = await _restClient.GetAsync<DatalytisUsersResponse>(req);
            // return res is { Success: true } ? res : new DatalytisUsersResponse();
            var restResponse = await _restClient.GetAsync(req);
            if (restResponse.IsSuccessful && restResponse.Content.IsNotNullOrEmpty())
            {
                var res = JsonConvert.DeserializeObject<DatalytisUsersResponse>(restResponse.Content.CleanJsonFormat());
                return res;
            }

            return null;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    /// <summary>
    /// API #2.1 -  Request Social Users
    /// https://api.datalytis.com/audiences/created?email=LookOn@gmail.com&token=$2y$10$MGx2hiARZBxlNw6b9zLNt.4BirOH3TWRRfJb8Ksixgt3QKf72xUta
    /// Tạo Audience
    /// -> crawl new ( auto set status Crawl New khi tạo mới
    /// -> process data
    /// -> có thể bắt đầu import hoặc chờ completed**)
    /// -> active (completed)**
    /// ---
    /// SocialId & Type: type = 1 (fanpage) ,type = 2 (group), type = 3(profile)
    /// </summary>
    /// <param name="request"></param>
    /// <returns>"status": 13 = scan in progress</returns>
    /*
     * {
"message": "social_id already exists",
"success": false,
"status_code": 500
}
     */
    public async Task<SocialUsers_Response> SocialUsers_Request(SocialUsers_Request request)
    {
        var url = $"/audiences/created?{DatalytisGlobalConfig.DefaultParam}";

        var req = new RestRequest(url);
        req.Method = Method.Post;
        req.AddStringBody(JsonConvert.SerializeObject(request), DataFormat.Json);
        // var res = await _restClient.PostAsync<SocialUsers_Response>(req);
        //
        // return res is { Success: true } ? res : new SocialUsers_Response();
        var restResponse = await _restClient.PostAsync(req);
        if (restResponse.IsSuccessful && restResponse.Content.IsNotNullOrEmpty())
        {
            var res = JsonConvert.DeserializeObject<SocialUsers_Response>(restResponse.Content.CleanJsonFormat());
            return res;
        }
        return null;
    }

    /// <summary>
    /// https://api.datalytis.com/audiences/detail?
    /// email=lookon@gmail.com
    /// &token=$2y$10$MGx2hiARZBxlNw6b9zLNt.4BirOH3TWRRfJb8Ksixgt3QKf72xUta
    /// &social_id=102605322300473
    /// </summary>
    /// <param name="socialCommunityId"></param>
    /// <returns></returns>
    public async Task<SocialUsers_Status> SocialUsers_Status(string socialCommunityId)
    {
        var url = $"/audiences/detail?{DatalytisGlobalConfig.DefaultParam}" 
                + $"&social_id={socialCommunityId}";

        var req = new RestRequest(url);
        //var res = await _restClient.GetAsync<SocialUsers_Status>(req);
        var restResponse     = await _restClient.GetAsync(req);
        if (restResponse.IsSuccessful && restResponse.Content.IsNotNullOrEmpty())
        {
            var res = JsonConvert.DeserializeObject<SocialUsers_Status>(restResponse.Content.CleanJsonFormat());
            return res;
        }

        return null;
    }
    
    # region INSIGHTS

    /// <summary>
    /// Get Insights of Social Users
    /// http://insight.datalytis.com/insights/{uids_id}?
    /// email=LookOn@gmail.com
    /// &token=$2y$10$MGx2hiARZBxlNw6b9zLNt.4BirOH3TWRRfJb8Ksixgt3QKf72xUta
    /// </summary>
    /// <param name="uid"></param>
    /// <param name="rateLimit"></param>
    /// <returns>type : 0 = like, 1 = checkin , 2 = group</returns>
    /// <returns>200 => done</returns>
    /// <returns>500 => scanning</returns>
    /*
     * {
"message": "Uid is in the process of scanning data",
"success": false,
"status_code": 500
}
     */
    /*
    {
        "data": {
            "paging": {
                "cursors": {
                    "before": "",
                    "after": ""
                },
                "next": ""
            }
        },
        "success": true,
        "status_code": 200
    } 
     */
    public async Task<DatalytisUserSocialInsightResponseData> Insights_User_Get(string uid, bool rateLimit = false)
    {
        if (rateLimit) await Task.Delay(DatalytisGlobalConfig.RateLimitInMs);

        var current = await Insights_User_GetPage(uid, string.Empty);
        if (current is null || current.Success is false || current.Data == null) return current;
        do
        {
            //TODO: Check logic next page
            var after   = current.Data?.Paging?.Cursors?.After;
            var before  = current.Data?.Paging?.Cursors?.Before;
            var nextUrl = current.Data?.Paging?.Next;
            if (after.IsNullOrWhiteSpace() || nextUrl.IsNullOrWhiteSpace() || before == after) break;

            var next = await Insights_User_GetPage(uid, after);
            if (next is null || !next.Success) break;

            current.Data.Insights.AddRange(next.Data.Insights);
            current.Data.Paging = next.Data.Paging;
        } while (true);
        
        return current;
    }

    private async Task<DatalytisUserSocialInsightResponseData> Insights_User_GetPage(string uid, string cursorTokenAfter)
    {
        var url = $"/insights/{uid}?{DatalytisGlobalConfig.DefaultParam}";
        if (cursorTokenAfter.IsNotNullOrWhiteSpace())
        {
            url = $"{url}&after={cursorTokenAfter}";
        }

        var req = new RestRequest(url);
        // var res = await _restClientInsight.GetAsync<DatalytisUserSocialInsightResponseData>(req);
        //
        // return res is { Success: true } ? res : new DatalytisUserSocialInsightResponseData();
        
        var restResponse = await _restClientInsight.GetAsync(req);
        if (restResponse.IsSuccessful && restResponse.Content.IsNotNullOrEmpty())
        {
            var res = JsonConvert.DeserializeObject<DatalytisUserSocialInsightResponseData>(restResponse.Content.CleanJsonFormat());
            return res;
        }

        return null;
    }

    /*
     *
     {
        "data": {
            "id": 8,
            "size": 2,
            "created_date": "2022-06-14 00:44:52",
            "status": 15 => scanning
        },
        "success": true,
        "status_code": 200
    }
     */
    /// <summary>
    /// Request user social insight scan from uid list, max 100
    /// https://insight.datalytis.com/uids/created?email=LookOn@gmail.com&token=$2y$10$MGx2hiARZBxlNw6b9zLNt.4BirOH3TWRRfJb8Ksixgt3QKf72xUta
    /// </summary>
    /// <param name="request"></param>
    /// <returns>"status": 15 => scanning</returns>
    /// <returns>"status": 10 => completed</returns>
    public async Task<DatalytisSocialInsight_Response> Insights_User_Request(DatalytisSocialInsight_Request request)
    {
        var url = $"/uids/created?{DatalytisGlobalConfig.DefaultParam}";

        var req = new RestRequest(url);
        req.Method = Method.Post;
        req.AddStringBody(JsonConvert.SerializeObject(request), DataFormat.Json);
        // var res = await _restClientInsight.PostAsync<DatalytisSocialInsight_Response>(req);
        //
        // return res is { Success: true } ? res : new DatalytisSocialInsight_Response();
        var restResponse = await _restClientInsight.PostAsync(req);
        if (restResponse.IsSuccessful && restResponse.Content.IsNotNullOrEmpty())
        {
            var res = JsonConvert.DeserializeObject<DatalytisSocialInsight_Response>(restResponse.Content.CleanJsonFormat());
            return res;
        }

        return null;
    }

    /// <summary>
    /// Get status of audiences request.
    /// https://insight.datalytis.com/uids/detail/{id}?email=LookOn@gmail.com&token=$2y$10$MGx2hiARZBxlNw6b9zLNt.4BirOH3TWRRfJb8Ksixgt3QKf72xUta
    /// </summary>
    /// <param name="id"></param>
    /// <returns>status 10 = completed</returns>
    /// <returns>status 15 = in progress / process data</returns>
    public async Task<bool> Insights_User_GetStatus(string id)
    {
        var url = $"/uids/detail/{id}?{DatalytisGlobalConfig.DefaultParam}";

        var req = new RestRequest(url);
        //var res = await _restClientInsight.GetAsync<DatalytisSocialInsight_Response>(req);
        
        var restResponse = await _restClientInsight.GetAsync(req);
        if (restResponse.IsSuccessful && restResponse.Content.IsNotNullOrEmpty())
        {
            var res = JsonConvert.DeserializeObject<DatalytisSocialInsight_Response>(restResponse.Content.CleanJsonFormat());
            if (res != null && res.Success && res.Data != null)
            {
                return res is { Success: true } && res.Data.Status.ToIntOrDefault() == DatalytisScanStatus.Completed.ToInt();
            }

        }

        return false;
    }

    #endregion
}