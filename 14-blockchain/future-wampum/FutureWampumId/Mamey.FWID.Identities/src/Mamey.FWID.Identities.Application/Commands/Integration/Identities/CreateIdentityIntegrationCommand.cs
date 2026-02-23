using Mamey.CQRS.Commands;
using Mamey.FWID.Identities.Domain.ValueObjects;
using Mamey.MessageBrokers;
using Mamey.Types;

namespace Mamey.FWID.Identities.Application.Commands.Integration.Identities;

/// <summary>
/// Integration command to create an identity from another service.
/// </summary>
[Message("identities")]
internal record CreateIdentityIntegrationCommand : ICommand
{
    /// <summary>
    /// Gets or sets the identity identifier.
    /// </summary>
    public Guid Id { get; init; } = Guid.NewGuid();

    /// <summary>
    /// Gets or sets the name of the identity.
    /// </summary>
    public Name Name { get; init; } = null!;

    /// <summary>
    /// Gets or sets the personal details.
    /// </summary>
    public PersonalDetails PersonalDetails { get; init; } = null!;

    /// <summary>
    /// Gets or sets the contact information.
    /// </summary>
    public ContactInformation ContactInformation { get; init; } = null!;

    /// <summary>
    /// Gets or sets the biometric data.
    /// </summary>
    public BiometricData BiometricData { get; init; } = null!;

    /// <summary>
    /// Gets or sets the zone.
    /// </summary>
    public string? Zone { get; init; }

    /// <summary>
    /// Gets or sets the clan registrar identifier.
    /// </summary>
    public Guid? ClanRegistrarId { get; init; }
}


