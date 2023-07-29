using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Hangfire;
 using Hangfire.SqlServer;
using HangfireBasicAuthenticationFilter;
using LookOn.BackgroundWorkers.Datalytis;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.MicrosoftAccount;
using Microsoft.AspNetCore.Authentication.Twitter;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using LookOn.MongoDB;
using LookOn.MultiTenancy;
using StackExchange.Redis;
using Microsoft.OpenApi.Models;
using LookOn.HealthChecks;
using Microsoft.AspNetCore.Localization;
using Volo.Abp.Caching.StackExchangeRedis;
using Volo.Abp;
using Volo.Abp.Account;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc.UI.MultiTenancy;
using Volo.Abp.AspNetCore.Serilog;
using Volo.Abp.Autofac;
using Volo.Abp.BackgroundJobs.Hangfire;
using Volo.Abp.BackgroundWorkers;
using Volo.Abp.BackgroundWorkers.Hangfire;
using Volo.Abp.Caching;
using Volo.Abp.Identity.AspNetCore;
using Volo.Abp.Modularity;
using Volo.Abp.Swashbuckle;
using Volo.Abp.UI.Navigation.Urls;
using Volo.Abp.VirtualFileSystem;

namespace LookOn;
[DependsOn(typeof(AbpBackgroundWorkersHangfireModule))]
[DependsOn(typeof(AbpBackgroundJobsHangfireModule))]
[DependsOn(typeof(LookOnBackgroundWorkerDomainModule))]


[DependsOn(typeof(LookOnHttpApiModule),
              typeof(AbpAutofacModule),
              typeof(AbpCachingStackExchangeRedisModule),
              typeof(AbpAspNetCoreMvcUiMultiTenancyModule),
              typeof(AbpIdentityAspNetCoreModule),
              typeof(LookOnApplicationModule),
              typeof(AbpSwashbuckleModule),
              typeof(AbpAspNetCoreSerilogModule),
              typeof(LookOnMongoDbModule))]
public class LookOnHttpApiHostModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        var configuration      = context.Services.GetConfiguration();
        var hostingEnvironment = context.Services.GetHostingEnvironment();

        ConfigureUrls(configuration);
        ConfigureConventionalControllers();
        ConfigureAuthentication(context, configuration);
        ConfigureSwagger(context, configuration);
        ConfigureCache(configuration);
        ConfigureVirtualFileSystem(context);
        ConfigureDataProtection(context, configuration, hostingEnvironment);
        ConfigureCors(context, configuration);
        ConfigureExternalProviders(context);
        ConfigureHealthChecks(context);
        
        ConfigureHangfire(context, configuration);
    }

    private void ConfigureHealthChecks(ServiceConfigurationContext context)
    {
        context.Services.AddLookOnHealthChecks();
    }

    private void ConfigureHangfire(ServiceConfigurationContext context, IConfiguration configuration)
    {
        // hangfire - disable retry
        GlobalJobFilters.Filters.Add(new AutomaticRetryAttribute { Attempts = 0 });

            
            // JobStorage.Current = new SqlServerStorage(configuration.GetConnectionString("Hangfire"));
        context.Services.AddHangfire(config =>
        {
            config.UseSqlServerStorage(configuration.GetConnectionString("Hangfire"),
                                       new SqlServerStorageOptions
                                       {
                                           CommandBatchMaxTimeout       = TimeSpan.FromMinutes(5),
                                           SlidingInvisibilityTimeout   = TimeSpan.FromMinutes(5),
                                           QueuePollInterval            = TimeSpan.Zero,
                                           UseRecommendedIsolationLevel = true,
                                           DisableGlobalLocks           = true,
                                           SchemaName                   = "LookOnJob"
                                       });
            // hangfire - disable retry
            // use GlobalJobFilters above
            // config.UseFilter(new AutomaticRetryAttribute { Attempts = 0 });
        });
    }

    private void ConfigureUrls(IConfiguration configuration)
    {
        Configure<AppUrlOptions>(options =>
        {
            options.Applications["Angular"].RootUrl                                 = configuration["App:AngularUrl"];
            options.Applications["Angular"].Urls[AccountUrlNames.PasswordReset]     = "account/reset-password";
            options.Applications["Angular"].Urls[AccountUrlNames.EmailConfirmation] = "account/email-confirmation";
        });
    }

    private void ConfigureCache(IConfiguration configuration)
    {
        Configure<AbpDistributedCacheOptions>(options => { options.KeyPrefix = "LookOn:"; });
    }

    private void ConfigureVirtualFileSystem(ServiceConfigurationContext context)
    {
        var hostingEnvironment = context.Services.GetHostingEnvironment();
        if (hostingEnvironment.IsDevelopment())
        {
            Configure<AbpVirtualFileSystemOptions>(options =>
            {
                options.FileSets.ReplaceEmbeddedByPhysical<LookOnDomainSharedModule>(Path.Combine(hostingEnvironment.ContentRootPath,
                                                                                                  string.Format("..{0}..{0}src{0}LookOn.Domain.Shared", Path.DirectorySeparatorChar)));
                options.FileSets.ReplaceEmbeddedByPhysical<LookOnDomainModule>(Path.Combine(hostingEnvironment.ContentRootPath,
                                                                                            string.Format("..{0}..{0}src{0}LookOn.Domain", Path.DirectorySeparatorChar)));
                options.FileSets.ReplaceEmbeddedByPhysical<LookOnApplicationContractsModule>(Path.Combine(hostingEnvironment.ContentRootPath,
                                                                                                          string.Format("..{0}..{0}src{0}LookOn.Application.Contracts", Path.DirectorySeparatorChar)));
                options.FileSets.ReplaceEmbeddedByPhysical<LookOnApplicationModule>(Path.Combine(hostingEnvironment.ContentRootPath,
                                                                                                 string.Format("..{0}..{0}src{0}LookOn.Application", Path.DirectorySeparatorChar)));
                options.FileSets.ReplaceEmbeddedByPhysical<LookOnHttpApiModule>(Path.Combine(hostingEnvironment.ContentRootPath,
                                                                                             string.Format("..{0}..{0}src{0}LookOn.HttpApi", Path.DirectorySeparatorChar)));
            });
        }
    }

    private void ConfigureConventionalControllers()
    {
        Configure<AbpAspNetCoreMvcOptions>(options => { options.ConventionalControllers.Create(typeof(LookOnApplicationModule).Assembly); });
    }

    private void ConfigureAuthentication(ServiceConfigurationContext context, IConfiguration configuration)
    {
        context.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
               .AddJwtBearer(options =>
                {
                    options.Authority            = configuration["AuthServer:Authority"];
                    options.RequireHttpsMetadata = Convert.ToBoolean(configuration["AuthServer:RequireHttpsMetadata"]);
                    options.Audience             = "LookOn";
                });
    }

    private static void ConfigureSwagger(ServiceConfigurationContext context, IConfiguration configuration)
    {
        context.Services.AddAbpSwaggerGenWithOAuth(configuration["AuthServer:Authority"],
                                                   new Dictionary<string, string> { { "LookOn", "LookOn API" } },
                                                   options =>
                                                   {
                                                       options.SwaggerDoc("v1", new OpenApiInfo { Title = "LookOn API", Version = "v1" });
                                                       options.DocInclusionPredicate((docName, description) => true);
                                                       options.CustomSchemaIds(type => type.FullName);
                                                   });
    }

    private void ConfigureDataProtection(ServiceConfigurationContext context, IConfiguration configuration, IWebHostEnvironment hostingEnvironment)
    {
        var dataProtectionBuilder = context.Services.AddDataProtection().SetApplicationName("LookOn");
        if (!hostingEnvironment.IsDevelopment())
        {
            var redis = ConnectionMultiplexer.Connect(configuration["Redis:Configuration"]);
            dataProtectionBuilder.PersistKeysToStackExchangeRedis(redis, "LookOn-Protection-Keys");
        }
    }

    private void ConfigureCors(ServiceConfigurationContext context, IConfiguration configuration)
    {
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
    }

    private void ConfigureExternalProviders(ServiceConfigurationContext context)
    {
        context.Services.AddDynamicExternalLoginProviderOptions<GoogleOptions>(GoogleDefaults.AuthenticationScheme,
                                                                               options =>
                                                                               {
                                                                                   options.WithProperty(x => x.ClientId);
                                                                                   options.WithProperty(x => x.ClientSecret, isSecret: true);
                                                                               })
               .AddDynamicExternalLoginProviderOptions<MicrosoftAccountOptions>(MicrosoftAccountDefaults.AuthenticationScheme,
                                                                                options =>
                                                                                {
                                                                                    options.WithProperty(x => x.ClientId);
                                                                                    options.WithProperty(x => x.ClientSecret, isSecret: true);
                                                                                })
               .AddDynamicExternalLoginProviderOptions<TwitterOptions>(TwitterDefaults.AuthenticationScheme,
                                                                       options =>
                                                                       {
                                                                           options.WithProperty(x => x.ConsumerKey);
                                                                           options.WithProperty(x => x.ConsumerSecret, isSecret: true);
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

        //app.UseAbpRequestLocalization();
        var supportedCultures = new[] { new CultureInfo("vi") };
        app.UseAbpRequestLocalization(options =>
        {
            options.DefaultRequestCulture   = new RequestCulture("vi");
            options.SupportedCultures       = supportedCultures;
            options.SupportedUICultures     = supportedCultures;
            options.RequestCultureProviders = new List<IRequestCultureProvider> { new QueryStringRequestCultureProvider(), new CookieRequestCultureProvider() };
        });

        app.UseStaticFiles();
        app.UseRouting();
        app.UseCors();
        app.UseAuthentication();
#pragma warning disable CS0162
        if (MultiTenancyConsts.IsEnabled)
        {
            app.UseMultiTenancy();
        }
#pragma warning restore CS0162
        
        app.UseAuthorization();
        app.UseSwagger();
        app.UseAbpSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "LookOn API");
            var configuration = context.GetConfiguration();
            options.OAuthClientId(configuration["AuthServer:SwaggerClientId"]);
            options.OAuthClientSecret(configuration["AuthServer:SwaggerClientSecret"]);
        });
        app.UseAuditing();
        app.UseAbpSerilogEnrichers();
        app.UseUnitOfWork();
        app.UseConfiguredEndpoints();
        
        app.UseHangfireDashboard("/hangfire", new DashboardOptions { Authorization = new[] { new HangfireCustomBasicAuthenticationFilter { User = "admin", Pass = "123321" } } });

    }
}