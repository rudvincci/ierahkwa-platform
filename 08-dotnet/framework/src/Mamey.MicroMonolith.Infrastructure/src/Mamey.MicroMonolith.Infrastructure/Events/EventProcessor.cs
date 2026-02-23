using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Mamey.CQRS;
using Mamey.CQRS.Events;
using Mamey.MessageBrokers;
using Mamey.MicroMonolith.Abstractions.Messaging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Mamey.MicroMonolith.Infrastructure.Events;

internal sealed class EventProcessor : IEventProcessor
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly IEventMapper _eventMapper;
    private readonly IMessageBroker _messageBroker;
    private readonly ILogger<EventProcessor> _logger;

    public EventProcessor(IServiceScopeFactory serviceScopeFactory, IEventMapper eventMapper,
        IMessageBroker messageBroker, ILogger<EventProcessor> logger)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _eventMapper = eventMapper;
        _messageBroker = messageBroker;
        _logger = logger;
    }

    public async Task ProcessAsync(IEnumerable<IDomainEvent> events, CancellationToken cancellationToken = default)
    {
        if (events is null)
        {
            return;
        }

        _logger.LogTrace("Processing domain events...");
        var integrationEvents = await HandleDomainEvents(events);
        if (!integrationEvents.Any())
        {
            return;
        }

        _logger.LogTrace("Processing integration events...");
        await _messageBroker.PublishAsync((IMessage)integrationEvents, cancellationToken);
    }

    private async Task<List<IEvent>> HandleDomainEvents(IEnumerable<IDomainEvent> events)
    {
        var integrationEvents = new List<IEvent>();
        using var scope = _serviceScopeFactory.CreateScope();
        foreach (var @event in events)
        {
            var eventType = @event.GetType();
            _logger.LogTrace($"Handling domain event: {eventType.Name}");
            var handlerType = typeof(IDomainEventHandler<>).MakeGenericType(eventType);
            dynamic handlers = scope.ServiceProvider.GetServices(handlerType);
            foreach (var handler in handlers)
            {
                await handler.HandleAsync((dynamic)@event);
            }

            var integrationEvent = _eventMapper.Map(@event);
            if (integrationEvent is null)
            {
                continue;
            }

            integrationEvents.Add(integrationEvent);
        }

        return integrationEvents;
    }
    
}

