using System;
using System.Collections.Generic;

namespace LookOn.Dashboards.DashboardBase;

public class EcomMetric_SummaryDto
{
    public int     OrderCount                     { get; set; }
    public decimal OrderGrowthRate                { get; set; }
    public bool    IsOrderCountPositive           { get; set; }
    
    public int     PurchasedCustomerCount           { get; set; }
    public decimal PurchasedCustomerGrowthRate      { get; set; }
    public bool    IsPurchasedCustomerCountPositive { get; set; }
    
    public decimal AOV                            { get; set; }
    public decimal AOVGrowthRate                  { get; set; }
    public bool    IsAOVPositive                  { get; set; }
    
    public decimal CLV                            { get; set; }
    public decimal CLVGrowthRate                  { get; set; }
    public bool    IsCLVPositive                  { get; set; }
    
    public decimal ReturnedOrderRate              { get; set; }
    public decimal ReturnedOrderGrowthRate        { get; set; }
    public bool    IsReturnedOrderRatePositive    { get; set; }
    
    public decimal CancelledOrderRate             { get; set; }
    public decimal CancelledOrderGrowthRate       { get; set; }
    public bool    IsCancelledOrderRatePositive   { get; set; }
    
    public decimal RetentionRate                  { get; set; }
    public decimal RetentionGrowthRate            { get; set; }
    public bool    IsRetentionRatePositive        { get; set; }
    
    public decimal AvgProductCount                         { get; set; }
    public decimal AvgProductCountRate               { get; set; }
    public bool    IsAvgProductPositive               { get; set; }
    
    public decimal OrderGrowthPercent         => Math.Round(OrderGrowthRate             * 100, 2);
    public decimal PurchasedCustomerGrowthPercent => Math.Round(PurchasedCustomerGrowthRate * 100, 2);
    public decimal AOVGrowthPercent               => Math.Round(AOVGrowthRate               * 100, 2);
    public decimal CLVGrowthPercent               => Math.Round(CLVGrowthRate               * 100, 2);
    public decimal ReturnedOrderPercent           => Math.Round(ReturnedOrderRate           * 100, 2);
    public decimal ReturnedOrderGrowthRatePercent => Math.Round(ReturnedOrderGrowthRate     * 100, 2);
    public decimal CancelledOrderPercent          => Math.Round(CancelledOrderRate          * 100, 2);
    public decimal CancelledOrderGrowthPercent    => Math.Round(CancelledOrderGrowthRate    * 100, 2);
    public decimal RetentionPercent               => Math.Round(RetentionRate               * 100, 2);
    public decimal RetentionGrowthPercent         => Math.Round(RetentionGrowthRate         * 100, 2);
    public decimal AvgSKUGrowthPercent            => Math.Round(AvgProductCountRate            * 100, 2);
}

public class EcomMetric_RevenueSummaryDto
{
    public decimal                            RevenueToday              { get; set; }
    public decimal                            RevenueTodayGrowthRate    { get; set; }
    public decimal                            Revenue                   { get; set; }
    public decimal                            RevenueGrowthRate         { get; set; }
    public IList<EcomMetric_RevenueByDateDto> RevenueByDates            { get; set; }
    public decimal                            RevenueTodayGrowthPercent => Math.Round(RevenueTodayGrowthRate * 100, 2);
    public decimal                            RevenueGrowthPercent      => Math.Round(RevenueGrowthRate      * 100, 2);
}

public class EcomMetric_RevenueByDateDto
{
    public decimal  Revenue       { get; set; }
    public DateTime DateTime      { get; set; }
    public string   DateTimeLabel => DateTime.ToString("dd/MM");
}

public class EcomMetric_RevenueByProductDto
{
    public string  ProductName { get; set; }
    public decimal SaleAmount  { get; set; }
    public decimal Rate        { get; set; }
    public decimal Percent     => Math.Round(Rate * 100, 2);
}

public class EcomMetric_SaleCountByProductDto
{
    public decimal TotalSaleCount { get; set; }
    public string  ProductName    { get; set; }
    public decimal Rate           { get; set; }
    public decimal Percent        => Math.Round(Rate * 100, 2);
}

public class EcomMetric_RevenueByLocationDto
{
    public string  Name    { get; set; }
    public decimal Value   { get; set; }
    public decimal Rate    { get; set; }
    public decimal Percent => Math.Round(Rate * 100, 2);
}