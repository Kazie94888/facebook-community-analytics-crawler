using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LookOn.Core.Extensions;
using LookOn.Merchants;
using Volo.Abp.Domain.Repositories;

namespace LookOn.Dashboards.Page3;

public class Page3CacheManager : LookOnManager
{
    private readonly IRepository<Page3MetricEntity> _page3CacheRepo;
    private readonly IMerchantRepository            _merchantRepository;

    public Page3CacheManager(IRepository<Page3MetricEntity> page3CacheRepo, IMerchantRepository merchantRepository)
    {
        _page3CacheRepo          = page3CacheRepo;
        _merchantRepository = merchantRepository;
    }
    
    public async Task<IList<Page3MetricEntity>> Get(Guid merchantId)
    {
        return await _page3CacheRepo.GetListAsync(x => x.Metric.MerchantId == merchantId);
    }
    
    public async Task Save(Page3Metric metric, Page3Filter filter)
    {
        var existings =
            await _page3CacheRepo.GetListAsync(x => x.Metric.MerchantId == metric.MerchantId);

        existings = existings.Where(entity => (filter== null && entity.Filter == null) || (filter != null && entity.Filter != null && entity.Filter.IsMatching(filter))).ToList();

        if (existings.IsNotNullOrEmpty())
        {
            await _page3CacheRepo.DeleteManyAsync(existings);
        }
        
        var merchant = await _merchantRepository.FindAsync(metric.MerchantId);
        if (merchant is null) return;

        var page3MetricEntity = new Page3MetricEntity
        {
            MerchantId    = metric.MerchantId,
            MerchantEmail = merchant.Email,
            Metric        = metric, CreatedAt = DateTime.UtcNow,
            Filter = filter
        };

        await _page3CacheRepo.InsertAsync(page3MetricEntity);
    }
}