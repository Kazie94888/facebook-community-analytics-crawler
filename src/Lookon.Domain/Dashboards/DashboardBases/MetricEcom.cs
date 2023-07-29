using System;
using System.Collections.Generic;

namespace LookOn.Dashboards.DashboardBases;

public class EcomMetric_Summary
{
    public int     OrderCount           { get; set; }
    public decimal OrderGrowthRate      { get; set; }
    public bool    IsOrderCountPositive { get; set; }

    //
    public int     PurchasedCustomerCount           { get; set; }
    public decimal PurchasedCustomerGrowthRate      { get; set; }
    public bool    IsPurchasedCustomerCountPositive { get; set; }

    //
    public decimal AOV           { get; set; }
    public decimal AOVGrowthRate { get; set; } 
    public bool    IsAOVPositive { get; set; }

    //
    public decimal CLV           { get; set; }
    public decimal CLVGrowthRate { get; set; }
    public bool    IsCLVPositive { get; set; }

    //
    public decimal ReturnedOrderRate           { get; set; }
    public decimal ReturnedOrderGrowthRate     { get; set; }
    public bool    IsReturnedOrderRatePositive { get; set; }

    //
    public decimal CancelledOrderRate           { get; set; }
    public decimal CancelledOrderGrowthRate     { get; set; }
    public bool    IsCancelledOrderRatePositive { get; set; }

    //
    public decimal RetentionRate       { get; set; }
    public decimal RetentionGrowthRate { get; set; }
    public bool    IsRetentionRatePositive { get; set; }

    //
    public decimal AvgProductCount           { get; set; }
    public decimal AvgProductCountRate { get; set; }
    public bool IsAvgProductPositive { get; set; }

    public EcomMetric_Summary()
    {
        IsOrderCountPositive             = true;
        IsPurchasedCustomerCountPositive = true;
        IsAOVPositive                    = true;
        IsCLVPositive                    = true;
        IsReturnedOrderRatePositive      = false;
        IsCancelledOrderRatePositive     = false;
        IsRetentionRatePositive          = true;
        IsAvgProductPositive                 = true;
    }
}

public class EcomMetric_RevenueSummary
{
    public decimal                        RevenueToday           { get; set; }
    public decimal                        RevenueTodayGrowthRate { get; set; }
    public decimal                        Revenue                { get; set; }
    public decimal                        RevenueGrowthRate      { get; set; }
    public List<EcomMetric_RevenueByDate> RevenueByDates         { get; set; }
}

public class EcomMetric_RevenueByDate
{
    public decimal  Revenue  { get; set; }
    public DateTime DateTime { get; set; }
}

public class EcomMetric_RevenueByProduct
{
    public        string  ProductName { get; set; }
    public        decimal SaleAmount  { get; set; }
    public        decimal Rate        { get; set; }
    public static int     Amount      => 5;
}

public class EcomMetric_RevenueByLocation
{
    public        string  Name   { get; set; }
    public        decimal Value  { get; set; }
    public        decimal Rate   { get; set; }
    public static int     Amount => 5;
}