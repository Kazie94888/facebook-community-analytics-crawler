namespace LookOn.UserInfos;

public class UserDto
{
    public string UserName { get; set; }

    public string Email { get; set; }

    public string Name { get; set; }

    public string Surname { get; set; }
    public string PhoneNumber { get; set; }
    public bool IsActive { get; set; }
    public bool IsLockedOut { get; set; }
}