namespace LookOn.Integrations.Haravan.Models.RawModels;

public class HRVWebhookCallbackRaw
{
    public string Mode         { get; set; }
    public string Verify_Token { get; set; }
    public string Challenge    { get; set; }
}