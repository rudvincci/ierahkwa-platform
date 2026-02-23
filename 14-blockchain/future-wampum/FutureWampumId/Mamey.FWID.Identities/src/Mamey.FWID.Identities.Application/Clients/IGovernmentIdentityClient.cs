namespace Mamey.FWID.Identities.Application.Clients;

/// <summary>
/// Client interface for Government Identity blockchain operations via MameyNode GovernmentService.
/// 
/// TDD Reference: Line 1594-1703 (Feature Domain 1: Biometric Identity)
/// BDD Reference: Lines 86-112 (I.1-I.3 Executive Summary - Sovereign Identity)
/// </summary>
internal interface IGovernmentIdentityClient
{
    /// <summary>
    /// Creates a sovereign identity on the blockchain.
    /// </summary>
    Task<CreateIdentityResult> CreateIdentityAsync(
        CreateIdentityRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets an identity from the blockchain.
    /// </summary>
    Task<GetIdentityResult?> GetIdentityAsync(
        string identityId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an identity on the blockchain.
    /// </summary>
    Task<UpdateIdentityResult> UpdateIdentityAsync(
        string identityId,
        Dictionary<string, string> updates,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Verifies an identity against the blockchain.
    /// </summary>
    Task<VerifyIdentityResult> VerifyIdentityAsync(
        string identityId,
        string verificationType,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// Request to create a sovereign identity on the blockchain.
/// </summary>
internal record CreateIdentityRequest(
    string CitizenId,
    string FirstName,
    string LastName,
    string DateOfBirth,
    string Nationality,
    Dictionary<string, string>? Metadata = null);

/// <summary>
/// Result of creating a sovereign identity on the blockchain.
/// </summary>
internal record CreateIdentityResult(
    bool Success,
    string? IdentityId,
    string? BlockchainAccount,
    string? ErrorMessage);

/// <summary>
/// Result of getting an identity from the blockchain.
/// </summary>
internal record GetIdentityResult(
    string IdentityId,
    string CitizenId,
    string FirstName,
    string LastName,
    string DateOfBirth,
    string Nationality,
    string Status,
    bool Success,
    string? ErrorMessage);

/// <summary>
/// Result of updating an identity on the blockchain.
/// </summary>
internal record UpdateIdentityResult(
    bool Success,
    string? ErrorMessage);

/// <summary>
/// Result of verifying an identity against the blockchain.
/// </summary>
internal record VerifyIdentityResult(
    bool Verified,
    string? VerificationResult,
    bool Success,
    string? ErrorMessage);
