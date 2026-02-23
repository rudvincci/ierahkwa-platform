using Mamey.CQRS;
using Mamey.FWID.Identities.Domain.Entities;

namespace Mamey.FWID.Identities.Domain.Events;

/// <summary>
/// Event raised when consent is granted.
/// </summary>
internal record ConsentGranted(
    ConsentId ConsentId,
    IdentityId IdentityId,
    string GranteeId,
    List<string> GrantedScopes,
    string Purpose,
    DateTime GrantedAt) : IDomainEvent;

/// <summary>
/// Event raised when consent is revoked.
/// </summary>
internal record ConsentRevoked(
    ConsentId ConsentId,
    IdentityId IdentityId,
    string GranteeId,
    string Reason,
    DateTime RevokedAt) : IDomainEvent;

/// <summary>
/// Event raised when consent scopes are updated.
/// </summary>
internal record ConsentScopesUpdated(
    ConsentId ConsentId,
    IdentityId IdentityId,
    string GranteeId,
    List<string> NewScopes) : IDomainEvent;

/// <summary>
/// Event raised when consent expiration is extended.
/// </summary>
internal record ConsentExtended(
    ConsentId ConsentId,
    IdentityId IdentityId,
    string GranteeId,
    DateTime? NewExpiration) : IDomainEvent;
