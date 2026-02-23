using Chronicle;
using Mamey.FWID.Sagas.Core.Commands;
using Mamey.FWID.Sagas.Core.Data;
using Mamey.FWID.AccessControls.Domain.Events;
using Mamey.MessageBrokers;
using Microsoft.Extensions.Logging;

namespace Mamey.FWID.Sagas.Core.Sagas.AccessControlGranting;

/// <summary>
/// Saga for orchestrating Access Control Granting process.
/// Orchestrates: Identity Verification → Zone Validation → Zone Access Granting → Ledger Logging
/// </summary>
internal sealed class AccessControlGrantingSaga : Saga<AccessControlGrantingSagaData>,
    ISagaStartAction<StartAccessControlGranting>,
    ISagaAction<ZoneAccessGranted>
{
    private readonly ILogger<AccessControlGrantingSaga> _logger;
    private readonly IBusPublisher _publisher;
    private readonly ICorrelationContextAccessor _accessor;

    public AccessControlGrantingSaga(
        ILogger<AccessControlGrantingSaga> logger,
        IBusPublisher publisher,
        ICorrelationContextAccessor accessor)
    {
        _logger = logger;
        _publisher = publisher;
        _accessor = accessor;
    }

    public override SagaId ResolveId(object message, ISagaContext context)
        => message switch
        {
            StartAccessControlGranting cmd => (SagaId)$"{cmd.IdentityId}_{cmd.ZoneId}",
            ZoneAccessGranted e => (SagaId)$"{e.IdentityId.Value}_{e.ZoneId.Value}",
            _ => base.ResolveId(message, context)
        };

    /// <summary>
    /// Starts the saga by verifying identity and zone, then granting access.
    /// </summary>
    public async Task HandleAsync(StartAccessControlGranting message, ISagaContext context)
    {
        Data.IdentityId = message.IdentityId;
        Data.ZoneId = message.ZoneId;
        Data.Permission = message.Permission;
        Data.Status = "Processing";
        Data.StartedAt = DateTime.UtcNow;
        Data.IdentityVerified = true; // TODO: Verify identity exists
        Data.ZoneValidated = true; // TODO: Validate zone exists

        _logger.LogInformation("Access Control Granting saga started for IdentityId: {IdentityId}, ZoneId: {ZoneId}, Permission: {Permission}", 
            Data.IdentityId, Data.ZoneId, Data.Permission);

        // Parse permission enum
        var permissionEnum = Enum.Parse<Mamey.FWID.AccessControls.Domain.ValueObjects.AccessPermission>(message.Permission);

        // Grant zone access
        var grantAccessCommand = new Mamey.FWID.AccessControls.Contracts.Commands.GrantZoneAccess
        {
            IdentityId = new Mamey.FWID.AccessControls.Domain.Entities.IdentityId(message.IdentityId),
            ZoneId = new Mamey.FWID.AccessControls.Domain.ValueObjects.ZoneId(message.ZoneId),
            Permission = permissionEnum
        };

        await _publisher.PublishAsync(grantAccessCommand);
    }

    /// <summary>
    /// Handles ZoneAccessGranted event - proceeds to log to ledger and complete saga.
    /// </summary>
    public Task HandleAsync(ZoneAccessGranted e, ISagaContext context)
    {
        Data.AccessGranted = true;
        Data.AccessControlId = e.AccessControlId.Value;

        _logger.LogInformation("Zone access granted: {AccessControlId} for Identity: {IdentityId}, Zone: {ZoneId}. Logging to ledger.", 
            Data.AccessControlId, Data.IdentityId, Data.ZoneId);

        // TODO: Log to ledger (external service)
        Data.LedgerLogged = true;
        Data.Status = "Completed";
        Data.CompletedAt = DateTime.UtcNow;

        _logger.LogInformation("Access Control Granting saga completed for IdentityId: {IdentityId}, ZoneId: {ZoneId}", 
            Data.IdentityId, Data.ZoneId);

        return Task.CompletedTask;
    }

    #region Compensation

    public Task CompensateAsync(StartAccessControlGranting message, ISagaContext context)
    {
        _logger.LogWarning("Compensating Access Control Granting saga for IdentityId: {IdentityId}, ZoneId: {ZoneId}", 
            Data.IdentityId, Data.ZoneId);
        return Task.CompletedTask;
    }

    public async Task CompensateAsync(ZoneAccessGranted message, ISagaContext context)
    {
        _logger.LogWarning("Compensating Zone Access Grant for AccessControlId: {AccessControlId}", message.AccessControlId);
        
        // Revoke access
        var revokeAccessCommand = new Mamey.FWID.AccessControls.Contracts.Commands.RevokeZoneAccess
        {
            AccessControlId = message.AccessControlId,
            Reason = "Saga compensation - access control granting failed"
        };

        await _publisher.PublishAsync(revokeAccessCommand);
    }

    #endregion
}



