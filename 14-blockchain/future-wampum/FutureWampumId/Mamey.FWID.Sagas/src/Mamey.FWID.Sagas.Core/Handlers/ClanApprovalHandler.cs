using Mamey.CQRS.Commands;
using Mamey.FWID.Sagas.Core.Commands;
using Mamey.MessageBrokers;
using Microsoft.Extensions.Logging;

namespace Mamey.FWID.Sagas.Core.Handlers;

/// <summary>
/// Handles clan approval decisions during identity registration.
/// </summary>
public class ClanApprovalHandler : ICommandHandler<ProcessClanApproval>
{
    private readonly ILogger<ClanApprovalHandler> _logger;
    private readonly IBusPublisher _publisher;
    
    public ClanApprovalHandler(
        ILogger<ClanApprovalHandler> logger,
        IBusPublisher publisher)
    {
        _logger = logger;
        _publisher = publisher;
    }
    
    public async Task HandleAsync(ProcessClanApproval command, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Processing clan approval for identity {IdentityId}. Registrar: {RegistrarId}, Approved: {Approved}",
            command.IdentityId, command.RegistrarId, command.Approved);
        
        if (command.Approved)
        {
            await _publisher.PublishAsync(new ClanApprovalGranted
            {
                IdentityId = command.IdentityId,
                ApprovalId = command.ApprovalId,
                RegistrarId = command.RegistrarId,
                Notes = command.Notes,
                ApprovedAt = DateTime.UtcNow
            });
            
            _logger.LogInformation("Clan approval granted for identity {IdentityId}", command.IdentityId);
        }
        else
        {
            await _publisher.PublishAsync(new ClanApprovalDenied
            {
                IdentityId = command.IdentityId,
                ApprovalId = command.ApprovalId,
                RegistrarId = command.RegistrarId,
                Reason = command.Notes ?? "Approval denied by registrar",
                DeniedAt = DateTime.UtcNow
            });
            
            _logger.LogWarning("Clan approval denied for identity {IdentityId}. Reason: {Reason}",
                command.IdentityId, command.Notes);
        }
    }
}

/// <summary>
/// Handles registration cancellation requests.
/// </summary>
public class CancelRegistrationHandler : ICommandHandler<CancelRegistration>
{
    private readonly ILogger<CancelRegistrationHandler> _logger;
    private readonly IBusPublisher _publisher;
    
    public CancelRegistrationHandler(
        ILogger<CancelRegistrationHandler> logger,
        IBusPublisher publisher)
    {
        _logger = logger;
        _publisher = publisher;
    }
    
    public async Task HandleAsync(CancelRegistration command, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Cancelling registration for identity {IdentityId}. Reason: {Reason}",
            command.IdentityId, command.Reason);
        
        await _publisher.PublishAsync(new RegistrationCancelled
        {
            IdentityId = command.IdentityId,
            Reason = command.Reason ?? "User requested cancellation",
            CancelledAt = DateTime.UtcNow
        });
    }
}

/// <summary>
/// Event published when clan approval is granted.
/// </summary>
public record ClanApprovalGranted
{
    public Guid IdentityId { get; init; }
    public Guid ApprovalId { get; init; }
    public Guid RegistrarId { get; init; }
    public string? Notes { get; init; }
    public DateTime ApprovedAt { get; init; }
}

/// <summary>
/// Event published when clan approval is denied.
/// </summary>
public record ClanApprovalDenied
{
    public Guid IdentityId { get; init; }
    public Guid ApprovalId { get; init; }
    public Guid RegistrarId { get; init; }
    public string Reason { get; init; } = null!;
    public DateTime DeniedAt { get; init; }
}

/// <summary>
/// Event published when registration is cancelled.
/// </summary>
public record RegistrationCancelled
{
    public Guid IdentityId { get; init; }
    public string Reason { get; init; } = null!;
    public DateTime CancelledAt { get; init; }
}

/// <summary>
/// Event published when clan approval times out.
/// </summary>
public record ClanApprovalTimedOut
{
    public Guid IdentityId { get; init; }
    public Guid ApprovalId { get; init; }
    public DateTime SubmittedAt { get; init; }
    public DateTime TimedOutAt { get; init; }
}
