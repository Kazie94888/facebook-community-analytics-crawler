namespace LookOn.Core.Consts;

public static class CssConst
{
    public static string GetArrowClass(this decimal rate, bool isPositive = true)
    {
        if(isPositive) return rate >= 0 ? "lo-price-up-icon" : "lo-price-down-icon" ;
        return rate >= 0 ? "lo-price-up-red-icon" : "lo-price-down-green-icon";
    }

    public static string GetColorClass(this decimal rate, bool isPositive = true)
    {
        if (rate is 0) return "veek-support-current-font";
        
        if (isPositive) return rate > 0 ? "veek-positive-current-font" : "veek-negative-current-font";
        return rate > 0 ? "veek-negative-current-font" : "veek-positive-current-font";
    }

    public static string GetColorBorderClass(this decimal rate, bool isPositive = true)
    {
        if (isPositive) return rate >= 0 ? "card-border-top-color-green" : "card-border-top-color-danger";

        return rate >= 0 ? "card-border-top-color-danger" : "card-border-top-color-green";
    }

    public static string GetCompareProgressColor(decimal main, decimal sub)
    {
        return main >= sub ? "veek-positive-current-background" : "veek-support-current-background";
    }

    public static string GetProgressColorClass(this int index)
    {
        return $"LookOn-color-{index} btn-border-radius-20";
    }
}