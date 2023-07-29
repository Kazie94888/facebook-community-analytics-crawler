using LookOn.Integrations.Haravan.Models.Entities;
using LookOn.Integrations.Haravan.Models.RawModels;
using MongoDB.Driver;
using Volo.Abp.Data;
using Volo.Abp.MongoDB;

namespace LookOn.Integrations.Haravan;

[ConnectionStringName("LookOnHaravan")]
public class HaravanDbContext : AbpMongoDbContext
{
    /* Add mongo collections here. Example:
     * public IMongoCollection<Question> Questions => Collection<Question>();
     */
    [MongoCollection("HaravanStores")]    public IMongoCollection<HaravanStore>    HaravanStores    => Collection<HaravanStore>();
    [MongoCollection("HaravanCustomers")] public IMongoCollection<HaravanCustomer> HaravanCustomers => Collection<HaravanCustomer>();
    [MongoCollection("HaravanOrders")]    public IMongoCollection<HaravanOrder>    HaravanOrders    => Collection<HaravanOrder>();
    [MongoCollection("HaravanOrderRaws")] public IMongoCollection<HRVOrderRaw>     HaravanOrderRaws => Collection<HRVOrderRaw>();

    protected override void CreateModel(IMongoModelBuilder modelBuilder)
    {
        base.CreateModel(modelBuilder);

        //Customize the configuration for your collections.
    }
}