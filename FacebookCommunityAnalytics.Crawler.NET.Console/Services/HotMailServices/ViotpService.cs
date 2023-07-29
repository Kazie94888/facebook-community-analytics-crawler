using System;
using System.Text.RegularExpressions;
using System.Threading;
using FacebookCommunityAnalytics.Crawler.NET.Client.Clients;
using FacebookCommunityAnalytics.Crawler.NET.Client.Configurations;
using FacebookCommunityAnalytics.Crawler.NET.Client.Entities;

namespace FacebookCommunityAnalytics.Crawler.NET.Console.Services.HotMailServices
{
    public class ViotpService
    {
        private readonly GlobalConfig _globalConfig;
        private readonly ApiClient _apiClient;
        
        
        public ViotpService(GlobalConfig globalConfig)
        {
            _globalConfig = globalConfig;
            _apiClient = new ApiClient(globalConfig.ApiConfig);
        }

        public PhoneNumberDetails RegisterPhoneNumber()
        {
            while (true)
            {
                try
                {
                    // service Id = 5 : Hotmail/Outlook/Azure (Microsoft)
                    // network Mobile phone
                    var response = _apiClient.Viotp.RequestPhoneNumber();
                    if (!response.Resource.success)
                        throw new Exception($"{response.Resource.status_code} - {response.Resource.message}");
            
                    var phoneNumber = response.Resource.data.phone_number;
                    System.Console.WriteLine($"Phone Number : {phoneNumber}");

                    return response.Resource.data;
                }
                catch (Exception e)
                {
                    if (!e.Message.Contains("Hiện không có sẵn số điện thoại phù hợp, vui lòng thử lại sau"))
                    {
                        throw;
                    }

                    using (var autoResetEvent = new AutoResetEvent(false))
                    {
                        autoResetEvent.WaitOne(120000);
                    }
                }
            }
            
        }

        public string GetPhoneCode(string requestId)
        {
            var phoneCode = string.Empty;
            var currentDateTime = DateTime.Now;
            while (string.IsNullOrWhiteSpace(phoneCode))
            {
                var response = _apiClient.Viotp.GetPhoneCode(requestId);
                if (!response.Resource.success)
                    throw new Exception($"{response.Resource.status_code} - {response.Resource.message}");

                var smsContent = response.Resource.data.SmsContent;
                if (!string.IsNullOrWhiteSpace(smsContent))
                {
                    System.Console.WriteLine($"sms content : {smsContent}");
            
                    //TODO: get get code by Regex ([0-9])\w+
                    var regex = new Regex("([0-9])\\w+");
                    var match = regex.Match(response.Resource.data.SmsContent);

                    phoneCode = match.Success ? match.Groups[0].Value : string.Empty;
                }
                else
                {
                    if (DateTime.Now.Subtract(currentDateTime).TotalMinutes > 10)
                    {
                        return string.Empty;
                    }
                }
                
            }

            return phoneCode;
        }
        
    }
}