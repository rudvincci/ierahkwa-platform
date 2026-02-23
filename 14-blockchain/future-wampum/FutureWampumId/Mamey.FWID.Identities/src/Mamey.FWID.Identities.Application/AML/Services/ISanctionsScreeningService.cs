using Mamey.FWID.Identities.Application.AML.Models;

namespace Mamey.FWID.Identities.Application.AML.Services;

/// <summary>
/// Interface for sanctions screening service.
/// </summary>
public interface ISanctionsScreeningService
{
    /// <summary>
    /// Screens an identity against sanctions lists.
    /// </summary>
    Task<ScreeningResult> ScreenIdentityAsync(
        SanctionsScreeningRequest request,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Screens a name against sanctions lists.
    /// </summary>
    Task<ScreeningResult> ScreenNameAsync(
        string firstName,
        string lastName,
        DateTime? dateOfBirth = null,
        string? nationality = null,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets cached screening result for an identity.
    /// </summary>
    Task<ScreeningResult?> GetCachedResultAsync(
        Guid identityId,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Invalidates cached screening result.
    /// </summary>
    Task InvalidateCacheAsync(
        Guid identityId,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets available sanctions lists.
    /// </summary>
    Task<IReadOnlyList<SanctionsList>> GetSanctionsListsAsync(
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Triggers update of sanctions lists.
    /// </summary>
    Task<int> UpdateSanctionsListsAsync(
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Marks a match as false positive.
    /// </summary>
    Task<bool> MarkAsFalsePositiveAsync(
        Guid screeningId,
        Guid matchId,
        Guid reviewerId,
        string notes,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Confirms a match.
    /// </summary>
    Task<bool> ConfirmMatchAsync(
        Guid screeningId,
        Guid matchId,
        Guid reviewerId,
        string notes,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// Request for sanctions screening.
/// </summary>
public class SanctionsScreeningRequest
{
    public Guid IdentityId { get; set; }
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string? MiddleName { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? PlaceOfBirth { get; set; }
    public List<string> Nationalities { get; set; } = new();
    public List<string>? Aliases { get; set; }
    public string? PassportNumber { get; set; }
    public string? NationalId { get; set; }
    public bool UseCache { get; set; } = true;
    public List<string>? SpecificLists { get; set; }
}
