using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityModel;
using LookOn.Integrations.Haravan.Components.ApiConsumers;
using LookOn.Integrations.Haravan.Models.Entities;
using LookOn.Integrations.Haravan.Models.RawModels;
using LookOn.Merchants;
using LookOn.MerchantStores;
using LookOn.MerchantUsers;
using LookOn.UserInfos;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Owl.reCAPTCHA;
using Volo.Abp.Account.ExternalProviders;
using Volo.Abp.Account.Public.Web;
using Volo.Abp.Account.Public.Web.Pages.Account;
using Volo.Abp.Account.Security.Recaptcha;
using Volo.Abp.Data;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Identity;
using Volo.Abp.Security.Claims;
using Volo.Abp.Uow;
using Volo.Saas.Tenants;

namespace LookOn.Pages.Account;

public class CustomLoginModel : LoginModel
{
    private readonly IMerchantRepository       _merchantRepo;
    private readonly IMerchantStoreRepository  _merchantStoreRepo;
    private readonly IRepository<HaravanStore> _haravanStoreRepository;
    private readonly IMerchantUserRepository   _merchantUserRepo;
    private readonly MerchantManager           _merchantManager;
    private readonly UserInfoManager           _userInfoManager;
    private static   string                    _nonce = "abcxyz123";
    public CustomLoginModel(IAuthenticationSchemeProvider      schemeProvider,
                            IOptions<AbpAccountOptions>        accountOptions,
                            IAbpRecaptchaValidatorFactory      recaptchaValidatorFactory,
                            IAccountExternalProviderAppService accountExternalProviderAppService,
                            ICurrentPrincipalAccessor          currentPrincipalAccessor,
                            IOptions<IdentityOptions>          identityOptions,
                            IOptionsSnapshot<reCAPTCHAOptions> reCaptchaOptions,
                            IMerchantRepository                merchantRepo,
                            IIdentityUserRepository            identityUserRepository,
                            ITenantRepository                  tenantRepository,
                            ITenantManager                     tenantManager,
                            IDataFilter                        dataFilter,
                            IMerchantStoreRepository           merchantStoreRepo,
                            IRepository<HaravanStore>          haravanStoreRepository,
                            IMerchantUserRepository            merchantUserRepo,
                            MerchantManager                    merchantManager,
                            UserInfoManager                    userInfoManager) : base(schemeProvider,
                                                                                       accountOptions,
                                                                                       recaptchaValidatorFactory,
                                                                                       accountExternalProviderAppService,
                                                                                       currentPrincipalAccessor,
                                                                                       identityOptions,
                                                                                       reCaptchaOptions)
    {
        _merchantRepo           = merchantRepo;
        _merchantStoreRepo      = merchantStoreRepo;
        _haravanStoreRepository = haravanStoreRepository;
        _merchantUserRepo       = merchantUserRepo;
        _merchantManager        = merchantManager;
        _userInfoManager   = userInfoManager;
    }

    public override async Task<IActionResult> OnGetAsync()
    {
        var provider = "Haravan";
        if (!ReturnUrl.IsNullOrEmpty() && ReturnUrl.Contains("HRVLoginRaw"))
        {
            var redirectUrl = Url.Page("./Login",
                                       pageHandler: "CustomExternalLoginCallback",
                                       values: new { ReturnUrl, ReturnUrlHash, Provider = provider });

            var properties = SignInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            properties.Parameters.Add("nonce", _nonce);
            properties.Items["scheme"] = provider;

            return await Task.FromResult(Challenge(properties, provider));
        }

        return await base.OnGetAsync();
    }

    [UnitOfWork] public override async Task<IActionResult> OnPostExternalLogin(string provider)
    {
        var redirectUrl = Url.Page("./Login", pageHandler: "ExternalLoginCallback", values: new { ReturnUrl, ReturnUrlHash });
        if (provider == "Haravan")
        {
            redirectUrl = Url.Page("./Login",
                                   pageHandler: "CustomExternalLoginCallback",
                                   values: new { ReturnUrl, ReturnUrlHash, Provider = provider });
        }

        var properties = SignInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
        properties.Parameters.Add("nonce", _nonce);
        properties.Items["scheme"] = provider;

        return await Task.FromResult(Challenge(properties, provider));
    }

    [UnitOfWork] public async Task<IActionResult> OnGetCustomExternalLoginCallbackAsync(string returnUrl     = "",
                                                                                        string returnUrlHash = "",
                                                                                        string provider      = "Haravan")
    {
        // read external identity from the temporary cookie
        var resultAuth = await HttpContext.AuthenticateAsync(provider);
        if (resultAuth?.Succeeded != true)
        {
            throw new Exception("External authentication error");
        }

        // retrieve claims of the external user
        var resultAuthPrincipal = resultAuth.Principal;
        if (resultAuthPrincipal == null)
        {
            throw new Exception("External authentication error");
        }

        // retrieve claims of the external user
        var claims = resultAuthPrincipal.Claims.ToList();
        if (claims.All(x => x.Value != "admin"))
        {
            Alerts.Danger("User is not admin store");
        }

        var orgId = claims.FirstOrDefault(x => x.Type == "orgid")?.Value;

        // try to determine the unique id of the external user - the most common claim type for that are the sub claim and the NameIdentifier
        // depending on the external provider, some other claim type might be used
        var userIdClaim = claims.FirstOrDefault(x => x.Type == JwtClaimTypes.Subject);
        if (userIdClaim == null)
        {
            userIdClaim = claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);
        }

        if (userIdClaim == null)
        {
            throw new Exception("Unknown userid");
        }

        var externalUserId = userIdClaim.Value;
        var loginInfo      = new ExternalLoginInfo(resultAuthPrincipal, provider, externalUserId, externalUserId);

        //TODO: Handle other cases for result!

        var email = loginInfo.Principal.FindFirstValue(AbpClaimTypes.Email);
        if (email.IsNullOrWhiteSpace())
        {
            return RedirectToPage("./Register",
                                  new { IsExternalLogin = true, ExternalLoginAuthSchema = loginInfo.LoginProvider, ReturnUrl = returnUrl });
        }

        var externalUser = await UserManager.FindByEmailAsync(email);
        if (externalUser == null)
        {
            externalUser = await CreateExternalUserAsync(loginInfo);
        }
        else
        {
            if (await UserManager.FindByLoginAsync(loginInfo.LoginProvider, loginInfo.ProviderKey) == null)
            {
                CheckIdentityErrors(await UserManager.AddLoginAsync(externalUser, loginInfo));
            }
        }

        if (await HasRequiredIdentitySettings())
        {
            Logger.LogWarning($"New external user is created but confirmation is required!");

            await StoreConfirmUser(externalUser);
            return RedirectToPage("./ConfirmUser", new { returnUrl = ReturnUrl, returnUrlHash = ReturnUrlHash });
        }

        await SignInManager.SignInAsync(externalUser, false);

        await _userInfoManager.InitUserInfo(externalUser.Id);

        var merchantUser = (await _merchantUserRepo.GetQueryableAsync()).FirstOrDefault(x => x.AppUserId == externalUser.Id);
        if (merchantUser != null)
        {
            return RedirectSafely(returnUrl, returnUrlHash);
        }

        var redirectUrl = Url.Page("./Login", pageHandler: "HaravanGrantCallback", values: new { ReturnUrl, ReturnUrlHash, Provider = provider });
        var properties  = SignInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
        properties.SetParameter("com.read_shippings",      true);
        properties.SetParameter("com.read_orders",         true);
        properties.SetParameter("com.read_shipping_zones", true);
        properties.SetParameter("com.read_customers",      true);
        properties.SetParameter("com.read_shop",           true);
        
        properties.SetParameter("wh_api",           true);
        properties.SetParameter("grant_service",           true);
        properties.SetParameter("offline_access",          true);
        properties.Parameters.Add("nonce", _nonce);
        properties.Parameters.Add("orgid",orgId);
        properties.Items["scheme"] = provider;

        return await Task.FromResult(Challenge(properties, provider));
    }

    /// <summary>
    /// The callback after Haravan grants the permission, user will be redirected to LookOn backoffice and the pre-process for registration goes on
    /// </summary>
    /// <param name="returnUrl"></param>
    /// <param name="returnUrlHash"></param>
    /// <param name="provider"></param>
    /// <returns></returns>
    [UnitOfWork] public async Task<IActionResult> OnGetHaravanGrantCallbackAsync(string returnUrl     = "",
                                                                                 string returnUrlHash = "",
                                                                                 string provider      = "Haravan")
    {
        var resultAuth = await HttpContext.AuthenticateAsync(provider);

        if (CurrentUser.IsAuthenticated && CurrentUser.Id.HasValue && resultAuth.Properties != null)
        {
            var accessToken     = resultAuth.Properties.GetTokens().FirstOrDefault(m => m.Name == "access_token")?.Value;
            var refreshToken = resultAuth.Properties.GetTokens().FirstOrDefault(m => m.Name == "refresh_token")?.Value;
            if (accessToken == null || refreshToken == null)
            {
                return NotFound();
            }

            var shopInfo = await HaravanApiConsumer.GetShopInfo(accessToken);
            if (shopInfo?.HrvShopRaw == null) return RedirectSafely(returnUrl, returnUrlHash);
            // if (CurrentUser?.Id is null ) return RedirectSafely(returnUrl,     returnUrlHash);
            
            await _merchantManager.InitMerchant(shopInfo.HrvShopRaw.Email,
                                                CurrentUser.Id.Value,
                                                shopInfo.HrvShopRaw,
                                                accessToken,
                                                refreshToken);
            
            //Regist Webhook
            await HaravanApiConsumer.SubscribeWebhook(accessToken);
        }

        return RedirectSafely(returnUrl, returnUrlHash);
    }
}