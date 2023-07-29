using LookOn.Shared;
using System;
using System.Threading.Tasks;
using LookOn.HaravanWebhooks;
using LookOn.RequestResponses;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace LookOn.MerchantSubscriptions
{
    public interface IMerchantSubscriptionsAppService : IApplicationService
    {
        Task<PagedResultDto<MerchantSubscriptionWithNavigationPropertiesDto>> GetListAsync(GetMerchantSubscriptionsInput input);

        Task<MerchantSubscriptionWithNavigationPropertiesDto> GetWithNavigationPropertiesAsync(Guid id);

        Task<MerchantSubscriptionDto> GetAsync(Guid id);

        Task<PagedResultDto<LookupDto<Guid>>> GetMerchantLookupAsync(LookupRequestDto input);

        Task DeleteAsync(Guid id);

        Task<MerchantSubscriptionDto> CreateAsync(MerchantSubscriptionCreateDto input);

        Task<MerchantSubscriptionDto> UpdateAsync(Guid id, MerchantSubscriptionUpdateDto input);
        Task UpdateSubscriptionStatus(UpdateSubscriptionStatusInput input);
        Task SetSubscription(SetSubscriptionInput               input);
        Task SetSubscriptionByWebhook(AppSubscriptionInput input);
        
        Task CancelSubscription(AppSubscriptionInput input);
        
        Task<MerchantSubscriptionDto> GetActiveSubscription(Guid merchantId);
    }
}