using System;

namespace LookOn.Dashboards.DashboardBase;

public class MetricAdvancedDto
{
    
}

public class EcomMetric_AdvancedDto
{
    public long    EcomCustomersNoOrderInXMonthsCount                      { get; set; }
    public decimal EcomCustomersNoOrderInXMonthsRate                       { get; set; }
    public bool    IsEcomCustomersNoOrderInXMonthsPositive                 { get; set; }
    public decimal AverageOrderValueComparedToTheWorld                     { get; set; }
    public decimal AverageOrderValueComparedToTheWorldRate                 { get; set; }
    public long    Only1OrderPurchasedCustomersCount                            { get; set; }
    public decimal Only1OrderPurchasedCustomersRate                             { get; set; }
    public bool    IsOnly1OrderPurchasedCustomersPositive                       { get; set; }
    public decimal RevenueEcomCustomerHasMoreThan1000FollowerAndFriend     { get; set; }
    public decimal RevenueEcomCustomerHasMoreThan1000FollowerAndFriendRate { get; set; }
    
    public decimal EcomCustomersNoOrderInXMonthsPercent       => Math.Round(EcomCustomersNoOrderInXMonthsRate       * 100, 2);
    public decimal AverageOrderValueComparedToTheWorldPercent => Math.Round(AverageOrderValueComparedToTheWorldRate * 100, 2);
    public decimal Only1OrderPurchasedCustomersPercent         => Math.Round(Only1OrderPurchasedCustomersRate             * 100, 2);
    public decimal RevenueEcomCustomerHasMoreThan1000FollowerAndFriendPercent         => Math.Round(RevenueEcomCustomerHasMoreThan1000FollowerAndFriendRate             * 100, 2);
    
    public EcomMetric_AdvancedDto()
    {
        IsEcomCustomersNoOrderInXMonthsPositive = false;
        IsOnly1OrderPurchasedCustomersPositive       = false;
    }
}