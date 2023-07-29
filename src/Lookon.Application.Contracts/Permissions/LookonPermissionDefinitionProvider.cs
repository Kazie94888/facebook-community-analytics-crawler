using LookOn.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;
using Volo.Abp.MultiTenancy;

namespace LookOn.Permissions;

public class LookOnPermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var lookonGroup = context.AddGroup(LookOnPermissions.GroupName);

        lookonGroup.AddPermission(LookOnPermissions.Dashboard.Host, L("Permission:Dashboard"), MultiTenancySides.Host);
        lookonGroup.AddPermission(LookOnPermissions.Dashboard.Tenant, L("Permission:Dashboard"), MultiTenancySides.Tenant);

        var merchantPermission = lookonGroup.AddPermission(LookOnPermissions.Merchants.Default, L("Permission:Merchants"));
        merchantPermission.AddChild(LookOnPermissions.Merchants.Create,     L("Permission:Create"));
        merchantPermission.AddChild(LookOnPermissions.Merchants.Edit,       L("Permission:Edit"));
        merchantPermission.AddChild(LookOnPermissions.Merchants.Delete,     L("Permission:Delete"));
        merchantPermission.AddChild(LookOnPermissions.Merchants.ManageInfo, L("Permission:ManageInfo"));
        merchantPermission.AddChild(LookOnPermissions.Merchants.View,       L("Permission:View"));
        merchantPermission.AddChild(LookOnPermissions.Merchants.ManageView, L("Permission:ManageView"));

        var merchantStorePermission = lookonGroup.AddPermission(LookOnPermissions.MerchantStores.Default, L("Permission:MerchantStores"));
        merchantStorePermission.AddChild(LookOnPermissions.MerchantStores.Create, L("Permission:Create"));
        merchantStorePermission.AddChild(LookOnPermissions.MerchantStores.Edit,   L("Permission:Edit"));
        merchantStorePermission.AddChild(LookOnPermissions.MerchantStores.Delete, L("Permission:Delete"));
        merchantStorePermission.AddChild(LookOnPermissions.MerchantStores.View,   L("Permission:View"));

        var merchantSyncInfoPermission = lookonGroup.AddPermission(LookOnPermissions.MerchantSyncInfos.Default, L("Permission:MerchantSyncInfos"));
        merchantSyncInfoPermission.AddChild(LookOnPermissions.MerchantSyncInfos.Create, L("Permission:Create"));
        merchantSyncInfoPermission.AddChild(LookOnPermissions.MerchantSyncInfos.Edit,   L("Permission:Edit"));
        merchantSyncInfoPermission.AddChild(LookOnPermissions.MerchantSyncInfos.Delete, L("Permission:Delete"));
        merchantSyncInfoPermission.AddChild(LookOnPermissions.MerchantSyncInfos.View,   L("Permission:View"));
        
        var merchantSocialCommunityPermission = lookonGroup.AddPermission(LookOnPermissions.MerchantSocialCommunities.Default, L("Permission:MerchantSocialCommunities"));
        merchantSocialCommunityPermission.AddChild(LookOnPermissions.MerchantSocialCommunities.Create, L("Permission:Create"));
        merchantSocialCommunityPermission.AddChild(LookOnPermissions.MerchantSocialCommunities.Edit,   L("Permission:Edit"));
        merchantSocialCommunityPermission.AddChild(LookOnPermissions.MerchantSocialCommunities.Delete, L("Permission:Delete"));
        merchantSocialCommunityPermission.AddChild(LookOnPermissions.MerchantSocialCommunities.View,   L("Permission:View"));
        
        var subscriptionPermission = lookonGroup.AddPermission(LookOnPermissions.Subscriptions.Default, L("Permission:Subscriptions"));
        subscriptionPermission.AddChild(LookOnPermissions.Subscriptions.Create, L("Permission:Create"));
        subscriptionPermission.AddChild(LookOnPermissions.Subscriptions.Edit,   L("Permission:Edit"));
        subscriptionPermission.AddChild(LookOnPermissions.Subscriptions.Delete, L("Permission:Delete"));
        subscriptionPermission.AddChild(LookOnPermissions.Subscriptions.View,   L("Permission:View"));
        
        var categoryPermission = lookonGroup.AddPermission(LookOnPermissions.Categories.Default, L("Permission:Categories"));
        categoryPermission.AddChild(LookOnPermissions.Categories.Create, L("Permission:Create"));
        categoryPermission.AddChild(LookOnPermissions.Categories.Edit,   L("Permission:Edit"));
        categoryPermission.AddChild(LookOnPermissions.Categories.Delete, L("Permission:Delete"));
        categoryPermission.AddChild(LookOnPermissions.Categories.View,   L("Permission:View"));
        
        var platformPermission = lookonGroup.AddPermission(LookOnPermissions.Platforms.Default, L("Permission:Platforms"));
        platformPermission.AddChild(LookOnPermissions.Platforms.Create, L("Permission:Create"));
        platformPermission.AddChild(LookOnPermissions.Platforms.Edit,   L("Permission:Edit"));
        platformPermission.AddChild(LookOnPermissions.Platforms.Delete, L("Permission:Delete"));
        platformPermission.AddChild(LookOnPermissions.Platforms.View,   L("Permission:View"));

        var userInfoPermission = lookonGroup.AddPermission(LookOnPermissions.UserInfos.Default, L("Permission:UserInfos"));
        userInfoPermission.AddChild(LookOnPermissions.UserInfos.Create, L("Permission:Create"));
        userInfoPermission.AddChild(LookOnPermissions.UserInfos.Edit,   L("Permission:Edit"));
        userInfoPermission.AddChild(LookOnPermissions.UserInfos.Delete, L("Permission:Delete"));
        userInfoPermission.AddChild(LookOnPermissions.UserInfos.View,   L("Permission:View"));
        userInfoPermission.AddChild(LookOnPermissions.UserInfos.ViewAccountInfo,   L("Permission:ViewAccountInfo"));

        var merchantSubscriptionPermission = lookonGroup.AddPermission(LookOnPermissions.MerchantSubscriptions.Default, L("Permission:MerchantSubscriptions"));
        merchantSubscriptionPermission.AddChild(LookOnPermissions.MerchantSubscriptions.Create, L("Permission:Create"));
        merchantSubscriptionPermission.AddChild(LookOnPermissions.MerchantSubscriptions.Edit,   L("Permission:Edit"));
        merchantSubscriptionPermission.AddChild(LookOnPermissions.MerchantSubscriptions.Delete, L("Permission:Delete"));
        merchantSubscriptionPermission.AddChild(LookOnPermissions.MerchantSubscriptions.View,   L("Permission:View"));

        // MERCHANT PAGES
        
        var merchantUserPermission = lookonGroup.AddPermission(LookOnPermissions.MerchantStaffs.Default, L("Permission:MerchantStaffs"));
        merchantUserPermission.AddChild(LookOnPermissions.MerchantStaffs.Create,      L("Permission:Create"));
        merchantUserPermission.AddChild(LookOnPermissions.MerchantStaffs.Edit,        L("Permission:Edit"));
        merchantUserPermission.AddChild(LookOnPermissions.MerchantStaffs.Delete,      L("Permission:Delete"));
        merchantUserPermission.AddChild(LookOnPermissions.MerchantStaffs.ManageStaff, L("Permission:ManageStaff"));
        merchantUserPermission.AddChild(LookOnPermissions.MerchantStaffs.View,        L("Permission:View"));
        
        var page1Permission = lookonGroup.AddPermission(LookOnPermissions.Page1.Default, L("Permission:Page1"));
        page1Permission.AddChild(LookOnPermissions.Page1.View, L("Permission:View"));
        
        var page2Permission = lookonGroup.AddPermission(LookOnPermissions.Page2.Default, L("Permission:Page2"));
        page2Permission.AddChild(LookOnPermissions.Page2.View, L("Permission:View"));
        
        var page3Permission = lookonGroup.AddPermission(LookOnPermissions.Page3.Default, L("Permission:Page3"));
        page3Permission.AddChild(LookOnPermissions.Page3.View, L("Permission:View"));
        
        var pageInsightPermission = lookonGroup.AddPermission(LookOnPermissions.Insight.Default, L("Permission:Insight"));
        pageInsightPermission.AddChild(LookOnPermissions.Insight.View, L("Permission:View"));
        
        var page4Permission = lookonGroup.AddPermission(LookOnPermissions.Page4.Default, L("Permission:Page4"));
        page4Permission.AddChild(LookOnPermissions.Page4.View, L("Permission:View"));
        
        var page5Permission = lookonGroup.AddPermission(LookOnPermissions.Page5.Default, L("Permission:Page5"));
        page5Permission.AddChild(LookOnPermissions.Page5.View, L("Permission:View"));
        
        var supportPermission = lookonGroup.AddPermission(LookOnPermissions.Support.Default, L("Permission:Support"));
        supportPermission.AddChild(LookOnPermissions.Support.View, L("Permission:View"));
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<LookOnResource>(name);
    }
}