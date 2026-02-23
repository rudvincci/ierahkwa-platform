using System;
using System.Threading;
using System.Threading.Tasks;
using Chronicle;
using Mamey.CQRS.Events;
using Mamey.Government.Modules.Saga.Api.Messages.Citizenship;
using Mamey.Government.Modules.Saga.Api.Messages.Citizenship;
using Microsoft.Extensions.Logging;

namespace Mamey.Government.Modules.Saga.Api.Integration;

/// <summary>
/// Handles events from the CitizenshipApplications module and forwards them to the saga.
/// </summary>
internal sealed class CitizenshipApplicationEventHandler :
    IEventHandler<ApplicationSubmittedEvent>,
    IEventHandler<ApplicationApprovedEvent>,
    IEventHandler<ApplicationRejectedEvent>
{
    private readonly ISagaCoordinator _sagaCoordinator;
    private readonly ILogger<CitizenshipApplicationEventHandler> _logger;

    public CitizenshipApplicationEventHandler(
        ISagaCoordinator sagaCoordinator,
        ILogger<CitizenshipApplicationEventHandler> logger)
    {
        _sagaCoordinator = sagaCoordinator;
        _logger = logger;
    }

    public async Task HandleAsync(ApplicationSubmittedEvent @event, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Forwarding ApplicationSubmitted event to saga: {ApplicationId}", @event.ApplicationId);
        
        var message = new ApplicationSubmittedEvent(
            @event.ApplicationId,
            @event.ApplicationNumber);

        await _sagaCoordinator.ProcessAsync(message, SagaContext.Empty);
    }

    public async Task HandleAsync(ApplicationApprovedEvent @event, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Forwarding ApplicationApproved event to saga: {ApplicationId}", @event.ApplicationId);
        
        var message = new ApplicationApprovedEvent(
            @event.ApplicationId,
            @event.ApplicationNumber,
            @event.ApprovedBy,
            @event.ApprovedAt);

        await _sagaCoordinator.ProcessAsync(message, SagaContext.Empty);
    }

    public async Task HandleAsync(ApplicationRejectedEvent @event, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Forwarding ApplicationRejected event to saga: {ApplicationId}", @event.ApplicationId);
        
        var message = new ApplicationRejectedEvent(
            @event.ApplicationId,
            @event.ApplicationNumber,
            @event.Reason,
            @event.RejectedBy,
            @event.RejectedAt);

        await _sagaCoordinator.ProcessAsync(message, SagaContext.Empty);
    }
}
