using System.Threading.Tasks;
using LookOn.Services.Account.Emailing;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Volo.Abp;
using Volo.Abp.Account;
using Volo.Abp.Account.Emailing;
using Volo.Abp.Account.Localization;
using Volo.Abp.Account.Settings;
using Volo.Abp.Application.Services;
using Volo.Abp.Identity;
using Volo.Abp.ObjectExtending;
using Volo.Abp.Settings;
using IdentityUser = Volo.Abp.Identity.IdentityUser;

namespace LookOn.Services;

public class CustomCustomAccountAppService : ApplicationService, ICustomAccountAppService
{
    protected IIdentityRoleRepository    RoleRepository             { get; }
    protected IdentityUserManager        UserManager                { get; }
    protected ICustomAccountEmailer      CustomAccountEmailer       { get; }
    protected IdentitySecurityLogManager IdentitySecurityLogManager { get; }
    protected IOptions<IdentityOptions>  IdentityOptions            { get; }

    public CustomCustomAccountAppService(
        IdentityUserManager        userManager,
        IIdentityRoleRepository    roleRepository,
        IdentitySecurityLogManager identitySecurityLogManager,
        IOptions<IdentityOptions>  identityOptions,
        ICustomAccountEmailer      customAccountEmailer)
    {
        RoleRepository             = roleRepository;
        IdentitySecurityLogManager = identitySecurityLogManager;
        UserManager                = userManager;
        IdentityOptions            = identityOptions;
        CustomAccountEmailer       = customAccountEmailer;

        LocalizationResource = typeof(AccountResource);
    }

    public virtual async Task<IdentityUserDto> RegisterAsync(RegisterDto input)
    {
        await CheckSelfRegistrationAsync();

        await IdentityOptions.SetAsync();

        var user = new IdentityUser(GuidGenerator.Create(), input.UserName, input.EmailAddress, CurrentTenant.Id);

        input.MapExtraPropertiesTo(user);

        (await UserManager.CreateAsync(user, input.Password)).CheckErrors();

        await UserManager.SetEmailAsync(user, input.EmailAddress);
        await UserManager.AddDefaultRolesAsync(user);

        return ObjectMapper.Map<IdentityUser, IdentityUserDto>(user);
    }

    public virtual async Task SendPasswordResetCodeAsync(SendPasswordResetCodeDto input)
    {
        var user = await GetUserByEmailAsync(input.Email);
        var resetToken = await UserManager.GeneratePasswordResetTokenAsync(user);
        await CustomAccountEmailer.SendPasswordResetLinkAsync(user, resetToken, input.AppName, input.ReturnUrl, input.ReturnUrlHash);
    }

    public virtual async Task ResetPasswordAsync(ResetPasswordDto input)
    {
        await IdentityOptions.SetAsync();

        var user = await UserManager.GetByIdAsync(input.UserId);
        (await UserManager.ResetPasswordAsync(user, input.ResetToken, input.Password)).CheckErrors();

        await IdentitySecurityLogManager.SaveAsync(new IdentitySecurityLogContext
        {
            Identity = IdentitySecurityLogIdentityConsts.Identity,
            Action = IdentitySecurityLogActionConsts.ChangePassword
        });
    }

    protected virtual async Task<IdentityUser> GetUserByEmailAsync(string email)
    {
        var user = await UserManager.FindByEmailAsync(email);
        if (user == null)
        {
            throw new UserFriendlyException(L["Volo.Account:InvalidEmailAddress", email]);
        }

        return user;
    }

    protected virtual async Task CheckSelfRegistrationAsync()
    {
        if (!await SettingProvider.IsTrueAsync(AccountSettingNames.IsSelfRegistrationEnabled))
        {
            throw new UserFriendlyException(L["SelfRegistrationDisabledMessage"]);
        }
    }
}