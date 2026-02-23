namespace Mamey.FWID.Identities.Application.AML.Models;

/// <summary>
/// Represents a sanctions list source.
/// </summary>
public class SanctionsList
{
    /// <summary>
    /// List identifier.
    /// </summary>
    public string ListId { get; set; } = null!;
    
    /// <summary>
    /// Source name (OFAC, UN, EU, etc.).
    /// </summary>
    public string Source { get; set; } = null!;
    
    /// <summary>
    /// Full name of the list.
    /// </summary>
    public string Name { get; set; } = null!;
    
    /// <summary>
    /// List description.
    /// </summary>
    public string? Description { get; set; }
    
    /// <summary>
    /// URL for list updates.
    /// </summary>
    public string? UpdateUrl { get; set; }
    
    /// <summary>
    /// Number of entries in the list.
    /// </summary>
    public int EntryCount { get; set; }
    
    /// <summary>
    /// When the list was last updated.
    /// </summary>
    public DateTime LastUpdated { get; set; }
    
    /// <summary>
    /// When the list data expires.
    /// </summary>
    public DateTime ExpiresAt { get; set; }
    
    /// <summary>
    /// Update frequency in hours.
    /// </summary>
    public int UpdateFrequencyHours { get; set; } = 24;
    
    /// <summary>
    /// Whether the list is currently active.
    /// </summary>
    public bool IsActive { get; set; } = true;
}

/// <summary>
/// Entry in a sanctions list.
/// </summary>
public class SanctionsEntry
{
    /// <summary>
    /// Entry ID.
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();
    
    /// <summary>
    /// Source list ID.
    /// </summary>
    public string ListId { get; set; } = null!;
    
    /// <summary>
    /// Original ID from source.
    /// </summary>
    public string SourceId { get; set; } = null!;
    
    /// <summary>
    /// Primary name.
    /// </summary>
    public string PrimaryName { get; set; } = null!;
    
    /// <summary>
    /// Name variations and aliases.
    /// </summary>
    public List<string> Aliases { get; set; } = new();
    
    /// <summary>
    /// Entity type.
    /// </summary>
    public SanctionsEntityType EntityType { get; set; }
    
    /// <summary>
    /// Date of birth (for individuals).
    /// </summary>
    public DateTime? DateOfBirth { get; set; }
    
    /// <summary>
    /// Place of birth.
    /// </summary>
    public string? PlaceOfBirth { get; set; }
    
    /// <summary>
    /// Nationalities.
    /// </summary>
    public List<string> Nationalities { get; set; } = new();
    
    /// <summary>
    /// Identification documents.
    /// </summary>
    public List<SanctionsIdentification> Identifications { get; set; } = new();
    
    /// <summary>
    /// Addresses.
    /// </summary>
    public List<SanctionsAddress> Addresses { get; set; } = new();
    
    /// <summary>
    /// Sanctions programs/categories.
    /// </summary>
    public List<string> Programs { get; set; } = new();
    
    /// <summary>
    /// Reason for listing.
    /// </summary>
    public string? Remarks { get; set; }
    
    /// <summary>
    /// When listed.
    /// </summary>
    public DateTime? ListedDate { get; set; }
    
    /// <summary>
    /// When last updated.
    /// </summary>
    public DateTime LastUpdated { get; set; }
    
    /// <summary>
    /// Search tokens (normalized names for matching).
    /// </summary>
    public List<string> SearchTokens { get; set; } = new();
}

/// <summary>
/// Type of sanctioned entity.
/// </summary>
public enum SanctionsEntityType
{
    Individual = 1,
    Entity = 2,
    Vessel = 3,
    Aircraft = 4
}

/// <summary>
/// Identification document in sanctions entry.
/// </summary>
public class SanctionsIdentification
{
    public string Type { get; set; } = null!;
    public string Number { get; set; } = null!;
    public string? Country { get; set; }
    public DateTime? IssueDate { get; set; }
    public DateTime? ExpiryDate { get; set; }
}

/// <summary>
/// Address in sanctions entry.
/// </summary>
public class SanctionsAddress
{
    public string? Street { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? Country { get; set; }
    public string? PostalCode { get; set; }
}
