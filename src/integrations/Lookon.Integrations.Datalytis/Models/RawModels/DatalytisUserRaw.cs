using System.Collections.Generic;
using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace LookOn.Integrations.Datalytis.Models.RawModels;

public class DatalytisUserRaw
{
    [JsonProperty("uid")] public string Uid { get; set; }

    // 84383960591
    [JsonProperty("phone")]    public string Phone    { get; set; }
    [JsonProperty("email")]    public string Email    { get; set; }
    [JsonProperty("name")]     public string Name     { get; set; }
    [JsonProperty("fullname")] public string FullName { get; set; }

    //"1989-07-31"
    [JsonProperty("birthday")]     public string Birthday     { get; set; }
    [JsonProperty("sex")]          public string Sex          { get; set; }
    [JsonProperty("relationship")] public string Relationship { get; set; }
    [JsonProperty("address")]         public string Address      { get; set; }
    [JsonProperty("city")]         public string City         { get; set; }
    [JsonProperty("friends")]      public int    Friends      { get; set; }
    [JsonProperty("follow")]       public int    Follow       { get; set; }
    [JsonProperty("cmnd")]         public string Cmnd         { get; set; }
    
    // Notes
    [JsonProperty("note1")]        public string Note1        { get; set; }
    [JsonProperty("note2")]        public string Note2        { get; set; }
    [JsonProperty("note3")]        public string Note3        { get; set; }
    [JsonProperty("note4")]        public string Note4        { get; set; }
    [JsonProperty("note5")]        public string Note5        { get; set; }
    [JsonProperty("note6")]        public string Note6        { get; set; }
    [JsonProperty("note7")]        public string Note7        { get; set; }
}

public class DatalytisUsersResponse : DatalytisListBaseResponse
{
    [JsonProperty("data")] public List<DatalytisUserRaw> Data { get; set; }
}