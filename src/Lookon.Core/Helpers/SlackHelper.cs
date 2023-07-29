using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using LookOn.Core.Extensions;
using Newtonsoft.Json;

namespace LookOn.Core.Helpers
{
    public static class SlackHelper
    {
        private const string DefaultWebhookUrl     = "https://hooks.slack.com/services/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";

        public static async Task Log(string methodName, string message, string slackWebhookUrl = null)
        {
            if(EnvironmentHelper.IsProduction())
            {
                if (slackWebhookUrl.IsNullOrEmpty()) slackWebhookUrl = DefaultWebhookUrl;

                var logMessage = $"---> {methodName} | {message}";
                await SendSlackMessage(slackWebhookUrl, logMessage);
            }
        }

        private static async Task SendSlackMessage(string slackWebhookUrl, string logMessage)
        {
            using var request = new HttpRequestMessage()
            {
                Method     = HttpMethod.Post,
                RequestUri = new Uri(slackWebhookUrl),
                Content    = new StringContent(JsonConvert.SerializeObject(new {text = logMessage}), Encoding.UTF8, "application/json")
            };

            var client = new HttpClient();
            _ = await client.SendAsync(request);
        }
    }
}