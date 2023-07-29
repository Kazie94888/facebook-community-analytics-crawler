using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace LookOn.Integrations.Datalytis.Models.RawModels;

public class SocialUsers_Request
{
    /// <summary>
    /// page id, group id, profile id
    /// </summary>
    [JsonProperty("social_id")] public string PageId { get; set; }
    /// <summary>
    /// type = 1 (fanpage) ,type = 2 (group), type = 3(profile)
    /// </summary>
    [JsonProperty("type")] public int Type { get; set; }

    // others
    [JsonProperty("name")]        public string Name        { get; set; }
    [JsonProperty("description")] public string Description { get; set; }
}

public class SocialUsers_ResponsePayload
{
    [JsonProperty("id")]           public int    Id          { get; set; }
    [JsonProperty("social_id")]    public string SocialId    { get; set; }
    [JsonProperty("name")]         public string Name        { get; set; }
    [JsonProperty("description")]  public string Description { get; set; }
    [JsonProperty("type")]         public int    Type        { get; set; }
    [JsonProperty("tags")]         public object Tags        { get; set; }
    [JsonProperty("category_id")]  public int    CategoryId  { get; set; }
    [JsonProperty("created_by")]   public string CreatedBy   { get; set; }
    [JsonProperty("created_date")] public string CreatedDate { get; set; }
}

public class SocialUsers_Response : DatalytisBaseResponse
{
    [JsonProperty("data")] public SocialUsers_ResponsePayload Data { get; set; }
}

public class SocialUsers_Status : DatalytisBaseResponse
{
    [JsonProperty("data")] public SocialUsers_Status_Data Data { get; set; }
}

public class SocialUsers_Status_Data
{
    [JsonProperty("id")]     public string Id;
    [JsonProperty("status")] public string Status;

    // [JsonProperty("name")]         public string Name;
    // [JsonProperty("description")]  public string Description;
    // [JsonProperty("created_by")]   public string CreatedBy;
    // [JsonProperty("file_id")]      public string FileId;
    // [JsonProperty("created_date")] public string CreatedDate;
    // [JsonProperty("tags")]         public object Tags;
    // [JsonProperty("category_id")]  public string CategoryId;
    // [JsonProperty("type")]         public string Type;
}