using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Castle.Core.Internal;
using LookOn.Core.Extensions;
using LookOn.Emails;
using LookOn.Enums;
using LookOn.Integrations.Datalytis.Components.ApiServices;
using LookOn.Integrations.Datalytis.Components.Repositories.Interfaces;
using LookOn.Integrations.Datalytis.Configs;
using LookOn.Integrations.Datalytis.Models.Entities;
using LookOn.Integrations.Datalytis.Models.Enums;
using LookOn.Integrations.Datalytis.Models.RawModels;
using LookOn.Integrations.Haravan.Components.Repositories.Interfaces;
using LookOn.Integrations.Haravan.Models.Enums;
using LookOn.Merchants;
using LookOn.MerchantSyncInfos;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Emailing;
using Volo.Abp.Emailing.Smtp;

namespace LookOn.Integrations.Datalytis;

public class DatalytisManager : LookOnManager
{
    // datalytis
    private readonly IDatalytisUserRepository    _datalytisUserRepo;
    private readonly IMerchantRepository         _merchantRepo;
    private readonly IMerchantSyncInfoRepository _merchantSyncInfoRepo;
    private readonly DatalytisUserService        _datalytisUserService;
    private readonly DatalytisInsightService     _datalytisInsightService;
    private readonly EmailManager                _emailManager;
    private readonly IHaravanCustomerRepository  _haravanCustomerRepository;

    public DatalytisManager(IMerchantSyncInfoRepository merchantSyncInfoRepo,
                            DatalytisUserService        datalytisUserService,
                            IDatalytisUserRepository    datalytisUserRepo,
                            DatalytisInsightService     datalytisInsightService,
                            IMerchantRepository         merchantRepo,
                            EmailManager                emailManager,
                            IHaravanCustomerRepository  haravanCustomerRepository)
    {
        _merchantSyncInfoRepo      = merchantSyncInfoRepo;
        _datalytisUserService      = datalytisUserService;
        _datalytisUserRepo         = datalytisUserRepo;
        _datalytisInsightService   = datalytisInsightService;
        _merchantRepo              = merchantRepo;
        _emailManager              = emailManager;
        _haravanCustomerRepository = haravanCustomerRepository;
    }

    #region SOCIAL USERS

    public async Task<bool?> SocialUsers_Status(Guid merchantId, string socialCommunityId)
    {
        var merchantSyncInfo = await _merchantSyncInfoRepo.GetAsync(_ => _.MerchantId == merchantId);

        var userScan = merchantSyncInfo?.SocialScan.UserScans.FirstOrDefault(_ => _.SocialCommunityId == socialCommunityId);
        if (userScan is null) return null;

        if (userScan.IsScanCompleted) return null;

        var statusResponse = await _datalytisUserService.SocialPageUsers_Status(userScan.SocialCommunityId);
        if (statusResponse is not { Success: true }) return false;

        if (statusResponse.Data.Status == DatalytisScanStatus.Completed.ToInt().ToString())
        {
            userScan.IsScanCompleted = true;
            userScan.ScanCompletedAt = DateTime.UtcNow;
            await _merchantSyncInfoRepo.UpdateAsync(merchantSyncInfo);
            return true;
        }

        userScan.ScanCheckedAt = DateTime.UtcNow;
        await _merchantSyncInfoRepo.UpdateAsync(merchantSyncInfo);
        return false;
    }

    public async Task SocialUsers_Sync(Guid merchantId, string socialCommunityId)
    {
        var merchantSyncInfo = await _merchantSyncInfoRepo.GetAsync(_ => _.MerchantId == merchantId);

        var userScan = merchantSyncInfo?.SocialScan.UserScans.FirstOrDefault(_ => _.SocialCommunityId == socialCommunityId);
        if (userScan is null) return;

        // TODOO Improve this: should implement a timeout/threshold to re-sync the users
        // e.g. vaithuhay at https://www.facebook.com/vaithuhayofficial grows only 2k user over 110k user in 2 months
        if (userScan.SyncStatus == MerchantJobStatus.Completed) return;

        var isSuccess = await _datalytisUserService.SocialUsers_Sync(userScan.SocialCommunityId);
        if (isSuccess)
        {
            userScan.SyncStatus = MerchantJobStatus.Completed;
            userScan.SyncedAt   = DateTime.UtcNow;
            await _merchantSyncInfoRepo.UpdateAsync(merchantSyncInfo);
        }
    }

    public async Task SocialUsers_Request(Guid merchantId, string socialCommunityId)
    {
        var merchant         = await _merchantRepo.GetAsync(merchantId);
        var merchantSyncInfo = await _merchantSyncInfoRepo.GetAsync(_ => _.MerchantId == merchantId);

        var userScan = merchantSyncInfo?.SocialScan.UserScans.FirstOrDefault(_ => _.SocialCommunityId == socialCommunityId);
        if (userScan is null) return;

        var socialCommunity = merchant.Communities?.FirstOrDefault(x => x.SocialCommunityId == userScan.SocialCommunityId.Trim());
        if (!userScan.IsRequestCompleted && socialCommunity != null)
        {
            var checkResponse = await _datalytisUserService.SocialUsers_Request(new SocialUsers_Request
            {
                PageId      = userScan.SocialCommunityId.Trim(),
                Type        = DatalytisCommunityUserType.Page.ToInt(),
                Name        = socialCommunity.SocialCommunityName, //"SocialUsers_Request",
                Description = $"{merchantSyncInfo.MerchantEmail}-{DateTime.UtcNow}"
            });

            if (checkResponse.Success)
            {
                userScan.RequestId          = checkResponse.Data.Id;
                userScan.RequestedAt        = DateTime.UtcNow;
                userScan.IsRequestCompleted = true;
                await _merchantSyncInfoRepo.UpdateAsync(merchantSyncInfo);
            }
            else if (checkResponse.StatusCode == 500)
            {
                var statusReponse = await _datalytisUserService.SocialPageUsers_Status(userScan.SocialCommunityId);
                if (statusReponse.Success
                 && statusReponse.StatusCode                   == 200
                 && statusReponse.Data.Status.ToIntOrDefault() == DatalytisScanStatus.Completed.ToInt())
                {
                    // {
                    //     "message": "social_id already exists",
                    //     "success": false,
                    //     "status_code": 500
                    // }
                    userScan.RequestId          = statusReponse.Data.Id.ToNullableInt();
                    userScan.RequestedAt        = DateTime.UtcNow;
                    userScan.IsRequestCompleted = true;
                    await _merchantSyncInfoRepo.UpdateAsync(merchantSyncInfo);
                }
            }
        }
    }

    public async Task SocialUserByPhones_Request(Guid merchantId)
    {
        var customers = await _haravanCustomerRepository.GetListAsync(_ => _.MerchantId == merchantId);
        var phones    = customers.Where(_ => _.Phone.IsNotNullOrEmpty()).Select(_ => Regex.Match(_.Phone, @"\d+").Value).Distinct().ToList();
        Console.WriteLine($"----------------------Haravan Phones Count: {phones.Count}---------------------------");
        var data  = new List<DatalytisUserRaw>();
        var count = 0;
        foreach (var batch in phones.ListPartition(300))
        {
            count++;
            Console.WriteLine($"----------------------Haravan Phones Left Count: {phones.Count - (count * 300)}---------------------------");
            var response = await _datalytisUserService.DatalytisUsers_Request(batch, rateLimit: true);
            if (!response.Success || response.Data.IsNullOrEmpty()) continue;
            data.AddRange(response.Data);
            Console.WriteLine($"----------------------Return data count: {data.Count}---------------------------");
        }

        data = data.DistinctBy(_ => _.Phone).ToList();
        Console.WriteLine($"----------------------Clean data count: {data.Count}---------------------------");
        var datalytisUsers     = await _datalytisUserRepo.GetListAsync();
        var initDatalytisUsers = new List<DatalytisUser>();
        var editDatalytisUsers = new List<DatalytisUser>();
        foreach (var result in data)
        {
            var datalytisUser = datalytisUsers.FirstOrDefault(_ => result.Phone.ToInternationalPhoneNumberFromVN() == _.Phone);
            if (datalytisUser is null)
            {
                var newDatalytisUser = ObjectMapper.Map<DatalytisUserRaw, DatalytisUser>(result);
                initDatalytisUsers.Add(newDatalytisUser);
                Console.WriteLine($"----------------------Init Phone: {result.Phone}---------------------------");
                continue;
            }

            ObjectMapper.Map(result, datalytisUser);
            editDatalytisUsers.Add(datalytisUser);
            Console.WriteLine($"----------------------Update Phone: {result.Phone}---------------------------");
        }

        // Parallel.ForEach(data,
        //                  result =>
        //                  {
        //                      var datalytisUser = datalytisUsers.FirstOrDefault(_ => result.Phone.ToInternationalPhoneNumberFromVN() == _.Phone);
        //                      if (datalytisUser is null)
        //                      {
        //                          var newDatalytisUser = ObjectMapper.Map<DatalytisUserRaw, DatalytisUser>(result);
        //                          initDatalytisUsers.Add(newDatalytisUser);
        //                          Console.WriteLine($"----------------------Init Phone: {result.Phone}---------------------------");
        //                      }
        //                      else
        //                      {
        //                          ObjectMapper.Map(result, datalytisUser);
        //                          editDatalytisUsers.Add(datalytisUser);
        //                          Console.WriteLine($"----------------------Update Phone: {result.Phone}---------------------------");
        //                      }
        //                  });

        Console.WriteLine($"----------------------Init data count: {initDatalytisUsers.Count}---------------------------");
        Console.WriteLine($"----------------------Update data count: {editDatalytisUsers.Count}---------------------------");
        foreach (var batch in initDatalytisUsers.Partition(100))
        {
            await _datalytisUserRepo.InsertManyAsync(batch);
        }

        foreach (var batch in initDatalytisUsers.Partition(100))
        {
            await _datalytisUserRepo.UpdateManyAsync(batch, true);
        }
    }

    #endregion

    #region SOCIAL INSIGHTS

    public async Task MetricSocialInsights_Request(Guid merchantId)
    {
        var merchantSyncInfo = await _merchantSyncInfoRepo.GetAsync(_ => _.MerchantId == merchantId);
        if (merchantSyncInfo is null) return;

        var insightScan = merchantSyncInfo.SocialScan.InsightScan;
        if (insightScan.InsightScanRequests is not null)
        {
            foreach (var scanRequest in insightScan.InsightScanRequests.Where(_ => !_.IsRequested))
            {
                var response = await _datalytisInsightService.UserSocialInsights_Request(new DatalytisSocialInsight_Request
                                                                                             { Uids = scanRequest.Uids });

                if (response.Success)
                {
                    scanRequest.IsRequested = true;
                    scanRequest.RequestId   = response.Data.Id;
                    scanRequest.RequestedAt = DateTime.UtcNow;
                    scanRequest.DataStatus  = response.Data.Status.ToEnumOrDefault<DatalytisScanStatus>();
                }
            }
        }
        await _merchantSyncInfoRepo.UpdateAsync(merchantSyncInfo);
    }

    public async Task MetricSocialInsights_Status(Guid merchantId)
    {
        var merchantSyncInfo = await _merchantSyncInfoRepo.GetAsync(_ => _.MerchantId == merchantId);
        if (merchantSyncInfo is null) return;

        var insightScan = merchantSyncInfo.SocialScan.InsightScan;
        if (insightScan.InsightScanRequests is not null)
        {
            foreach (var request in insightScan.InsightScanRequests.Where(_ => _.IsRequested))
            {
                var success = await _datalytisInsightService.UserSocialInsights_Status(request.RequestId);
                if (success)
                {
                    request.IsScanned  = true;
                    request.ScannedAt  = DateTime.UtcNow;
                    request.DataStatus = DatalytisScanStatus.Completed;
                }
            }
        }

        await _merchantSyncInfoRepo.UpdateAsync(merchantSyncInfo);
    }

    public async Task MetricSocialInsights_Sync(Guid merchantId)
    {
        var merchantSyncInfo = await _merchantSyncInfoRepo.FirstOrDefaultAsync(_ => _.MerchantId == merchantId);
        if (merchantSyncInfo is null) return;

        var insightScan = merchantSyncInfo.SocialScan.InsightScan;
        if (insightScan.InsightScanRequests is not null)
        {
            foreach (var scanRequest in insightScan.InsightScanRequests.Where(_ => _.IsScanned && _.SyncStatus != MerchantJobStatus.Completed))
            {
                var status = await _datalytisInsightService.UserSocialInsights_Sync(scanRequest.RequestId);
                if (status == DatalytisSyncStatus.Completed)
                {
                    scanRequest.SyncStatus = MerchantJobStatus.Completed;
                    scanRequest.SyncedAt   = DateTime.UtcNow;
                }
            }
        }

        await _merchantSyncInfoRepo.UpdateAsync(merchantSyncInfo);
    }

    #endregion
}