using System.ComponentModel.DataAnnotations;

namespace LookOn.Core.Extensions;

public static class EmailExtensions
{
    public static bool ValidEmail(this string email)
    {
        var addressAttribute = new EmailAddressAttribute();
        return addressAttribute.IsValid(email);
    }
}