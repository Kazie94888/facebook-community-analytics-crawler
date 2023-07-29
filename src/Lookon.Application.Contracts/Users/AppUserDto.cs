using Volo.Abp.Identity;

namespace LookOn.Users;

public class AppUserDto : IdentityUserDto
{
    public string GetFullName()
    {
        return $"{Surname} {Name}";
    }
    public string GetFullNameWithUserName()
    {
        return $"{Surname} {Name} - ({UserName})";
    }
    public string GetFullNameWithEmail()
    {
        return $"{Surname} {Name} - ({Email})";
    }
}