using System.Threading.Tasks;
using LookOn.Consts;
using LookOn.Core.Extensions;
using LookOn.Enums;
using LookOn.HaravanWebhooks;
using LookOn.MerchantSubscriptions;
using LookOn.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace LookOn.Web.Controllers.Haravans
{
    [Route("lookon/webhook/haravan")]
    [AllowAnonymous]
    public class HaravanWebhookController : LookOnBaseController
    {
        private          string                           _verifyToken = string.Empty;
        private readonly IMerchantSubscriptionsAppService _merchantSubscriptionsAppService;

        public HaravanWebhookController(IMerchantSubscriptionsAppService merchantSubscriptionsAppService)
        {
            _merchantSubscriptionsAppService = merchantSubscriptionsAppService;
        }

        // POST api/values
        [ServiceFilter(typeof(HaravanWebhookAuthorize))]
        [HttpPost]
        public async Task Post(string orgId, string topic, string body)
        {
            switch (topic)
            {
                case HaravanWebhookConst.AppSubscriptions:
                    if (body.IsNotNullOrEmpty())
                    {
                        var appSubscriptionResponse = JsonConvert.DeserializeObject<AppSubscriptionResponse>(body);
                        if (appSubscriptionResponse != null)
                        {
                            appSubscriptionResponse.StoreCode = orgId;
                            var appSubscriptionInput = ObjectMapper.Map<AppSubscriptionResponse, AppSubscriptionInput>(appSubscriptionResponse);
                            if (appSubscriptionInput.Status == HaravanSubscriptionStatus.Active)
                            {
                                await _merchantSubscriptionsAppService.SetSubscriptionByWebhook(appSubscriptionInput);
                            }
                            else
                            {
                                await _merchantSubscriptionsAppService.CancelSubscription(appSubscriptionInput);
                            }
                        }
                    }
                    break;
                case HaravanWebhookConst.AppUninstalled:
                    break;
            }
        }

        [HttpGet]
        public string VerifyWebhook()
        {
            string verify_token = HttpContext.Request.Query["hub.verify_token"].ToString();

            _verifyToken = Configuration["HaravanApp:WebhookVerifyToken"];
            string challenge = HttpContext.Request.Query["hub.challenge"].ToString();
            
            if (verify_token.Equals(_verifyToken))
                return challenge;

            return null;
        }
    }
}