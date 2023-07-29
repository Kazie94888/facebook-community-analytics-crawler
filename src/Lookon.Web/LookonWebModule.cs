using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using LookOn.Localization;
using LookOn.MultiTenancy;
using LookOn.Permissions;
using LookOn.Web.Components.DevExtremeJs;
using LookOn.Web.Contributors;
using LookOn.Web.Menus;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using StackExchange.Redis;
using Volo.Abp.Caching.StackExchangeRedis;
using Microsoft.OpenApi.Models;
using Volo.Abp;
using Volo.Abp.Account.Admin.Web;
using Volo.Abp.Account.Public.Web.Impersonation;
using Volo.Abp.AspNetCore.Authentication.OpenIdConnect;
using Volo.Abp.AspNetCore.Mvc.AntiForgery;
using Volo.Abp.AspNetCore.Mvc.Client;
using Volo.Abp.AspNetCore.Mvc.Localization;
using Volo.Abp.AspNetCore.Mvc.UI;
using Volo.Abp.AspNetCore.Mvc.UI.Bootstrap;
using Volo.Abp.AspNetCore.Mvc.UI.Bundling;
using Volo.Abp.AspNetCore.Mvc.UI.Components.LayoutHook;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.Commercial;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.Lepton;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.Lepton.Bundling;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.Shared;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.Shared.Bundling;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.Shared.Toolbars;
using Volo.Abp.AspNetCore.Serilog;
using Volo.Abp.AuditLogging.Web;
using Volo.Abp.Autofac;
using Volo.Abp.AutoMapper;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.Caching;
using Volo.Abp.FeatureManagement;
using Volo.Abp.Http.Client.IdentityModel.Web;
using Volo.Abp.Identity.Web;
using Volo.Abp.Http.Client.Web;
using Volo.Abp.IdentityServer.Web;
using Volo.Abp.LanguageManagement;
using Volo.Abp.LeptonTheme.Management;
using Volo.Abp.Modularity;
using Volo.Abp.MultiTenancy;
using Volo.Abp.PermissionManagement.Web;
using Volo.Abp.Swashbuckle;
using Volo.Abp.TextTemplateManagement.Web;
using Volo.Abp.UI;
using Volo.Abp.UI.Navigation.Urls;
using Volo.Abp.UI.Navigation;
using Volo.Abp.VirtualFileSystem;
using Volo.Saas.Host;
#pragma warning disable CS0162

namespace LookOn.Web;

[DependsOn(
    typeof(LookOnHttpApiClientModule),
    typeof(LookOnHttpApiModule),
    typeof(AbpAccountPublicWebImpersonationModule),
    typeof(AbpAspNetCoreMvcClientModule),
    typeof(AbpHttpClientWebModule),
    typeof(AbpAutofacModule),
    typeof(AbpCachingStackExchangeRedisModule),
    typeof(AbpFeatureManagementWebModule),
    typeof(AbpAccountAdminWebModule),
    typeof(AbpHttpClientIdentityModelWebModule),
    typeof(AbpIdentityWebModule),
    typeof(AbpAuditLoggingWebModule),
    typeof(LeptonThemeManagementWebModule),
    typeof(SaasHostWebModule),
    typeof(AbpIdentityServerWebModule),
    typeof(AbpAspNetCoreMvcUiLeptonThemeModule),
    typeof(LanguageManagementWebModule),
    typeof(TextTemplateManagementWebModule),
    typeof(AbpSwashbuckleModule),
    typeof(AbpAspNetCoreSerilogModule)
    //typeof(HaravanManagementWebModule)
    )]
public class LookOnWebModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.PreConfigure<AbpMvcDataAnnotationsLocalizationOptions>(options =>
        {
            options.AddAssemblyResource(
                typeof(LookOnResource),
                typeof(LookOnDomainSharedModule).Assembly,
                typeof(LookOnApplicationContractsModule).Assembly,
                typeof(LookOnWebModule).Assembly
            );
        });
    }

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        var hostingEnvironment = context.Services.GetHostingEnvironment();
        var configuration = context.Services.GetConfiguration();

        ConfigureBundles();
        ConfigurePages(configuration);
        ConfigureCache(configuration);
        ConfigureDataProtection(context, configuration, hostingEnvironment);
        ConfigureUrls(configuration);
        ConfigureAuthentication(context, configuration);
        ConfigureImpersonation(context, configuration);
        ConfigureAutoMapper();
        ConfigureVirtualFileSystem(hostingEnvironment);
        ConfigureNavigationServices(configuration);
        ConfigureSwaggerServices(context.Services);
        ConfigureMultiTenancy();
        ConfigureBackgroundJobs();
        ConfigureDevExtreme();
        
        Configure<AbpAntiForgeryOptions>(options =>
        {
            options.TokenCookie.Expiration = TimeSpan.FromDays(365);
        });
    }

    private void ConfigureBackgroundJobs()
    {
        Configure<AbpBackgroundJobOptions>(options =>
        {
            options.IsJobExecutionEnabled = false;
        });
    }

    private void ConfigureDevExtreme()
    {
        Configure<AbpBundlingOptions>(options =>
        {
            options.StyleBundles.Get(StandardBundles.Styles.Global).AddContributors(typeof(DevExtremeStyleContributor));
        });

        Configure<AbpLayoutHookOptions>(options =>
        {
            options.Add(
                LayoutHooks.Head.Last, //The hook name
                typeof(DevExtremeJsViewComponent) //The component to add
            );
        });
    }
    private void ConfigureBundles()
    {
        Configure<AbpBundlingOptions>(options =>
        {
            options.StyleBundles.Configure(
                LeptonThemeBundles.Styles.Global,
                bundle =>
                {
                    bundle.AddFiles("/global-styles.css");
                    bundle.AddFiles("/libs/font-family/Open Sans.css");
                    bundle.AddFiles("/libs/veek-libs/veek.css");
                }
            );
        });
    }

    private void ConfigurePages(IConfiguration configuration)
    {
        Configure<RazorPagesOptions>(options =>
        {
            options.Conventions.AuthorizePage("/HostDashboard", LookOnPermissions.Dashboard.Host);
            options.Conventions.AuthorizePage("/TenantDashboard", LookOnPermissions.Dashboard.Tenant);
            options.Conventions.AuthorizePage("/Insights/Index", LookOnPermissions.Insight.View);
            options.Conventions.AuthorizePage("/Merchants/Index", LookOnPermissions.Merchants.View);
            options.Conventions.AuthorizePage("/MerchantConnects/Index", LookOnPermissions.Merchants.View);
            options.Conventions.AuthorizePage("/MerchantSubscriptionDetails/Index", LookOnPermissions.Merchants.View);
            options.Conventions.AuthorizePage("/UserInfos/Index", LookOnPermissions.Merchants.View);
            options.Conventions.AuthorizePage("/Subscriptions/Index", LookOnPermissions.Subscriptions.View);
            options.Conventions.AuthorizePage("/Categories/Index", LookOnPermissions.Categories.View);
            options.Conventions.AuthorizePage("/UserInfos/Index", LookOnPermissions.UserInfos.View);
            options.Conventions.AuthorizePage("/Platforms/Index", LookOnPermissions.Platforms.View);
            options.Conventions.AuthorizePage("/MerchantStores/Index", LookOnPermissions.MerchantStores.View);
            options.Conventions.AuthorizePage("/MerchantSubscriptions/Index", LookOnPermissions.MerchantSubscriptions.View);
            options.Conventions.AuthorizePage("/MerchantStaffs/Index", LookOnPermissions.MerchantStaffs.View);
            options.Conventions.AuthorizePage("/MerchantSyncInfos/Index", LookOnPermissions.MerchantSyncInfos.View);
            options.Conventions.AuthorizePage("/Support/Support", LookOnPermissions.Support.View);
        });
    }

    private void ConfigureCache(IConfiguration configuration)
    {
        Configure<AbpDistributedCacheOptions>(options =>
        {
            options.KeyPrefix = "LookOn:";
        });
    }

    private void ConfigureUrls(IConfiguration configuration)
    {
        Configure<AppUrlOptions>(options =>
        {
            options.Applications["MVC"].RootUrl = configuration["App:SelfUrl"];
        });
    }

    private void ConfigureMultiTenancy()
    {
        Configure<AbpMultiTenancyOptions>(options => { options.IsEnabled = MultiTenancyConsts.IsEnabled; });
    }

    private void ConfigureAuthentication(ServiceConfigurationContext context, IConfiguration configuration)
    {
        context.Services.AddAuthentication(options =>
            {
                options.DefaultScheme = "Cookies";
                options.DefaultChallengeScheme = "oidc";
            })
            .AddCookie("Cookies", options =>
            {
                options.ExpireTimeSpan = TimeSpan.FromDays(365);
            })
            .AddAbpOpenIdConnect("oidc", options =>
            {
                options.Authority = configuration["AuthServer:Authority"];
                options.RequireHttpsMetadata = Convert.ToBoolean(configuration["AuthServer:RequireHttpsMetadata"]); ;
                options.ResponseType = OpenIdConnectResponseType.CodeIdToken;

                options.ClientId = configuration["AuthServer:ClientId"];
                options.ClientSecret = configuration["AuthServer:ClientSecret"];

                options.SaveTokens = true;
                options.GetClaimsFromUserInfoEndpoint = true;

                options.Scope.Add("role");
                options.Scope.Add("email");
                options.Scope.Add("phone");
                options.Scope.Add("LookOn");
                options.Scope.Add("HaravanManagement");

                options.Events = new OpenIdConnectEvents();
                options.Events.OnRedirectToIdentityProvider = (ctx) =>
                {
                    var parameter = ctx.Properties.GetParameter<bool?>("Haravan");
                    if (parameter != null)
                    {
                        //ctx.ProtocolMessage.Parameters.Add("LoginSource", "Haravan");
                        ctx.ProtocolMessage.SetParameter("LoginSource", "HaravanLogin");
                    }

                    return Task.CompletedTask;
                };
                
                //Handle remote failure
                options.Events.OnRemoteFailure = (ctx) =>
                {
                    ctx.Response.Redirect("/");
                    return Task.CompletedTask;
                };
            });
    }

    private void ConfigureImpersonation(ServiceConfigurationContext context, IConfiguration configuration)
    {
        context.Services.Configure<AbpSaasHostWebOptions>(options =>
        {
            options.EnableTenantImpersonation = true;
        });
        context.Services.Configure<AbpIdentityWebOptions>(options =>
        {
            options.EnableUserImpersonation = true;
        });
    }

    private void ConfigureAutoMapper()
    {
        Configure<AbpAutoMapperOptions>(options =>
        {
            options.AddMaps<LookOnWebModule>();
        });
    }

    private void ConfigureVirtualFileSystem(IWebHostEnvironment hostingEnvironment)
    {
        Configure<AbpVirtualFileSystemOptions>(options =>
        {
            options.FileSets.AddEmbedded<LookOnWebModule>();

            if (hostingEnvironment.IsDevelopment())
            {
                options.FileSets.ReplaceEmbeddedByPhysical<LookOnDomainSharedModule>(Path.Combine(hostingEnvironment.ContentRootPath, string.Format("..{0}LookOn.Domain.Shared", Path.DirectorySeparatorChar)));
                options.FileSets.ReplaceEmbeddedByPhysical<LookOnApplicationContractsModule>(Path.Combine(hostingEnvironment.ContentRootPath, string.Format("..{0}LookOn.Application.Contracts", Path.DirectorySeparatorChar)));
                options.FileSets.ReplaceEmbeddedByPhysical<LookOnWebModule>(hostingEnvironment.ContentRootPath);
            }
        });
    }

    private void ConfigureNavigationServices(IConfiguration configuration)
    {
        Configure<AbpNavigationOptions>(options =>
        {
            options.MenuContributors.Add(new LookOnMenuContributor(configuration));
        });

        Configure<AbpToolbarOptions>(options =>
        {
            options.Contributors.Add(new LookOnToolbarContributor());
        });
    }

    private void ConfigureSwaggerServices(IServiceCollection services)
    {
        services.AddAbpSwaggerGen(
            options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "LookOn API", Version = "v1" });
                options.DocInclusionPredicate((docName, description) => true);
                options.CustomSchemaIds(type => type.FullName);
            }
        );
    }

    private void ConfigureDataProtection(
        ServiceConfigurationContext context,
        IConfiguration configuration,
        IWebHostEnvironment hostingEnvironment)
    {
        var dataProtectionBuilder = context.Services.AddDataProtection().SetApplicationName("LookOn");
        if (!hostingEnvironment.IsDevelopment())
        {
            var redis = ConnectionMultiplexer.Connect(configuration["Redis:Configuration"]);
            dataProtectionBuilder.PersistKeysToStackExchangeRedis(redis, "LookOn-Protection-Keys");
        }
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

        app.UseStaticFiles();
        app.UseRouting();
        app.UseAuthentication();

        if (MultiTenancyConsts.IsEnabled)
        {
            app.UseMultiTenancy();
        }

        app.UseAuthorization();
        app.UseSwagger();
        app.UseAbpSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "LookOn API");
        });
        app.UseAbpSerilogEnrichers();
        app.UseConfiguredEndpoints();
    }
}