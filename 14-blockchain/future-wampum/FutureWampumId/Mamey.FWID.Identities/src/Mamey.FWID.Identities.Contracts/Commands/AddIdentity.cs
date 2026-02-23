using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using Mamey.CQRS.Commands;
using Mamey.FWID.Identities.Contracts.DTOs;
using Mamey.Microservice.Abstractions.Messaging;
using Mamey.Types;

[assembly: InternalsVisibleTo("Mamey.FWID.Identities.Tests.Integration.Async")]
namespace Mamey.FWID.Identities.Contracts.Commands;

/// <summary>
/// Command to register a new identity.
/// </summary>
[Contract]
public record AddIdentity : ICommand
{
    /// <summary>
    /// Gets or sets the identity identifier.
    /// </summary>
    public Guid Id { get; init; } = Guid.NewGuid();

    /// <summary>
    /// Gets or sets the name of the identity.
    /// </summary>
    [Required]
    public Name Name { get; init; } = null!;

    /// <summary>
    /// Gets or sets the personal details.
    /// </summary>
    [Required]
    public PersonalDetailsDto PersonalDetails { get; init; } = null!;

    /// <summary>
    /// Gets or sets the contact information.
    /// </summary>
    [Required]
    public ContactInformationDto ContactInformation { get; init; } = null!;

    /// <summary>
    /// Gets or sets the biometric data.
    /// </summary>
    [Required]
    public BiometricDataDto BiometricData { get; init; } = null!;

    /// <summary>
    /// Gets or sets the zone.
    /// </summary>
    public string? Zone { get; init; }

    /// <summary>
    /// Gets or sets the clan registrar identifier.
    /// </summary>
    public Guid? ClanRegistrarId { get; init; }
}
