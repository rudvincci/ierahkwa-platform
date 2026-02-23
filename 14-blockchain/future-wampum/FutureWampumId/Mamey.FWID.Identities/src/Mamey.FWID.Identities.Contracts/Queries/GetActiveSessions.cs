using System.ComponentModel.DataAnnotations;
using Mamey.CQRS.Queries;

namespace Mamey.FWID.Identities.Contracts.Queries;

/// <summary>
/// Query to get active sessions for an identity.
/// </summary>
public record GetActiveSessions : IQuery<List<SessionDto>>
{
    /// <summary>
    /// Gets or sets the identity identifier.
    /// </summary>
    [Required]
    public Guid IdentityId { get; init; }
}

/// <summary>
/// DTO for session information.
/// </summary>
public class SessionDto
{
    /// <summary>
    /// The session identifier.
    /// </summary>
    public Guid SessionId { get; set; }

    /// <summary>
    /// The creation date and time.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// The last accessed date and time.
    /// </summary>
    public DateTime LastAccessedAt { get; set; }

    /// <summary>
    /// The expiration date and time.
    /// </summary>
    public DateTime ExpiresAt { get; set; }

    /// <summary>
    /// The IP address.
    /// </summary>
    public string? IpAddress { get; set; }

    /// <summary>
    /// The user agent.
    /// </summary>
    public string? UserAgent { get; set; }
}

