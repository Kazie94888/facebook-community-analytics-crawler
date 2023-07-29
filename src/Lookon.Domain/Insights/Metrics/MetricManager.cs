using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LookOn.Core.Extensions;
using LookOn.Core.Helpers;
using LookOn.Core.Shared.Enums;
using LookOn.Enums;
using LookOn.Integrations.Datalytis.Components.Repositories.Interfaces;
using LookOn.Integrations.Datalytis.Models.Entities;
using LookOn.Integrations.Haravan;
using LookOn.Integrations.Haravan.Components.Repositories.Interfaces;
using LookOn.Integrations.Haravan.Models.Enums;
using LookOn.Merchants;
using LookOn.MerchantSyncInfos;
using Volo.Abp.Domain.Repositories;

namespace LookOn.Insights.Metrics;

public class MetricManager : MetricCalculatorManager
{
    private readonly IHaravanOrderRepository               _haravanOrderRepo;
    private readonly IHaravanCustomerRepository            _haravanCustomerRepo;
    private readonly IDatalytisUserRepository              _datalytisUserRepo;
    private readonly IDatalytisUserSocialInsightRepository _datalytisSocialInsightRepository;
    private readonly IMetricRepository                     _metricRepository;
    private readonly IMerchantRepository                   _merchantRepository;

    public async Task<Metric> GetMetrics(Metric metric, GetMetricRequest request)
    {
        var dataSource = await GetDataSource(request.MerchantId,
                                             request.MetricDataSourceType,
                                             request.TimeFrame,
                                             request.From,
                                             request.To);

        ///////////////////////////////////////////////////

        MetricItem metricItem = new()
        {
            DataSourceType = request.MetricDataSourceType,
            TimeFrameType  = request.TimeFrame
        };
        if (request.MetricDataSourceType.ToInt().IsInsightDataType(InsightDataHelper.EcomDataNums))
        {
            if (request.From.HasValue && request.To.HasValue)
            {
                metricItem.EcomSummary = GetEcomSummary(dataSource.AllOrders,
                                                        dataSource.FilteredOrders,
                                                        request.TimeFrame,
                                                        request.From.Value,
                                                        request.To.Value);
                metricItem.EcomRevenueSummary = GetEcomRevenueSummary(dataSource.AllOrders,
                                                                      dataSource.FilteredOrders,
                                                                      request.TimeFrame,
                                                                      request.From.Value,
                                                                      request.To.Value);
            }

            metricItem.EcomRevenueByProducts  = GetEcomRevenueByProducts(dataSource.FilteredOrders);
            metricItem.EcomRevenueByLocations = GetEcomRevenueByLocations(dataSource.FilteredOrders);
        }

        if (request.MetricDataSourceType.ToInt().IsInsightDataType(InsightDataHelper.AdvanceDataNums))
        {
            metricItem.EcomAdvanced = await GetEcomAdvancedMetrics(dataSource.MetricConfigs,
                                                                   dataSource.AllOrders,
                                                                   dataSource.FilteredOrders,
                                                                   dataSource.AllSocialUsers,
                                                                   dataSource.FilteredCustomers.Count,
                                                                   dataSource.AllCustomers.Count);
        }

        // social metrics
        if (request.MetricDataSourceType.ToInt().IsInsightDataType(InsightDataHelper.SocialDataNums))
        {
            metricItem.SocialDemographic          = GetSocialDemographic(dataSource.FilteredSocialUsers, dataSource.AllCustomers);
            metricItem.SocialLocationByProvinces  = GetSocialLocationByProvince(dataSource.FilteredSocialUsers);
            metricItem.SocialCommunityInteraction = GetCommunityInteraction(dataSource.FilteredSocialUsers, dataSource.Insights);
        }

        metricItem.SocialAboveNormalInfluencerPhones = GetAboveNormalInfluencerPhones(dataSource.AllSocialUsers);
        metricItem.SocialInsight                     = GetSocialInsightMetric(dataSource.AllSocialUsers, dataSource.AllCustomers, dataSource.Insights);
        metricItem.SocialComparision                 = GetComparisionMetric(dataSource.AllEcomUsers, dataSource.AllSocialUsers);
        
        metric.Items.Add(metricItem);
        return metric;
    }

    public async Task<Metric> GetMetricsCache(Guid merchantId)
    {
        return await _metricRepository.FirstOrDefaultAsync(x => x.MerchantId == merchantId);
    }

    public bool IsExpired(Metric metric)
    {
        return (DateTime.UtcNow.Date.AddTicks(1) - metric.CreatedAt.Date).Days > 0;
    }

    public async Task DoSaveMetrics(GetMetricRequest request, Metric metric)
    {
        var metricExist = await GetMetricsCache(request.MerchantId);

        if (metricExist != null)
        {
            await _metricRepository.DeleteAsync(metricExist);
        }

        await _metricRepository.InsertAsync(metric);
    }

    public async Task<InsightUser> GetInsightUserCount(Guid merchantId)
    {
        var allOrders = await _haravanOrderRepo.GetListAsync(x => x.MerchantId        == merchantId
                                                               && x.FulfillmentStatus == HaravanFulfillmentStatus.Fulfilled
                                                               && x.ConfirmedAt.HasValue
                                                               && x.CreatedAt.HasValue);

        var customerIds = allOrders.Where(order => order.HaravanCustomerId.HasValue)
                                   .Select(order => order.HaravanCustomerId.Value)
                                   .Distinct()
                                   .ToList();
        var customers =
            await _haravanCustomerRepo.GetListAsync(customer => customer.CustomerId.HasValue && customerIds.Contains(customer.CustomerId.Value));
        var phones    = customers.Select(c => c.Phone).Distinct();
        var ecomUsers = await _datalytisUserRepo.GetListAsync(_ => phones.Contains(_.Phone));

        var merchant             = await MerchantRepository.GetAsync(merchantId);
        var merchantCommunityIds = merchant.GetSocialCommunityIds();
        var socialUsers          = new List<DatalytisUser>();
        foreach (var merchantCommunityId in merchantCommunityIds)
        {
            var list = await _datalytisUserRepo.GetListAsync(u => u.MerchantFbPageIds.Contains(merchantCommunityId));
            socialUsers.AddRange(list);
        }

        return new InsightUser
        {
            EcomUserCount      = customers.Count,
            SocialUserCount    = socialUsers.Count,
            IntersectUserCount = ecomUsers.Count
        };
    }

    private async Task<MetricDataSource> GetDataSource(Guid                 merchantId,
                                                       MetricDataSourceType metricDataSourceType,
                                                       TimeFrameType        timeFrame,
                                                       DateTime?            from,
                                                       DateTime?            to)
    {
        MetricDataSource dataSource = new();

        var allOrders = await _haravanOrderRepo.GetListAsync(x => x.MerchantId        == merchantId
                                                               && x.FulfillmentStatus == HaravanFulfillmentStatus.Fulfilled
                                                               && x.ConfirmedAt.HasValue
                                                               && x.CreatedAt.HasValue);

        var customerIds = allOrders.Where(order => order.HaravanCustomerId.HasValue)
                                   .Select(order => order.HaravanCustomerId.Value)
                                   .Distinct()
                                   .ToList();
        var customers = await _haravanCustomerRepo.GetListAsync(customer => customer.CustomerId.HasValue && customerIds.Contains(customer.CustomerId.Value));

        var merchant             = await MerchantRepository.GetAsync(merchantId);
        var merchantCommunityIds = merchant.GetSocialCommunityIds();

        var phones    = customers.Select(c => c.Phone).Distinct();
        var ecomUsers = await _datalytisUserRepo.GetListAsync(_ => phones.Contains(_.Phone));

        var socialUsers = new List<DatalytisUser>();
        foreach (var merchantCommunityId in merchantCommunityIds)
        {
            var list = await _datalytisUserRepo.GetListAsync(u => u.MerchantFbPageIds.Contains(merchantCommunityId));
            socialUsers.AddRange(list);
        }

        var uids     = socialUsers.Select(u => u.Uid).Distinct().ToList();
        var insights = await _datalytisSocialInsightRepository.GetListAsync(i => uids.Contains(i.Uid));

        // filter out data source by metricDataSourceType
        switch (metricDataSourceType)
        {
            case MetricDataSourceType.Ecom:
            {
                var timeFrameOrders = allOrders;

                if (timeFrame is not TimeFrameType.AllTime && from.HasValue && to.HasValue)
                {
                    timeFrameOrders = allOrders.Where(x => x.CreatedAt.HasValue && x.CreatedAt.Value >= from && x.CreatedAt.Value <= to).ToList();
                }

                var timeFrameCustomerIds = timeFrameOrders.Where(order => order.HaravanCustomerId.HasValue)
                                                          .Select(order => order.HaravanCustomerId.Value)
                                                          .Distinct()
                                                          .ToList();
                var timeFrameCustomers = await _haravanCustomerRepo.GetListAsync(customer => customer.CustomerId.HasValue
                                                                                          && timeFrameCustomerIds.Contains(customer.CustomerId.Value));

                var timeFramePhones    = timeFrameCustomers.Select(c => c.Phone).Distinct();
                var timeFrameEcomUsers = await _datalytisUserRepo.GetListAsync(_ => timeFramePhones.Contains(_.Phone));

                dataSource.FilteredOrders    = timeFrameOrders;
                dataSource.FilteredCustomers = timeFrameCustomers;
                dataSource.FilteredEcomUsers = timeFrameEcomUsers;

                break;
            }

            case MetricDataSourceType.EcomOnly:
            {
                var timeFrameOrders    = allOrders;
                var timeFrameCustomers = customers;
                var timeFrameEcomUsers = ecomUsers;

                if (timeFrame is not TimeFrameType.AllTime && from.HasValue && to.HasValue)
                {
                    timeFrameOrders = allOrders.Where(x => x.CreatedAt.HasValue && x.CreatedAt.Value >= from && x.CreatedAt.Value <= to).ToList();
                    var timeFrameCustomerIds = timeFrameOrders.Where(order => order.HaravanCustomerId.HasValue)
                                                              .Select(order => order.HaravanCustomerId.Value)
                                                              .Distinct()
                                                              .ToList();
                    timeFrameCustomers =
                        await _haravanCustomerRepo.GetListAsync(customer => customer.CustomerId.HasValue && timeFrameCustomerIds.Contains(customer.CustomerId.Value));
                    var timeFramePhones    = timeFrameCustomers.Select(c => c.Phone).Distinct();
                    timeFrameEcomUsers = await _datalytisUserRepo.GetListAsync(_ => timeFramePhones.Contains(_.Phone));
                }

                var socialUserPhones   = socialUsers.Select(_ => _.Phone);
                var ecomOnlyUsers      = timeFrameEcomUsers.Where(_ => !socialUserPhones.Contains(_.Phone)).ToList();
                var ecomOnlyCustomers = timeFrameCustomers.Where(_ => !socialUserPhones.Contains(_.Phone)).ToList();
                var ecomOnlyUserPhones = ecomOnlyCustomers.Select(_ => _.Phone);
                var filteredCustomers    = await _haravanCustomerRepo.GetListAsync(customer => customer.CustomerId.HasValue && ecomOnlyUserPhones.Contains(customer.Phone));

                var filteredCustomerIds = filteredCustomers.Select(_ => _.CustomerId);
                var filteredOrders      = timeFrameOrders.Where(_ => filteredCustomerIds.Contains(_.HaravanCustomerId)).ToList();

                dataSource.FilteredOrders    = filteredOrders;
                dataSource.FilteredCustomers = filteredCustomers;
                dataSource.FilteredEcomUsers = ecomOnlyUsers;

                break;
            }

            case MetricDataSourceType.Social:
            {
                dataSource.FilteredSocialUsers = socialUsers;
                break;
            }

            case MetricDataSourceType.SocialOnly:
            {
                var ecomUserPhones    = ecomUsers.Select(_ => _.Phone);
                var filteredSocialUsers = socialUsers.Where(_ => !ecomUserPhones.Contains(_.Phone)).ToList();
                dataSource.FilteredSocialUsers = filteredSocialUsers;
                break;
            }

            case MetricDataSourceType.Intersect:
            {
                var timeFrameOrders    = allOrders;
                var timeFramePhones    = phones;
                var timeFrameEcomUsers = ecomUsers;

                if (timeFrame is not TimeFrameType.AllTime && from.HasValue && to.HasValue)
                {
                    timeFrameOrders = allOrders.Where(x => x.CreatedAt.HasValue && x.CreatedAt.Value >= from && x.CreatedAt.Value <= to).ToList();
                    var timeFrameCustomerIds = timeFrameOrders.Where(order => order.HaravanCustomerId.HasValue)
                                                              .Select(order => order.HaravanCustomerId.Value)
                                                              .Distinct()
                                                              .ToList();
                    var timeFrameCustomers =
                        await _haravanCustomerRepo.GetListAsync(customer => customer.CustomerId.HasValue && timeFrameCustomerIds.Contains(customer.CustomerId.Value));
                    timeFramePhones = timeFrameCustomers.Select(c => c.Phone).Distinct();
                    timeFrameEcomUsers = await _datalytisUserRepo.GetListAsync(_ => timeFramePhones.Contains(_.Phone));
                }

                var socialUserPhones   = socialUsers.Select(_ => _.Phone);
                var filteredEcomUsers        = timeFrameEcomUsers.Where(_ => socialUserPhones.Contains(_.Phone)).ToList();
                var filteredSocialUsers = socialUsers.Where(_ => timeFramePhones.Contains(_.Phone)).ToList();
                var ecomOnlyUserPhones = filteredEcomUsers.Select(_ => _.Phone);
                var filteredCustomers    = await _haravanCustomerRepo.GetListAsync(customer => customer.CustomerId.HasValue && ecomOnlyUserPhones.Contains(customer.Phone));

                var filteredCustomerIds = filteredCustomers.Select(_ => _.CustomerId);
                var filteredOrders      = timeFrameOrders.Where(_ => filteredCustomerIds.Contains(_.HaravanCustomerId)).ToList();

                dataSource.FilteredOrders      = filteredOrders;
                dataSource.FilteredCustomers   = filteredCustomers;
                dataSource.FilteredEcomUsers   = filteredEcomUsers;
                dataSource.FilteredSocialUsers = filteredSocialUsers;

                break;
            }

            default: throw new ArgumentOutOfRangeException(nameof(metricDataSourceType), metricDataSourceType, null);
        }

        dataSource.AllOrders      = allOrders;
        dataSource.AllCustomers   = customers;
        dataSource.AllEcomUsers   = ecomUsers;
        dataSource.AllSocialUsers = socialUsers;
        dataSource.Insights       = insights;
        dataSource.MetricConfigs  = merchant.MetricConfigs;

        return dataSource;
    }

    public MetricManager(IRepository<Metric>                   metricRepository,
                         IMerchantRepository                   merchantRepository,
                         IMerchantSyncInfoRepository           merchantSyncInfoRepo,
                         HaravanOrderManager                   haravanOrderManager,
                         HaravanCustomerManager                haravanCustomerManager,
                         IHaravanOrderRepository               haravanOrderRepo,
                         IHaravanCustomerRepository            haravanCustomerRepo,
                         IDatalytisUserRepository              datalytisUserRepo,
                         IDatalytisUserSocialInsightRepository datalytisSocialInsightRepository,
                         IMetricRepository                     metricRepository1) : base(metricRepository,
                                                                                         merchantRepository,
                                                                                         merchantSyncInfoRepo,
                                                                                         haravanOrderManager,
                                                                                         haravanCustomerManager)
    {
        _merchantRepository               = merchantRepository;
        _haravanOrderRepo                 = haravanOrderRepo;
        _haravanCustomerRepo              = haravanCustomerRepo;
        _datalytisUserRepo                = datalytisUserRepo;
        _datalytisSocialInsightRepository = datalytisSocialInsightRepository;
        _metricRepository                 = metricRepository1;
    }
}