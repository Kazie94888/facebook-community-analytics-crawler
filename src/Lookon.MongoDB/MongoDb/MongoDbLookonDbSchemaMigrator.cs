﻿using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using LookOn.Data;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.MongoDB;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Reflection;

namespace LookOn.MongoDB;

public class MongoDbLookOnDbSchemaMigrator : ILookOnDbSchemaMigrator, ITransientDependency
{
    private readonly IServiceProvider _serviceProvider;

    public MongoDbLookOnDbSchemaMigrator(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task MigrateAsync()
    {
        var dbContexts = _serviceProvider.GetServices<IAbpMongoDbContext>();
        var connectionStringResolver = _serviceProvider.GetRequiredService<IConnectionStringResolver>();

        if (_serviceProvider.GetRequiredService<ICurrentTenant>().IsAvailable)
        {
            dbContexts = dbContexts.Where(x => !x.GetType().IsDefined(typeof(IgnoreMultiTenancyAttribute)));
        }

        foreach (var dbContext in dbContexts)
        {
            var connectionString =
                await connectionStringResolver.ResolveAsync(
                    ConnectionStringNameAttribute.GetConnStringName(dbContext.GetType()));
            var mongoUrl = new MongoUrl(connectionString);
            var databaseName = mongoUrl.DatabaseName;
            var client = new MongoClient(mongoUrl);

            if (databaseName.IsNullOrWhiteSpace())
            {
                databaseName = ConnectionStringNameAttribute.GetConnStringName(dbContext.GetType());
            }

            (dbContext as AbpMongoDbContext)?.InitializeCollections(client.GetDatabase(databaseName));
        }
    }
}
