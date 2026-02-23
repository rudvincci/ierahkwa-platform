namespace Mamey.FWID.Identities.Application.AML.Models;

/// <summary>
/// Politically Exposed Person (PEP) record.
/// </summary>
public class PEPRecord
{
    /// <summary>
    /// Record ID.
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();
    
    /// <summary>
    /// Source database.
    /// </summary>
    public string Source { get; set; } = null!;
    
    /// <summary>
    /// Original ID from source.
    /// </summary>
    public string SourceId { get; set; } = null!;
    
    /// <summary>
    /// Primary name.
    /// </summary>
    public string PrimaryName { get; set; } = null!;
    
    /// <summary>
    /// Name variations.
    /// </summary>
    public List<string> Aliases { get; set; } = new();
    
    /// <summary>
    /// Date of birth.
    /// </summary>
    public DateTime? DateOfBirth { get; set; }
    
    /// <summary>
    /// Nationalities.
    /// </summary>
    public List<string> Nationalities { get; set; } = new();
    
    /// <summary>
    /// PEP category.
    /// </summary>
    public PEPCategory Category { get; set; }
    
    /// <summary>
    /// PEP tier/level.
    /// </summary>
    public PEPTier Tier { get; set; }
    
    /// <summary>
    /// Current or former positions.
    /// </summary>
    public List<PEPPosition> Positions { get; set; } = new();
    
    /// <summary>
    /// Related persons (family, close associates).
    /// </summary>
    public List<PEPRelation> Relations { get; set; } = new();
    
    /// <summary>
    /// Whether currently in office.
    /// </summary>
    public bool IsActive { get; set; }
    
    /// <summary>
    /// When left office (if former PEP).
    /// </summary>
    public DateTime? InactiveDate { get; set; }
    
    /// <summary>
    /// Risk level.
    /// </summary>
    public PEPRiskLevel RiskLevel { get; set; }
    
    /// <summary>
    /// Additional notes.
    /// </summary>
    public string? Notes { get; set; }
    
    /// <summary>
    /// When last updated.
    /// </summary>
    public DateTime LastUpdated { get; set; }
}

/// <summary>
/// PEP category.
/// </summary>
public enum PEPCategory
{
    /// <summary>
    /// Head of state, government, senior politicians.
    /// </summary>
    ForeignPEP = 1,
    
    /// <summary>
    /// Domestic political figures.
    /// </summary>
    DomesticPEP = 2,
    
    /// <summary>
    /// Senior officials of international organizations.
    /// </summary>
    InternationalOrganization = 3,
    
    /// <summary>
    /// Family members of PEPs.
    /// </summary>
    FamilyMember = 4,
    
    /// <summary>
    /// Close associates of PEPs.
    /// </summary>
    CloseAssociate = 5,
    
    /// <summary>
    /// State-owned enterprise executive.
    /// </summary>
    StateOwnedEnterprise = 6
}

/// <summary>
/// PEP tier/level.
/// </summary>
public enum PEPTier
{
    /// <summary>
    /// Heads of state, cabinet members, supreme court justices.
    /// </summary>
    Tier1 = 1,
    
    /// <summary>
    /// Senior officials, legislators, political party leaders.
    /// </summary>
    Tier2 = 2,
    
    /// <summary>
    /// Mid-level officials, local government leaders.
    /// </summary>
    Tier3 = 3,
    
    /// <summary>
    /// Family and close associates.
    /// </summary>
    Tier4 = 4
}

/// <summary>
/// PEP risk level.
/// </summary>
public enum PEPRiskLevel
{
    Low = 1,
    Medium = 2,
    High = 3,
    VeryHigh = 4
}

/// <summary>
/// PEP position.
/// </summary>
public class PEPPosition
{
    public string Title { get; set; } = null!;
    public string? Organization { get; set; }
    public string Country { get; set; } = null!;
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool IsCurrent { get; set; }
}

/// <summary>
/// Related person to PEP.
/// </summary>
public class PEPRelation
{
    public string Name { get; set; } = null!;
    public PEPRelationType RelationType { get; set; }
    public string? RelatedPEPId { get; set; }
}

/// <summary>
/// Type of PEP relation.
/// </summary>
public enum PEPRelationType
{
    Spouse = 1,
    Child = 2,
    Parent = 3,
    Sibling = 4,
    BusinessPartner = 5,
    CloseAssociate = 6
}
