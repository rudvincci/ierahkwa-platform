namespace Mamey.ApplicationName.Modules.Identity.Core.Events;

/// <summary>
/// Published when an issued token (access or refresh) is explicitly revoked.
/// </summary>
public sealed record TokenRevoked(
    string TokenId,
    string Reason,
    DateTime OccurredAt = default)
{
    public TokenRevoked(string tokenId, string reason)
        : this(tokenId, reason, DateTime.UtcNow) { }
}