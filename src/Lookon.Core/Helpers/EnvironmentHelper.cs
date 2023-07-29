using System;

namespace LookOn.Core.Helpers;

public class EnvironmentHelper
{
    private const string ProductionEnvironment = "Production";
    private const string DevelopmentEnvironment = "Development";

    public static bool IsProduction()
    {
        return Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == ProductionEnvironment
            || Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT")     == ProductionEnvironment;
    }
    
    public static bool IsDevelopment()
    {
        return Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == DevelopmentEnvironment
            || Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT")     == DevelopmentEnvironment;
    }
}