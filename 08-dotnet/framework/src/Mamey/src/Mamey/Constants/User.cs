namespace Mamey.Constants.User;


/// <summary>
/// Every permission should specify which environment it is applicable to.
/// When determining which permission applies to which environment, DENY BY DEFAULT will be used.
/// </summary>
[Flags]
public enum Permission : long
{
    [Description("No permissions assigned")]
    //[SystemEnvironmentAllowed(SystemEnvironment.All)]
    None = 0,

    [Description("Create pages through the docking framework")]
    //[SystemEnvironmentAllowed(SystemEnvironment.RD | SystemEnvironment.Staging)]
    PageDesigner = 1 << 0,

    ManageAll =  1 << 1,

    // 1 << 2 - is available...

    [Description("Manage Poll")]
    //[SystemEnvironmentAllowed(SystemEnvironment.All)]
    ManagePoll = 1 << 3,

    [Description("Manage Products")]
    //[SystemEnvironmentAllowed(SystemEnvironment.RD | SystemEnvironment.Staging)]
    ManageProducts = 1 << 4,

    [Description("Manage Experts")]
    //[SystemEnvironmentAllowed(SystemEnvironment.RD | SystemEnvironment.Staging)]
    ManageExperts = 1 << 5,

    [Description("Manage Organization Tags")]
    //[SystemEnvironmentAllowed(SystemEnvironment.RD | SystemEnvironment.Staging)]
    ManageOrganizationTags = 1 << 6,

    //[Description("Manage QR Articles")]
    //[SystemEnvironmentAllowed(SystemEnvironment.RD | SystemEnvironment.Staging)]
    //            ManageQRArticles = 1 << 7,

    //NOTE: 1 << 7 is available!!

    [Description("Manage Enrollment Codes")]
    //[SystemEnvironmentAllowed(SystemEnvironment.All)]
    ManageEnrollmentCodes = 1 << 8,

    [Description("Manage Video Library")]
    //[SystemEnvironmentAllowed(SystemEnvironment.RD | SystemEnvironment.Staging)]
    ManageVideoLibrary = 1 << 9,

    [Description("Assignment Administrator")]
    //[SystemEnvironmentAllowed(SystemEnvironment.All)]
    AssignmentAdmin = 1 << 10,

    [Description("System Notification")]
    //[SystemEnvironmentAllowed(SystemEnvironment.All)]
    SystemNotification = 1 << 11,

    [Description("Organization Notification")]
    //[SystemEnvironmentAllowed(SystemEnvironment.All)]
    OrganizationNotification = 1 << 12,

    [Description("Manage Organization Reports")]
    //[SystemEnvironmentAllowed(SystemEnvironment.All)]
    ReportAdmin = 1 << 13,

    [Description("Manage Learners within Organization")]
    //[SystemEnvironmentAllowed(SystemEnvironment.All)]
    ManageLearners = 1 << 14,

    [Description("Manage Learning Product Sections")]
    //[SystemEnvironmentAllowed(SystemEnvironment.All)]
    ManageLearningProductSection = 1 << 15,

    [Description("Manage Credit Profiles")]
    //[SystemEnvironmentAllowed(SystemEnvironment.RD | SystemEnvironment.Staging)]
    ManageCreditProfiles = 1 << 16,

    [Description("Manage LTI Tool Providers")]
    //[SystemEnvironmentAllowed(SystemEnvironment.All)]
    ManageLTIToolProviders = 1 << 17,

    [Description("Manage Subscription Definitions")]
    //[SystemEnvironmentAllowed(SystemEnvironment.All)]
    ManageSubscriptionDefinitions = 1 << 18,

    [Description("Manage Subsidiary Enrollment Codes")]
    //[SystemEnvironmentAllowed(SystemEnvironment.RD | SystemEnvironment.RDDeployTest | SystemEnvironment.Live)]
    ManageSubsidiaryEnrollmentCodes = 1 << 19,

    [Description("Manage Subsidiaries")]
    //[SystemEnvironmentAllowed(SystemEnvironment.All)]
    ManageSubsidiaries = 1 << 20,

    [Description("Manage Badges")]
    //[SystemEnvironmentAllowed(SystemEnvironment.RD | SystemEnvironment.Staging)]
    ManageBadges = 1 << 21,

    [Description("Manage Communities")]
    //[SystemEnvironmentAllowed(SystemEnvironment.Staging | SystemEnvironment.RD | SystemEnvironment.RDDeployTest)]
    ManageCommunities = 1 << 22
}

/// <summary>
/// These types are the MemberTypeId's that are associated with a member
/// </summary>
[Flags]
public enum AdminType : byte
{
    [Description("None")]
    None = 0,
    [Description("Admin")]
    Admin = 1 << 0,
    [Description("Group 5 Administrator")]
    Group5Administrator = 1 << 1,
    [Description("Group 4 Administrator")]
    Group4Administrator = 1 << 2,
    [Description("Group 3 Administrator")]
    Group3Administrator = 1 << 3,
    [Description("Group 2 Administrator")]
    Group2Administrator = 1 << 4,
    [Description("Group 1 Administrator")]
    Group1Administrator = 1 << 5,
    [Description("Organization Administrator")]
    OrganizationAdministrator = 1 << 6,
    [Description("Master Administrator")]
    MasterAdministrator = 1 << 7,
    [Description("Group Administrator")]
    GroupAdmin = Group5Administrator | Group4Administrator | Group3Administrator | Group2Administrator | Group1Administrator,
    [Description("All Admins")]
    AllAdmins = Admin | Group5Administrator | Group4Administrator | Group3Administrator | Group2Administrator | Group1Administrator | OrganizationAdministrator | MasterAdministrator
}

[Flags]
public enum RegistrationType : byte
{
    [Description("Internal registration")]
    Internal = 1 << 0,

    [Description("Admin registration")]
    Admin = 1 << 1,

    [Description("External registration")]
    External = 1 << 2,

    [Description("Self registration")]
    Self = 1 << 3
}

public enum Status : byte
{
    [Description("Active")]
    Active = 1,

    [Description("Deleted")]
    Deleted = 2,

    [Description("Placeholder from another environment")]
    Placeholder = 3
}

