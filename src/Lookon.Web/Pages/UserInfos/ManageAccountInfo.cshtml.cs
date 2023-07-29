using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using LookOn.Core.Extensions;
using LookOn.UserInfos;
using LookOn.Users;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Volo.Abp;
using Volo.Abp.Account;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;
using Volo.Abp.Content;
using Volo.Abp.Users;

namespace LookOn.Web.Pages.UserInfos;

public class Manage : AbpPageModel
{
    private readonly IUserInfosAppService _userInfosAppService;
    private readonly ICurrentUser         _currentUser;
    
    public Manage(IUserInfosAppService userInfosAppService, ICurrentUser currentUser)
    {
        _userInfosAppService      = userInfosAppService;
        _currentUser              = currentUser;
    }
    
    [BindProperty] public UserInfoDto                   UserInfo             { get; set; }
    [BindProperty] public UserDto                       AppUser              { get; set; }
    [BindProperty] public UploadProfilePictureInfoModel PictureInfoModel     { get; set; }
    [BindProperty] public string                        PhoneNumber          { get; set; }
    [BindProperty] public string                        IdentificationNumber { get; set; }

    public async Task OnGetAsync()
    {
        var userId = _currentUser.GetId();
        UserInfo             = await _userInfosAppService.GetUserInfo(userId);
        AppUser              = await _userInfosAppService.GetAppUser(userId);
        PhoneNumber          = AppUser.PhoneNumber;
        IdentificationNumber = UserInfo.IdentificationNumber;
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (ModelState.IsValid)
        {
            var phoneNumber = PhoneNumber.RemoveNonNumeric().ToInternationalPhoneNumber();
            if (PhoneNumber.IsNotNullOrEmpty() && !phoneNumber.ValidatePhoneNumber())
            {
                throw new UserFriendlyException(L["PhoneNotValid"]);
            }
            
            if (IdentificationNumber.IsNotNullOrEmpty() && !IdentificationNumber.ValidateIdentificationNumber())
            {
                throw new UserFriendlyException(L["IdentificationNotValid"]);
            }

            if (!AppUser.Email.ValidEmail())
            {
                throw new UserFriendlyException(L["EmailNotValid"]);
            }

            AppUser.PhoneNumber           = phoneNumber;
            UserInfo.IdentificationNumber = IdentificationNumber;
            var updatedUserInfo = ObjectMapper.Map<UserInfoDto, UserInfoUpdateDto>(UserInfo);
            await _userInfosAppService.UpdateAsync(UserInfo.Id, updatedUserInfo);
            
            await _userInfosAppService.UpdateAppUser(_currentUser.GetId(), AppUser);
            if (PictureInfoModel.Picture != null)
            {
                await _userInfosAppService.UpdateProfilePictureAppUser(CurrentUser.GetId(), new UserUpdateProfileDto()
                {
                    Type = ProfilePictureType.Image,
                    FileBytes = await PictureInfoModel.Picture.GetAllBytesAsync(),
                    FileName = PictureInfoModel.Picture.FileName
                });
                
                return Redirect("~/account-info");
            }
            Alerts.Success(L["Successfully"]);
        }
        else
        {
            throw new UserFriendlyException(L["UpdateUserFail"]);
        }
        return NoContent();
    }
    
    public class UploadProfilePictureInfoModel
    {
        public ProfilePictureType Type { get; set; }

        public IFormFile Picture { get; set; }
    }
}