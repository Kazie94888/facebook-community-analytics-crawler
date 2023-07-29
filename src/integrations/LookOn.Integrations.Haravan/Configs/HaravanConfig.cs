namespace LookOn.Integrations.Haravan.Configs;

public class HaravanConfig
{
    public string response_mode { get; set; }
    public string url_authorize { get; set; }
    public string url_connect_token { get; set; }
    public string grant_type { get; set; }
    public string nonce { get; set; }
    public string response_type { get; set; }
    public string app_id { get; set; }
    public string app_secret { get; set; }
    public string scope_login { get; set; }
    public string login_callback_url { get; set; }
    public HaravanWebhookConfig webhook { get; set; }
}


public class HaravanWebhookConfig
{
    public string hrVerifyToken { get; set; }
    public string subscribe { get; set; }
}