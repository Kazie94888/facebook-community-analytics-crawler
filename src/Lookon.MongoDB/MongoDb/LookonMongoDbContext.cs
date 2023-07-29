using LookOn.MerchantSyncInfos;
using LookOn.MerchantUsers;
using LookOn.MerchantSubscriptions;
using LookOn.MerchantStores;
using LookOn.Platforms;
using LookOn.UserInfos;
using LookOn.Emails;
using LookOn.Feedbacks;
using LookOn.Insights;
using LookOn.Merchants;
using LookOn.SystemConfigs;
using LookOn.Users;
using MongoDB.Driver;
using Volo.Abp.Data;
using Volo.Abp.MongoDB;
using Category = LookOn.Categories.Category;

namespace LookOn.MongoDB;

[ConnectionStringName("Default")]
public class LookOnMongoDbContext : AbpMongoDbContext
{
    public IMongoCollection<Merchant>             Merchants             => Collection<Merchant>();
    public IMongoCollection<MerchantUser>         MerchantUsers         => Collection<MerchantUser>();
    public IMongoCollection<MerchantStore>        MerchantStores        => Collection<MerchantStore>();
    public IMongoCollection<MerchantSubscription> MerchantSubscriptions => Collection<MerchantSubscription>();
    public IMongoCollection<MerchantSyncInfo>     MerchantSyncInfos     => Collection<MerchantSyncInfo>();

    // others
    public IMongoCollection<SystemConfig>   SystemConfigs  => Collection<SystemConfig>();
    public IMongoCollection<Platform>       Platforms      => Collection<Platform>();
    public IMongoCollection<UserInfo>       UserInfos      => Collection<UserInfo>();
    public IMongoCollection<Category>       Categories     => Collection<Category>();
    public IMongoCollection<EmailHistories> EmailHistories => Collection<EmailHistories>();
    public IMongoCollection<UserFeedback>   UserFeedbacks  => Collection<UserFeedback>();

    // dashboard data
    // public IMongoCollection<Page1MetricEntity> Page1Metrics => Collection<Page1MetricEntity>();
    // public IMongoCollection<Page2MetricEntity> Page2Metrics => Collection<Page2MetricEntity>();
    // public IMongoCollection<Page3MetricEntity> Page3Metrics => Collection<Page3MetricEntity>();
    public IMongoCollection<Metric> Metrics => Collection<Metric>();
    /* Add mongo collections here. */
    public IMongoCollection<AppUser> Users => Collection<AppUser>();

    protected override void CreateModel(IMongoModelBuilder modelBuilder)
    {
        base.CreateModel(modelBuilder);

        modelBuilder.Entity<AppUser>(b =>
        {
            /* Sharing the same "AbpUsers" collection
             * with the Identity module's IdentityUser class. */
            b.CollectionName = "AbpUsers";
        });

        // merchants
        modelBuilder.Entity<Merchant>(b => { b.CollectionName             = LookOnConsts.DbTablePrefix + "Merchants"; });
        modelBuilder.Entity<MerchantUser>(b => { b.CollectionName         = LookOnConsts.DbTablePrefix + "MerchantUsers"; });
        modelBuilder.Entity<MerchantStore>(b => { b.CollectionName        = LookOnConsts.DbTablePrefix + "MerchantStores"; });
        modelBuilder.Entity<MerchantSubscription>(b => { b.CollectionName = LookOnConsts.DbTablePrefix + "MerchantSubscriptions"; });
        modelBuilder.Entity<MerchantSyncInfo>(b => { b.CollectionName     = LookOnConsts.DbTablePrefix + "MerchantSyncInfos"; });
        // modelBuilder.Entity<Page1MetricEntity>(b => { b.CollectionName    = LookOnConsts.DbTablePrefix + "Page1Metrics"; });
        // modelBuilder.Entity<Page2MetricEntity>(b => { b.CollectionName    = LookOnConsts.DbTablePrefix + "Page2Metrics"; });
        // modelBuilder.Entity<Page3MetricEntity>(b => { b.CollectionName    = LookOnConsts.DbTablePrefix + "Page3Metrics"; });
        modelBuilder.Entity<Metric>(b => { b.CollectionName    = LookOnConsts.DbTablePrefix + "Metrics"; });

        // others
        modelBuilder.Entity<Category>(b => { b.CollectionName     = LookOnConsts.DbTablePrefix + "Categories"; });
        modelBuilder.Entity<UserInfo>(b => { b.CollectionName     = LookOnConsts.DbTablePrefix + "UserInfos"; });
        modelBuilder.Entity<Platform>(b => { b.CollectionName     = LookOnConsts.DbTablePrefix + "Platforms"; });
        modelBuilder.Entity<EmailHistories>(b => { b.CollectionName = LookOnConsts.DbTablePrefix + "EmailHistories"; });
        modelBuilder.Entity<UserFeedback>(b => { b.CollectionName = LookOnConsts.DbTablePrefix + "UserFeedbacks"; });
        modelBuilder.Entity<SystemConfig>(b => { b.CollectionName = LookOnConsts.DbTablePrefix + "SystemConfigs"; });
    }
}