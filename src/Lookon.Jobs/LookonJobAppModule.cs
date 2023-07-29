using System;
using Hangfire;
using Hangfire.SqlServer;
using HangfireBasicAuthenticationFilter;
using LookOn.Jobs.Jobs;
using LookOn.MongoDB;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Serilog;
using Volo.Abp.Autofac;
using Volo.Abp.AutoMapper;
using Volo.Abp.BackgroundJobs.Hangfire;
using Volo.Abp.Modularity;

namespace LookOn.Jobs;

[DependsOn(typeof(AbpAspNetCoreMvcModule))]
[DependsOn(
    typeof(AbpAutofacModule), 
    typeof(LookOnApplicationModule),
    typeof(AbpAspNetCoreSerilogModule),
    typeof(AbpBackgroundJobsHangfireModule),
    typeof(LookOnDomainSharedModule),
    typeof(LookOnMongoDbModule),
    typeof(AbpAutoMapperModule)
    )] //Add dependency to ABP Autofac module
public class LookOnJobAppModule : AbpModule
{
    
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        var configuration = context.Services.GetConfiguration();
        var hostingEnvironment = context.Services.GetHostingEnvironment();
        
        ConfigureHangfire(context,configuration);
        
        context.Services.AddAutoMapperObjectMapper<LookOnJobAppModule>();
        Configure<AbpAutoMapperOptions>(options => { options.AddMaps<LookOnJobAppModule>(validate: true); });

    }
    private void ConfigureHangfire(ServiceConfigurationContext context, IConfiguration configuration)
    {
        // hangfire - disable retry
        GlobalJobFilters.Filters.Add(new AutomaticRetryAttribute { Attempts = 0 });
        
        context.Services.AddHangfire(config =>
        {
            config.UseSqlServerStorage
                (
                 configuration.GetConnectionString("Hangfire"),
                 new SqlServerStorageOptions
                 {
                     CommandBatchMaxTimeout       = TimeSpan.FromMinutes(5),
                     SlidingInvisibilityTimeout   = TimeSpan.FromMinutes(5),
                     QueuePollInterval            = TimeSpan.Zero,
                     UseRecommendedIsolationLevel = true,
                     DisableGlobalLocks           = true,
                     SchemaName                   = "LookOnJob"
                 }
                );
            // hangfire - disable retry
            // use GlobalJobFilters above
            // config.UseFilter(new AutomaticRetryAttribute { Attempts = 0 });
        });
    }
    
    public override void OnApplicationInitialization(ApplicationInitializationContext context)
    {
        var app = context.GetApplicationBuilder();
        var env = context.GetEnvironment();

        // Configure the HTTP request pipeline.
        if (env.IsDevelopment())
        {
            app.UseExceptionHandler("/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }
        
        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseRouting();
        app.UseConfiguredEndpoints();
        
        app.UseHangfireDashboard
        (
            "/hangfire",
            new DashboardOptions
            {
                Authorization = new[]
                {
                    new HangfireCustomBasicAuthenticationFilter
                    {
                        User = "admin",
                        Pass = "123321"
                    }
                }
            }
        );
        
        var hostEnvironment = context.ServiceProvider.GetRequiredService<IHostEnvironment>();
        
        InitJobs.Setup(hostEnvironment);
    }

}