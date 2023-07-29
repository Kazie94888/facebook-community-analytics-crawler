using System;
using System.Globalization;

namespace LookOn.Core.Extensions
{
    public static class NumericExtensions
    {
        private static readonly Random Random;

        static NumericExtensions()
        {
            Random = new Random();
        }

        public static string ToCommaStyle(this decimal value, string zeroString = "---")
        {
            return value == 0 ? zeroString : $"{value.ToString("N0", CultureInfo.CreateSpecificCulture("en-US"))}";
        }

        public static string ToCommaStyle(this object value, string zeroString = "---")
        {
            decimal.TryParse(value.ToString(), out var number);
            return ToCommaStyle(number, zeroString);
        }

        public static decimal ToNonVATAmount(this decimal vatAmount, decimal vatPercent)
        {
            if (vatPercent <= 0)
            {
                return vatAmount;
            }

            var nonVATAmount = vatAmount / (1 + ((decimal)vatPercent / 100));

            return nonVATAmount;
        }

        // public static decimal ToVATAmount(this decimal value, int vatPercentage)
        // {
        //     if (vatPercentage <= 0)
        //     {
        //         return value;
        //     }
        //
        //     return value + value * (decimal)vatPercentage / 100;
        // }

        public static decimal RandomDecimal(decimal minValue, decimal maxValue, int roundDecimals = -1)
        {
            var result = Random.NextDouble().ToDecimal() * (maxValue - minValue) + minValue;
            return roundDecimals < 0 ? result : Math.Round(result, roundDecimals);
        }

        public static int RandomInt(int minvalue, int maxValue)
        {
            var result = Random.Next(minvalue, maxValue);
            return result;
        }

        public static string FormatNumber(this decimal num)
        {
            var value = Math.Abs(num);
            return num switch
            {
                >= 1000000000 => Math.Round(value / 1000000000, 2).ToString(CultureInfo.CreateSpecificCulture("en-US")) + "B",
                >= 1000000    => Math.Round(value / 1000000,    2).ToString(CultureInfo.CreateSpecificCulture("en-US")) + "M",
                >= 100000     => Math.Round(value / 1000,       2).ToString(CultureInfo.CreateSpecificCulture("en-US")) + "K",
                _             => Math.Round(value, 2).ToCommaStyle("0")
            };
        }

        public static string FormatNumber(this double num)
        {
            var value = Math.Abs(num);
            return num switch
            {
                >= 1000000000 => Math.Round(value / 1000000000, 2).ToString(CultureInfo.CreateSpecificCulture("en-US")) + "B",
                >= 1000000    => Math.Round(value / 1000000,    2).ToString(CultureInfo.CreateSpecificCulture("en-US")) + "M",
                >= 100000     => Math.Round(value / 1000,       2).ToString(CultureInfo.CreateSpecificCulture("en-US")) + "K",
                _             => Math.Round(value, 2).ToCommaStyle("0")
            };
        }

        public static string FormatNumber(this long num)
        {
            var value = Math.Abs(num);
            return num switch
            {
                >= 1000000000 => Math.Round((decimal)value / 1000000000, 2).ToString(CultureInfo.CreateSpecificCulture("en-US")) + "B",
                >= 1000000    => Math.Round((decimal)value / 1000000,    2).ToString(CultureInfo.CreateSpecificCulture("en-US")) + "M",
                >= 100000     => Math.Round((decimal)value / 1000,       2).ToString(CultureInfo.CreateSpecificCulture("en-US")) + "K",
                _             => Math.Round((decimal)value, 2).ToCommaStyle("0")
            };
        }

        public static string FormatNumber(this int num)
        {
            var value = Math.Abs(num);
            return num switch
            {
                >= 1000000000 => Math.Round((decimal)value / 1000000000, 2).ToString(CultureInfo.CreateSpecificCulture("en-US")) + "B",
                >= 1000000    => Math.Round((decimal)value / 1000000,    2).ToString(CultureInfo.CreateSpecificCulture("en-US")) + "M",
                >= 100000     => Math.Round((decimal)value / 1000,       2).ToString(CultureInfo.CreateSpecificCulture("en-US")) + "K",
                _             => Math.Round((decimal)value, 2).ToCommaStyle("0")
            };
        }

        public static string ToPositivePercent(this string value)
        {
            if (value.Contains("-") && value != "---") value = value.Replace("-", string.Empty); //--- ToCommaStyle value 0
            return $"{value}%";
        }

        public static string ToPositivePercent(this int value)
        {
            return $"{Math.Abs(value).ToString(CultureInfo.CreateSpecificCulture("en-US"))}%";
        }

        public static string ToPositivePercent(this decimal value)
        {
            return $"{Math.Round(Math.Abs(value), 2).ToString(CultureInfo.CreateSpecificCulture("en-US"))}%";
        }

        public static string ToPositivePercent(this double value)
        {
            return $"{Math.Round(Math.Abs(value), 2).ToString(CultureInfo.CreateSpecificCulture("en-US"))}%";
        }

        public static decimal ValueOrDefault(this decimal? value)
        {
            return value ?? 0;
        }

        public static string FormatCurrency(this decimal value, int placeNumber = 2, string zeroString = "---")
        {
            value  = Math.Abs(value);
            var result = value switch
            {
                >= 1000000000 => Math.Round(value / 1000000000, placeNumber, MidpointRounding.AwayFromZero),
                >= 1000000    => Math.Round(value / 1000000,    placeNumber, MidpointRounding.AwayFromZero),
                >= 1000       => Math.Round(value / 1000,       placeNumber, MidpointRounding.AwayFromZero),
                _            => value
            };
            return result == 0 ? zeroString : $"{result.ToString($"N{placeNumber}", CultureInfo.CreateSpecificCulture("en-US"))}";
        }

        public static string Numeral(this decimal value)
        {
            return value switch
            {
                >= 1000000000 => "T", >= 1000000 => "tr", >= 1000 => "k", _ => string.Empty
            };
        }
    }
}