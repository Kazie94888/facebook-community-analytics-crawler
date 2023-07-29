using System.Threading.Tasks;
using LookOn.Core.Extensions;
using LookOn.Merchants;
using LookOn.MerchantSubscriptions;
using LookOn.MerchantSyncInfos;
using Microsoft.Extensions.Logging;
using Volo.Abp.DependencyInjection;

namespace LookOn.Console.Dev.Services;

public class DevService : ITransientDependency
{
    public  ILogger<DevService>         Logger { get; set; }

    public virtual async Task Test()
    {
        Logger.LogCritical("DevService: ITransientDependency");
        await Task.Delay(1000);
    }

    public void TestPhoneCompare()
    {
        var internationalPhone  = "+84907708951";
        var internationalPhone2 = "84907708951";
        var internationalPhone3 = "+840907708951";
        var internationalPhone4 = "840907708951";
        var localPhone          = "0907708951";
        var localPhone2         = "907708951";

        var p1 = internationalPhone.ToInternationalPhoneNumber();
        var p2 = internationalPhone2.ToInternationalPhoneNumber();
        var p3 = internationalPhone3.ToInternationalPhoneNumber();
        var p4 = internationalPhone4.ToInternationalPhoneNumber();
        var p5 = localPhone.ToInternationalPhoneNumber();
        var p6 = localPhone2.ToInternationalPhoneNumber();
        
        Log(GetType().Name);
    }

    public void Log(string message)
    {
        if (message == null)
        {
            Logger.LogError(">>>>>>>>>>>>>>>>>>>>>>>>>>>> NULLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLL");
        }
        else
        {
            Logger.LogWarning($">>>>>>>>>>>>>>>>>>>>>>>>>>>> {message}");
        }
    }

}