using Mamey.CQRS.Queries;
using Mamey.FWID.Identities.Contracts.DTOs;

namespace Mamey.FWID.Identities.Contracts.Queries;

/// <summary>
/// Query to find identities with filters.
/// Note: Queries should NOT have [Contract] attribute.
/// </summary>
public record FindIdentities(string? Zone, IdentityStatus? Status) : IQuery<List<IdentityDto>>;

