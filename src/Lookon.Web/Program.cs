using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.Elasticsearch;

namespace LookOn.Web;

public class Program
{
    public async static Task<int> Main(string[] args)
    {
        var configuration = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
                                                      .AddJsonFile("appsettings.json")
                                                      .AddEnvironmentVariables()
                                                      .Build();

        Log.Logger = new LoggerConfiguration()

                     //#if DEBUG
                    .MinimumLevel.Debug()

                     //#else
                    .MinimumLevel.Information()

                     //#endif
                    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                    .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning)
                    .Enrich.FromLogContext()
                    .WriteTo.Async(c => c.File($"Logs/logs-{DateTime.Now:ddMMyyyy}.txt"))
                    .WriteTo.Logger(lc => lc.Filter.ByIncludingOnly(_ => _.Level.IsIn(LogEventLevel.Warning, LogEventLevel.Error))
                                            .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri(configuration["ElasticSearch:Url"]))
                                             {
                                                 // AutoRegisterTemplate        = true,
                                                 // AutoRegisterTemplateVersion = AutoRegisterTemplateVersion.ESv6,
                                                 AutoRegisterTemplate = true,
                                                 TypeName             = null,
                                                 IndexFormat          = "lookon-log-insight-{0:yyyy.MM}"
                                             }))

                     //#if DEBUG
                    .WriteTo.Async(c => c.Console())

                     //#endif
                    .CreateLogger();

        try
        {
            Log.Information("Starting web host.");
            var builder = WebApplication.CreateBuilder(args);
            builder.Host.ConfigureAppConfiguration((hostingContext, config) =>
                    {
                        var env = hostingContext.HostingEnvironment;

                        if (!env.IsProduction())
                        {
                            config.AddJsonFile(Path.Combine(env.ContentRootPath, "../..", "Configs/globalconfigs.json"), true, true)
                                  .AddJsonFile(Path.Combine(env.ContentRootPath, "../..", $"Configs/globalconfigs.{env.EnvironmentName}.json"),
                                               true,
                                               true);

                            //Haravan config
                            config.AddJsonFile(Path.Combine(env.ContentRootPath, "../..", "Configs/haravanconfigs.json"), true, true)
                                  .AddJsonFile(Path.Combine(env.ContentRootPath, "../..", $"Configs/haravanconfigs.{env.EnvironmentName}.json"),
                                               true,
                                               true);
                        }
                        else
                        {
                            config.AddJsonFile("Configs/globalconfigs.json", true, true)
                                  .AddJsonFile($"Configs/globalconfigs.{env.EnvironmentName}.json", true, true);

                            //Haravan config
                            config.AddJsonFile("Configs/haravanconfigs.json", true, true)
                                  .AddJsonFile($"Configs/haravanconfigs.{env.EnvironmentName}.json", true, true);
                        }
                    })
                   .AddAppSettingsSecretsJson()
                   .UseAutofac()
                   .UseSerilog();
            await builder.AddApplicationAsync<LookOnWebModule>();
            var app = builder.Build();
            await app.InitializeApplicationAsync();
            await app.RunAsync();
            return 0;
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Host terminated unexpectedly!");
            return 1;
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }
}