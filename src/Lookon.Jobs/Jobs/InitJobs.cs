using Hangfire;
using LookOn.Jobs.Jobs.Datalytis;
using LookOn.Jobs.Jobs.Haravan;
// using LookOn.Jobs.Jobs.LookOn;
using LookOn.Jobs.Jobs.MerchantSubscriptions;
using Microsoft.Extensions.Hosting;

namespace LookOn.Jobs.Jobs;

public static class InitJobs
{
    public static void Setup(IHostEnvironment hostEnvironment)
    {
        var cronNever  = Cron.Never();
        var cronHourly = Cron.Hourly();
        var cronDaily  = Cron.Daily();

        var cron30Minutes = "*/30 * * * *"; // every 30 minutes

        if (hostEnvironment.IsProduction())
        {
            // DATALYTIS - Orders
            RecurringJob.AddOrUpdate<Ecom_OrderRawSync_HRV_Job>("Ecom_OrderRawSync_HRV_Job", o => o.Execute(), cron30Minutes);
            RecurringJob.AddOrUpdate<Ecom_OrderSyncClean_HRV_Job>("Ecom_OrderSyncClean_HRV_Job", o => o.Execute(), cron30Minutes);
        
            //DATALYTIS - Social SocialUsers
            RecurringJob.AddOrUpdate<Social_UserRequest_Job>("Social_UserRequest_Job", o => o.Execute(), cron30Minutes);
            RecurringJob.AddOrUpdate<Social_UserStatus_Job>("Social_UserStatus_Job", o => o.Execute(), cron30Minutes);
            RecurringJob.AddOrUpdate<Social_UserSync_Job>("Social_UserSync_Job", o => o.Execute(), cron30Minutes);
        
            //DATALYTIS - Insight SocialUsers
            RecurringJob.AddOrUpdate<Page1_Social_InsightRequest_Job>("Page1_Social_InsightRequest_Job", o => o.Execute(), cron30Minutes);
            RecurringJob.AddOrUpdate<Page1_Social_InsightStatus_Job>("Page1_Social_InsightStatus_Job", o => o.Execute(), cron30Minutes);
            RecurringJob.AddOrUpdate<Page1_Social_InsightSync_Job>("Page1_Social_InsightSync_Job", o => o.Execute(), cron30Minutes);

            RecurringJob.AddOrUpdate<Page2_Social_InsightRequest_Job>("Page2_Social_InsightRequest_Job", o => o.Execute(), cron30Minutes);
            RecurringJob.AddOrUpdate<Page2_Social_InsightStatus_Job>("Page2_Social_InsightStatus_Job", o => o.Execute(), cron30Minutes);
            RecurringJob.AddOrUpdate<Page2_Social_InsightSync_Job>("Page2_Social_InsightSync_Job", o => o.Execute(), cron30Minutes);
        
            RecurringJob.AddOrUpdate<Subscription_Scan_Job>("Subscription_Scan_Job", o => o.Execute(), cronDaily);

            // // DATA
            // RecurringJob.AddOrUpdate<Data_MetricsPrepareData_Job>("Data_Page1PrepareData_Job", o => o.Execute(), cron30Minutes);
            // RecurringJob.AddOrUpdate<Data_Page2PrepareData_Job>("Data_Page2PrepareData_Job", o => o.Execute(), cron30Minutes);
        }
        else
        {
            RecurringJob.AddOrUpdate<Subscription_Scan_Job>("Subscription_Scan_Job", o => o.Execute(), cronNever);
        }
    }
}