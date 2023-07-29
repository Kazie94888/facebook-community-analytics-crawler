using System.Collections.Generic;
using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace LookOn.Integrations.Datalytis.Models.RawModels;

public class DatalytisUserSocialInsightResponseData : DatalytisBaseResponse
{
    [JsonProperty("data")] public DatalytisUserSocialInsightRaw Data { get; set; }
}

public class DatalytisUserSocialInsightRaw
{
    [JsonProperty("uid")]     public string                     Uid      { get; set; }
    [JsonProperty("insight")] public List<UserSocialInsightRaw> Insights { get; set; }
    [JsonProperty("paging")]  public Paging                     Paging   { get; set; }

    public DatalytisUserSocialInsightRaw()
    {
        Insights = new List<UserSocialInsightRaw>();
    }
}

public class UserSocialInsightCategoryItemRaw
{
    [JsonProperty("Id")]
    public string Id { get; set; }
    [JsonProperty("Name")]
    public string Name { get; set; }
}

public class UserSocialInsightRaw
{
    [JsonProperty("id")]       public string                       Id       { get; set; }
    [JsonProperty("uid")]      public string                       UId      { get; set; }
    [JsonProperty("name")]     public string                       Name     { get; set; }
    [JsonProperty("category")] public UserSocialInsightCategoryRaw Category { get; set; }
    [JsonProperty("url")]      public string                       Url      { get; set; }

    // "2022-02-07 11:30:38"
    [JsonProperty("created_time")] public string CreatedTime { get; set; }
    [JsonProperty("location")]     public string Location    { get; set; }
    [JsonProperty("type")]         public int    Type        { get; set; }
}

public class UserSocialInsightCategoryRaw
{
    [JsonProperty("category")]      public string                                 Category     { get; set; }
    [JsonProperty("category_list")] public List<UserSocialInsightCategoryItemRaw> CategoryList { get; set; }
}

public class Cursors
{
    [JsonProperty("before")] public string Before { get; set; }
    [JsonProperty("after")]  public string After  { get; set; }
}

public class Paging
{
    [JsonProperty("cursors")] public Cursors Cursors { get; set; }
    [JsonProperty("next")]    public string  Next    { get; set; }
}

public class DatalytisSocialInsight_Request
{
    [JsonProperty("uids")] public List<string> Uids { get; set; }
}

public class DatalytisSocialInsight_Response : DatalytisListBaseResponse
{
    [JsonProperty("data")] public DatalytisSocialInsight_Response_Data Data { get; set; }
}

public class DatalytisSocialInsight_Response_Data : DatalytisInsight_Response_Data
{
    [JsonProperty("process")] public string Process { get; set; }
}

public class DatalytisInsight_Response_Data
{
    // "2022-01-20 08:10:29"
    [JsonProperty("created_date")] public string CreatedDateString { get; set; }
    [JsonProperty("id")]           public string Id                { get; set; }

    //  process data (status = 15)
    //  active/ready (status = 10) => scan done
    [JsonProperty("status")] public string Status { get; set; }
    [JsonProperty("size")]   public string Size   { get; set; }
}