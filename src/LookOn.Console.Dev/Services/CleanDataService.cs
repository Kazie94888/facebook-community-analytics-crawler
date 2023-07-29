using System;
using System.Threading.Tasks;
using LookOn.Core.Extensions;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;

namespace LookOn.Console.Dev.Services;

public class CleanDataService : ITransientDependency
{
    private readonly IRepository<Page1MetricEntity> _page1CacheRepo;

    public CleanDataService(IRepository<Page1MetricEntity> page1CacheRepo)
    {
        _page1CacheRepo = page1CacheRepo;
    }

    public async Task CleanUp()
    {
        var merchantId   = "353493F5-FB3F-599D-432D-3A04263BDAB7";
        var merchantGuid = Guid.Parse(merchantId);
        var existings = await _page1CacheRepo.GetListAsync();

        if (existings.IsNotNullOrEmpty())
        {
            await _page1CacheRepo.DeleteManyAsync(existings);
        }

    }
}