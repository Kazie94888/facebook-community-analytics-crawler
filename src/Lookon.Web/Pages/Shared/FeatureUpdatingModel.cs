namespace LookOn.Web.Pages.Shared;

public class FeatureUpdatingModel
{
    public bool   HasContent { get; set; }
    public bool   HasImage   { get; set; } = true;
    public bool   IsInverse  { get; set; }
    public string Title    { get; set; }
}