using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LookOn.Core.Extensions;
using LookOn.Core.Shared.Enums;
using LookOn.Merchants;
using Volo.Abp.Domain.Repositories;

namespace LookOn.Dashboards.Page1;

public class Page1CacheManager : LookOnManager
{
    private readonly IRepository<Page1MetricEntity> _page1CacheRepo;
    private readonly IMerchantRepository            _merchantRepository;

    public Page1CacheManager(IRepository<Page1MetricEntity> page1CacheRepo, IMerchantRepository merchantRepository)
    {
        _page1CacheRepo     = page1CacheRepo;
        _merchantRepository = merchantRepository;
    }

    public async Task<Page1MetricEntity> Get(Guid merchantId, TimeFrameType timeFrameType = TimeFrameType.Weekly)
    {
        return await _page1CacheRepo.FirstOrDefaultAsync(x => x.Metric.MerchantId == merchantId && x.Metric.TimeFrameType == timeFrameType);
    }

    public async Task<IList<Page1MetricEntity>> GetAll(Guid merchantId, TimeFrameType timeFrameType = TimeFrameType.Weekly)
    {
        return await _page1CacheRepo.GetListAsync(x => x.Metric.MerchantId == merchantId && x.Metric.TimeFrameType == timeFrameType);
    }

    public async Task Save(Page1Metric metric)
    {
        var existings =
            await _page1CacheRepo.GetListAsync(x => x.Metric.MerchantId == metric.MerchantId);

        if (existings.IsNotNullOrEmpty())
        {
            await _page1CacheRepo.DeleteManyAsync(existings);
        }

        var merchant = await _merchantRepository.FindAsync(metric.MerchantId);
        if (merchant is null) return;

        var page1MetricEntity = new Page1MetricEntity
        {
            MerchantId    = metric.MerchantId,
            MerchantEmail = merchant.Email,
            TimeFrame     = metric.TimeFrameType,
            Metric        = metric,
        };

        await _page1CacheRepo.InsertAsync(page1MetricEntity);
    }
}