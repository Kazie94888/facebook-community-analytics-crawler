using LookOn.Localization;
using Volo.Abp.AuditLogging;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.FeatureManagement;
using Volo.Abp.Identity;
using Volo.Abp.IdentityServer;
using Volo.Abp.LanguageManagement;
using Volo.Abp.LeptonTheme.Management;
using Volo.Abp.Localization;
using Volo.Abp.Localization.ExceptionHandling;
using Volo.Abp.Validation.Localization;
using Volo.Abp.Modularity;
using Volo.Abp.PermissionManagement;
using Volo.Abp.SettingManagement;
using Volo.Abp.TextTemplateManagement;
using Volo.Abp.VirtualFileSystem;
using Volo.Saas;
using Volo.Abp.BlobStoring.Database;
using Volo.Abp.GlobalFeatures;

namespace LookOn;

[DependsOn(typeof(AbpAuditLoggingDomainSharedModule),
              typeof(AbpBackgroundJobsDomainSharedModule),
              typeof(AbpFeatureManagementDomainSharedModule),
              typeof(AbpIdentityProDomainSharedModule),
              typeof(AbpIdentityServerDomainSharedModule),
              typeof(AbpPermissionManagementDomainSharedModule),
              typeof(AbpSettingManagementDomainSharedModule),
              typeof(LanguageManagementDomainSharedModule),
              typeof(SaasDomainSharedModule),
              typeof(TextTemplateManagementDomainSharedModule),
              typeof(LeptonThemeManagementDomainSharedModule),
              typeof(AbpGlobalFeaturesModule),
              typeof(BlobStoringDatabaseDomainSharedModule),
              typeof(GlobalConfigurationModule))]
public class LookOnDomainSharedModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        LookOnGlobalFeatureConfigurator.Configure();
        LookOnModuleExtensionConfigurator.Configure();
    }

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpVirtualFileSystemOptions>(options => { options.FileSets.AddEmbedded<LookOnDomainSharedModule>(); });

        Configure<AbpLocalizationOptions>(options =>
        {
            // options.Resources.Add<LookOnResource>("en").AddBaseTypes(typeof(AbpValidationResource)).AddVirtualJson("/Localization/LookOn");
            // options.Resources.Add<LookOnErrorResource>("en").AddBaseTypes(typeof(AbpValidationResource)).AddVirtualJson("/Localization/LookOnError");

            options.Resources.Add<LookOnResource>("vi").AddBaseTypes(typeof(AbpValidationResource)).AddVirtualJson("/Localization/LookOn");
            options.Resources.Add<LookOnErrorResource>("vi").AddBaseTypes(typeof(AbpValidationResource)).AddVirtualJson("/Localization/LookOnError");

            options.DefaultResourceType = typeof(LookOnResource);
        });

        Configure<AbpExceptionLocalizationOptions>(options =>
        {
            options.MapCodeNamespace("LookOn",      typeof(LookOnResource));
            options.MapCodeNamespace("LookOnError", typeof(LookOnErrorResource));
        });
    }
}