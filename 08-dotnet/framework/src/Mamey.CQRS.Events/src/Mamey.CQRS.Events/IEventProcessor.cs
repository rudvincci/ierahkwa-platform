namespace Mamey.CQRS.Events;

public interface IEventProcessor
{
    Task ProcessAsync(IEnumerable<IDomainEvent> events, CancellationToken cancellationToken = default);
}