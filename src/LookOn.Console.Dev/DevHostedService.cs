using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Volo.Abp;

namespace LookOn.Console.Dev;

public class DevHostedService : IHostedService
{
    private IAbpApplicationWithInternalServiceProvider _abpApplication;

    private readonly IConfiguration   _configuration;
    private readonly IHostEnvironment _hostEnvironment;

    public DevHostedService(IConfiguration configuration, IHostEnvironment hostEnvironment)
    {
        _configuration   = configuration;
        _hostEnvironment = hostEnvironment;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _abpApplication =  await AbpApplicationFactory.CreateAsync<DevModule>(options =>
        {
            options.Services.ReplaceConfiguration(_configuration);
            options.Services.AddSingleton(_hostEnvironment);

            options.UseAutofac();
            options.Services.AddLogging(loggingBuilder => loggingBuilder.AddSerilog());
        });

        await _abpApplication.InitializeAsync();

        // WARNING: Do not create any methods after this line.
        // DO NOT REMOVE THIS, ALL TEST SERVICES SHOULD BE INSIDE STARTSERVICE.EXECUTE()
        var startService = _abpApplication.ServiceProvider.GetRequiredService<StartService>();
        await startService.Execute();
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await _abpApplication.ShutdownAsync();
    }
}
