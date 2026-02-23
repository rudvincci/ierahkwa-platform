using System.ComponentModel.DataAnnotations;
using Mamey.CQRS.Commands;
using Mamey.Microservice.Abstractions.Messaging;

namespace Mamey.FWID.Sagas.Core.Commands;

/// <summary>
/// Command to start the Credential Issuance saga.
/// </summary>
[Contract]
public record StartCredentialIssuance : ICommand
{
    [Required]
    public Guid IdentityId { get; init; }
    
    [Required]
    [MinLength(1)]
    public string CredentialType { get; init; } = null!;
    
    [Required]
    public Dictionary<string, object> Claims { get; init; } = null!;
    
    [Required]
    public Guid IssuerId { get; init; }
    
    public DateTime? ExpiresAt { get; init; }
}



