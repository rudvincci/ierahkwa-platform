using System.ComponentModel.DataAnnotations;
using Mamey.CQRS.Commands;
using Mamey.Microservice.Abstractions.Messaging;

namespace Mamey.FWID.Sagas.Core.Commands;

/// <summary>
/// Command to start the Access Control Granting saga.
/// </summary>
[Contract]
public record StartAccessControlGranting : ICommand
{
    [Required]
    public Guid IdentityId { get; init; }
    
    [Required]
    public Guid ZoneId { get; init; }
    
    [Required]
    public string Permission { get; init; } = null!; // "Read", "Write", "Admin"
}



