using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using LookOn.MultiTenancy;
using Volo.Abp.AuditLogging;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.Emailing;
using Volo.Abp.Localization;
using Volo.Abp.Modularity;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Emailing.Smtp;
using Volo.Abp.BackgroundWorkers.Hangfire;
using Volo.Abp.BackgroundJobs.Hangfire;
using Volo.Abp.BlobStoring.Database;
using Volo.Abp.Commercial.SuiteTemplates;
using Volo.Abp.FeatureManagement;
using Volo.Abp.Identity;
using Volo.Abp.IdentityServer;
using Volo.Abp.LanguageManagement;
using Volo.Abp.LeptonTheme.Management;
using Volo.Abp.PermissionManagement.Identity;
using Volo.Abp.PermissionManagement.IdentityServer;
using Volo.Abp.SettingManagement;
using Volo.Abp.TextTemplateManagement;
using Volo.Saas;

namespace LookOn;

[DependsOn(typeof(LookOnDomainSharedModule),
              typeof(AbpAuditLoggingDomainModule),
              typeof(AbpBackgroundJobsDomainModule),
              typeof(AbpFeatureManagementDomainModule),
              typeof(AbpIdentityProDomainModule),
              typeof(AbpPermissionManagementDomainIdentityModule),
              typeof(AbpIdentityServerDomainModule),
              typeof(AbpPermissionManagementDomainIdentityServerModule),
              typeof(AbpSettingManagementDomainModule),
              typeof(SaasDomainModule),
              typeof(TextTemplateManagementDomainModule),
              typeof(LeptonThemeManagementDomainModule),
              typeof(LanguageManagementDomainModule),
              typeof(VoloAbpCommercialSuiteTemplatesModule),
              typeof(AbpEmailingModule),
              typeof(BlobStoringDatabaseDomainModule))]


public class LookOnDomainModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpMultiTenancyOptions>(options => { options.IsEnabled = MultiTenancyConsts.IsEnabled; });

        Configure<AbpLocalizationOptions>(options =>
        {
            //options.Languages.Add(new LanguageInfo("en", "en", "English", "gb"));
            options.Languages.Add(new LanguageInfo("vi",
                                                   "vi",
                                                   "Vietnamese",
                                                   "vn"));
        });

        context.Services.Replace(ServiceDescriptor.Singleton<IEmailSender, SmtpEmailSender>());

        // #if DEBUG
        //         context.Services.Replace(ServiceDescriptor.Singleton<IEmailSender, NullEmailSender>());
        // #endif
    }
}