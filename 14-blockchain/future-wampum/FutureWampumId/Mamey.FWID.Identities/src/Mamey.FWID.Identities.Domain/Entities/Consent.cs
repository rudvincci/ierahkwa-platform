using System.Runtime.CompilerServices;
using Mamey.FWID.Identities.Domain.Events;
using Mamey.Types;

[assembly: InternalsVisibleTo("Mamey.FWID.Identities.Tests.Unit.Core.Entities")]
namespace Mamey.FWID.Identities.Domain.Entities;

/// <summary>
/// Represents a consent record for data sharing and access permissions.
/// </summary>
internal class Consent : AggregateRoot<ConsentId>
{
    /// <summary>
    /// Private parameterless constructor for Entity Framework Core.
    /// </summary>
    private Consent()
    {
        GrantedScopes = new List<string>();
        Metadata = new Dictionary<string, object>();
    }

    /// <summary>
    /// Initializes a new instance of the Consent aggregate root.
    /// </summary>
    /// <param name="id">The consent identifier.</param>
    /// <param name="identityId">The identity that granted the consent.</param>
    /// <param name="granteeId">The identity or service receiving the consent.</param>
    /// <param name="grantedScopes">The scopes being granted.</param>
    /// <param name="purpose">The purpose of the consent.</param>
    /// <param name="expiresAt">Optional expiration date.</param>
    public Consent(
        ConsentId id,
        IdentityId identityId,
        string granteeId,
        List<string> grantedScopes,
        string purpose,
        DateTime? expiresAt = null)
        : base(id)
    {
        IdentityId = identityId ?? throw new ArgumentNullException(nameof(identityId));
        GranteeId = granteeId ?? throw new ArgumentNullException(nameof(granteeId));
        GrantedScopes = grantedScopes ?? new List<string>();
        Purpose = purpose ?? throw new ArgumentNullException(nameof(purpose));
        Status = ConsentStatus.Active;
        GrantedAt = DateTime.UtcNow;
        ExpiresAt = expiresAt;
        Version = 1;

        AddEvent(new ConsentGranted(Id, IdentityId, GranteeId, GrantedScopes, Purpose, GrantedAt));
    }

    #region Properties

    /// <summary>
    /// The identity that granted the consent.
    /// </summary>
    public IdentityId IdentityId { get; private set; }

    /// <summary>
    /// The identity or service receiving the consent.
    /// </summary>
    public string GranteeId { get; private set; }

    /// <summary>
    /// The scopes being granted.
    /// </summary>
    public List<string> GrantedScopes { get; private set; }

    /// <summary>
    /// The purpose of the consent.
    /// </summary>
    public string Purpose { get; private set; }

    /// <summary>
    /// The status of the consent.
    /// </summary>
    public ConsentStatus Status { get; private set; }

    /// <summary>
    /// Date and time the consent was granted.
    /// </summary>
    public DateTime GrantedAt { get; private set; }

    /// <summary>
    /// Date and time the consent expires.
    /// </summary>
    public DateTime? ExpiresAt { get; private set; }

    /// <summary>
    /// Date and time the consent was revoked.
    /// </summary>
    public DateTime? RevokedAt { get; private set; }

    /// <summary>
    /// The reason for revocation.
    /// </summary>
    public string? RevocationReason { get; private set; }

    /// <summary>
    /// Additional metadata.
    /// </summary>
    public Dictionary<string, object> Metadata { get; private set; }

    #endregion

    #region Domain Methods

    /// <summary>
    /// Revokes the consent.
    /// </summary>
    /// <param name="reason">The reason for revocation.</param>
    public void Revoke(string reason)
    {
        if (Status == ConsentStatus.Revoked)
            return;

        Status = ConsentStatus.Revoked;
        RevokedAt = DateTime.UtcNow;
        RevocationReason = reason;
        IncrementVersion();

        AddEvent(new ConsentRevoked(Id, IdentityId, GranteeId, reason, RevokedAt.Value));
    }

    /// <summary>
    /// Updates the granted scopes.
    /// </summary>
    /// <param name="newScopes">The new scopes to grant.</param>
    public void UpdateScopes(List<string> newScopes)
    {
        if (Status != ConsentStatus.Active)
            throw new InvalidOperationException("Cannot update scopes of inactive consent");

        GrantedScopes = newScopes ?? new List<string>();
        IncrementVersion();

        AddEvent(new ConsentScopesUpdated(Id, IdentityId, GranteeId, GrantedScopes));
    }

    /// <summary>
    /// Extends the consent expiration.
    /// </summary>
    /// <param name="newExpiration">The new expiration date.</param>
    public void ExtendExpiration(DateTime? newExpiration)
    {
        if (Status != ConsentStatus.Active)
            throw new InvalidOperationException("Cannot extend expiration of inactive consent");

        ExpiresAt = newExpiration;
        IncrementVersion();

        AddEvent(new ConsentExtended(Id, IdentityId, GranteeId, ExpiresAt));
    }

    /// <summary>
    /// Checks if the consent is valid for the given scope.
    /// </summary>
    /// <param name="scope">The scope to check.</param>
    /// <returns>True if the consent is valid for the scope.</returns>
    public bool IsValidForScope(string scope)
    {
        return Status == ConsentStatus.Active &&
               (ExpiresAt == null || ExpiresAt > DateTime.UtcNow) &&
               GrantedScopes.Contains(scope);
    }

    /// <summary>
    /// Checks if the consent is currently active.
    /// </summary>
    /// <returns>True if the consent is active and not expired.</returns>
    public bool IsActive()
    {
        return Status == ConsentStatus.Active &&
               (ExpiresAt == null || ExpiresAt > DateTime.UtcNow);
    }

    #endregion
}

/// <summary>
/// Represents the status of a consent.
/// </summary>
internal enum ConsentStatus
{
    Active,
    Revoked,
    Expired
}
