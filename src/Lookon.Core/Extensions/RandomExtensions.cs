using System;
using System.Text;
using LookOn.Core.SampleData;

namespace LookOn.Core.Extensions;

public static class RandomExtensions
{
    private static readonly Random  Rand = new Random();
    public static string RandomString(int length = 7)
    {
        // creating a StringBuilder object()
        var strBuild = new StringBuilder();

        for (var i = 0; i < length; i++)
        {
            var flt = Rand.NextDouble();
            var shift = Convert.ToInt32(Math.Floor(25 * flt));
            var letter = Convert.ToChar(shift + 65);
            strBuild.Append(letter);  
        }

        return strBuild.ToString();
    }
    
    public static string RandomNumber(int length = 9)
    {
        // creating a StringBuilder object()
        var strBuild = new StringBuilder();

        for (var i = 0; i < length; i++)
        {
            var number = Rand.Next(0, 9);
            strBuild.Append(number);  
        }

        return strBuild.ToString();
    }
    
    public static bool RandomBool()
    {
        return Rand.Next(0, 1) == 0;
    }

    public static DateTime RandomBirthDay()
    {
        var start = new DateTime(1922, 1, 1);
        var range = (DateTime.UtcNow.Date.AddYears(-13) - start).Days;           
        return start.AddDays(Rand.Next(range));
    }
    
    public static string RandomGender()
    {
        return Rand.Next(2) == 0 ? "Male" : "Female";
    }

    public static string RandomFirstName(string gender)
    {
        var isMale = gender == "Male";
        var items = isMale ? SampleName.MaleNames : SampleName.FemaleNames;

        return items?[Rand.Next(items.Count)] ?? string.Empty;
    }

    public static string RandomSurName()
    {
        var items = SampleName.SurNames;
        return items?[Rand.Next(items.Count)] ?? string.Empty;
    }
    
    public static string RandomProvince()
    {
        var items = VietNamProvince.Data;
        return items?[Rand.Next(items.Count)] ?? string.Empty;
    }
}