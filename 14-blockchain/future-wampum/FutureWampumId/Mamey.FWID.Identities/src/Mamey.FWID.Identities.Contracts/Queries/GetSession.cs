using System.ComponentModel.DataAnnotations;
using Mamey.CQRS.Queries;

namespace Mamey.FWID.Identities.Contracts.Queries;

/// <summary>
/// Query to get a session by identifier.
/// </summary>
public record GetSession : IQuery<SessionDto>
{
    /// <summary>
    /// Gets or sets the session identifier.
    /// </summary>
    [Required]
    public Guid SessionId { get; init; }
}

