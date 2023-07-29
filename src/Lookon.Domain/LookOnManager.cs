using System;
using System.Diagnostics;
using LookOn.Configs;
using LookOn.Localization;
using Microsoft.Extensions.Localization;
using Volo.Abp.Domain.Services;
using Microsoft.Extensions.Configuration;

namespace LookOn;

public abstract class LookOnManager : DomainService
{
    public GlobalConfig                          GlobalConfig    { get; set; }
    public IStringLocalizer<LookOnResource>      L               { get; set; }
    public IStringLocalizer<LookOnErrorResource> Err             { get; set; }
    public Volo.Abp.ObjectMapping.IObjectMapper  ObjectMapper    { get; set; }
    public IConfiguration                        Configuration   { get; set; }

    public void LogDebug(Type type, string value)
    {
        var logMessage = $"---> {type.Name}: {value}";
        Debug.WriteLine(logMessage);
    }
}