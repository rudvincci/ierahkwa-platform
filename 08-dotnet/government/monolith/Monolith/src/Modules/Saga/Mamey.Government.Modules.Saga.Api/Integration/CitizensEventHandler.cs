using System;
using System.Threading;
using System.Threading.Tasks;
using Chronicle;
using Mamey.CQRS.Events;
using Mamey.Government.Modules.Saga.Api.Messages.Citizens;
using Mamey.Government.Modules.Saga.Api.Messages.Citizenship;
using Mamey.MicroMonolith.Abstractions.Contexts;
using Microsoft.Extensions.Logging;

namespace Mamey.Government.Modules.Saga.Api.Integration;

/// <summary>
/// Handles events from the Citizens module and forwards them to the saga.
/// </summary>
internal sealed class CitizensEventHandler :
    IEventHandler<CitizenCreatedEvent>
{
    private readonly ISagaCoordinator _sagaCoordinator;
    private readonly ILogger<CitizensEventHandler> _logger;
    private readonly IContext _context;

    public CitizensEventHandler(
        ISagaCoordinator sagaCoordinator,
        ILogger<CitizensEventHandler> logger, IContext context)
    {
        _sagaCoordinator = sagaCoordinator;
        _logger = logger;
        _context = context;
    }

    public async Task HandleAsync(CitizenCreatedEvent @event, CancellationToken cancellationToken = default)
    {
        // Only forward to saga if this citizen was created from an application
        if (!@event.ApplicationId.HasValue || @event.ApplicationId == Guid.Empty)
        {
            return;
        }
        
        _logger.LogInformation("Forwarding CitizenCreated event to saga: {CitizenId}, ApplicationId: {ApplicationId}", 
            @event.CitizenId, @event.ApplicationId);
        
        var message = new CitizenCreatedEvent(
            @event.CitizenId,
            _context.TenantId.Value,
            @event.FirstName,
            @event.LastName,
            $"{@event.FirstName} {@event.LastName}");

        await _sagaCoordinator.ProcessAsync(message, SagaContext.Empty);
    }
}
