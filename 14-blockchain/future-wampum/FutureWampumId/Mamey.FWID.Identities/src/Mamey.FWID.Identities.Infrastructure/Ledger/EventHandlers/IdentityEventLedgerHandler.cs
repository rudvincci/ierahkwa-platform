using Mamey.Contexts;
using Mamey.CQRS;
using Mamey.FWID.Identities.Domain.Events;
using Mamey.Microservice.Infrastructure;
using Microsoft.Extensions.Logging;

namespace Mamey.FWID.Identities.Infrastructure.Ledger.EventHandlers;

/// <summary>
/// Event handler for logging identity events to FutureWampumLedger.
/// Provides immutable audit trail for all identity lifecycle events.
/// </summary>
internal sealed class IdentityEventLedgerHandler : IDomainEventHandler<IdentityCreated>, IDomainEventHandler<IdentityModified>, IDomainEventHandler<IdentityRemoved>
{
    private readonly ILedgerAuditService _ledgerAuditService;
    private readonly IContext _context;
    private readonly ILogger<IdentityEventLedgerHandler> _logger;

    public IdentityEventLedgerHandler(
        ILedgerAuditService ledgerAuditService,
        IContext context,
        ILogger<IdentityEventLedgerHandler> logger)
    {
        _ledgerAuditService = ledgerAuditService;
        _context = context;
        _logger = logger;
    }

    public async Task HandleAsync(IdentityCreated @event, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Logging IdentityCreated event to ledger: IdentityId={IdentityId}",
            @event.IdentityId.Value);

        try
        {
            var success = await _ledgerAuditService.LogIdentityCreatedAsync(
                @event.IdentityId.Value,
                @event.Name?.FirstName,
                @event.Name?.LastName,
                @event.Email?.Value,
                _context.CorrelationId.ToString(),
                cancellationToken);

            if (success)
            {
                _logger.LogInformation(
                    "Successfully logged IdentityCreated to ledger: IdentityId={IdentityId}",
                    @event.IdentityId.Value);
            }
            else
            {
                _logger.LogWarning(
                    "Failed to log IdentityCreated to ledger: IdentityId={IdentityId}",
                    @event.IdentityId.Value);
            }
        }
        catch (Exception ex)
        {
            // Log error but don't fail the event handler - ledger logging is best effort
            _logger.LogError(ex,
                "Error logging IdentityCreated event to ledger: IdentityId={IdentityId}",
                @event.IdentityId.Value);
        }
    }

    public async Task HandleAsync(IdentityModified @event, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Logging IdentityModified event to ledger: IdentityId={IdentityId}",
            @event.Identity.Id.Value);

        try
        {
            // Extract changed attributes from the identity
            var metadata = new Dictionary<string, object>
            {
                { "modifiedAt", DateTime.UtcNow }
            };

            var success = await _ledgerAuditService.LogIdentityUpdatedAsync(
                @event.Identity.Id.Value,
                metadata,
                _context.CorrelationId.ToString(),
                cancellationToken);

            if (success)
            {
                _logger.LogInformation(
                    "Successfully logged IdentityModified to ledger: IdentityId={IdentityId}",
                    @event.Identity.Id.Value);
            }
            else
            {
                _logger.LogWarning(
                    "Failed to log IdentityModified to ledger: IdentityId={IdentityId}",
                    @event.Identity.Id.Value);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Error logging IdentityModified event to ledger: IdentityId={IdentityId}",
                @event.Identity.Id.Value);
        }
    }

    public async Task HandleAsync(IdentityRemoved @event, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Logging IdentityRemoved event to ledger: IdentityId={IdentityId}",
            @event.Identity.Id.Value);

        try
        {
            var success = await _ledgerAuditService.LogIdentityDeletedAsync(
                @event.Identity.Id.Value,
                _context.CorrelationId.ToString(),
                cancellationToken);

            if (success)
            {
                _logger.LogInformation(
                    "Successfully logged IdentityRemoved to ledger: IdentityId={IdentityId}",
                    @event.Identity.Id.Value);
            }
            else
            {
                _logger.LogWarning(
                    "Failed to log IdentityRemoved to ledger: IdentityId={IdentityId}",
                    @event.Identity.Id.Value);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Error logging IdentityRemoved event to ledger: IdentityId={IdentityId}",
                @event.Identity.Id.Value);
        }
    }
}
