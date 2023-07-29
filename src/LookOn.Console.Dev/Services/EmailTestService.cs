using System.Threading.Tasks;
using LookOn.Emails;
using LookOn.Merchants;
using LookOn.MerchantSyncInfos;

namespace LookOn.Console.Dev.Services;

public class EmailTestService : DevService
{
    private readonly EmailManager _emailManager;

    public EmailTestService(EmailManager emailManager)
    {
        _emailManager = emailManager;
    }

    public void Send_SyncNoti()
    {
        // await _emailManager.Send_SyncNotification("tranvantuanit210@gmail.com", MerchantSyncStatus.Pending);
    }
}