using System;
using System.Threading.Tasks;
using LookOn.Console.Dev.Services;
using LookOn.Core.Extensions;
using LookOn.Core.Helpers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Volo.Abp.DependencyInjection;

namespace LookOn.Console.Dev;

public class StartService : ITransientDependency
{
    public ILogger<StartService>   Logger                  { get; set; }
    public DevService              DevService              { get; set; }
    public EmailTestService        EmailTestService        { get; set; }
    public DatalytisUserService    DatalytisUserService    { get; set; }
    public HaravanJobTestService   HaravanJobTestService   { get; set; }
    public SubscriptionTestService SubscriptionTestService { get; set; }
    public MerchantDevService      MerchantDevService      { get; set; }
    // public CleanDataService        CleanDataService        { get; set; }

    public StartService()   
    {
        Logger = NullLogger<StartService>.Instance;
    }

    public async Task Execute()
    {
        Logger.LogCritical("_________________________________________________");
        Logger.LogCritical("This is the entry point for all testing purposes!");
        Logger.LogCritical("_________________________________________________");

        await DevService.Test();
        // DevService.TestPhoneCompare();
        
        // await DatalytisUserService.PrintRelationships();
        // await DatalytisUserService.PrintSexes();
        // await DatalytisUserService.Migrate_Relationship_Gender();

        // await HaravanJobTestService.SyncRawOrders();
        // await HaravanJobTestService.SyncCleanOrders();
        // await MerchantDevService.InitMerchantData();

        //await CleanDataService.CleanUp();

        // await MerchantDevService.InitMerchant();
        
        await MerchantDevService.DeleteUser("tina.hoang@veek.vn");
        await MerchantDevService.DeleteUser("tina.hoanglebao@gmail.com");

        Logger.LogCritical("_________________________________________________");
        Logger.LogCritical("END! GO HOME, WHY STAYING?");
        Logger.LogCritical("_________________________________________________");
    }
}