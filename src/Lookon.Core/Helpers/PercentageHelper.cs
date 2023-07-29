using System;

namespace LookOn.Core.Helpers
{
    public static class PercentageHelper
    {
        public static double GetPercentageDouble(double current, double target)
        {
            if (target<=0) return 0;
            
            return (current / target)*100 > 100? 100: (current / target)*100;
            
        }

        public static int GetPercentage(decimal current, decimal target)
        {
            return (int)Math.Round(GetPercentageDouble((double)current, (double)target), MidpointRounding.ToEven);
        }
        
        public static int GetPercentage(double current, double target)
        {
            return (int)Math.Round(GetPercentageDouble(current, target), MidpointRounding.ToEven);
        }
        
        public static int GetPercentage(int current, int target)
        {
            return (int)Math.Round(GetPercentageDouble(current, target), MidpointRounding.ToEven);
        }
    }
}