namespace Mamey.Modules;

[Flags]
public enum MameyAppModuleType : long
{
    [Description("Bank")]
    Bank = 1 << 0,
    [Description("Marketplace")]
    Marketplace = 1 << 1,
    [Description("Prison Management")]
    PrisonManagement = 1 << 2,
    [Description("Real Estate")]
    RealEstate = 1 << 3,
    [Description("Restaurant")]
    Restaurant = 1 << 4,
    [Description("Hospital")]
    Hospital = 1 << 5,
    [Description("Wallet")]
    Wallet = 1 << 6,
    [Description("Point of Sale (POS)")]
    PointOfSale = 1 << 7,
    [Description("Automated Teller Machine (ATM)")]
    ATM,
    [Description("Customer Relationship Management (CRM)")]
    CustomerRelationshipManagement,
    [Description("Education")]
    Education,
    [Description("Human Resources")]
    HumanResources,
    Campaign,
    [Description("Supply Chain Management")]
    SupplyChain,
    [Description("Accounting")]
    Accounting,
    [Description("Call Center")]
    CallCenter,
    [Description("Help Desk")]
    HelpDesk,
    [Description("Project Management")]
    ProjectManagement
}
