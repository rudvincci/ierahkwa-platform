using Mamey.CQRS.Queries;
using Mamey.FWID.Identities.Contracts.DTOs;

namespace Mamey.FWID.Identities.Contracts.Queries;

/// <summary>
/// Query to get an identity by identifier.
/// Note: Queries should NOT have [Contract] attribute.
/// </summary>
public record GetIdentity(Guid IdentityId) : IQuery<IdentityDto>;

