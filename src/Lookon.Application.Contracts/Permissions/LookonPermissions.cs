namespace LookOn.Permissions;

public static class LookOnPermissions
{
    public const string GroupName = "LookOn";

    public static class Dashboard
    {
        public const string DashboardGroup = GroupName      + ".Dashboard";
        public const string Host           = DashboardGroup + ".Host";
        public const string Tenant         = DashboardGroup + ".Tenant";
    }

    public class Merchants
    {
        public const string Default    = GroupName + ".Merchants";
        public const string Edit       = Default   + ".Edit";
        public const string Create     = Default   + ".Create";
        public const string Delete     = Default   + ".Delete";
        public const string ManageInfo = Default   + ".ManageInfo";
        public const string View       = Default   + ".View";
        public const string ManageView       = Default   + ".ManageView";
    }

    public class Subscriptions
    {
        public const string Default = GroupName + ".Subscriptions";
        public const string Edit    = Default   + ".Edit";
        public const string Create  = Default   + ".Create";
        public const string Delete  = Default   + ".Delete";
        public const string View    = Default   + ".View";
    }

    public class Categories
    {
        public const string Default = GroupName + ".Categories";
        public const string Edit    = Default   + ".Edit";
        public const string Create  = Default   + ".Create";
        public const string Delete  = Default   + ".Delete";
        public const string View    = Default   + ".View";
    }

    public class UserInfos
    {
        public const string Default         = GroupName + ".UserInfos";
        public const string Edit            = Default   + ".Edit";
        public const string Create          = Default   + ".Create";
        public const string Delete          = Default   + ".Delete";
        public const string View            = Default   + ".View";
        public const string ViewAccountInfo = Default   + ".ViewAccountInfo";
    }

    public class Platforms
    {
        public const string Default = GroupName + ".Platforms";
        public const string Edit    = Default   + ".Edit";
        public const string Create  = Default   + ".Create";
        public const string Delete  = Default   + ".Delete";
        public const string View    = Default   + ".View";
    }

    public class MerchantStores
    {
        public const string Default = GroupName + ".MerchantStores";
        public const string Edit    = Default   + ".Edit";
        public const string Create  = Default   + ".Create";
        public const string Delete  = Default   + ".Delete";
        public const string View    = Default   + ".View";
    }

    public class MerchantSubscriptions
    {
        public const string Default    = GroupName + ".MerchantSubscriptions";
        public const string Edit       = Default   + ".Edit";
        public const string Create     = Default   + ".Create";
        public const string Delete     = Default   + ".Delete";
        public const string ManageInfo = Default   + ".ManageInfo";
        public const string View       = Default   + ".View";
    }

    public class Insight
    {
        public const string Default = GroupName + ".Insight";
        public const string View    = Default   + ".View";
    }
    
    public class Page1
    {
        public const string Default = GroupName + ".Page1";
        public const string View    = Default   + ".View";
    }

    public class Page2
    {
        public const string Default = GroupName + ".Page2";
        public const string View    = Default   + ".View";
    }

    public class Page3
    {
        public const string Default = GroupName + ".Page3";
        public const string View    = Default   + ".View";
    }

    public class Page4
    {
        public const string Default = GroupName + ".Page4";
        public const string View    = Default   + ".View";
    }

    public class Page5
    {
        public const string Default = GroupName + ".Page5";
        public const string View    = Default   + ".View";
    }

    public class MerchantStaffs
    {
        public const string Default     = GroupName + ".MerchantStaffs";
        public const string Edit        = Default   + ".Edit";
        public const string Create      = Default   + ".Create";
        public const string Delete      = Default   + ".Delete";
        public const string ManageStaff = Default   + ".ManageStaff";
        public const string View        = Default   + ".View";
    }

    public class MerchantSyncInfos
    {
        public const string Default = GroupName + ".MerchantSyncInfos";
        public const string Edit    = Default   + ".Edit";
        public const string Create  = Default   + ".Create";
        public const string Delete  = Default   + ".Delete";
        public const string View    = Default   + ".View";
    }

    public class MerchantSocialCommunities
    {
        public const string Default = GroupName + ".MerchantSocialCommunities";
        public const string Edit    = Default   + ".Edit";
        public const string Create  = Default   + ".Create";
        public const string Delete  = Default   + ".Delete";
        public const string View    = Default   + ".View";
    }
    
    public class Support
    {
        public const string Default = GroupName + ".Support";
        public const string View    = Default   + ".View";
    }
}