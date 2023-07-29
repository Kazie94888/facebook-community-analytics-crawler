using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace LookOn.Integrations.Datalytis.Models.RawModels;

public class DatalytisBaseResponse
{
    [JsonProperty("message")]     public string Message    { get; set; }
    [JsonProperty("success")]     public bool   Success    { get; set; }
    [JsonProperty("status_code")] public int    StatusCode { get; set; }
}

public class DatalytisListBaseResponse: DatalytisBaseResponse
{
    public string limit       { get; set; }
    public int    total       { get; set; }
}

