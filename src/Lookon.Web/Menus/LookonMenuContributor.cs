using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using Localization.Resources.AbpUi;
using LookOn.Core.Extensions;
using LookOn.Core.Shared.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using LookOn.Localization;
using LookOn.Permissions;
using Volo.Abp.Account.Localization;
using Volo.Abp.AuditLogging.Web.Navigation;
using Volo.Abp.Identity.Web.Navigation;
using Volo.Abp.IdentityServer.Web.Navigation;
using Volo.Abp.LanguageManagement.Navigation;
using Volo.Abp.SettingManagement.Web.Navigation;
using Volo.Abp.TextTemplateManagement.Web.Navigation;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.UI.Navigation;
using Volo.Abp.Users;
using Volo.Saas.Host.Navigation;

namespace LookOn.Web.Menus;

public class LookOnMenuContributor : IMenuContributor
{
    private readonly IConfiguration _configuration;

    public LookOnMenuContributor(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task ConfigureMenuAsync(MenuConfigurationContext context)
    {
        if (context.Menu.Name == StandardMenus.Main)
        {
            await ConfigureMainMenuAsync(context);
        }
        else if (context.Menu.Name == StandardMenus.User)
        {
            await ConfigureUserMenuAsync(context);
        }
    }

    private static Task ConfigureMainMenuAsync(MenuConfigurationContext context)
    {
        var l = context.GetLocalizer<LookOnResource>();

        //Home
        // context.Menu.AddItem(new ApplicationMenuItem(LookOnMenus.Home,
        //                                              l["Menu:Home"],
        //                                              "~/",
        //                                              icon: "fa fa-home",
        //                                              order: 1));

        //Tenant Dashboard
        // context.Menu.AddItem(new ApplicationMenuItem(LookOnMenus.TenantDashboard,
        //                                              l["Menu:Dashboard"],
        //                                              "~/Dashboard",
        //                                              icon: "fa fa-line-chart",
        //                                              order: 2).RequirePermissions(LookOnPermissions.Dashboard.Tenant));

        // //Saas
        // context.Menu.SetSubItemOrder(SaasHostMenuNames.GroupName, 3);

        //Administration
        var administration = context.Menu.GetAdministration();
        administration.Order = 5;

        //Administration->Identity
        administration.SetSubItemOrder(IdentityMenuNames.GroupName, 1);

        //Administration->Identity Server
        administration.SetSubItemOrder(AbpIdentityServerMenuNames.GroupName, 2);

        //Administration->Language Management
        administration.SetSubItemOrder(LanguageManagementMenuNames.GroupName, 4);

        //Administration->Text Template Management
        administration.SetSubItemOrder(TextTemplateManagementMainMenuNames.GroupName, 5);

        //Administration->Audit Logs
        administration.SetSubItemOrder(AbpAuditLoggingMainMenuNames.GroupName, 6);

        //Administration->Settings
        administration.SetSubItemOrder(SettingManagementMenuNames.GroupName, 7);

        //Platforms Dashboard
        // context.Menu.AddItem(
        //     new ApplicationMenuItem(
        //         LookOnMenus.PlatformDashboard,
        //         l["Menu:PlatformDashboard"],
        //         "~/platforms",
        //         icon: "fa fa-list-ul",
        //         order: 7
        //     )//.RequirePermissions(LookOnPermissions.Dashboard.Tenant)
        // );

        //Host Dashboard
        // context.Menu.AddItem(new ApplicationMenuItem(LookOnMenus.HostDashboard,
        //                                              l["Menu:Dashboard"],
        //                                              "~/HostDashboard",
        //                                              icon: "fa fa-line-chart", 
        //                                              requiredPermissionName: LookOnPermissions.Dashboard.Host));

        // Transaction Insights
        // context.Menu.AddItem(new ApplicationMenuItem(LookOnMenus.MerchantDashboard,
        //                                              l["Menu:Page1"],
        //                                              url: "~/transaction-insights/timeframe=weekly",
        //                                              icon: "lo-page1-icon",
        //                                              requiredPermissionName: LookOnPermissions.Page1.View));
        //
        // //Social Overview Dashboard
        // context.Menu.AddItem(new ApplicationMenuItem(LookOnMenus.SocialOverviewDashboard,
        //                                              l["Menu:Page2"],
        //                                              url : "/social-overview",
        //                                              icon: "lo-page2-icon",
        //                                              requiredPermissionName: LookOnPermissions.Page2.View));
        //
        // context.Menu.AddItem(new ApplicationMenuItem(LookOnMenus.SegmentInsight,
        //                                              l["Menu:Page3"],
        //                                              "~/segment-insight",
        //                                              icon: "lo-page3-icon",
        //                                              requiredPermissionName: LookOnPermissions.Page3.View));
        
        context.Menu.AddItem(new ApplicationMenuItem(LookOnMenus.Insight,
                                                     l["Menu:Insight"],
                                                     url: "/Insights",
                                                     icon: "lo-page1-icon",
                                                     requiredPermissionName: LookOnPermissions.Insight.View));

        context.Menu.AddItem(new ApplicationMenuItem(LookOnMenus.MerchantConnects,
                                                     l["Menu:MerchantConnects"],
                                                     url: "/merchant-connects",
                                                     icon: "lo-connect-icon",
                                                     requiredPermissionName: LookOnPermissions.Page4.View));

        context.Menu.AddItem(new ApplicationMenuItem(LookOnMenus.MerchantSubscriptionDetails,
                                                     l["Menu:MerchantSubscriptionDetails"],
                                                     url: "/subscription-details",
                                                     icon: "lo-subscription-icon",
                                                     requiredPermissionName: LookOnPermissions.Page5.View));

        var merchantMenu = new ApplicationMenuItem(LookOnMenus.Merchants,
                                                   l["Menu:Merchants"],
                                                   url: "#",
                                                   icon: "lo-merchant-icon",
                                                   requiredPermissionName: LookOnPermissions.Merchants.View);

        merchantMenu.AddItem(new ApplicationMenuItem(LookOnMenus.MerchantManage,
                                                     l["Menu:MerchantManagement"],
                                                     url: "/Merchants/Manage",
                                                     icon: "fa fa-file-alt",
                                                     requiredPermissionName: LookOnPermissions.Merchants.ManageView));

        merchantMenu.AddItem(new ApplicationMenuItem(LookOnMenus.MerchantSubscriptions,
                                                     l["Menu:MerchantSubscriptions"],
                                                     url: "/MerchantSubscriptions",
                                                     icon: "fa fa-file-alt",
                                                     requiredPermissionName: LookOnPermissions.MerchantSubscriptions.View));

        merchantMenu.AddItem(new ApplicationMenuItem(LookOnMenus.MerchantSocialCommunities,
                                                     l["Menu:SocialCommunities"],
                                                     url: "/social-community-manager",
                                                     icon: "fa fa-users",
                                                     requiredPermissionName: LookOnPermissions.MerchantSyncInfos.View));

        merchantMenu.AddItem(new ApplicationMenuItem(LookOnMenus.MerchantSyncInfos,
                                                     l["Menu:MerchantSyncInfos"],
                                                     url: "/MerchantSyncInfos",
                                                     icon: "fa fa-file-alt",
                                                     requiredPermissionName: LookOnPermissions.MerchantSyncInfos.View));
        context.Menu.AddItem(merchantMenu);

        var staffAccountMenu = new ApplicationMenuItem(LookOnMenus.MerchantStaffs,
                                                     l["Menu:MerchantStaffs"],
                                                     url: "/merchant-staffs",
                                                     icon: "lo-account-manage-icon",
                                                     requiredPermissionName: LookOnPermissions.MerchantStaffs.View);
        context.Menu.AddItem(staffAccountMenu);

        var userInfoMenu = new ApplicationMenuItem(LookOnMenus.Merchants,
                                               l["Menu:UserInfos"],
                                               url: "/UserInfos",
                                               icon: "lo-account-icon",
                                               requiredPermissionName: LookOnPermissions.UserInfos.View);

        context.Menu.AddItem(userInfoMenu);
        
        context.Menu.AddItem(new ApplicationMenuItem(LookOnMenus.ManageUserInfo,
                                                     l["Menu:AccountInfo"],
                                                     url: "/account-info",
                                                     icon: "lo-account-icon",
                                                     requiredPermissionName: LookOnPermissions.UserInfos.ViewAccountInfo));
        
        var taxonomyMenuItem = new ApplicationMenuItem(LookOnMenus.Taxonomy,
                                                   l["Menu:Taxonomy"],
                                                   url: "#",
                                                   icon: "fa fa-list-ul",
                                                   requiredPermissionName: LookOnPermissions.Categories.View);

        taxonomyMenuItem.AddItem(new ApplicationMenuItem(LookOnMenus.Categories,
                                                         l["Menu:Categories"],
                                                         url: "/Categories",
                                                         icon: "fa fa-list-ul",
                                                         requiredPermissionName: LookOnPermissions.Categories.View));
        taxonomyMenuItem.AddItem(new ApplicationMenuItem(LookOnMenus.Platforms,
                                                         l["Menu:Platforms"],
                                                         url: "/Platforms",
                                                         icon: "fa fa-laptop",
                                                         requiredPermissionName: LookOnPermissions.Platforms.View));
        context.Menu.AddItem(taxonomyMenuItem);
        
        var support = new ApplicationMenuItem(LookOnMenus.Support,
                                                   l["Menu:Support"],
                                                   url: "/support",
                                                   icon: "lo-phone-icon",
                                                   requiredPermissionName: LookOnPermissions.Support.View);
        context.Menu.AddItem(support);

        return Task.CompletedTask;
    }

    private Task ConfigureUserMenuAsync(MenuConfigurationContext context)
    {
        var identityServerUrl = _configuration["AuthServer:Authority"] ?? "~";
        var uiResource        = context.GetLocalizer<AbpUiResource>();
        var accountResource   = context.GetLocalizer<AccountResource>();

        // context.Menu.AddItem(new ApplicationMenuItem("Account.Manage", accountResource["MyAccount"], $"{identityServerUrl.EnsureEndsWith('/')}Account/Manage", icon: "fa fa-cog", order: 1000, null, "_blank").RequireAuthenticated());
        // context.Menu.AddItem(new ApplicationMenuItem("Account.SecurityLogs", accountResource["MySecurityLogs"], $"{identityServerUrl.EnsureEndsWith('/')}Account/SecurityLogs", target: "_blank").RequireAuthenticated());

        // context.Menu.AddItem(new ApplicationMenuItem("Merchant.Manage", accountResource["MyMerchant"], "/Merchants/Manage", icon: "fa fa-cog", order: 1000).RequireAuthenticated());
        context.Menu.AddItem(new ApplicationMenuItem("Account.Logout",
                                                     uiResource["Logout"],
                                                     url: "~/Account/Logout",
                                                     icon: "fa fa-power-off",
                                                     order: int.MaxValue - 1000).RequireAuthenticated());

        return Task.CompletedTask;
    }
}