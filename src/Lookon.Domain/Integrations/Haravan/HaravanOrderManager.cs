using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LookOn.Integrations.Haravan.Components.Repositories.Interfaces;
using LookOn.Integrations.Haravan.Models.Entities;
using LookOn.Integrations.Haravan.Models.Enums;

namespace LookOn.Integrations.Haravan;

public class HaravanOrderManager : LookOnManager
{
    private readonly IHaravanOrderRepository _haravanOrderRepo;

    public HaravanOrderManager(IHaravanOrderRepository haravanOrderRepo)
    {
        _haravanOrderRepo = haravanOrderRepo;
    }

    public List<HaravanOrder> GetReturnedOrders(List<HaravanOrder> orders)
    {
        var carrierStatusCode = "return";

        return orders.Where(order => order.Fulfillments != null
                                  && order.Fulfillments.Any(fulfillment => fulfillment.CarrierStatusCode == carrierStatusCode))
                     .ToList();
    }

    public async Task<List<HaravanOrder>> GetReturnedOrders(Guid merchantId, DateTime fromDateTime, DateTime endDateTime)
    {
        var orders = await _haravanOrderRepo.GetListAsync(merchantId, HaravanFulfillmentStatus.Fulfilled, fromDateTime, endDateTime);

        var carrierStatusCode = "return";
        return orders.Where(order => order.Fulfillments != null
                                  && order.Fulfillments.Any(fulfillment => fulfillment.CarrierStatusCode == carrierStatusCode))
                     .ToList();
    }

    public List<HaravanOrder> GetCancelledOrders(List<HaravanOrder> orders)
    {
        var carrierStatusCode = "cancel";
        return orders.Where(order => order.Fulfillments != null
                                  && order.Fulfillments.Any(fulfillment => fulfillment.CarrierStatusCode == carrierStatusCode))
                     .ToList();
    }

    public async Task<List<HaravanOrder>> GetCancelledOrders(Guid merchantId, DateTime fromDateTime, DateTime endDateTime)
    {
        var orders = await _haravanOrderRepo.GetListAsync(merchantId, HaravanFulfillmentStatus.Fulfilled, fromDateTime, endDateTime);

        var carrierStatusCode = "cancel";
        return orders.Where(order => order.Fulfillments != null
                                  && order.Fulfillments.Any(fulfillment => fulfillment.CarrierStatusCode == carrierStatusCode))
                     .ToList();
    }
}