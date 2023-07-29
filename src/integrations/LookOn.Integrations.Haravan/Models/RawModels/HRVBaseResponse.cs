using Newtonsoft.Json;

namespace LookOn.Integrations.Haravan.Models.RawModels;

public class HRVBaseResponse
{
    [JsonProperty("error")]   public bool Error   { get; set; }
    [JsonProperty("message")] public string Message { get; set; }
}