using System.ComponentModel.DataAnnotations;
using Mamey.CQRS.Queries;

namespace Mamey.FWID.Identities.Contracts.Queries;

/// <summary>
/// Query to get MFA status for an identity.
/// </summary>
public record GetIdentityMfaStatus : IQuery<MfaStatusDto>
{
    /// <summary>
    /// Gets or sets the identity identifier.
    /// </summary>
    [Required]
    public Guid IdentityId { get; init; }
}

/// <summary>
/// DTO for MFA status.
/// </summary>
public class MfaStatusDto
{
    /// <summary>
    /// Indicates whether MFA is enabled.
    /// </summary>
    public bool IsEnabled { get; set; }

    /// <summary>
    /// The preferred MFA method.
    /// </summary>
    public MfaMethod? PreferredMethod { get; set; }

    /// <summary>
    /// List of enabled MFA methods.
    /// </summary>
    public List<MfaMethod> EnabledMethods { get; set; } = new();
}

