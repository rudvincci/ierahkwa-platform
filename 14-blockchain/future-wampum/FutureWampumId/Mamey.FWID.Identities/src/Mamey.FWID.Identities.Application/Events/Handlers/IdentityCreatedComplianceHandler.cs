using Mamey.Contexts;
using Mamey.CQRS;
using Mamey.FWID.Identities.Application.Services;
using Mamey.FWID.Identities.Domain.Events;
using Mamey.Microservice.Infrastructure;
using Microsoft.Extensions.Logging;

namespace Mamey.FWID.Identities.Application.Events.Handlers;

/// <summary>
/// Event handler that logs IdentityCreated events to the compliance audit trail.
/// Ensures regulatory compliance for all identity creation events.
/// 
/// TDD Reference: Lines 1476-1498 (Compliance Requirements)
/// BDD Reference: Lines 645-692 (VII. Compliance and Regulatory Framework)
/// Regulations: 2025-AM01, 2025-ID01, GOV-005
/// </summary>
internal sealed class IdentityCreatedComplianceHandler : IDomainEventHandler<IdentityCreated>
{
    private readonly IAuditTrailService _auditTrailService;
    private readonly IContext _context;
    private readonly ILogger<IdentityCreatedComplianceHandler> _logger;

    public IdentityCreatedComplianceHandler(
        IAuditTrailService auditTrailService,
        IContext context,
        ILogger<IdentityCreatedComplianceHandler> logger)
    {
        _auditTrailService = auditTrailService ?? throw new ArgumentNullException(nameof(auditTrailService));
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task HandleAsync(IdentityCreated @event, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Handling IdentityCreated event for compliance audit: IdentityId={IdentityId}, Name={Name}",
            @event.IdentityId.Value, @event.Name?.FullName);

        try
        {
            // Log identity creation to compliance audit trail
            var result = await _auditTrailService.LogIdentityCreatedAsync(
                @event.IdentityId.Value.ToString(),
                @event.Zone,
                null, // ClanRegistrarId would come from a richer event
                _context.CorrelationId.ToString(),
                cancellationToken);

            if (result.Success)
            {
                _logger.LogInformation(
                    "Successfully logged IdentityCreated to compliance audit trail: AuditEntryId={AuditEntryId}",
                    result.AuditEntryId);
            }
            else
            {
                _logger.LogWarning(
                    "Failed to log IdentityCreated to compliance audit trail: Error={Error}",
                    result.ErrorMessage);
            }
        }
        catch (Exception ex)
        {
            // Log error but don't fail the event handler - compliance logging is best effort
            _logger.LogError(ex,
                "Error logging IdentityCreated to compliance audit trail: IdentityId={IdentityId}",
                @event.IdentityId.Value);
        }
    }
}
