using System.ComponentModel.DataAnnotations;
using Mamey.CQRS.Commands;
using Mamey.Microservice.Abstractions.Messaging;
using Mamey.Types;

namespace Mamey.FWID.Sagas.Core.Commands;

/// <summary>
/// Command to start the Identity Registration saga.
/// </summary>
[Contract]
public record StartIdentityRegistration : ICommand
{
    [Required]
    public string FirstName { get; init; } = null!;
    
    [Required]
    public string LastName { get; init; } = null!;
    
    public string? Email { get; init; }
    
    public DateTime? DateOfBirth { get; init; }
    
    public string? Zone { get; init; }
    
    public string? Clan { get; init; }
    
    public Guid? IdentityId { get; init; }
    
    /// <summary>
    /// For backward compatibility.
    /// </summary>
    public string Name => $"{FirstName} {LastName}";
}

/// <summary>
/// Command to submit biometric data for the registration saga.
/// </summary>
[Contract]
public record SubmitBiometrics : ICommand
{
    [Required]
    public Guid IdentityId { get; init; }
    
    public byte[]? FingerprintTemplate { get; init; }
    
    public byte[]? FaceTemplate { get; init; }
    
    public double QualityScore { get; init; }
    
    public string? DeviceId { get; init; }
}

/// <summary>
/// Command when clan approval is received.
/// </summary>
[Contract]
public record ProcessClanApproval : ICommand
{
    [Required]
    public Guid IdentityId { get; init; }
    
    [Required]
    public Guid ApprovalId { get; init; }
    
    [Required]
    public Guid RegistrarId { get; init; }
    
    public bool Approved { get; init; }
    
    public string? Notes { get; init; }
}

/// <summary>
/// Command to cancel an in-progress registration.
/// </summary>
[Contract]
public record CancelRegistration : ICommand
{
    [Required]
    public Guid IdentityId { get; init; }
    
    public string? Reason { get; init; }
}



