namespace Mamey.Types;

public enum OrganizationType
{
    [Display(Name="SCNRFP Incorporated Company")]
    INKGIncorporatedCompany,
    [Display(Name="Sovereign Akwesasne Government (SAG)")]
    SAGCompany,
    [Display(Name="Sole Proprietor")]
    SoleProprietor,
    [Display(Name = "Single Member LLC")]
    SingleMemberLLC,
    [Display(Name = "Limited Liability Company")]
    LimitedLiabilityCompany,
    [Display(Name = "General Partnership")]
    GeneralPartnership,
    [Display(Name = "Unlisted-Corporation")]
    UnlistedCorporation,
    [Display(Name = "Publicly-Traded-Corporation")]
    PubliclyTradedCorporation,
    [Display(Name = "Association")]
    Association,
    [Display(Name = "Non-Profit")]
    NonProfit,
    [Display(Name = "Government-Organization")]
    GovernmentOrganization,
    [Display(Name = "Revocable Trust")]
    RevocableTrust,
    [Display(Name = "Irrevocable-Trust")]
    IrrevocableTrust,
    [Display(Name = "Estate")]
    Estate,
    [Display(Name = "Limited-Partnership")]
    LimitedPartnership,
    [Display(Name = "Limited-Liability-Partnership")]
    LimitedLiabilityPartnership,
    [Display(Name = "Limited-Liability-Confederacy")]
    LimitedLiabilityConfederacy
}
