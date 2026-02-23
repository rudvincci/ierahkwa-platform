using Mamey.FWID.Identities.Application.AML.Models;

namespace Mamey.FWID.Identities.Application.AML.Services;

/// <summary>
/// Interface for Politically Exposed Persons (PEP) detection service.
/// </summary>
public interface IPEPDetectionService
{
    /// <summary>
    /// Screens an identity for PEP status.
    /// </summary>
    Task<ScreeningResult> ScreenForPEPAsync(
        PEPScreeningRequest request,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets PEP record by ID.
    /// </summary>
    Task<PEPRecord?> GetPEPRecordAsync(
        Guid recordId,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Searches PEP database.
    /// </summary>
    Task<IReadOnlyList<PEPRecord>> SearchPEPDatabaseAsync(
        string searchTerm,
        int limit = 20,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets cached PEP screening result.
    /// </summary>
    Task<ScreeningResult?> GetCachedResultAsync(
        Guid identityId,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Updates PEP database from sources.
    /// </summary>
    Task<int> UpdatePEPDatabaseAsync(
        CancellationToken cancellationToken = default);
}

/// <summary>
/// Request for PEP screening.
/// </summary>
public class PEPScreeningRequest
{
    public Guid IdentityId { get; set; }
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public DateTime? DateOfBirth { get; set; }
    public List<string> Nationalities { get; set; } = new();
    public string? CurrentPosition { get; set; }
    public string? Organization { get; set; }
    public bool IncludeRelatives { get; set; } = true;
    public bool UseCache { get; set; } = true;
}
