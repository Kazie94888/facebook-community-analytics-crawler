using LookOn.Integrations.Haravan.Models.RawModels;
using Volo.Abp.Domain.Entities.Auditing;

namespace LookOn.Integrations.Haravan.Models.Entities;

public class HaravanStore : FullAuditedEntity<Guid>
{
    public Guid                       MerchantId      { get; set; }
    public Guid                       MerchantStoreId { get; set; }
    public Guid                       AppUserId       { get; set; }
    public string                     StoreId         { get; set; }
    public string                     StoreName       { get; set; }
    public string                     Email           { get; set; }
    public string                     Phone           { get; set; }
    public string                     Address         { get; set; }
    public Dictionary<string, string> Claims          { get; set; }
    public HRVTokenRaw               Token    { get; set; }
}

public class HaravanStoreConsts
{
    private const string DefaultSorting = "{0}StoreName asc";

    public static string GetDefaultSorting(bool withEntityName)
    {
        return string.Format(DefaultSorting, withEntityName ? "HaravanStore." : string.Empty);
    }

}