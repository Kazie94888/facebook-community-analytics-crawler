using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using FluentDateTime;
using LookOn.Core.Extensions;
using LookOn.Enums;
using LookOn.Integrations.Datalytis;
using LookOn.Integrations.Haravan.Components.ApiConsumers;
using LookOn.Integrations.Haravan.Components.Repositories.Interfaces;
using LookOn.Integrations.Haravan.Models.Entities;
using LookOn.Integrations.Haravan.Models.RawModels;
using LookOn.MerchantSyncInfos;
using Volo.Abp.Domain.Repositories;

namespace LookOn.Integrations.Haravan;

public class HaravanSyncManager : LookOnManager
{
    private readonly IHaravanStoreRepository     _haravanStoreRepo;
    private readonly IHaravanCustomerRepository  _haravanCustomerRepo;
    private readonly IHaravanOrderRepository     _haravanOrderRepo;
    private readonly IRepository<HRVOrderRaw>    _haravanOrderRawRepo;
    private readonly IMerchantSyncInfoRepository _merchantSyncInfoRepository;
    private readonly DatalytisManager            _datalytisManager;

    public HaravanSyncManager(IHaravanStoreRepository     haravanStoreRepo,
                              IHaravanCustomerRepository  haravanCustomerRepo,
                              IHaravanOrderRepository     haravanOrderRepo,
                              IRepository<HRVOrderRaw>    haravanOrderRawRepo,
                              IMerchantSyncInfoRepository merchantSyncInfoRepository,
                              DatalytisManager            datalytisManager)
    {
        _haravanStoreRepo           = haravanStoreRepo;
        _haravanCustomerRepo        = haravanCustomerRepo;
        _haravanOrderRepo           = haravanOrderRepo;
        _haravanOrderRawRepo        = haravanOrderRawRepo;
        _merchantSyncInfoRepository = merchantSyncInfoRepository;
        _datalytisManager      = datalytisManager;
    }

    /// <summary>
    /// Pre-process raw orders in to clean orders and store to clean order collection 'HaravanOrders'
    /// </summary>
    /// <param name="merchantId"></param>
    public async Task SyncOrders(Guid merchantId)
    {
        var haravanStore = await _haravanStoreRepo.GetAsync(_ => _.MerchantId == merchantId);
        if (haravanStore is null) return;

        await DoSyncOrders(haravanStore);
    }

    public async Task SyncRawOrders(Guid merchantId)
    {
        var haravanStore = await _haravanStoreRepo.GetAsync(_ => _.MerchantId == merchantId);
        if (haravanStore is null) return;

        await DoSyncRawOrders(haravanStore);
    }

    private async Task DoSyncOrders(HaravanStore haravanStore)
    {
        var merchantSyncInfo = await _merchantSyncInfoRepository.GetAsync(_ => _.MerchantId == haravanStore.MerchantId);
        if (!merchantSyncInfo.EcomScan.IsFirstSyncCompleted)
        {
            return;
        }

        if (!merchantSyncInfo.EcomScan.ShouldSyncCleanOrder)
        {
            return;
        }

        // Get all orders that are not synced yet
        var rawOrders = await _haravanOrderRawRepo.GetListAsync(_ => _.StoreId == haravanStore.Id && !_.IsProcessed);
        if (rawOrders.IsNullOrEmpty()) return;
        LogDebug(GetType(), $"DoSyncOrders - FOUND not processed order {rawOrders.Count} ");

        merchantSyncInfo.EcomScan.CleanOrderSyncStatus = MerchantJobStatus.InProgress;
        merchantSyncInfo.EcomScan.LastCleanOrderRanAt  = DateTime.UtcNow;
        await _merchantSyncInfoRepository.UpdateAsync(merchantSyncInfo, true);

        // group orders by order number
        var groupRawOrders = rawOrders.GroupBy(n => n.OrderNumber).Select(r => new { OrderNumber = r.Key, RawOrders = r.ToList() }).ToList();
        foreach (var groupOrder in groupRawOrders)
        {
            // get valid order
            var rawOrder = groupOrder.RawOrders.MaxBy(r => r.CreatedAt);
            if (rawOrder is null) continue;

            // create new order
            var newCleanOrder = ObjectMapper.Map<HRVOrderRaw, HaravanOrder>(rawOrder);

            // order existed => no sync
            var isExistingOrder = await _haravanOrderRepo.AnyAsync(_ => _.OrderId == newCleanOrder.OrderId && _.OrderNumber == newCleanOrder.OrderNumber && _.MerchantId == haravanStore.MerchantId);
            if (isExistingOrder)
            {
                var currentOrder = await _haravanOrderRepo.GetAsync(_ => _.OrderNumber == rawOrder.OrderNumber);
                var shouldUpdate = newCleanOrder.CreatedAt.HasValue
                                && currentOrder.CreatedAt.HasValue
                                && newCleanOrder.CreatedAt.Value.IsBefore(currentOrder.CreatedAt.Value);

                if (currentOrder is not null && currentOrder.OrderId != newCleanOrder.OrderId && shouldUpdate)
                {
                    ObjectMapper.Map(newCleanOrder, currentOrder);
                    await _haravanOrderRepo.UpdateAsync(currentOrder, true);
                    LogDebug(GetType(), $"DoSyncOrders - UPDATED order {currentOrder.OrderNumber} ");
                }
            }
            else
            {
                newCleanOrder.StoreId         = haravanStore.Id;
                newCleanOrder.MerchantId      = haravanStore.MerchantId;
                newCleanOrder.MerchantStoreId = haravanStore.MerchantStoreId;

                await _haravanOrderRepo.InsertAsync(newCleanOrder, true);
                LogDebug(GetType(), $"DoSyncOrders - INSERTED new order {newCleanOrder.OrderNumber} ");
            }

            // create new customer
            if (rawOrder.Customer is not null)
            {
                var isExistingCustomer = await _haravanCustomerRepo.AnyAsync(_ => _.CustomerId == rawOrder.Customer.Id);
                if (isExistingCustomer)
                {
                    var currentCustomer = await _haravanCustomerRepo.FirstOrDefaultAsync(customer => customer.CustomerId == rawOrder.Customer.Id);
                    if (currentCustomer?.CustomerId != null)
                    {
                        currentCustomer.Phone = rawOrder.Customer.Phone.ToInternationalPhoneNumber();
                    }

                    await _haravanCustomerRepo.UpdateAsync(currentCustomer, true);
                }
                else
                {
                    var newCustomer = ObjectMapper.Map<HRVCustomerRaw, HaravanCustomer>(rawOrder.Customer);
                    newCustomer.StoreId         = haravanStore.Id;
                    newCustomer.MerchantId      = haravanStore.MerchantId;
                    newCustomer.MerchantStoreId = haravanStore.MerchantStoreId;

                    await _haravanCustomerRepo.InsertAsync(newCustomer, true);
                }
            }

            // update raw order
            foreach (var updateRawOrder in groupOrder.RawOrders)
            {
                updateRawOrder.IsProcessed = true;
                updateRawOrder.ProcessedAt = DateTime.UtcNow;
            }

            await _haravanOrderRawRepo.UpdateManyAsync(groupOrder.RawOrders, true);

            await _merchantSyncInfoRepository.UpdateAsync(merchantSyncInfo, true);
        }

        merchantSyncInfo.EcomScan.CleanOrderSyncStatus   = MerchantJobStatus.Completed;
        merchantSyncInfo.EcomScan.LastCleanOrderSyncedAt = DateTime.UtcNow;
        await _merchantSyncInfoRepository.UpdateAsync(merchantSyncInfo, true);

        await _datalytisManager.SocialUserByPhones_Request(haravanStore.MerchantId);
    }

    private async Task DoSyncRawOrders(HaravanStore haravanStore)
    {
        var merchantSyncInfo = await _merchantSyncInfoRepository.GetAsync(_ => _.MerchantId == haravanStore.MerchantId);
        if (!merchantSyncInfo.EcomScan.ShouldSyncRawOrder)
        {
            return;
        }

        merchantSyncInfo.EcomScan.RawOrderSyncStatus = MerchantJobStatus.InProgress;
        merchantSyncInfo.EcomScan.LastRawOrderRanAt  = DateTime.UtcNow;
        await _merchantSyncInfoRepository.UpdateAsync(merchantSyncInfo, true);

        var isFirstSync = merchantSyncInfo.EcomScan.IsFirstSyncCompleted is false || merchantSyncInfo.EcomScan.FirstSyncCompletedAt is null;
        if (isFirstSync)
        {
            var lastSyncTime = merchantSyncInfo.EcomScan.LastRawOrderSyncedAt ?? DateTime.UtcNow.AddYears(-10);
            foreach (var year in lastSyncTime.EachYear(DateTime.UtcNow))
            {
                foreach (var monthSync in year.FirstDayOfYear().EachMonth(year.LastDayOfYear()))
                {
                    if (monthSync.Month <= lastSyncTime.Month) continue;

                    var orderRaws            = await GetRawOrders(haravanStore, monthSync.FirstDayOfMonth(), monthSync.LastDayOfMonth());
                    var lastRawOrderSyncedAt = monthSync.LastDayOfMonth();
                    if (orderRaws.IsNotNullOrEmpty())
                    {
                        foreach (var batch in orderRaws.Partition(100)) await _haravanOrderRawRepo.InsertManyAsync(batch, autoSave: true);

                        var lastOrderRaw                               = orderRaws.MaxBy(_ => _.CreatedAt);
                        if (lastOrderRaw != null) lastRawOrderSyncedAt = lastOrderRaw.CreatedAt ?? monthSync.LastDayOfMonth();
                    }

                    merchantSyncInfo.EcomScan.LastRawOrderSyncedAt = lastRawOrderSyncedAt;
                    await _merchantSyncInfoRepository.UpdateAsync(merchantSyncInfo, true);
                }
            }

            merchantSyncInfo.EcomScan.IsFirstSyncCompleted = true;
            merchantSyncInfo.EcomScan.FirstSyncCompletedAt = DateTime.UtcNow;

            await _merchantSyncInfoRepository.UpdateAsync(merchantSyncInfo);
        }
        else
        {
            var from = merchantSyncInfo.EcomScan.LastRawOrderSyncedAt ?? merchantSyncInfo.EcomScan.FirstSyncCompletedAt;
            var to   = DateTime.UtcNow;

            var orderRaws = await GetRawOrders(haravanStore, from.Value, to);
            if (orderRaws.IsNotNullOrEmpty())
            {
                foreach (var batch in orderRaws.Partition(100)) await _haravanOrderRawRepo.InsertManyAsync(batch, autoSave: true);
            }
        }

        merchantSyncInfo.EcomScan.RawOrderSyncStatus   = MerchantJobStatus.Completed;
        merchantSyncInfo.EcomScan.LastRawOrderSyncedAt = DateTime.UtcNow;

        await _merchantSyncInfoRepository.UpdateAsync(merchantSyncInfo);
    }

    private async Task<ConcurrentBag<HRVOrderRaw>> GetRawOrders(HaravanStore haravanStore, DateTime from, DateTime to)
    {
        var newOrders = new ConcurrentBag<HRVOrderRaw>();
        var page      = 0;
        while (true)
        {
            page++;

            // get orders from haravan
            var orderResponse = await HaravanApiConsumer.GetOrders(haravanStore.Token.AccessToken,
                                                                   page,
                                                                   from,
                                                                   to,
                                                                   true);

            var rawOrders = orderResponse.Orders;
            if (rawOrders.IsNullOrEmpty()) break;

            Parallel.ForEach(rawOrders,
                             rawOrder =>
                             {
                                 rawOrder.MerchantId      = haravanStore.MerchantId;
                                 rawOrder.StoreId         = haravanStore.Id;
                                 rawOrder.MerchantStoreId = haravanStore.MerchantStoreId;
                                 rawOrder.IsProcessed     = false;
                             });

            newOrders.AddRange(rawOrders);
        }

        return newOrders;
    }
}