using System.Threading.Tasks;
using LookOn.BackgroundWorkers.Datalytis;
using LookOn.BackgroundWorkers.Haravan;
using LookOn.BackgroundWorkers.LookOn;
using LookOn.BackgroundWorkers.MerchantSubscriptions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Volo.Abp;
using Volo.Abp.BackgroundWorkers;
using Volo.Abp.Modularity;

namespace LookOn;
public class LookOnBackgroundWorkerDomainModule : AbpModule
{
    public override Task OnApplicationInitializationAsync(
        ApplicationInitializationContext context)
    {
        ConfigBackgroundWorker(context);
        return Task.CompletedTask;
    }
    
    private Task ConfigBackgroundWorker(ApplicationInitializationContext context)
    {
        var hostEnvironment = context.ServiceProvider.GetRequiredService<IHostEnvironment>();

        if (hostEnvironment.IsProduction())
        {
            // DATALYTIS - Orders
            context.AddBackgroundWorkerAsync<Ecom_OrderRawSync_HRV_Job>();
            context.AddBackgroundWorkerAsync<Ecom_OrderSyncClean_HRV_Job>();
        
            //DATALYTIS - Social SocialUsers
            context.AddBackgroundWorkerAsync<Social_UserRequest_Job>();
            context.AddBackgroundWorkerAsync<Social_UserStatus_Job>();
            context.AddBackgroundWorkerAsync<Social_UserSync_Job>();
        
            //DATALYTIS - Insight SocialUsers
            context.AddBackgroundWorkerAsync<Metric_Social_InsightRequest_Job>();
            context.AddBackgroundWorkerAsync<Metric_Social_InsightStatus_Job>();
            context.AddBackgroundWorkerAsync<Metric_Social_InsightSync_Job>();
        
            context.AddBackgroundWorkerAsync<Subscription_Scan_Job>();
        
            // DATA
            context.AddBackgroundWorkerAsync<Data_MetricsPrepareData_Job>();
        }
        else
        {
            context.AddBackgroundWorkerAsync<Subscription_Scan_Job>();
        }
        
        return Task.CompletedTask;
    }
}