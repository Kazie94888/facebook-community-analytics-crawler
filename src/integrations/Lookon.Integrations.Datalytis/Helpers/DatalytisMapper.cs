using System;
using System.Globalization;
using LookOn.Core.Extensions;
using LookOn.Enums;
using LookOn.Integrations.Datalytis.Models.Enums;

namespace LookOn.Integrations.Datalytis.Helpers;

public static class DatalytisMapper
{
    public static RelationshipStatus ToRelationshipStatus(string value)
    {
        if (value.IsNullOrWhiteSpace()) return RelationshipStatus.Unknown;

        var single         = new[] { "single", "It's complicated", "độc thân", "complicated" };
        var married        = new[] { "Married", "đã kết hôn", "Engaged", "In a civil union", "In a domestic partnership" };
        var inRelationship = new[] { "In a relationship", "In an open relationship", "hẹn hò" };
        var divorced       = new[] { "Divorced", "ly dị" };
        var separated      = new[] { "separated" };
        var widowed        = new[] { "Widowed" };

        if (value.IsInIgnoreCase(single)) return RelationshipStatus.Single;
        if (value.IsInIgnoreCase(married)) return RelationshipStatus.Married;
        if (value.IsInIgnoreCase(inRelationship)) return RelationshipStatus.InRelationship;
        if (value.IsInIgnoreCase(divorced)) return RelationshipStatus.Divorced;
        if (value.IsInIgnoreCase(separated)) return RelationshipStatus.Separated;
        if (value.IsInIgnoreCase(widowed)) return RelationshipStatus.Widowed;

        return RelationshipStatus.Unknown;
    }

    public static GenderType ToGenderType(string value)
    {
        if (value.IsNullOrWhiteSpace()) return GenderType.Unknown;

        var male   = new[] { "male", "name", "đàn ông" };
        var female = new[] { "female", "nữ", "phụ nữ", "đàn bà" };

        if (value.IsInIgnoreCase(male)) return GenderType.Male;
        if (value.IsInIgnoreCase(female)) return GenderType.Female;

        return GenderType.Unknown;
    }

    /// <summary>
    /// ToDatalytisBirthday
    /// </summary>
    /// <param name="value">"1989-07-31"</param>
    /// <returns></returns>
    public static DateTime? ToDatalytisBirthday(string value)
    {
        if (value.IsNullOrWhiteSpace()) return null;

        var canParse = DateTime.TryParseExact(value,
                                              "yyyy-MM-dd",
                                              null,
                                              DateTimeStyles.None,
                                              out var dateTime);

        return canParse ? dateTime : null;
    }
}