using System;

namespace FacebookCommunityAnalytics.Crawler.NET.Client.Entities
{
    public class ViotpResult
    {
        
    }

    public class RequestPhoneNumberResponse
    {
        // {"status_code":200,"success":true,"message":"successful","data":{"phone_number":"987654321","balance":50000,"request_id":"122314"}}
        public int status_code { get; set; }
        public bool success { get; set; }
        public string message { get; set; }
        public PhoneNumberDetails data { get; set; }
    }

    public class PhoneNumberDetails
    {
        public string phone_number { get; set; }
        public int balance { get; set; }
        public string request_id { get; set; }
    }

    public class SessionResponse
    {
        //{"status_code":200,"success":true,"message":"successful","data":{"ID":58098,"ServiceID":1,"ServiceName":"Momo","Status":1,"Price":600,"Phone":"987654321","SmsContent":"486460 la ma xac thuc OTP dang ky vi MoMo. De tranh bi mat tien, tuyet doi KHONG chia se ma nay voi bat ky ai. zniHFS3HBXG","IsSound":"false","CreatedTime":"2020-08-06T17:13:24.88","Code":"486460"}}
        public int status_code { get; set; }
        public bool success { get; set; }
        public string message { get; set; }
        public SessionDetails data { get; set; }
    }

    public class SessionDetails
    {
        public int ID { get; set; }
        public int ServiceID { get; set; }
        public string ServiceName { get; set; }
        public int Status { get; set; }
        public int Price { get; set; }
        public string Phone { get; set; }
        public string SmsContent { get; set; }
        public bool IsSound { get; set; }
        public DateTime CreatedTime { get; set; } 
        public string Code { get; set; }
    }
}