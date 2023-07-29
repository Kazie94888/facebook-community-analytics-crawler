using System;
using System.Threading.Tasks;
using LookOn.Integrations.Haravan;
using LookOn.Merchants;
using Volo.Abp.Domain.Repositories;

namespace LookOn.Console.Dev.Services;

public class HaravanJobTestService : DevService
{
    public HaravanSyncManager HaravanSyncManager { get; set; }
    public IMerchantRepository                  MerchantRepository                  { get; set; }

    public Task Test_OrderSync()
    {
        return Task.CompletedTask;

        // await HaravanSyncManager.SyncOrders(new Guid("F5933435-3FFB-9D59-432D-3A04263BDAB7"));
    }

    public async Task SyncRawOrders()
    {
        var merchant = await MerchantRepository.FirstOrDefaultAsync(m => m.Email.Contains("vaithuhay"));
        await HaravanSyncManager.SyncRawOrders(merchant.Id);
    }
    public async Task SyncCleanOrders()
    {
        var merchant = await MerchantRepository.FirstOrDefaultAsync(m => m.Email.Contains("vaithuhay"));
        await HaravanSyncManager.SyncOrders(merchant.Id);
    }
}