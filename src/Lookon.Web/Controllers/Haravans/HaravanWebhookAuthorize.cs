using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.DependencyInjection;

namespace LookOn.Web.Controllers.Haravans
{
    public class HaravanWebhookAuthorize : Attribute, IAsyncActionFilter, ITransientDependency
    {
        private readonly IConfiguration _configuration;
        public HaravanWebhookAuthorize(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public static string HashHmacSHA256(string body, string secretKey)
        {
            var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secretKey));
            return Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(body)));
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var request = context.HttpContext.Request;
            var configuration = context.HttpContext.RequestServices.GetService<IConfiguration>();

            var hmac = (string)request.Headers["X-Haravan-HmacSha256"];

            if (string.IsNullOrWhiteSpace(hmac))
            {
                context.Result = new StatusCodeResult(401);
                return;
            }

            using (var reader = new StreamReader(request.Body))
            {
                var bodyString  = await reader.ReadToEndAsync();
                var clientSecret = _configuration["HaravanApp:ClientSecret"];
                var signature    = HashHmacSHA256(bodyString, clientSecret);

                if (signature != hmac)
                {
                    context.Result = new StatusCodeResult(401);
                    return;
                }

                context.ActionArguments.Add("topic", (string)request.Headers["X-Haravan-Topic"]);
                context.ActionArguments.Add("orgId", (string)request.Headers["X-Haravan-Org-Id"]);
                context.ActionArguments.Add("body", bodyString);
                //context.ActionArguments.Add("test", request.Headers.ContainsKey("X-Haravan-Test"));
            }

            await next();
        }
    }
}
