using System;
using System.Threading.Tasks;
using LookOn.Core.Extensions;
using LookOn.Merchants;
using Volo.Abp.Domain.Repositories;

namespace LookOn.Dashboards.Page2;

public class Page2CacheManager : LookOnManager
{
    private readonly IRepository<Page2MetricEntity> _page2CacheRepo;
    private readonly IMerchantRepository            _merchantRepository;

    public Page2CacheManager(IRepository<Page2MetricEntity> page2CacheRepo, IMerchantRepository merchantRepository)
    {
        _page2CacheRepo          = page2CacheRepo;
        _merchantRepository = merchantRepository;
    }
    
    public async Task<Page2MetricEntity> Get(Guid merchantId)
    {
        return await _page2CacheRepo.FirstOrDefaultAsync(x => x.Metric.MerchantId == merchantId);
    }
    
    public async Task Save(Page2Metric metric)
    {
        var existings =
            await _page2CacheRepo.GetListAsync(x => x.Metric.MerchantId    == metric.MerchantId);

        if (existings.IsNotNullOrEmpty())
        {
            await _page2CacheRepo.DeleteManyAsync(existings);
        }
        
        var merchant = await _merchantRepository.FindAsync(metric.MerchantId);
        if (merchant is null) return;

        var page1MetricEntity = new Page2MetricEntity
        {
            MerchantId    = metric.MerchantId,
            MerchantEmail = merchant.Email,
            Metric        = metric, CreatedAt = DateTime.UtcNow,
        };

        await _page2CacheRepo.InsertAsync(page1MetricEntity);
    }
}