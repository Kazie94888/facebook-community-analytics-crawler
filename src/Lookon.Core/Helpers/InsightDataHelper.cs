using System.Collections.Generic;
using System.Linq;

namespace LookOn.Core.Helpers;

public static class InsightDataHelper
{
    public static readonly List<int> EcomDataNums     = new() { 1, 2, 5 };
    public static readonly List<int> SocialDataNums   = new() { 3, 4, 5 };
    public static readonly List<int> TotalityDataNums = new() { 1, 3, 5 };
    public static readonly List<int> AdvanceDataNums  = new() { 1, 2, 3, 5 };

    public static bool IsInsightDataType(this int typeNum, IEnumerable<int> dataTypes)
    {
        return dataTypes.Contains(typeNum);
    }
}