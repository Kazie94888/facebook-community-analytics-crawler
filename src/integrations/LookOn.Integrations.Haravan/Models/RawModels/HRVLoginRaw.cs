namespace LookOn.Integrations.Haravan.Models.RawModels;

public class HRVLoginRaw
{
    public string code          { get; set; }
    public string id_token      { get; set; }
    public string scope         { get; set; }
    public string session_state { get; set; }
}