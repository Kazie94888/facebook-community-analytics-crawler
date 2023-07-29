using System;
using System.Collections.Generic;
using LookOn.Core.Shared;

namespace LookOn.MerchantDashboards;


public class CustomerDetail
{
    public CustomerDetailData    TotalUserWithoutOrderInTwoMonths { get; set; }
    public AverageOrderValueData AverageOrderValue                { get; set; }
    public CustomerDetailData    TotalUsersHasOnlyOneOrder        { get; set; }
    public TopRevenuesDetail     TopRevenuesDetail                { get; set; }
}

public class CustomerDetailData
{
    private decimal _rate;
    public  long    TotalCustomers { get; set; }
    public decimal Rate
    {
        get => Math.Round(_rate, 2);
        set => _rate = value;
    }
}

public class AverageOrderValueData
{
    private decimal _rate;
    private decimal _price;
    public decimal Price
    {
        get => Math.Round(_price, 2);
        set => _price = value;
    }
    public decimal Rate
    {
        get => Math.Round(_rate, 2);
        set => _rate = value;
    }
}

public class HaravanCustomerId
{
    public IList<long> CustomerIdsInTimeFrame         { get; set; }
    public IList<long> CustomerIdsInPreviousTimeFrame { get; set; }
}

public class TopRevenuesDetail
{
    private decimal _rate;
    private decimal _revenues;
    public decimal TotalRevenues
    {
        get => Math.Round(_revenues, 2);
        set => _revenues = value;
    }
    public decimal Rate
    {
        get => Math.Round(_rate, 2);
        set => _rate = value;
    }
}