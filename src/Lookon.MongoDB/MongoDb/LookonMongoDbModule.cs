using LookOn.MerchantSyncInfos;
using LookOn.MerchantUsers;
using LookOn.MerchantSubscriptions;
using LookOn.MerchantStores;
using LookOn.Platforms;
using LookOn.UserInfos;
using LookOn.Categories;
using LookOn.Insights;
using LookOn.Integrations.Datalytis;
using LookOn.Integrations.Haravan;
using LookOn.Merchants;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.AuditLogging.MongoDB;
using Volo.Abp.BackgroundJobs.MongoDB;
using Volo.Abp.FeatureManagement.MongoDB;
using Volo.Abp.Identity.MongoDB;
using Volo.Abp.IdentityServer.MongoDB;
using Volo.Abp.LanguageManagement.MongoDB;
using Volo.Abp.Modularity;
using Volo.Abp.PermissionManagement.MongoDB;
using Volo.Abp.SettingManagement.MongoDB;
using Volo.Abp.TextTemplateManagement.MongoDB;
using Volo.Saas.MongoDB;
using Volo.Abp.BlobStoring.Database.MongoDB;
using Volo.Abp.Uow;

namespace LookOn.MongoDB;

[DependsOn(
    typeof(LookOnDomainModule),
    typeof(AbpPermissionManagementMongoDbModule),
    typeof(AbpSettingManagementMongoDbModule),
    typeof(AbpIdentityProMongoDbModule),
    typeof(AbpIdentityServerMongoDbModule),
    typeof(AbpBackgroundJobsMongoDbModule),
    typeof(AbpAuditLoggingMongoDbModule),
    typeof(AbpFeatureManagementMongoDbModule),
    typeof(LanguageManagementMongoDbModule),
    typeof(SaasMongoDbModule),
    typeof(TextTemplateManagementMongoDbModule),
    typeof(BlobStoringDatabaseMongoDbModule),
    typeof(DatalytisMongoDbModule),
    typeof(HaravanMongoDbModule)
)]
public class LookOnMongoDbModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddMongoDbContext<LookOnMongoDbContext>(options =>
        {
            options.AddDefaultRepositories(true);
            options.AddRepository<Merchant, Merchants.MongoMerchantRepository>();

            options.AddRepository<Category, Categories.MongoCategoryRepository>();

            options.AddRepository<UserInfo, UserInfos.MongoUserInfoRepository>();

            options.AddRepository<Platform, Platforms.MongoPlatformRepository>();

            options.AddRepository<MerchantStore, MerchantStores.MongoMerchantStoreRepository>();

            options.AddRepository<MerchantSubscription, MerchantSubscriptions.MongoMerchantSubscriptionRepository>();

            options.AddRepository<MerchantUser, MerchantUsers.MongoMerchantUserRepository>();

            options.AddRepository<MerchantSyncInfo, MerchantSyncInfos.MongoMerchantSyncInfoRepository>();
            options.AddRepository<Metric, MetricRepository>();
        });

        Configure<AbpUnitOfWorkDefaultOptions>(options =>
        {
            options.TransactionBehavior = UnitOfWorkTransactionBehavior.Disabled;
        });
    }
}