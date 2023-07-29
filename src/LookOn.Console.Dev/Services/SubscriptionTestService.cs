using System.Threading.Tasks;
using LookOn.Enums;
using LookOn.MerchantSubscriptions;

namespace LookOn.Console.Dev.Services;

public class SubscriptionTestService:  DevService
{
    private readonly IMerchantSubscriptionRepository _subscriptionRepository;

    public SubscriptionTestService(IMerchantSubscriptionRepository subscriptionRepository)
    {
        _subscriptionRepository = subscriptionRepository;
    }

    public Task InitSub()
    {
        Log("This is InitSub");
        return Task.CompletedTask;
    }
}