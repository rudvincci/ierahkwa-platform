using Mamey.Types;

namespace Mamey.FWID.Identities.Contracts.DTOs;

/// <summary>
/// Data transfer object for identity information.
/// </summary>
public class IdentityDto
{
    /// <summary>
    /// Gets or sets the identity identifier.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the name of the identity.
    /// </summary>
    public Name Name { get; set; } = null!;

    /// <summary>
    /// Gets or sets the status of the identity.
    /// </summary>
    public IdentityStatus Status { get; set; }

    /// <summary>
    /// Gets or sets the zone of the identity.
    /// </summary>
    public string? Zone { get; set; }

    /// <summary>
    /// Gets or sets the date and time the identity was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the date and time the identity was verified.
    /// </summary>
    public DateTime? VerifiedAt { get; set; }

    /// <summary>
    /// Gets or sets the date and time the identity was last verified.
    /// </summary>
    public DateTime? LastVerifiedAt { get; set; }
}
