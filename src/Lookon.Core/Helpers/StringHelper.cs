using System;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Web;
using LookOn.Core.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace LookOn.Core.Helpers
{
    public static class StringHelper
    {
        private static Random random = new Random();
        private const string GreenColor = "color:forestgreen;";
        private const string RedColor = "color:red;";
        private const string BlackColor = "color:black;";
        private const string BoldText = "font-weight: bold;";

        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";

            var randomed = Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)])
                .ToArray();
            return $"{new string(randomed)}";
        }

        public static string RandomStringAll(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            const string specialChars = "!@#$%^&*";

            var randomed = Enumerable.Repeat(chars, length - 1)
                .Select(s => s[random.Next(s.Length)])
                .ToArray();
            var randomedSpecialString = Enumerable.Repeat(specialChars, 1)
                .Select(s => s[random.Next(s.Length)])
                .ToArray();
            return $"{new string(randomed)}{new string(randomedSpecialString)}";
        }

        public static bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch { return false; }
        }

        public static string FormatGrowthColor(bool? isIncrease)
        {
            switch (isIncrease)
            {
                case true: return GreenColor + BoldText;
                case false: return RedColor + BoldText;
                default: return BlackColor;
            }
        }

        public static Color FormatGrowthColorBla(bool? isIncrease)
        {
            switch (isIncrease)
            {
                case true: return Color.Green;
                case false: return Color.Red;
                default: return Color.Black;
            }
        }

        public static string FormatNumberInExcel(int number)
        {
            return String.Format(CultureInfo.InvariantCulture, "{0:#,#0}", number);
        }

        // public static string FormatDecimal(this string input)
        // {
        //     if (input.IsNullOrEmpty())
        //     {
        //         return input;
        //     }
        //     input = input.Replace(".", string.Empty);
        //     //var cul = CultureInfo.GetCultureInfo("en-Us");
        //     return decimal.Parse(input).ToString("N0");
        // }

        public static string ToHtmlBreak(this string input)
        {
            if (input.IsNullOrEmpty())
            {
                return string.Empty;
            }

            return input.Replace("\n", "<br/>");
        }

        public static string ToQueryString(this object input)
        {
            var properties = from p in input.GetType().GetProperties()
                where p.GetValue(input, null) != null
                select p.Name + "=" + HttpUtility.UrlEncode(p.GetValue(input, null).ToString());

            string queryString = String.Join("&", properties.ToArray());
            return queryString;
        }
        
        public static string GetQueryString(this object obj, string prefix = "")
        {
            var vQueryString = (JsonConvert.SerializeObject(obj));

            var jObj = (JObject)JsonConvert.DeserializeObject(vQueryString);
            var query = String.Join("&",
                jObj.Children().Cast<JProperty>()
                    .Select(jp =>
                        {
                            if (jp.Value.Type == JTokenType.Array)
                            {
                                var count = 0;
                                var arrValue = String.Join("&", jp.Value.ToList().Select<JToken, string>(p =>
                                {
                                    var tmp = JsonConvert.DeserializeObject(p.ToString()).GetQueryString(jp.Name + HttpUtility.UrlEncode("[") + count++ + HttpUtility.UrlEncode("]"));
                                    return tmp;
                                }));
                                return arrValue;
                            }

                            return (prefix.Length > 0 ? prefix + HttpUtility.UrlEncode("[") + jp.Name + HttpUtility.UrlEncode("]") : jp.Name) + "=" + HttpUtility.UrlEncode(jp.Value.ToString());
                        }
                    ));
            
            return query;
        }
        
        public static string NumberToWords(this long number)
        {
            if (number < 1000)
            {
                return number.ToString();
            }
            long mag = (long)(Math.Floor(Math.Log10(number))/3); // Truncates to 6, divides to 2
            double divisor = Math.Pow(10, mag*3);

            double shortNumber = number / divisor;

            string suffix = null;
            switch(mag)
            {
                case 0:
                    suffix = string.Empty;
                    break;
                case 1:
                    suffix = "k";
                    break;
                case 2:
                    suffix = "m";
                    break;
                case 3:
                    suffix = "b";
                    break;
            }
            string result = shortNumber.ToString("N1") + suffix; // 4.3m
            return result.ToUpper();
        }
    }
}