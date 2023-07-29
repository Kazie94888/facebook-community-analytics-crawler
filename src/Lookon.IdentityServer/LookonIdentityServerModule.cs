using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Localization.Resources.AbpUi;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.MicrosoftAccount;
using Microsoft.AspNetCore.Authentication.Twitter;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.DataProtection;
using Volo.Abp.Caching.StackExchangeRedis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using LookOn.MongoDB;
using LookOn.Localization;
using LookOn.MultiTenancy;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using StackExchange.Redis;
using Volo.Abp;
using Volo.Abp.Account;
using Volo.Abp.Account.Public.Web;
using Volo.Abp.Account.Public.Web.ExternalProviders;
using Volo.Abp.Account.Web;
using Volo.Abp.Account.Public.Web.Impersonation;
using Volo.Abp.AspNetCore.Mvc.AntiForgery;
using Volo.Abp.AspNetCore.Mvc.UI.Bundling;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.Lepton;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.Lepton.Bundling;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.Shared;
using Volo.Abp.AspNetCore.Serilog;
using Volo.Abp.Auditing;
using Volo.Abp.Autofac;
using Volo.Abp.AutoMapper;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.Caching;
using Volo.Abp.Identity;
using Volo.Abp.Localization;
using Volo.Abp.Modularity;
using Volo.Abp.UI.Navigation.Urls;
using Volo.Abp.VirtualFileSystem;
using Volo.Saas.Host;

namespace LookOn;

[DependsOn(typeof(AbpAutofacModule),
              typeof(AbpCachingStackExchangeRedisModule),
              typeof(AbpAspNetCoreSerilogModule),
              typeof(AbpAccountPublicWebIdentityServerModule),
              typeof(AbpAccountPublicHttpApiModule),
              typeof(AbpAspNetCoreMvcUiLeptonThemeModule),
              typeof(AbpAccountPublicApplicationModule),
              typeof(AbpAccountPublicWebImpersonationModule),
              typeof(SaasHostApplicationContractsModule),
              typeof(LookOnMongoDbModule),
              typeof(AbpAutoMapperModule))]
public class LookOnIdentityServerModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        //You can disable this setting in production to avoid any potential security risks.
        Microsoft.IdentityModel.Logging.IdentityModelEventSource.ShowPII = true;

        var hostingEnvironment = context.Services.GetHostingEnvironment();
        var configuration      = context.Services.GetConfiguration();

        Configure<AbpLocalizationOptions>(options => { options.Resources.Get<LookOnResource>().AddBaseTypes(typeof(AbpUiResource)); });

        Configure<AbpBundlingOptions>(options => { options.StyleBundles.Configure(LeptonThemeBundles.Styles.Global, bundle => { bundle.AddFiles("/global-styles.css"); }); });

        Configure<AbpAuditingOptions>(options =>
        {
            //options.IsEnabledForGetRequests = true;
            options.ApplicationName = "AuthServer";
        });

        if (hostingEnvironment.IsDevelopment())
        {
            Configure<AbpVirtualFileSystemOptions>(options =>
            {
                options.FileSets.ReplaceEmbeddedByPhysical<LookOnDomainSharedModule>(Path.Combine(hostingEnvironment.ContentRootPath,
                                                                                                  string.Format("..{0}LookOn.Domain.Shared", Path.DirectorySeparatorChar)));
                options.FileSets.ReplaceEmbeddedByPhysical<LookOnDomainModule>(Path.Combine(hostingEnvironment.ContentRootPath, string.Format("..{0}LookOn.Domain", Path.DirectorySeparatorChar)));
            });
        }

        Configure<AppUrlOptions>(options =>
        {
            options.Applications["MVC"].RootUrl = configuration["App:SelfUrl"];
            options.RedirectAllowedUrls.AddRange(configuration["App:RedirectAllowedUrls"].Split(','));
        });

        Configure<AbpBackgroundJobOptions>(options => { options.IsJobExecutionEnabled = false; });

        Configure<AbpDistributedCacheOptions>(options => { options.KeyPrefix = "LookOn:"; });

        var dataProtectionBuilder = context.Services.AddDataProtection().SetApplicationName("LookOn");
        if (!hostingEnvironment.IsDevelopment())
        {
            var redis = ConnectionMultiplexer.Connect(configuration["Redis:Configuration"]);
            dataProtectionBuilder.PersistKeysToStackExchangeRedis(redis, "LookOn-Protection-Keys");
        }

        context.Services.AddCors(options =>
        {
            options.AddDefaultPolicy(builder =>
            {
                builder.WithOrigins(configuration["App:CorsOrigins"]
                                   .Split(",", StringSplitOptions.RemoveEmptyEntries)
                                   .Select(o => o.Trim().RemovePostFix("/"))
                                   .ToArray())
                       .WithAbpExposedHeaders()
                       .SetIsOriginAllowedToAllowWildcardSubdomains()
                       .AllowAnyHeader()
                       .AllowAnyMethod()
                       .AllowCredentials();
            });
        });
        var authority = "https://accounts.haravan.com";

        context.Services.Configure<CookiePolicyOptions>(options =>
        {
            // This lambda determines whether user consent for non-essential cookies is needed for a given request.
            options.CheckConsentNeeded    = ctx => true;
            options.MinimumSameSitePolicy = SameSiteMode.None;

            options.OnAppendCookie = cookieContext => CheckSameSite(cookieContext.Context, cookieContext.CookieOptions);
            options.OnDeleteCookie = cookieContext => CheckSameSite(cookieContext.Context, cookieContext.CookieOptions);
        });

        context.Services.AddAuthentication()
               //.AddCookie()

               .AddCookie("Cookies.External")
               .AddOpenIdConnect("Haravan",
                                 "Login With Haravan",
                                 options =>
                                 {
                                     options.RequireHttpsMetadata          = false;
                                     options.SignInScheme                  = "Cookies.External";
                                     options.Authority                     = authority;
                                     options.ClientId                      = configuration["HaravanApp:ClientId"];
                                     options.ClientSecret                  = configuration["HaravanApp:ClientSecret"];
                                     options.ResponseType                  = "code id_token";
                                     options.SaveTokens                    = true;
                                     options.GetClaimsFromUserInfoEndpoint = true;
                                     options.Scope.Add("org");
                                     options.Scope.Add("userinfo");
                                     options.Scope.Add("email");

                                     options.Events = new OpenIdConnectEvents();
                                     options.Events.OnRedirectToIdentityProvider = (ctx) =>
                                     {
                                         var grantService = ctx.Properties.GetParameter<bool?>("grant_service");
                                         if (grantService != null)
                                         {
                                             ctx.ProtocolMessage.Scope += " grant_service";
                                         }

                                         var offlineAccess = ctx.Properties.GetParameter<bool?>("offline_access");
                                         if (offlineAccess != null)
                                         {
                                             ctx.ProtocolMessage.Scope += " offline_access";
                                         }
                                         
                                         var webhookAccess = ctx.Properties.GetParameter<bool?>("wh_api");
                                         if (webhookAccess != null)
                                         {
                                             ctx.ProtocolMessage.Scope += " wh_api";
                                         }

                                         foreach (var parameter in ctx.Properties.Parameters.Where(x => x.Key.Contains("com.")))
                                         {
                                             ctx.ProtocolMessage.Scope += $" {parameter.Key}";
                                         }
                                         
                                         

                                         var orgid = ctx.Properties.GetParameter<string>("orgid");
                                         if (orgid != null)
                                         {
                                             ctx.ProtocolMessage.Parameters.Add("orgid", orgid);
                                         }

                                         return Task.CompletedTask;
                                     };
                                 })
               .AddJwtBearer(options =>
                {
                    options.Authority            = configuration["AuthServer:Authority"];
                    options.RequireHttpsMetadata = Convert.ToBoolean(configuration["AuthServer:RequireHttpsMetadata"]);
                    options.Audience             = "LookOn";
                })
               .AddGoogle(GoogleDefaults.AuthenticationScheme, _ => { })
               .WithDynamicOptions<GoogleOptions, GoogleHandler>(GoogleDefaults.AuthenticationScheme,
                                                                 options =>
                                                                 {
                                                                     options.WithProperty(x => x.ClientId);
                                                                     options.WithProperty(x => x.ClientSecret, isSecret: true);
                                                                 })
               .AddMicrosoftAccount(MicrosoftAccountDefaults.AuthenticationScheme,
                                    options =>
                                    {
                                        //Personal Microsoft accounts as an example.
                                        options.AuthorizationEndpoint = "https://login.microsoftonline.com/consumers/oauth2/v2.0/authorize";
                                        options.TokenEndpoint         = "https://login.microsoftonline.com/consumers/oauth2/v2.0/token";
                                    })
               .WithDynamicOptions<MicrosoftAccountOptions, MicrosoftAccountHandler>(MicrosoftAccountDefaults.AuthenticationScheme,
                                                                                     options =>
                                                                                     {
                                                                                         options.WithProperty(x => x.ClientId);
                                                                                         options.WithProperty(x => x.ClientSecret, isSecret: true);
                                                                                     })
               .AddTwitter(TwitterDefaults.AuthenticationScheme, options => options.RetrieveUserDetails = true)
               .WithDynamicOptions<TwitterOptions, TwitterHandler>(TwitterDefaults.AuthenticationScheme,
                                                                   options =>
                                                                   {
                                                                       options.WithProperty(x => x.ConsumerKey);
                                                                       options.WithProperty(x => x.ConsumerSecret, isSecret: true);
                                                                   });

        context.Services.Configure<AbpAccountOptions>(options =>
        {
            options.TenantAdminUserName           = "admin";
            options.ImpersonationTenantPermission = SaasHostPermissions.Tenants.Impersonation;
            options.ImpersonationUserPermission   = IdentityPermissions.Users.Impersonation;
        });

        Configure<AbpAutoMapperOptions>(options => { options.AddMaps<LookOnIdentityServerModule>(); });

        //ConfigureIdentityOptions(context);

        //Set expired token reset password
        Configure<DataProtectionTokenProviderOptions>(option => { option.TokenLifespan = TimeSpan.FromHours(1); });

        Configure<AbpAntiForgeryOptions>(options => { options.TokenCookie.Expiration = TimeSpan.FromDays(365); });
    }

    private static void CheckSameSite(HttpContext httpContext, CookieOptions options)
    {
        if (options.SameSite == SameSiteMode.None)
        {
            var userAgent = httpContext.Request.Headers["User-Agent"].ToString();
            if (!httpContext.Request.IsHttps || DisallowsSameSiteNone(userAgent))
            {
                // For .NET Core < 3.1 set SameSite = (SameSiteMode)(-1)
                options.SameSite = SameSiteMode.Unspecified;
            }
        }
    }

    private static bool DisallowsSameSiteNone(string userAgent)
    {
        // Cover all iOS based browsers here. This includes:
        // - Safari on iOS 12 for iPhone, iPod Touch, iPad
        // - WkWebview on iOS 12 for iPhone, iPod Touch, iPad
        // - Chrome on iOS 12 for iPhone, iPod Touch, iPad
        // All of which are broken by SameSite=None, because they use the iOS networking stack
        if (userAgent.Contains("CPU iPhone OS 12") || userAgent.Contains("iPad; CPU OS 12"))
        {
            return true;
        }

        // Cover Mac OS X based browsers that use the Mac OS networking stack. This includes:
        // - Safari on Mac OS X.
        // This does not include:
        // - Chrome on Mac OS X
        // Because they do not use the Mac OS networking stack.
        if (userAgent.Contains("Macintosh; Intel Mac OS X 10_14") && userAgent.Contains("Version/") && userAgent.Contains("Safari"))
        {
            return true;
        }

        // Cover Chrome 50-69, because some versions are broken by SameSite=None,
        // and none in this range require it.
        // Note: this covers some pre-Chromium Edge versions,
        // but pre-Chromium Edge does not require SameSite=None.
        if (userAgent.Contains("Chrome/5") || userAgent.Contains("Chrome/6"))
        {
            return true;
        }

        return false;
    }

    private void ConfigureIdentityOptions(ServiceConfigurationContext context)
    {
        context.Services.Configure<IdentityOptions>(options =>
        {
            options.SignIn.RequireConfirmedAccount     = true;
            options.SignIn.RequireConfirmedEmail       = true;
            options.SignIn.RequireConfirmedPhoneNumber = false;
        });
    }

    public override void OnApplicationInitialization(ApplicationInitializationContext context)
    {
        var app = context.GetApplicationBuilder();
        var env = context.GetEnvironment();

        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseAbpRequestLocalization();

        if (!env.IsDevelopment())
        {
            app.UseErrorPage();
        }

        app.UseCorrelationId();
        app.UseStaticFiles();
        app.UseRouting();
        app.UseCors();
        
        app.UseCookiePolicy();
        app.UseAuthentication();
        app.UseJwtTokenMiddleware();

#pragma warning disable CS0162
        if (MultiTenancyConsts.IsEnabled)
        {
            app.UseMultiTenancy();
        }
#pragma warning restore CS0162

        app.UseUnitOfWork();
        app.UseIdentityServer();
        app.UseAuthorization();

        app.UseAuditing();
        app.UseAbpSerilogEnrichers();
        app.UseConfiguredEndpoints();
    }
}