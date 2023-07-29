using System;
using System.Linq;
using System.Threading.Tasks;
using LookOn.Services;
using LookOn.UserInfos;
using Volo.Abp.Account;
using Volo.Abp.Account.Public.Web.Pages.Account;
using Volo.Abp.Identity;
using Volo.Saas;
using Volo.Saas.Host;
using Volo.Saas.Host.Dtos;
using Volo.Saas.Tenants;

namespace LookOn.Pages.Account;

public class CustomRegisterModel : RegisterModel
{
    private readonly EmailService    _emailService;
    private readonly UserInfoManager _userInfoManager;
    public CustomRegisterModel(EmailService emailService, UserInfoManager userInfoManager)
    {
        _emailService         = emailService;
        _userInfoManager = userInfoManager;
    }

    protected override async Task<IdentityUser> RegisterLocalUserAsync()
    {
        var user = await base.RegisterLocalUserAsync();
        await _userInfoManager.InitUserInfo(user.Id);
        return user;
    }
}