namespace Mamey.ApplicationName.Modules.Identity.Core.Events;

/// <summary>
/// Published when a specific user or role claim is revoked.
/// </summary>
public sealed record ClaimRevoked(
    Guid ClaimId,
    string RevokedBy,
    DateTime OccurredAt = default)
{
    public ClaimRevoked(Guid claimId, string revokedBy)
        : this(claimId, revokedBy, DateTime.UtcNow) { }
}